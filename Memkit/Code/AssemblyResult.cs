namespace Memkit.Code
{
	public readonly struct AssemblyResult
	{
		/// <summary>
		/// The original input code to <see cref="Assembler.Assemble"/>
		/// </summary>
		public string Assembly { get; }
		
		/// <summary>
		/// Original output from object dump.
		/// </summary>
		public string[] Output { get; }
		
		/// <summary>
		/// Parsed assembly instructions.
		/// </summary>
		public Instruction[] Instructions { get; }

		public AssemblyResult(string assembly, string[] output, Instruction[] instructions)
		{
			Assembly = assembly;
			Output = output;
			Instructions = instructions;
		}
	}
}