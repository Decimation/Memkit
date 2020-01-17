using System;
using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Memkit.Interop
{
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

	[Flags]
	public enum ThreadAccess : int
	{
		TERMINATE            = (0x0001),
		SUSPEND_RESUME       = (0x0002),
		GET_CONTEXT          = (0x0008),
		SET_CONTEXT          = (0x0010),
		SET_INFORMATION      = (0x0020),
		QUERY_INFORMATION    = (0x0040),
		SET_THREAD_TOKEN     = (0x0080),
		IMPERSONATE          = (0x0100),
		DIRECT_IMPERSONATION = (0x0200),
		THREAD_HIJACK        = SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT,

		THREAD_ALL = TERMINATE | SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT | SET_INFORMATION | QUERY_INFORMATION |
		             SET_THREAD_TOKEN | IMPERSONATE | DIRECT_IMPERSONATION
	}

	public enum ContextFlags : uint
	{
		CONTEXT_i386               = 0x10000,
		CONTEXT_i486               = 0x10000,             //  same as i386
		CONTEXT_CONTROL            = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
		CONTEXT_INTEGER            = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
		CONTEXT_SEGMENTS           = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
		CONTEXT_FLOATING_POINT     = CONTEXT_i386 | 0x08, // 387 state
		CONTEXT_DEBUG_REGISTERS    = CONTEXT_i386 | 0x10, // DB 0-3,6,7
		CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
		CONTEXT_FULL               = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,

		CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT |
		              CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
	}

	// x86 float save
	[StructLayout(LayoutKind.Sequential)]
	public struct FLOATING_SAVE_AREA
	{
		public uint ControlWord;

		public uint Cr0NpxState;
		public uint DataOffset;
		public uint DataSelector;
		public uint ErrorOffset;
		public uint ErrorSelector;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
		public byte[] RegisterArea;

		public uint StatusWord;
		public uint TagWord;
	}

	// x86 context structure (not used in this example)
	[StructLayout(LayoutKind.Sequential)]
	public struct CONTEXT
	{
		public uint ContextFlags; //set this to an appropriate value 

		// Retrieved by CONTEXT_DEBUG_REGISTERS 
		public uint Dr0;
		public uint Dr1;
		public uint Dr2;
		public uint Dr3;
		public uint Dr6;

		public uint Dr7;

		public uint Eax;

		// Retrieved by CONTEXT_CONTROL 
		public uint Ebp;
		public uint Ebx;
		public uint Ecx;

		// Retrieved by CONTEXT_INTEGER 
		public uint Edi;
		public uint Edx;
		public uint EFlags;
		public uint Eip;
		public uint Esi;
		public uint Esp;

		// Retrieved by CONTEXT_EXTENDED_REGISTERS 
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] ExtendedRegisters;

		// Retrieved by CONTEXT_FLOATING_POINT 
		public FLOATING_SAVE_AREA FloatSave;
		public uint               SegCs;

		public uint SegDs;
		public uint SegEs;
		public uint SegFs;

		// Retrieved by CONTEXT_SEGMENTS 
		public uint SegGs;

		public uint SegSs;
	}

	// x64 m128a
	[StructLayout(LayoutKind.Sequential)]
	public struct M128A
	{
		public ulong High;
		public long  Low;

		public override string ToString()
		{
			return string.Format("High:{0}, Low:{1}", this.High, this.Low);
		}
	}

	// x64 save format
	[StructLayout(LayoutKind.Sequential, Pack = 16)]
	public struct XSAVE_FORMAT64
	{
		public ushort ControlWord;
		public uint   DataOffset;
		public ushort DataSelector;
		public uint   ErrorOffset;
		public ushort ErrorOpcode;
		public ushort ErrorSelector;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public M128A[] FloatRegisters;

		public uint   MxCsr;
		public uint   MxCsr_Mask;
		public byte   Reserved1;
		public ushort Reserved2;
		public ushort Reserved3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
		public byte[] Reserved4;

		public ushort StatusWord;
		public byte   TagWord;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public M128A[] XmmRegisters;
	}

	// x64 context structure
	[StructLayout(LayoutKind.Sequential, Pack = 16)]
	public struct CONTEXT64
	{
		public ContextFlags ContextFlags;

		public ulong DebugControl;

		public ulong Dr0;
		public ulong Dr1;
		public ulong Dr2;
		public ulong Dr3;
		public ulong Dr6;
		public ulong Dr7;

		public XSAVE_FORMAT64 DUMMYUNIONNAME;
		public uint           EFlags;
		public ulong          LastBranchFromRip;
		public ulong          LastBranchToRip;
		public ulong          LastExceptionFromRip;
		public ulong          LastExceptionToRip;
		public uint           MxCsr;
		public ulong          P1Home;
		public ulong          P2Home;
		public ulong          P3Home;
		public ulong          P4Home;
		public ulong          P5Home;
		public ulong          P6Home;
		public ulong          R10;
		public ulong          R11;
		public ulong          R12;
		public ulong          R13;
		public ulong          R14;
		public ulong          R15;
		public ulong          R8;
		public ulong          R9;

		public ulong Rax;
		public ulong Rbp;
		public ulong Rbx;
		public ulong Rcx;
		public ulong Rdi;
		public ulong Rdx;
		public ulong Rip;
		public ulong Rsi;
		public ulong Rsp;

		public ushort SegCs;
		public ushort SegDs;
		public ushort SegEs;
		public ushort SegFs;
		public ushort SegGs;
		public ushort SegSs;

		public ulong VectorControl;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
		public M128A[] VectorRegister;
	}
}