using System;

//
// LoadInSeperateAppDomainAttribute.cs: attribute that tells msbuild to load a class in a seperate appdomain.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LoadInSeparateAppDomainAttribute : Attribute
	{
		public LoadInSeparateAppDomainAttribute ()
		{}
	}
}
