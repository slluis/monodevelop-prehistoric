// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ObjectBrowser
{
	public class ObjectBrowserCommand : ICSharpCode.Core.AddIns.Codons.AbstractMenuCommand
	{
		public override void Run() {
			DisplayInformationWrapper vw;
			
			vw = new DisplayInformationWrapper();
			
			vw.LoadStdAssemblies();
			vw.LoadRefAssemblies();
			
			WorkbenchSingleton.Workbench.ShowView(vw);
		}
			
	}
}
