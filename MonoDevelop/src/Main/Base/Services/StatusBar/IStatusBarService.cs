// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System.Drawing;
using ICSharpCode.SharpDevelop.Gui;
#if GTK
using Gtk;
#else
using System.Windows.Forms;
#endif

namespace ICSharpCode.SharpDevelop.Services
{
	public interface IStatusBarService
	{
		IProgressMonitor ProgressMonitor {
			get;
		}
		
		Widget Control {
			get; 
		}

		bool CancelEnabled {
			get;
			set;
		}
		
		void ShowErrorMessage(string message);
		
		void SetMessage(string message);
#if !LINUX					
		void SetMessage(Image image, string message);
#endif		
		void SetCaretPosition(int x, int y, int charOffset);
		void SetInsertMode(bool insertMode);
		
		void RedrawStatusbar();
	}
}
