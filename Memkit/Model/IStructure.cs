using Memkit.Pointers;

namespace Memkit.Model
{
	/// <summary>
	/// Represents a structure that has unique identifiable properties.
	/// <seealso cref="INativeStructure"/>
	/// <seealso cref="INativeSubclass{TSuper}"/>
	/// <seealso cref="IWrapper{TNative}"/>
	/// </summary>
	public interface IStructure
	{
		/// <summary>
		/// Name of this structure.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Offset of this structure in memory.
		/// </summary>
		int Offset { get; }

		/// <summary>
		/// Size of this structure.
		/// </summary>
		int Size { get; }

		Pointer<byte> GetAddress<T>(ref T value);

		object GetValue(object value);
	}
}