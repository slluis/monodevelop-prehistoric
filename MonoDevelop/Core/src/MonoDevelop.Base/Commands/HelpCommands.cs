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

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;

namespace MonoDevelop.Commands
{
	public enum HelpCommands
	{
		TipOfTheDay,
		About
	}
	
	public class TipOfTheDayHandler: CommandHandler
	{
		protected override void Run ()
		{
			TipOfTheDayWindow totdw = new TipOfTheDayWindow ();
			totdw.Show ();
		}
	}
		
	public class AboutHandler: CommandHandler
	{
		protected override void Run ()
		{
			using (CommonAboutDialog ad = new CommonAboutDialog ()) {
				ad.Run ();
				ad.Hide ();
			}
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
			Runtime.FileService.OpenFile (site);
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
			string file = site.StartsWith("home://") ? Runtime.FileUtilityService.GetDirectoryNameWithSeparator (Runtime.FileUtilityService.SharpDevelopRootPath) + "bin" + Path.DirectorySeparatorChar + site.Substring(7).Replace('/', Path.DirectorySeparatorChar) : site;
			try {
				Process.Start(file);
			} catch (Exception) {
				Runtime.MessageService.ShowError(String.Format (GettextCatalog.GetString ("Can not execute or view {0}\n Please check that the file exists and that you can open this file."), file));
			}
		}
	}
}
