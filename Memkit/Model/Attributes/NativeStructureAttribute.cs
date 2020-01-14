using System;
using JetBrains.Annotations;

namespace Memkit.Model.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public class NativeStructureAttribute : Attribute {}
}