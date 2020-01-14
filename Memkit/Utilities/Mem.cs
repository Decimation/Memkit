using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Memkit.Interop;
using Memkit.Model;
using Memkit.Pointers;

namespace Memkit.Utilities
{
	/// <summary>
	///     Provides utilities for manipulating pointers, memory, and types. This class has
	///     <seealso cref="System.Runtime.CompilerServices.Unsafe" /> built in.
	///     Also see JitHelpers from CompilerServices.
	///     <seealso cref="BitConverter" />
	///     <seealso cref="System.Convert" />
	///     <seealso cref="MemoryMarshal" />
	///     <seealso cref="Marshal" />
	///     <seealso cref="Span{T}" />
	///     <seealso cref="Memory{T}" />
	///     <seealso cref="Buffer" />
	///     <seealso cref="Mem" />
	///     <seealso cref="System.Runtime.CompilerServices.Unsafe" />
	///     <seealso cref="System.Runtime.CompilerServices" />
	/// </summary>
	public static unsafe class Mem
	{
		public static Region StackRegion {
			get {
				var info = new MemoryBasicInfo();
				var ptr  = new IntPtr(&info);
				info = Native.VirtualQuery(ptr);

				// todo: verify
				long size = (info.BaseAddress.ToInt64() - info.AllocationBase.ToInt64()) + info.RegionSize.ToInt64();

				return new Region(info.AllocationBase, size);
			}
		}

		public static bool IsAddressInRange(Pointer<byte> p, Region r)
		{
			return IsAddressInRange(p, r.Low, r.High);
		}

		/// <param name="p">Operand</param>
		/// <param name="lo">Start address (inclusive)</param>
		/// <param name="hi">End address (inclusive)</param>
		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, Pointer<byte> hi)
		{
			// [lo, hi]

			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();

			return p <= hi && p >= lo;
		}

		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, long size)
		{
			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();


			return (p >= lo && p < lo + size);
		}

		/// <summary>
		/// Calculates the total byte size of <paramref name="elemCnt"/> elements with
		/// the size of <paramref name="elemSize"/>.
		/// </summary>
		/// <param name="elemSize">Byte size of one element</param>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>Total byte size of all elements</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FlatSize(int elemSize, int elemCnt)
		{
			// (void*) (((long) m_value) + byteOffset)
			// (void*) (((long) m_value) + (elemOffset * ElementSize))
			return elemCnt * elemSize;
		}

		/// <summary>
		/// Reads a simple structure using stack allocation.
		/// </summary>
		public static T ReadStructure<T>(byte[] bytes) where T : struct
		{
			// Pin the managed memory while, copy it out the data, then unpin it

			byte* handle = stackalloc byte[bytes.Length];
			Marshal.Copy(bytes, 0, (IntPtr) handle, bytes.Length);

			var value = (T) Marshal.PtrToStructure((IntPtr) handle, typeof(T));

			return value;
		}

		public static T ReadStructure<T>(this BinaryReader reader) where T : struct
		{
			// Read in a byte array
			byte[] bytes = reader.ReadBytes(Marshal.SizeOf<T>());

			return Mem.ReadStructure<T>(bytes);
		}
		
		/*[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadFast<T>(void* source, int elemOfs)
		{
			IL.Emit.Ldarg(nameof(elemOfs));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Mul();
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Add();
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}*/

		public static int Val32(int i)
		{
			// todo
			return i;
		}

		public static short Val16(short i)
		{
			// todo
			return i;
		}

		public static string ReadString(sbyte* first, int len)
		{
			if (first == null || len <= 0) {
				return null;
			}

			return new string(first, 0, len);
		}

		/// <summary>
		///     Used for unsafe pinning of arbitrary objects.
		/// </summary>
		public static PinHelper GetPinHelper(object value) => Unsafe.As<PinHelper>(value);

		/// <summary>
		///     <para>Returns the address of <paramref name="value" />.</para>
		/// </summary>
		/// <param name="value">Value to return the address of.</param>
		/// <returns>The address of the type in memory.</returns>
		public static Pointer<T> AddressOf<T>(ref T value)
		{
			/*var tr = __makeref(t);
			return *(IntPtr*) (&tr);*/

			return Unsafe.AsPointer(ref value);
		}

		public static bool TryGetAddressOfHeap<T>(T value, OffsetOptions options, out Pointer<byte> ptr)
		{
			if (Inspector.IsStruct(value)) {
				ptr = null;
				return false;
			}

			ptr = AddressOfHeapInternal(value, options);
			return true;
		}

		public static bool TryGetAddressOfHeap<T>(T value, out Pointer<byte> ptr)
		{
			return TryGetAddressOfHeap(value, OffsetOptions.None, out ptr);
		}

		/// <summary>
		///     Returns the address of the data of <paramref name="value" />. If <typeparamref name="T" /> is a value type,
		///     this will return <see cref="AddressOf{T}" />. If <typeparamref name="T" /> is a reference type,
		///     this will return the equivalent of <see cref="AddressOfHeap{T}(T, OffsetOptions)" /> with
		///     <see cref="OffsetOptions.Fields" />.
		/// </summary>
		public static Pointer<byte> AddressOfFields<T>(ref T value)
		{
			Pointer<T> addr = AddressOf(ref value);

			return Inspector.IsStruct(value)
				? addr.Cast()
				: AddressOfHeapInternal(value, OffsetOptions.Fields);
		}


		/// <summary>
		///     Returns the address of reference type <paramref name="value" />'s heap memory, offset by the specified
		///     <see cref="OffsetOptions" />.
		///     <remarks>
		///         <para>
		///             Note: This does not pin the reference in memory if it is a reference type.
		///             This may require pinning to prevent the GC from moving the object.
		///             If the GC compacts the heap, this pointer may become invalid.
		///         </para>
		///     </remarks>
		/// </summary>
		/// <param name="value">Reference type to return the heap address of</param>
		/// <param name="offset">Offset type</param>
		/// <returns>The address of <paramref name="value" /></returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="offset"></paramref> is out of range.</exception>
		public static Pointer<byte> AddressOfHeap<T>(T value, OffsetOptions offset = OffsetOptions.None) where T : class
			=> AddressOfHeapInternal(value, offset);

		private static Pointer<byte> AddressOfHeapInternal<T>(T value, OffsetOptions offset)
		{
			// It is already assumed value is a class type

			//var tr = __makeref(value);
			//var heapPtr = **(IntPtr**) (&tr);

			Pointer<byte> heapPtr = AddressOf(ref value).ReadPointer();


			// NOTE:
			// Strings have their data offset by Offsets.OffsetToStringData
			// Arrays have their data offset by IntPtr.Size * 2 bytes (may be different for 32 bit)

			int offsetValue = 0;

			switch (offset) {
				case OffsetOptions.StringData:
					Trace.Assert(Inspector.IsString(value));
					offsetValue = Assets.OffsetToStringData;
					break;

				case OffsetOptions.ArrayData:
					Trace.Assert(Inspector.IsArray(value));
					offsetValue = Assets.OffsetToArrayData;
					break;

				case OffsetOptions.Fields:
					offsetValue = Assets.OffsetToData;
					break;

				case OffsetOptions.None:
					break;

				case OffsetOptions.Header:
					offsetValue = -Assets.OffsetToData;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
			}

			return heapPtr + offsetValue;
		}


		#region Fields

		/// <summary>
		/// Size of a pointer. Equals <see cref="IntPtr.Size"/>.
		/// </summary>
		public static readonly int Size = IntPtr.Size;

		/// <summary>
		/// Determines whether this process is 64-bit.
		/// </summary>
		public static readonly bool Is64Bit = Size == sizeof(long) && Environment.Is64BitProcess;

		public static readonly bool IsBigEndian = !BitConverter.IsLittleEndian;

		/// <summary>
		/// Represents a <c>null</c> <see cref="Pointer{T}"/>. Equivalent to <see cref="IntPtr.Zero"/>.
		/// </summary>
		public static readonly Pointer<byte> Nullptr = null;

		#endregion


		#region Sizes


		public static int QuickSizeOf<T>() => Unsafe.SizeOf<T>();

		#endregion

		#region Read / Write

		public static T ReadCurrentProcessMemory<T>(Pointer<byte> ptrBase) =>
			ReadProcessMemory<T>(Process.GetCurrentProcess(), ptrBase);

		public static T ReadProcessMemory<T>(Process proc, Pointer<byte> ptrBase)
		{
			T          t    = default;
			int        size = Mem.QuickSizeOf<T>();
			Pointer<T> ptr  = Mem.AddressOf(ref t);

			ReadProcessMemory(proc, ptrBase.Address, ptr.Address, size);

			return t;
		}

		public static void WriteCurrentProcessMemory<T>(Pointer<byte> ptrBase, T value) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);

		public static void WriteProcessMemory<T>(Process proc, Pointer<byte> ptrBase, T value)
		{
			int        dwSize = Mem.QuickSizeOf<T>();
			Pointer<T> ptr    = Mem.AddressOf(ref value);

			WriteProcessMemory(proc, ptrBase.Address, ptr.Address, dwSize);
		}

		#endregion

		#region Read / write raw bytes

		#region Read raw bytes

		public static void ReadProcessMemory(Process       proc,      Pointer<byte> ptrBase,
		                                     Pointer<byte> ptrBuffer, int           cb)
		{
			var hProc = Native.OpenProcess(proc);


			// Read the memory
			bool ok = (Native.ReadProcMemoryInternal(hProc, ptrBase.Address,
			                                         ptrBuffer.Address, cb,
			                                         out int numberOfBytesRead));

			Trace.Assert(numberOfBytesRead == cb && ok);

			// Close the handle
			Native.CloseHandle(hProc);
		}

		public static byte[] ReadProcessMemory(Process proc, Pointer<byte> ptrBase, int cb)
		{
			var mem = new byte[cb];

			fixed (byte* p = mem) {
				ReadProcessMemory(proc, ptrBase, (IntPtr) p, cb);
			}

			return mem;
		}


		#region Current process

		public static byte[] ReadCurrentProcessMemory(Pointer<byte> ptrBase, int cb) =>
			ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, cb);

		public static void ReadCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer, int cb) =>
			ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, cb);

		#endregion

		#endregion

		#region Write raw bytes

		#region Current process

		public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, byte[] value) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);

		public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
		                                             int           dwSize) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, dwSize);

		#endregion

		public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
		                                      int     dwSize)
		{
			var hProc = Native.OpenProcess(proc);

			// Write the memory
			bool ok = (Native.WriteProcMemoryInternal(hProc, ptrBase.Address, ptrBuffer.Address,
			                                          dwSize, out int numberOfBytesWritten));


			Trace.Assert(numberOfBytesWritten == dwSize && ok);


			// Close the handle
			Native.CloseHandle(hProc);
		}

		public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, byte[] value)
		{
			fixed (byte* rg = value) {
				WriteProcessMemory(proc, ptrBase, (IntPtr) rg, value.Length);
			}
		}

		#endregion

		#endregion
	}

	/// <summary>
	///     Offset options for <see cref="Mem.AddressOfHeap{T}(T,OffsetOptions)" />
	/// </summary>
	public enum OffsetOptions
	{
		/// <summary>
		///     Return the pointer offset by <c>-</c><see cref="Size" />,
		///     so it points to the object's <see cref="ObjHeader" />.
		/// </summary>
		Header,

		/// <summary>
		///     If the type is a <see cref="string" />, return the
		///     pointer offset by <see cref="Assets.OffsetToStringData" /> so it
		///     points to the string's characters.
		///     <remarks>
		///         Note: Equal to <see cref="GCHandle.AddrOfPinnedObject" /> and <c>fixed</c>.
		///     </remarks>
		/// </summary>
		StringData,

		/// <summary>
		///     If the type is an array, return
		///     the pointer offset by <see cref="Assets.OffsetToArrayData" /> so it points
		///     to the array's elements.
		///     <remarks>
		///         Note: Equal to <see cref="GCHandle.AddrOfPinnedObject" /> and <c>fixed</c>
		///     </remarks>
		/// </summary>
		ArrayData,

		/// <summary>
		///     If the type is a reference type, return
		///     the pointer offset by <see cref="Size" /> so it points
		///     to the object's fields.
		/// </summary>
		Fields,

		/// <summary>
		///     Don't offset the heap pointer at all, so it
		///     points to the <see cref="TypeHandle"/>
		/// </summary>
		None
	}

	/// <summary>
	///     <para>Helper class to assist with unsafe pinning of arbitrary objects. The typical usage pattern is:</para>
	///     <code>
	///  fixed (byte* pData = &amp;GetPinHelper(value).Data)
	///  {
	///  }
	///  </code>
	///     <remarks>
	///         <para><c>pData</c> is what <c>Object::GetData()</c> returns in VM.</para>
	///         <para><c>pData</c> is also equal to offsetting the pointer by <see cref="OffsetOptions.Fields" />. </para>
	///         <para>From <see cref="SOURCE" />. </para>
	///     </remarks>
	/// </summary>
	[UsedImplicitly]
	public sealed class PinHelper
	{
		private const string SOURCE = "System.Runtime.CompilerServices.JitHelpers";

		/// <summary>
		///     Represents the first field in an object, such as <see cref="OffsetOptions.Fields" />.
		/// </summary>
		public byte Data;

		private PinHelper() { }
	}
}