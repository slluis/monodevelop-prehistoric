// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <owner name="Lluis Sanchez" email="lluis@novell.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using MonoDevelop.Services;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;

using Gtk;
using Pango;

namespace MonoDevelop.EditorBindings.Gui.Pads
{	
	public class OutputProgressMonitor : BaseProgressMonitor
	{
		DefaultMonitorPad outputPad;
		
		public OutputProgressMonitor (DefaultMonitorPad pad, string title, string icon)
		{
			outputPad = pad;
			outputPad.BeginProgress (title);
		}
		
		[AsyncDispatch]
		public override void BeginTask (string name, int totalWork)
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.BeginTask (name, totalWork);
			base.BeginTask (name, totalWork);
		}
		
		[AsyncDispatch]
		public override void EndTask ()
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.EndTask ();
			base.EndTask ();
		}
		
		[AsyncDispatch]
		protected override void OnWriteLog (string text)
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.WriteText (text);
		}
		
		protected override void OnCompleted ()
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.WriteText ("\n");
			
			foreach (string msg in SuccessMessages)
				outputPad.WriteText (msg + "\n");
			
			foreach (string msg in Warnings)
				outputPad.WriteText (msg + "\n");
			
			foreach (string msg in Errors)
				outputPad.WriteText (msg + "\n");
			
			outputPad.EndProgress ();
			base.OnCompleted ();
			
			Runtime.TaskService.ReleasePad (outputPad);
			outputPad = null;
		}
		
		Exception GetDisposedException ()
		{
			return new InvalidOperationException ("Output progress monitor already disposed.");
		}
	}
}
