using System;

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class OutputAttribute : Attribute
	{
		public OutputAttribute ()
		{}
	}
}