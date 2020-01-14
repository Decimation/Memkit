using Memkit.Interop;

namespace Memkit.Utilities
{
	public static class EnumExtensions
	{
		// ((uThis & uFlag) == uFlag)
		// ((uThis & uFlag) != 0)
		
		public static bool HasFlagFast(this InspectionProperties value, InspectionProperties flag) => (value & flag) != 0;
		
		public static bool HasFlagFast(this ImageFileCharacteristics value, ImageFileCharacteristics flag) => (value & flag) != 0;
	}
}