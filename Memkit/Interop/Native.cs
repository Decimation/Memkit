using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
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
			if (CloseHandleInternal(ptr.Address)) {
				throw new Win32Exception();
			}
		}

		public static MemoryBasicInfo VirtualQuery(Pointer<byte> ptr)
		{
			var mbi     = new MemoryBasicInfo();
			var bufSize = VirtualQueryInternal(ptr.Address, ref mbi, Marshal.SizeOf<MemoryBasicInfo>());
			return mbi;
		}

		public static void VirtualProtect(Pointer<byte>    address, int size,
		                                  MemoryProtection newProtect)
		{
			if (!VirtualProtectInternal(address.Address, size, newProtect, out var oldProtect)) {
				throw new Win32Exception();
			}
		}

		public static IntPtr OpenProcess(Process proc, ProcessAccess flags = ProcessAccess.All) =>
			OpenProcessInternal(flags, false, proc.Id);

		public static IntPtr OpenCurrentProcess(ProcessAccess flags = ProcessAccess.All) =>
			OpenProcess(Process.GetCurrentProcess(), flags);
	}

	

	

	[Flags]
	public enum ProcessAccess : uint
	{
		All                     = 0x1F0FFF,
		Terminate               = 0x000001,
		CreateThread            = 0x000002,
		VmOperation             = 0x00000008,
		VmRead                  = 0x000010,
		VmWrite                 = 0x000020,
		DupHandle               = 0x00000040,
		CreateProcess           = 0x000080,
		SetInformation          = 0x00000200,
		QueryInformation        = 0x000400,
		QueryLimitedInformation = 0x001000,
		Synchronize             = 0x00100000
	}

	public enum MachineArchitecture : ushort
	{
		/// <summary>
		/// x86
		/// </summary>
		I386 = 0x014C,

		/// <summary>
		/// Intel Itanium
		/// </summary>
		IA64 = 0x0200,

		/// <summary>
		/// x64
		/// </summary>
		AMD64 = 0x8664
	}

	[Flags]
	public enum ConsoleMode : ushort
	{
		EnableEchoInput            = 0x0004,
		EnableExtendedFlags        = 0x0080,
		EnableInsertMode           = 0x0020,
		EnableLineInput            = 0x0002,
		EnableMouseInput           = 0x0010,
		EnableProcessedInput       = 0x0001,
		EnableQuickEditMode        = 0x0040,
		EnableWindowInput          = 0x0008,
		EnableVirtualTerminalInput = 0x0200,
	}

	public enum HandleOption
	{
		StdInputHandle  = -10,
		StdOutputHandle = -11,
		StdErrorHandle  = -12
	}

	[Flags]
	public enum OutputMode : ushort
	{
		EnableProcessedOutput           = 0x0001,
		EnableWrapAtEOLOutput           = 0x0002,
		EnableVirtualTerminalProcessing = 0x0004,
		DisableNewlineAutoReturn        = 0x0008,
		EnableLVBGridWorldwide          = 0x0010,
	}
}