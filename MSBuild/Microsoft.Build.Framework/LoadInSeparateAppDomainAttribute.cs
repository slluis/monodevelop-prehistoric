using System;

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LoadInSeparateAppDomainAttribute : Attribute
	{
		public LoadInSeparateAppDomainAttribute ()
		{}
	}
}
