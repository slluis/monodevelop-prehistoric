// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.Core.AddIns.Codons;

namespace Plugins.RegExpTk {
	
	public class RegExpTkCommand : AbstractMenuCommand
	{
		
		public override void Run()
		{
			using (RegExpTkDialog dialog = new RegExpTkDialog()) {
				dialog.ShowDialog();
			}
		}
	}
}
