using System;
using System.Runtime.InteropServices;
using Memkit.Utilities;

namespace Memkit.Interop
{
	public static partial class Native
	{
		[DllImport(KERNEL32_DLL, SetLastError = true, PreserveSig = true, EntryPoint = nameof(CloseHandle))]
		private static extern bool CloseHandleInternal(IntPtr obj);

		[DllImport(KERNEL32_DLL, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = nameof(OpenProcess))]
		private static extern IntPtr
			OpenProcessInternal(ProcessAccess desiredAccess, bool inheritHandle, int processId);

		[DllImport(KERNEL32_DLL, SetLastError = true, EntryPoint = nameof(GetCurrentProcess))]
		private static extern IntPtr GetCurrentProcessInternal();


		[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualQuery))]
		private static extern IntPtr VirtualQueryInternal(IntPtr              address,
		                                                  ref MemoryBasicInfo buffer, int length);

		[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualProtect))]
		private static extern bool VirtualProtectInternal(IntPtr           address,    int                  size,
		                                                  MemoryProtection newProtect, out MemoryProtection oldProtect);


		[DllImport(KERNEL32_DLL, CharSet = CharSet.Auto, EntryPoint = nameof(GetModuleHandle))]
		private static extern IntPtr GetModuleHandleInternal(string moduleName);

		[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(GetProcAddress))]
		private static extern IntPtr GetProcAddressInternal(IntPtr module, string procName);

		[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(LoadLibrary))]
		private static extern IntPtr LoadLibraryInternal(string name);


		[DllImport(KERNEL32_DLL, EntryPoint = nameof(Mem.ReadProcessMemory))]
		internal static extern bool ReadProcMemoryInternal(IntPtr proc, IntPtr  baseAddr, IntPtr buffer,
		                                                   int    size, out int numBytesRead);


		[DllImport(KERNEL32_DLL, SetLastError = true, EntryPoint = nameof(Mem.WriteProcessMemory))]
		internal static extern bool WriteProcMemoryInternal(IntPtr proc, IntPtr  baseAddr, IntPtr buffer,
		                                                    int    size, out int numberBytesWritten);
	}
}