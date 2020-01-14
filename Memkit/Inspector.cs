using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Memkit
{
	public static class Inspector
	{
		public static bool IsNil<T>([CanBeNull] T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default);
		}

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
		internal static bool IsCompileStruct<T>() => typeof(T).IsValueType;

		/// <summary>
		///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="Array" />.
		/// </summary>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <typeparamref name="T" /> is an <see cref="Array" />; <c>false</c> otherwise</returns>
		internal static bool IsCompileArray<T>() => typeof(T).IsArray || typeof(T) == typeof(Array);

		/// <summary>
		///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="string" />.
		/// </summary>
		/// <typeparam name="T">Type to test</typeparam>
		/// <returns><c>true</c> if <typeparamref name="T" /> is a <see cref="string" />; <c>false</c> otherwise</returns>
		internal static bool IsCompileString<T>() => typeof(T) == typeof(string);
	}
}