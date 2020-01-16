using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memkit.Interop;

namespace Memkit.Code
{
	public sealed class Assembler
	{
		// https://github.com/Decimation/RazorSharp/commit/c814dceeb48b22190c344742d6c2248dfcfc18bc
		// https://github.com/Squalr/Squalr
		// https://www.nasm.us/xdoc/2.11.02/html/nasmdoc2.html#section-2.1.3
		// https://sourceware.org/binutils/docs-2.33.1/binutils/objdump.html
		// https://linux.die.net/man/1/objdump

		// https://github.com/defuse/defuse.ca/blob/master/src/libs/Assembler.php
		// https://github.com/defuse/defuse.ca/blob/master/src/pages/services/online-x86-assembler.php

		public Assembler() { }


		private static QuickFile AllocAssemblyFile(string asm)
		{
			var sb = new StringBuilder();
			sb.AppendLine(".intel_syntax noprefix");
			sb.AppendLine("_main:");
			sb.AppendLine();
			sb.AppendLine(asm);
			sb.AppendLine();

			asm = sb.ToString();

			var qf = QuickFile.Alloc("__ASM.s");
			qf.WriteAllText(asm);

			return qf;
		}

		private static QuickFile AllocObjectFile()
		{
			return QuickFile.Alloc("__ASMOBJ.OBJ");
		}

		/// <summary>
		/// Convert assembly language to machine code.
		/// </summary>
		public AssemblyResult Assemble(string asm)
		{
			// gcc -m64 -c asm.s -o objfile.txt 2>&1

			using var asmFile = AllocAssemblyFile(asm);
			using var objFile = AllocObjectFile();


			string gccCmd = String.Format("gcc -m64 -c \"{0}\" -o \"{1}\" 2>&1",
			                              asmFile.FullName, objFile.FullName);
			var gccOutput = Cli.ShellOutput(gccCmd);


			// disasm with code from assemble():
			// objdump -z -M intel -d objfile.txt

			string objCmd = String.Format("objdump -z -M intel -d {0}", objFile.FullName);

			var objOutput = Cli.ShellOutput(objCmd).CleanOutput;

			var instructions = new List<Instruction>();

			const int SKIP_LINES = 3;
			foreach (string s in objOutput.Skip(SKIP_LINES)) {
				instructions.Add(Instruction.Parse(s));
			}

			var result = new AssemblyResult(asm, objOutput, instructions.ToArray());


			return result;
		}

		// objdump -b binary -m $this->arch -M intel -D $binary_path
		// objdump -b binary -m i386:x86-64 -M intel -D $binary_path
		// objcopy -O binary -j .text [in] [out]
	}
}