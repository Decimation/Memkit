using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Memkit.Pointers;

namespace Memkit.Utilities
{
	public static class Inspector
	{
		public static bool ImplementsGenericInterface(this Type type, Type genericType)
		{
			bool IsMatch(Type t)
			{
				return t.IsGenericType && t.GetGenericTypeDefinition() == genericType;
			}

			return type.GetInterfaces().Any(IsMatch);
		}

		public static bool ImplementsInterface(this Type type, string interfaceName) =>
			type.GetInterface(interfaceName) != null;

		/// <summary>
		///     Determines whether the value of <paramref name="value" /> is <c>default</c> or <c>null</c> bytes,
		///     or <paramref name="value" /> is <c>null</c>
		///     <remarks>"Nil" is <c>null</c> or <c>default</c>.</remarks>
		/// </summary>
		public static bool IsNil<T>([CanBeNull] T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default);
		}

		public static InspectionProperties ReadProperties(Type t)
		{
			var mp = new InspectionProperties();

			if (IsInteger(t)) {
				mp |= InspectionProperties.Integer;
			}

			if (IsReal(t)) {
				mp |= InspectionProperties.Real;
			}

			if (t.IsValueType) {
				mp |= InspectionProperties.Struct;
			}

			if (IsUnmanaged(t)) {
				mp |= InspectionProperties.Unmanaged;
			}

			if (IsEnumerableType(t)) {
				mp |= InspectionProperties.Enumerable;
			}

			if (IsAnyPointer(t)) {
				mp |= InspectionProperties.AnyPointer;
			}

			if (t.IsPointer) {
				mp |= InspectionProperties.Pointer;
			}

			return mp;
		}

		public static bool IsInteger(Type t)
		{
			return Type.GetTypeCode(t) switch
			{
				TypeCode.Byte => true,
				TypeCode.SByte => true,
				TypeCode.UInt16 => true,
				TypeCode.Int16 => true,
				TypeCode.UInt32 => true,
				TypeCode.Int32 => true,
				TypeCode.UInt64 => true,
				TypeCode.Int64 => true,
				_ => false
			};
		}

		public static bool IsReal(Type t)
		{
			return Type.GetTypeCode(t) switch
			{
				TypeCode.Decimal => true,
				TypeCode.Double => true,
				TypeCode.Single => true,
				_ => false
			};
		}

		public static bool IsAnyPointer(Type t)
		{
			// todo?
			bool isIPointer = t.ImplementsGenericInterface(typeof(IPointer<>));
			bool isIntPtr   = t == typeof(IntPtr) || t == typeof(UIntPtr);

			return t.IsPointer || isIPointer || isIntPtr;
		}

		public static bool IsEnumerableType(Type type) => type.ImplementsInterface(nameof(IEnumerable));

		#region Unmanaged

		/// <summary>
		///     Dummy class for use with <see cref="IsUnmanaged" /> and <see cref="IsUnmanaged" />
		/// </summary>
		private sealed class U<T> where T : unmanaged { }

		/// <summary>
		///     Determines whether this type fits the <c>unmanaged</c> type constraint.
		/// </summary>
		public static bool IsUnmanaged(Type t)
		{
			try {
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				typeof(U<>).MakeGenericType(t);
				return true;
			}
			catch (Exception e) {
				return false;
			}
		}

		#endregion

		/// <summary>
		///     Determines whether or not <paramref name="value" /> is a runtime value type.
		/// </summary>
		/// <param name="value">Value to test</param>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <paramref name="value" /> is a value type; <c>false</c> otherwise</returns>
		public static bool IsStruct<T>(T value) => value.GetType().IsValueType;


		/// <summary>
		///     Determines whether or not <paramref name="value" /> is a runtime <see cref="Array" />.
		/// </summary>
		/// <param name="value">Value to test</param>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <paramref name="value" /> is an <see cref="Array" />; <c>false</c> otherwise</returns>
		public static bool IsArray<T>(T value) => value is Array;

		/// <summary>
		///     Determines whether or not <paramref name="value" /> is a runtime <see cref="string" />.
		/// </summary>
		/// <param name="value">Value to test</param>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <paramref name="value" /> is a <see cref="string" />; <c>false</c> otherwise</returns>
		public static bool IsString<T>(T value) => value is string;

		/// <summary>
		///     Determines whether or not <typeparamref name="T" /> is a compile-time value type.
		/// </summary>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <typeparamref name="T" /> is a value type; <c>false</c> otherwise</returns>
		public static bool IsCompileStruct<T>() => typeof(T).IsValueType;

		/// <summary>
		///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="Array" />.
		/// </summary>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <typeparamref name="T" /> is an <see cref="Array" />; <c>false</c> otherwise</returns>
		public static bool IsCompileArray<T>() => typeof(T).IsArray || typeof(T) == typeof(Array);

		/// <summary>
		///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="string" />.
		/// </summary>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <typeparamref name="T" /> is a <see cref="string" />; <c>false</c> otherwise</returns>
		public static bool IsCompileString<T>() => typeof(T) == typeof(string);
	}

	/// <summary>
	/// Additional inspected type properties.
	/// </summary>
	[Flags]
	public enum InspectionProperties
	{
		None = 0,

		Integer = 1,

		Real = 1 << 1,

		Struct = 1 << 2,

		Pointer = 1 << 3,

		Unmanaged = 1 << 4,

		Enumerable = 1 << 5,

		AnyPointer = 1 << 6,

		Numeric = Integer & Real
	}
}