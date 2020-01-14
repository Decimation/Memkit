using System;
using System.Reflection;

namespace Memkit.Utilities
{
	internal static class Reflector
	{
		/// <summary>
		///     Executes a generic method
		/// </summary>
		/// <param name="method">Method to execute</param>
		/// <param name="args">Generic type parameters</param>
		/// <param name="value">Instance of type; <c>null</c> if the method is static</param>
		/// <param name="fnArgs">Method arguments</param>
		/// <returns>Return value of the method specified by <paramref name="method"/></returns>
		internal static object CallGeneric(MethodInfo method, Type[]          args,
		                                   object     value,  params object[] fnArgs)
		{
			return method.MakeGenericMethod(args).Invoke(value, fnArgs);
		}

		internal static object CallGeneric(MethodInfo method, Type            arg,
		                                   object     value,  params object[] fnArgs)
		{
			return method.MakeGenericMethod(arg).Invoke(value, fnArgs);
		}
	}
}