// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ViewGPLDialog 
	{
		[Glade.Widget] Gtk.TextView  view;
		[Glade.Widget] Gtk.Dialog  GPLDialog;

		public ViewGPLDialog () 
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string filename = fileUtilityService.SharpDevelopRootPath + 
			System.IO.Path.DirectorySeparatorChar + "doc" +
			System.IO.Path.DirectorySeparatorChar + "license.txt";
			
			if (fileUtilityService.TestFileExists(filename)) {
				Glade.XML gplDialog = new Glade.XML (null, "Base.glade", "GPLDialog", null);
				gplDialog.Autoconnect (this);
				GPLDialog.DefaultResponse = (int) ResponseType.Close;
				GPLDialog.TransientFor = (Gtk.Window) WorkbenchSingleton.Workbench;
 				StreamReader streamReader = new StreamReader (filename);
 				view.Buffer.Text = streamReader.ReadToEnd ();
			}
		}

		protected void OnCloseButtonClicked(object sender, EventArgs e)
		{
			GPLDialog.Hide();	
		}

	}
}
