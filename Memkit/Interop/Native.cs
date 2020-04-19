using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Memkit.Interop.Memkit.Interop;
using Memkit.Model;
using Memkit.Pointers;

namespace Memkit.Interop
{
	public static partial class Native
	{
		public const string KERNEL32_DLL = "Kernel32.dll";

		/// <summary>
		///     https://github.com/Microsoft/DbgShell/blob/master/DbgProvider/internal/Native/DbgHelp.cs
		/// </summary>
		public const string DBGHELP_DLL = "DbgHelp.dll";

		public const string DLL_EXT = ".dll";

		public const string PDB_EXT = ".pdb";

		public const string CMD_EXE = "cmd.exe";

		public const string SYMCHK_EXE = "symchk";


		public static CONTEXT GetContext(int id)
		{
			IntPtr pOpenThread = Native.OpenThread(ThreadAccess.THREAD_HIJACK, false, (uint) id);
			SuspendThread(pOpenThread);

			// Get thread context
			CONTEXT tContext = new CONTEXT();
			tContext.ContextFlags = (uint) ContextFlags.CONTEXT_FULL;
			if (GetThreadContext(pOpenThread, ref tContext)) {
				throw new Win32Exception("Could not get context");
			}

			ResumeThread(pOpenThread);

			return tContext;
		}


		public static Pointer<byte> GetProcAddress(Pointer<byte> mod, string name)
		{
			return GetProcAddressInternal(mod.Address, name);
		}

		public static Pointer<byte> LoadLibrary(string name)
		{
			return LoadLibraryInternal(name);
		}

		public static Pointer<byte> GetModuleHandle(string name)
		{
			return GetModuleHandleInternal(name);
		}

		public static Pointer<byte> GetCurrentProcess()
		{
			return GetCurrentProcessInternal();
		}


		public static void CloseHandle(Pointer<byte> ptr)
		{
			bool b = CloseHandleInternal(ptr.Address);
			
			/*if (b) {
				throw new Win32Exception();
			}*/
		}

		public static MemoryBasicInfo VirtualQuery(Pointer<byte> ptr)
		{
			var mbi     = new MemoryBasicInfo();
			var bufSize = VirtualQueryInternal(ptr.Address, ref mbi, Marshal.SizeOf<MemoryBasicInfo>());
			return mbi;
		}

		

		public static IntPtr OpenProcess(Process proc, ProcessAccess flags = ProcessAccess.All) =>
			OpenProcessInternal(flags, false, proc.Id);

		public static IntPtr OpenCurrentProcess(ProcessAccess flags = ProcessAccess.All) =>
			OpenProcess(Process.GetCurrentProcess(), flags);
	}


	
}