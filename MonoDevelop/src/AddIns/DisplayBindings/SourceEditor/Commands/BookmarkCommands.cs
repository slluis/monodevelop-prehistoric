using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.AddIns.Codons;

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