using System.Collections.Generic;
using System.Diagnostics;

namespace Memkit.Interop
{
	/// <summary>
	/// Represents a result from Windows CMD.
	/// </summary>
	public readonly struct CommandResult
	{
		/// <summary>
		/// The original command issued.
		/// </summary>
		public string Command { get; }

		/// <summary>
		/// Raw output from standard output.
		/// </summary>
		public string[] Output { get; }

		/// <summary>
		/// Exit code returned by the <see cref="Process"/>
		/// </summary>
		public int ExitCode { get; }

		/// <summary>
		/// Sanitized <see cref="Output"/>
		/// </summary>
		public string[] CleanOutput { get; }

		internal CommandResult(string command, string[] output, int exitCode)
		{
			Command     = command;
			Output      = output;
			ExitCode    = exitCode;
			CleanOutput = CreateCleanOutput(output);
		}

		private static string[] CreateCleanOutput(string[] output)
		{
			var list = new List<string>(output.Length);
			foreach (string s in output) {
				if (!string.IsNullOrWhiteSpace(s)) {
					list.Add(s);
				}
			}

			return list.ToArray();
		}
	}
}