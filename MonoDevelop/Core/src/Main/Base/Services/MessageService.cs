// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using MonoDevelop.Gui;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Services;

namespace MonoDevelop.Core.Services
{
	/// <summary>
	/// This interface must be implemented by all services.
	/// </summary>
	public class MessageService : AbstractService, IMessageService
	{
		StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
		
		public void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		public void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		public void ShowErrorFormatted(string formatstring, params string[] formatitems)
		{
			ShowError(null, String.Format(stringParserService.Parse(formatstring), formatitems));
		}

		private struct ErrorContainer
		{
			public Exception ex;
			public string message;

			public ErrorContainer (Exception e, string msg)
			{
				ex = e;
				message = msg;
			}
		}

		public void ShowError(Exception ex, string message)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.GuiDispatch (new StatefulMessageHandler (realShowError), new ErrorContainer (ex, message));
		}

		private void realShowError (object state)
		{
			ErrorContainer container = (ErrorContainer)state;
			string msg = String.Empty;
			
			if (container.message != null) {
				msg += container.message;
			}
			
			if (container.message != null && container.ex != null) {
				msg += "\n\n";
			}
			
			if (container.ex != null) {
				msg += "Exception occurred: " + container.ex.ToString();
			}
			Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, container.message);
			md.Response += new Gtk.ResponseHandler (OnErrorResponse);
			md.ShowAll ();
		}

		void OnErrorResponse (object o, Gtk.ResponseArgs args)
		{
			((Gtk.Dialog)o).Hide ();
		}
		
		public void ShowWarning(string message)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.GuiDispatch (new StatefulMessageHandler (realShowWarning), message);
		}

		private void realShowWarning (object state)
		{
			string message = state as string;
			Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Warning, Gtk.ButtonsType.Ok, message);
			md.Response += new Gtk.ResponseHandler (OnWarningResponse);
			md.ShowAll ();
		}

		void OnWarningResponse (object o, Gtk.ResponseArgs e)
		{
			((Gtk.Dialog)o).Hide ();
		}
		
		public void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public bool AskQuestion(string question, string caption)
		{
			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, question)) {
				int response = md.Run ();
				md.Hide ();
				
				if ((Gtk.ResponseType) response == Gtk.ResponseType.Yes)
					return true;
				else
					return false;
			}
		}
		
		public bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(stringParserService.Parse(formatstring), formatitems), caption);
		}
		
		public bool AskQuestionFormatted(string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public bool AskQuestion(string question)
		{
			return AskQuestion(stringParserService.Parse(question), GettextCatalog.GetString ("Question"));
		}
		
		public int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
		{
			// TODO
			return 0;
		}
		
		public void ShowMessage(string message)
		{
			ShowMessage(message, "MonoDevelop");
		}
		
		public void ShowMessageFormatted(string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(stringParserService.Parse(formatstring), formatitems), caption);
		}
		
		public void ShowMessage(string message, string caption)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.GuiDispatch (new StatefulMessageHandler (realShowMessage), message);
		}

		void realShowMessage (object state)
		{
			string message = state as string;
			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, message)) {
				md.Response += new Gtk.ResponseHandler(OnMessageResponse);
				md.ShowAll ();
			}
		}

		void OnMessageResponse (object o, Gtk.ResponseArgs e)
		{
			((Gtk.Dialog)o).Hide ();
		}
		
		// call this method to show a dialog and get a response value
		// returns null if cancel is selected
		public string GetTextResponse(string question, string caption, string initialValue)
		{
			string returnValue = null;
			
			using (Gtk.Dialog md = new Gtk.Dialog (caption, (Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent)) {
				// add a label with the question
				Gtk.Label questionLabel = new Gtk.Label(question);
				questionLabel.UseMarkup = true;
				questionLabel.Xalign = 0.0F;
				md.VBox.PackStart(questionLabel, true, false, 6);
				
				// add an entry with initialValue
				Gtk.Entry responseEntry = (initialValue != null) ? new Gtk.Entry(initialValue) : new Gtk.Entry();
				md.VBox.PackStart(responseEntry, false, true, 6);
				
				// add action widgets
				md.AddActionWidget(new Gtk.Button(Gtk.Stock.Cancel), Gtk.ResponseType.Cancel);
				md.AddActionWidget(new Gtk.Button(Gtk.Stock.Ok), Gtk.ResponseType.Ok);
				
				md.VBox.ShowAll();
				md.ActionArea.ShowAll();
				md.HasSeparator = false;
				md.BorderWidth = 6;
				
				int response = md.Run ();
				md.Hide ();
				
				if ((Gtk.ResponseType) response == Gtk.ResponseType.Ok) {
					returnValue =  responseEntry.Text;
				}
			}
			
			return returnValue;
		}
		
		public string GetTextResponse(string question, string caption)
		{
			return GetTextResponse(question, caption, string.Empty);
		}
	}
}
