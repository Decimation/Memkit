using System;

namespace Memkit.Pointers.ExtraPointers
{
	// todo: WIP
	
	public unsafe struct RelativeFixupPointer<T> : IRelativePointer<T> where T : unmanaged
	{
		// https://github.com/dotnet/coreclr/blob/master/src/inc/fixuppointer.h

		public ulong Value { get; }

		public T* NativeValue => (T*) Value;

		/// <summary>
		/// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		/// </summary>
		public Pointer<T> GetValue(ulong value)
		{
			const ulong FIXUP_POINTER_INDIRECTION = 1;

//			PRECONDITION(!IsNull());
//			PRECONDITION(!IsTagged(base));
//			TADDR addr = base + m_delta;
//			if ((addr & FIXUP_POINTER_INDIRECTION) != 0)
//				addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
//			return dac_cast<PTR_TYPE>(addr);

			ulong addr = value + Value;

			if ((addr & FIXUP_POINTER_INDIRECTION) != 0) {
				//addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
				// ???
				addr = *((ulong*) (addr - FIXUP_POINTER_INDIRECTION));
			}

			return (Pointer<T>) addr;
		}


		public static explicit operator void*(RelativeFixupPointer<T> p)
		{
			return (void*) p.Value;
		}

		public override string ToString()
		{
			return String.Format("{0:X} d", Value);
		}
	}
}