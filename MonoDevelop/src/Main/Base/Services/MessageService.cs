// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This interface must be implemented by all services.
	/// </summary>
	public class MessageService : AbstractService, IMessageService
	{
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
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
		
		public void ShowError(Exception ex, string message)
		{
			string msg = String.Empty;
			
			if (message != null) {
				msg += message;
			}
			
			if (message != null && ex != null) {
				msg += "\n\n";
			}
			
			if (ex != null) {
				msg += "Exception occurred: " + ex.ToString();
			}

			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, stringParserService.Parse(msg))) {
				md.Run ();
				md.Hide ();
			}
		}
		
		public void ShowWarning(string message)
		{
			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Warning, Gtk.ButtonsType.Ok, stringParserService.Parse(message))) {
				md.Run ();
				md.Hide ();
			}
		}
		
		public void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public bool AskQuestion(string question, string caption)
		{
			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, stringParserService.Parse(question))) {
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
			return AskQuestion(stringParserService.Parse(question), stringParserService.Parse("${res:Global.QuestionText}"));
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
			using (Gtk.MessageDialog md = new Gtk.MessageDialog ((Gtk.Window) WorkbenchSingleton.Workbench, Gtk.DialogFlags.Modal | Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Info, Gtk.ButtonsType.Ok, stringParserService.Parse(message))) {
				md.Run ();
				md.Hide ();
			}
		}
	}
}
