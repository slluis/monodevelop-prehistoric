// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace MonoDevelop.Internal.Project
{
	public class UnknownProjectVersionException : Exception
	{
		public UnknownProjectVersionException(string version) : base(version)
		{
		}
	}
	
}
