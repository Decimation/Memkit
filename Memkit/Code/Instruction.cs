using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Memkit.Code
{
	public readonly struct Instruction
	{
		/// <summary>
		/// Line from object dump.
		/// </summary>
		public string Line { get; }

		/// <summary>
		/// Binary byte representation of instruction
		/// </summary>
		public int[] Shellcode { get; }

		/// <summary>
		/// Mnemonic representation of instruction
		/// </summary>
		public string Mnemonic { get; }

		private Instruction(string line, int[] shellcode, string mnemonic)
		{
			Line      = line;
			Shellcode = shellcode;
			Mnemonic  = mnemonic;
		}

		public static Instruction Parse(string line)
		{
			// There must be a better way

			// 00000000 <_main>:
			//    0:   b8 01 00 00 00          mov    eax,0x1
			//    5:   90                      nop
			//    6:   90                      nop
			//    7:   90                      nop
			// 

			line = line.Trim();
			line = line.Split(':')[1].Trim();

			string[] split = line.Split(' ');
			var      codes = new List<int>();

			foreach (var ss in split) {
				if (Int32.TryParse(ss, NumberStyles.HexNumber, null, out int code)) {
					codes.Add(code);
				}
			}

			string lastCode      = split.Last(c => c.Length == 2);
			int    lastCodeIndex = line.LastIndexOf(lastCode, StringComparison.Ordinal);
			string mnemonic      = line.Substring(lastCodeIndex + lastCode.Length).Trim();

			return new Instruction(line, codes.ToArray(), mnemonic);
		}

		public override string ToString()
		{
			return Line;
		}
	}
}