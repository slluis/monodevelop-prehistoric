// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using MonoDevelop.Core.AddIns;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads;

namespace MonoDevelop.Commands.ClassScoutCommands
{
	public class ExportClassSignature : AbstractMenuCommand
	{
		public override void Run()
		{
			ClassScout browser = Owner as ClassScout;
			if (browser == null) {
				return;
			}
			AbstractClassScoutNode node = browser.SelectedNode as AbstractClassScoutNode;
			
			if (node != null) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowWarning("Not implemented");
			}
		}
	}

}
