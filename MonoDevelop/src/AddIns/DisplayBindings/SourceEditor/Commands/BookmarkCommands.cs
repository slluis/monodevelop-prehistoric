using System;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui;
using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.EditorBindings.Commands {
	public class ToggleBookmark : AbstractMenuCommand {
		public override void Run ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null)
				return;
			
			IBookmarkOperations o = window.ViewContent as IBookmarkOperations;
			if (o == null)
				return;
			
			o.ToggleBookmark ();
		}
	}
	
	public class PrevBookmark : AbstractMenuCommand	{
		public override void Run ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null)
				return;
			
			IBookmarkOperations o = window.ViewContent as IBookmarkOperations;
			if (o == null)
				return;
			
			o.PrevBookmark ();
		}
	}
		
	public class NextBookmark : AbstractMenuCommand	{
		public override void Run ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null)
				return;
			
			IBookmarkOperations o = window.ViewContent as IBookmarkOperations;
			if (o == null)
				return;
			
			o.NextBookmark ();
		}
	}
		
	public class ClearBookmarks : AbstractMenuCommand {
		public override void Run ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null)
				return;
			
			IBookmarkOperations o = window.ViewContent as IBookmarkOperations;
			if (o == null)
				return;
			
			o.ClearBookmarks ();
		}
	}
}