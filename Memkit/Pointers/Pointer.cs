﻿#nullable enable
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Memkit.Utilities;
#pragma warning disable 8603,8653
namespace Memkit.Pointers
{
	/// <summary>
	///     <para>Represents a native pointer. Equals the size of <see cref="P:System.IntPtr.Size" />.</para>
	///     <para>Can be represented as a native pointer in memory. </para>
	///     <para>
	///         Supports pointer arithmetic, reading/writing different any type, and more.
	///     </para>
	///     <list type="bullet">
	///         <item>
	///             <description>No bounds checking</description>
	///         </item>
	///         <item>
	///             <description>Minimum type safety</description>
	///         </item>
	///     </list>
	/// </summary>
	/// <typeparam name="T">Pointer element type</typeparam>
	public unsafe struct Pointer<T> : IPointer<T>
	{
		/// <summary>
		/// Internal pointer value.
		/// </summary>
		private void* m_value;

		#region Properties

		/// <summary>
		///     Size of element type <typeparamref name="T" />.
		/// </summary>
		public int ElementSize => Mem.QuickSizeOf<T>();

		/// <summary>
		///     Indexes <see cref="Address" /> as a reference.
		/// </summary>
		public ref T this[int index] => ref AsRef(index);

		/// <summary>
		///     Returns the current value as a reference.
		/// </summary>
		public ref T Reference => ref AsRef();

		/// <summary>
		///     Dereferences the pointer as the specified type.
		/// </summary>
		public T Value {
			get => Read();
			set => Write(value);
		}

		/// <summary>
		///     Address being pointed to.
		/// </summary>
		public IntPtr Address {
			get => (IntPtr) m_value;
			set => m_value = (void*) value;
		}

		/// <summary>
		///     Whether <see cref="Address" /> is <c>null</c> (<see cref="IntPtr.Zero" />).
		/// </summary>
		public bool IsNull => this == Mem.Nullptr;

		#endregion

		#region Constructors

		public Pointer(void* value)
		{
			m_value = value;
		}

		public Pointer(IntPtr value) : this(value.ToPointer()) { }

		#endregion

		/// <summary>
		/// Default offset for <see cref="Pointer{T}"/>
		/// </summary>
		private const int OFFSET = 0;

		/// <summary>
		/// Default increment/decrement/element count for <see cref="Pointer{T}"/>
		/// </summary>
		private const int ELEM_CNT = 1;

		#region Implicit / explicit conversions

		public static explicit operator Pointer<T>(ulong ul) => new Pointer<T>((void*) ul);

		public static explicit operator IntPtr(Pointer<T> ptr) => ptr.Address;

		public static explicit operator void*(Pointer<T> ptr) => ptr.ToPointer();

		public static explicit operator long(Pointer<T> ptr) => ptr.ToInt64();

		public static explicit operator ulong(Pointer<T> ptr) => ptr.ToUInt64();

		public static explicit operator Pointer<byte>(Pointer<T> ptr) => ptr.ToPointer();

		public static explicit operator Pointer<T>(long value) => new Pointer<T>((void*) value);

		public static implicit operator Pointer<T>(void* value) => new Pointer<T>(value);

		public static implicit operator Pointer<T>(IntPtr value) => new Pointer<T>(value);

		public static implicit operator Pointer<T>(Pointer<byte> ptr) => ptr.Address;

		#endregion

		#region Equality operators

		/// <summary>
		///     Checks to see if <see cref="other" /> is equal to the current instance.
		/// </summary>
		/// <param name="other">Other <see cref="Pointer{T}" />.</param>
		/// <returns></returns>
		public bool Equals(Pointer<T> other) => Address == other.Address;

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is Pointer<T> pointer && Equals(pointer);
		}

		public object CastAny(Type type)
		{
			var cast = GetType().GetMethods().First(f => f.Name == nameof(Cast) && f.IsGenericMethod);
			var ptr  = Reflector.CallGeneric(cast, type, this);
			return ptr;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			return unchecked((int) (long) m_value);
		}

		public static bool operator ==(Pointer<T> left, Pointer<T> right) => left.Equals(right);

		public static bool operator !=(Pointer<T> left, Pointer<T> right) => !left.Equals(right);

		#endregion

		#region Arithmetic

		/// <summary>
		///     Increment <see cref="Address" /> by the specified number of bytes
		/// </summary>
		/// <param name="byteCnt">Number of bytes to add</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="byteCnt"/> bytes added
		/// </returns>
		[Pure]
		public Pointer<T> Add(long byteCnt = 1)
		{
			long val = ToInt64() + byteCnt;
			return (void*) val;
		}


		/// <summary>
		///     Decrement <see cref="Address" /> by the specified number of bytes
		/// </summary>
		/// <param name="byteCnt">Number of bytes to subtract</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="byteCnt"/> bytes subtracted
		/// </returns>
		[Pure]
		public Pointer<T> Subtract(long byteCnt = 1) => Add(-byteCnt);

		public static Pointer<T> operator +(Pointer<T> left, long right) =>
			(void*) (left.ToInt64() + right);

		public static Pointer<T> operator -(Pointer<T> left, long right) =>
			(void*) (left.ToInt64() - right);

		public static Pointer<T> operator +(Pointer<T> left, Pointer<T> right) =>
			(void*) (left.ToInt64() + right.ToInt64());

		public static Pointer<T> operator -(Pointer<T> left, Pointer<T> right) =>
			(void*) (left.ToInt64() - right.ToInt64());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void* Offset(int elemCnt) => (void*) ((long) m_value + (Mem.FlatSize(ElementSize, elemCnt)));

		[Pure]
		public Pointer<T> AddressOfIndex(int index) => Offset(index);

		#region Operators

		/// <summary>
		///     Increment <see cref="Address" /> by the specified number of elements
		/// </summary>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="elemCnt"/> elements incremented
		/// </returns>
		[Pure]
		public Pointer<T> Increment(int elemCnt = ELEM_CNT) => Offset(elemCnt);


		/// <summary>
		///     Decrement <see cref="Address" /> by the specified number of elements
		/// </summary>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="elemCnt"/> elements decremented
		/// </returns>
		[Pure]
		public Pointer<T> Decrement(int elemCnt = ELEM_CNT) => Increment(-elemCnt);

		/// <summary>
		///     Increments the <see cref="Address" /> by the specified number of elements.
		///     <remarks>
		///         Equal to <see cref="Pointer{T}.Increment" />
		///     </remarks>
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <param name="i">Number of elements (<see cref="ElementSize" />)</param>
		public static Pointer<T> operator +(Pointer<T> ptr, int i) => ptr.Increment(i);

		/// <summary>
		///     Decrements the <see cref="Address" /> by the specified number of elements.
		///     <remarks>
		///         Equal to <see cref="Pointer{T}.Decrement" />
		///     </remarks>
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <param name="i">Number of elements (<see cref="ElementSize" />)</param>
		public static Pointer<T> operator -(Pointer<T> ptr, int i) => ptr.Decrement(i);

		/// <summary>
		///     Increments the <see cref="Pointer{T}" /> by one element.
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <returns>The offset <see cref="Address" /></returns>
		public static Pointer<T> operator ++(Pointer<T> ptr) => ptr.Increment();

		/// <summary>
		///     Decrements the <see cref="Pointer{T}" /> by one element.
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <returns>The offset <see cref="Address" /></returns>
		public static Pointer<T> operator --(Pointer<T> ptr) => ptr.Decrement();

		public static bool operator >(Pointer<T>  ptr, Pointer<T> b) => ptr.ToInt64() > b.ToInt64();
		public static bool operator >=(Pointer<T> ptr, Pointer<T> b) => ptr.ToInt64() >= b.ToInt64();

		public static bool operator <(Pointer<T>  ptr, Pointer<T> b) => ptr.ToInt64() < b.ToInt64();
		public static bool operator <=(Pointer<T> ptr, Pointer<T> b) => ptr.ToInt64() <= b.ToInt64();

		#endregion

		#endregion

		#region Read / write

		
		/// <summary>
		///     Writes a value of type <typeparamref name="T" /> to <see cref="Address" />.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		public void Write(T value, int elemOffset = OFFSET) => Unsafe.Write(Offset(elemOffset), value);


		/// <summary>
		///     Reads a value of type <typeparamref name="T" /> from <see cref="Address" />.
		/// </summary>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		/// <returns>The value read from the offset <see cref="Address" />.</returns>
		[Pure]
		public T Read(int elemOffset = OFFSET) => Unsafe.Read<T>(Offset(elemOffset));

		/*[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ReadFast(int elemOfs)
		{
			IL.Emit.Ldarg(nameof(elemOfs));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Mul();
//			IL.Push(ref this);
			IL.Emit.Ldarg_0();
			IL.Emit.Ldfld(new FieldRef(typeof(Pointer<T>), nameof(m_value)));
			IL.Emit.Add();
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}*/

		/// <summary>
		///     Reinterprets <see cref="Address" /> as a reference to a value of type <typeparamref name="T" />.
		/// </summary>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		/// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
		[Pure]
		public ref T AsRef(int elemOffset = OFFSET) => ref Unsafe.AsRef<T>(Offset(elemOffset));

		/// <summary>
		/// Zeros <paramref name="elemCnt"/> elements.
		/// </summary>
		/// <param name="elemCnt">Number of elements to zero</param>
		public void Clear(int elemCnt = ELEM_CNT)
		{
			for (int i = 0; i < elemCnt; i++)
				this[i] = default;
		}

		/// <summary>
		///     Writes all elements of <paramref name="rg" /> to the current pointer.
		/// </summary>
		/// <param name="rg">Values to write</param>
		public void WriteAll(T[] rg)
		{
			for (int j = 0; j < rg.Length; j++) {
				this[j] = rg[j];
			}
		}

		
		#region Any

		
		private MethodInfo GetMethod(Type t, string name, out object ptr)
		{
			ptr = CastAny(t);
			var fn = ptr.GetType().GetMethod(name);
			
			return fn;
		}

		public void WriteAny(Type type, object value, int elemOffset = OFFSET)
		{
			var fn = GetMethod(type, nameof(Write), out var ptr);
			fn.Invoke(ptr, new[] {value, elemOffset});
		}

		public object ReadAny(Type type, int elemOffset = OFFSET)
		{
			var fn = GetMethod(type, nameof(Read), out var ptr);
			return fn.Invoke(ptr, new object[] {elemOffset});
		}

		#endregion

		#region Pointer

		[Pure]
		public Pointer<byte> ReadPointer(int elemOffset = OFFSET) =>
			ReadPointer<byte>(elemOffset);

		[Pure]
		public Pointer<TType> ReadPointer<TType>(int elemOffset = OFFSET) =>
			Cast<Pointer<TType>>().Read(elemOffset);

		public void WritePointer<TType>(Pointer<TType> ptr, int elemOffset = OFFSET) =>
			Cast<Pointer<TType>>().Write(ptr, elemOffset);

		#endregion

		#endregion

		#region Copy

		/// <summary>
		///     Copies <paramref name="elemCnt" /> elements into an array of type <typeparamref name="T" />,
		///     starting from index <paramref name="startIndex" />
		/// </summary>
		/// <param name="startIndex">Index to begin copying from</param>
		/// <param name="elemCnt">Number of elements to copy</param>
		/// <returns>
		///     An array of length <paramref name="elemCnt" /> of type <typeparamref name="T" /> copied from
		///     the current pointer
		/// </returns>
		[Pure]
		public T[] Copy(int startIndex, int elemCnt)
		{
			var rg = new T[elemCnt];
			for (int i = startIndex; i < elemCnt + startIndex; i++)
				rg[i - startIndex] = this[i];

			return rg;
		}

		/// <summary>
		///     Copies <paramref name="elemCnt" /> elements into an array of type <typeparamref name="T" />,
		///     starting from index 0.
		/// </summary>
		/// <param name="elemCnt">Number of elements to copy</param>
		/// <returns>
		///     An array of length <paramref name="elemCnt" /> of type <typeparamref name="T" /> copied from
		///     the current pointer
		/// </returns>
		[Pure]
		public T[] Copy(int elemCnt) => Copy(0, elemCnt);

		#endregion

		#region Cast

		/// <summary>
		///     Creates a new <see cref="Pointer{T}" /> of type <typeparamref name="TNew" />, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <typeparam name="TNew">Type to point to</typeparam>
		/// <returns>A new <see cref="Pointer{T}" /> of type <typeparamref name="TNew" /></returns>
		public Pointer<TNew> Cast<TNew>() => m_value;

		/// <summary>
		///     Creates a new <see cref="Pointer{T}" /> of type <see cref="Byte"/>, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <returns>A new <see cref="Pointer{T}" /> of type <see cref="Byte"/></returns>
		public Pointer<byte> Cast() => Cast<byte>();

		/// <summary>
		///     Creates a native pointer of type <typeparamref name="TUnmanaged"/>, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <returns>A native pointer of type <typeparamref name="TUnmanaged"/></returns>
		[Pure]
		public TUnmanaged* ToPointer<TUnmanaged>() where TUnmanaged : unmanaged => (TUnmanaged*) m_value;

		/// <summary>
		///     Creates a native <c>void*</c> pointer, pointing to <see cref="Address" />
		/// </summary>
		/// <returns>A native <c>void*</c> pointer</returns>
		[Pure]
		public void* ToPointer() => m_value;
		

		[Pure]
		public ulong ToUInt64() => (ulong) m_value;

		[Pure]
		public long ToInt64() => (long) m_value;

		[Pure]
		public int ToInt32() => (int) m_value;

		[Pure]
		public uint ToUInt32() => (uint) m_value;

		#endregion


		public override string ToString()
		{
			const string HEX = "X";

			return Address.ToInt64().ToString(HEX);
		}
	}
}