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
//using System.Windows.Forms;
using System.Drawing; // Added
using System.ComponentModel; //Added
using System.Resources; // Added
using Gtk;
using GtkSharp;

using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.XmlForms; // Added

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
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string filename = fileUtilityService.SharpDevelopRootPath + 
			Path.DirectorySeparatorChar + "doc" +
			Path.DirectorySeparatorChar + "license.txt";
			if (fileUtilityService.TestFileExists(filename)) {
				Dialog hd = new Dialog ("GNU GENERAL PUBLIC LICENSE",  (Gtk.Window) WorkbenchSingleton.Workbench , DialogFlags.DestroyWithParent);	
				hd.SetDefaultSize (600, 400);
				hd.AddButton (Stock.Ok,(int) ResponseType.Ok);
				
				ScrolledWindow sw = new  ScrolledWindow ();
				sw.SetPolicy (PolicyType.Automatic, Gtk.PolicyType.Automatic);
				
				TextView view = new TextView ();
				TextBuffer buffer = view.Buffer;
				StreamReader streamReader = new StreamReader(filename);
				buffer.InsertAtCursor(streamReader.ReadToEnd());

				sw.Add (view);
				hd.VBox.PackStart(sw);
				hd.ShowAll();
				hd.Run ();
				hd.Hide ();
				hd.Dispose ();
			}
		//	using (ViewGPLDialog totdd = new ViewGPLDialog()) {
		//		totdd.Owner = (Form)WorkbenchSingleton.Workbench;
		//		totdd.ShowDialog();
		//	}
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
			//using (TipOfTheDayDialog totdd = new TipOfTheDayDialog()) {
			//	totdd.Owner = (Gtk.Window)WorkbenchSingleton.Workbench;
			//	totdd.ShowDialog();
			//}
		}
	}
	
	public class AboutSharpDevelop : AbstractMenuCommand
	{
		public override void Run()
		{
			CommonAboutDialog ad = new CommonAboutDialog("About SharpDevelop", (Window) WorkbenchSingleton.Workbench, DialogFlags.DestroyWithParent);
			ad.Run ();
			ad.Hide ();
			ad.Dispose ();
		}
	}
}
