using System;

namespace Memkit.Model
{
	/// <summary>
	/// Represents a structure or class that wraps a native structure.
	/// <seealso cref="INativeStructure"/>
	/// <seealso cref="INativeSubclass{TSuper}"/>
	/// <seealso cref="IStructure"/>
	/// </summary>
	/// <typeparam name="TNative">Structure which this object wraps</typeparam>
	public interface IWrapper<out TNative> where TNative : INativeStructure
	{
		TNative TryGetNativeValue()
		{
			throw new Exception("Cannot access the value of this structure.");
		}
	}
}