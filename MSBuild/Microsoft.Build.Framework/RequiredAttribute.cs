using System;

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RequiredAttribute : Attribute
	{
		public RequiredAttribute ()
		{}
	}
}
