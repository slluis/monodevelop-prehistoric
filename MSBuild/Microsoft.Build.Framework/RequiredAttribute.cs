using System;

//
// RequiredAttribute.cs: 
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RequiredAttribute : Attribute
	{
		public RequiredAttribute ()
		{}
	}
}
