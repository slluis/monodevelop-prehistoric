// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;
using System.Collections.Specialized;

namespace MonoDevelop.Internal.Parser
{
	public interface ICompilationUnitBase
	{
		bool ErrorsDuringCompile {
			get;
			set;
		}
		
		object Tag {
			get;
			set;
		}
	}
}
