using System;

//
// OutputAttribute.cs: No clue what this does.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class OutputAttribute : Attribute
	{
		public OutputAttribute ()
		{}
	}
}