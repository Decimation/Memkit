namespace Memkit.Model
{
	/// <summary>
	/// Represents a structure from a native source.
	/// <seealso cref="IStructure"/>
	/// <seealso cref="INativeSubclass{TSuper}"/>
	/// <seealso cref="IWrapper{TNative}"/>
	/// </summary>
	public interface INativeStructure
	{
		/// <summary>
		/// Native name of this structure.
		/// </summary>
		string NativeName { get; }
	}

	public enum NativeSource
	{
		Windows,
		Marshal,
		Clr
	}
}