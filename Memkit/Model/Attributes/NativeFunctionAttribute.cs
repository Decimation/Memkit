using System;
using JetBrains.Annotations;

namespace Memkit.Model.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class NativeFunctionAttribute : Attribute {}
}