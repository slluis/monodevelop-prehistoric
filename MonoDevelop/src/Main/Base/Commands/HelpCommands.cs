// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using Gtk;
using GtkSharp;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class ShowHelp : AbstractMenuCommand
	{
		public override void Run()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string fileName = fileUtilityService.SharpDevelopRootPath + 
			              Path.DirectorySeparatorChar + "doc" +
			              Path.DirectorySeparatorChar + "help" +
			              Path.DirectorySeparatorChar + "sharpdevelop.chm";
			//if (fileUtilityService.TestFileExists(fileName)) {
			//	Help.ShowHelp((Gtk.Window)WorkbenchSingleton.Workbench, fileName);
			//}
		}
	}
	
	public class ViewGPL : AbstractMenuCommand
	{
		public override void Run()
		{
			ViewGPLDialog vgd = new ViewGPLDialog();
			vgd.Run ();
			vgd.Hide ();
		}
	}
	
	public class GotoWebSite : AbstractMenuCommand
	{
		string site;
		
		public GotoWebSite(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(site);
		}
	}
	
	// FIXME: dont use Process Start for HTML files
	public class GotoLink : AbstractMenuCommand
	{
		string site;
		
		public GotoLink(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string file = site.StartsWith("home://") ? fileUtilityService.GetDirectoryNameWithSeparator(fileUtilityService.SharpDevelopRootPath) + "bin" + Path.DirectorySeparatorChar + site.Substring(7).Replace('/', Path.DirectorySeparatorChar) : site;
			try {
				Process.Start(file);
			} catch (Exception) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError("Can't execute/view " + file + "\n Please check that the file exists and that you can open this file.");
			}
		}
	}
	
	public class ViewTipOfTheDay : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			string didyouknowtext = resourceService.GetString("Dialog.TipOfTheDay.DidYouKnowText");	
			TipOfTheDayDialog totdd = new TipOfTheDayDialog (didyouknowtext);
			totdd.Run ();
		}
	}
	
	public class AboutSharpDevelop : AbstractMenuCommand
	{
		public override void Run()
		{
			CommonAboutDialog ad = new CommonAboutDialog ();
			ad.Run ();
			ad.Hide ();
		}
	}
}
