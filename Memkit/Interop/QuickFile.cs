using System;
using System.IO;

namespace Memkit.Interop
{
	/// <summary>
	/// Represents a temporary file for quick use.
	/// </summary>
	public sealed class QuickFile : IDisposable
	{
		public string FullName { get; }

		public bool IsDeleted { get; private set; }

		public QuickFile(string fullname)
		{
			FullName  = fullname;
			IsDeleted = false;
		}

		public static QuickFile Alloc(string filename = null)
		{
			return new QuickFile(FileSystem.CreateTempFile(filename));
		}

		private void AssertNotDeleted()
		{
			if (IsDeleted) {
				throw new ObjectDisposedException("File is deleted.");
			}
		}

		public void WriteAllText(string text)
		{
			AssertNotDeleted();
			File.WriteAllText(FullName, text);
		}

		public string ReadAllText()
		{
			AssertNotDeleted();
			return File.ReadAllText(FullName);
		}

		public byte[] ReadAllBytes()
		{
			AssertNotDeleted();
			return File.ReadAllBytes(FullName);
		}

		public void WriteAllBytes(byte[] bytes)
		{
			AssertNotDeleted();
			File.WriteAllBytes(FullName, bytes);
		}

		public void Dispose()
		{
			AssertNotDeleted();
			File.Delete(FullName);
			IsDeleted = true;
		}

		public override string ToString()
		{
			return FullName;
		}
	}
}