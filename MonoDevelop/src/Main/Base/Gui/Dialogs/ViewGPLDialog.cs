// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;
using System.Resources;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ViewGPLDialog : Dialog 
	{
		public ViewGPLDialog () : base ("GNU GENERAL PUBLIC LICENSE", (Window) WorkbenchSingleton.Workbench, DialogFlags.DestroyWithParent)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string filename = fileUtilityService.SharpDevelopRootPath + 
			System.IO.Path.DirectorySeparatorChar + "doc" +
			System.IO.Path.DirectorySeparatorChar + "license.txt";
			
			if (fileUtilityService.TestFileExists(filename)) {
				this.BorderWidth = 6;
				this.DefaultResponse = (int) ResponseType.Close;
				this.HasSeparator = false;	
				this.SetDefaultSize (600, 400);
				this.AddButton (Stock.Close, (int) ResponseType.Close);
				
				ScrolledWindow sw = new  ScrolledWindow ();
				sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
				
				TextView view = new TextView ();
				view.Editable = false;
				view.CursorVisible = false;
				StreamReader streamReader = new StreamReader (filename);
				view.Buffer.Text = streamReader.ReadToEnd ();

				sw.Add (view);
				this.VBox.PackStart(sw);
				this.ShowAll ();
			}
		}
	}
}
