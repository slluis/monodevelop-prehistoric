// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Resources;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using Gtk;
using Glade;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class GotoLineNumberDialog : IDisposable
	{
		public static bool IsVisible = false;
	
		[Widget] Dialog GotoLineDialog;
		[Widget] Entry line_number_entry;
		
		public GotoLineNumberDialog ()
		{
			new Glade.XML (null, "texteditoraddin.glade", "GotoLineDialog", null).Autoconnect (this);
		}
		
		public void Run ()
		{
			GotoLineDialog.ShowAll ();
			IsVisible = true;
			GotoLineDialog.Run ();
		}
		
		public void Hide ()
		{
			GotoLineDialog.Hide ();
			IsVisible = false;
		}
		
		void on_btn_close_clicked (object sender, EventArgs e)
		{
			GotoLineDialog.Hide ();
		}
		
		void on_btn_go_to_line_clicked (object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				
				if (window != null && window.ViewContent is IPositionable) {			
					int l = Math.Max (0, Int32.Parse(line_number_entry.Text) - 1);
					
					((IPositionable) window.ViewContent).JumpTo (l, 0);
				}
			} catch (Exception) {
				
			} finally {
				GotoLineDialog.Hide ();
			}
		}
		
		public void Dispose ()
		{
			if (GotoLineDialog != null) {
				GotoLineDialog.Dispose ();
				GotoLineDialog = null;
				line_number_entry = null;
				IsVisible = false;
			}
		}
	}
}
