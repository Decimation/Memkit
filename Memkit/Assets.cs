using System.Runtime.CompilerServices;

namespace Memkit
{
	/// <summary>
	/// Contains offsets, sizes, and other constants.
	/// </summary>
	public static class Assets
	{
		/// <summary>
		/// Common value representing an invalid value or a failure
		/// </summary>
		internal const int INVALID_VALUE = -1;

		internal const int BITS_PER_DWORD = 32;

		#region Sizes

		// https://github.com/dotnet/coreclr/blob/master/src/vm/object.h


		/// <summary>
		///     Size of the length field and padding (x64)
		/// </summary>
		public static readonly int ArrayOverhead = Mem.Size;

		#endregion

		#region Offset

		/// <summary>
		///     Heap offset to the first field.
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="Mem.Size" /> for <see cref="TypeHandle" /></description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int OffsetToData = Mem.Size;

		/// <summary>
		///     Heap offset to the first array element.
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="Mem.Size" /> for <see cref="TypeHandle" /></description>
		///         </item>
		///         <item>
		///             <description>+ 4 for length (<see cref="uint" />) </description>
		///         </item>
		///         <item>
		///             <description>+ 4 for padding (<see cref="uint" />) (x64 only)</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int OffsetToArrayData = Assets.OffsetToData + ArrayOverhead;

		/// <summary>
		///     Heap offset to the first string character.
		/// On 64 bit platforms, this should be 12 (8 + 4) and on 32 bit 8 (4 + 4).
		/// (<see cref="Mem.Size"/> + <see cref="int"/>)
		/// </summary>
		public static readonly int OffsetToStringData = RuntimeHelpers.OffsetToStringData;

		#endregion
	}
}