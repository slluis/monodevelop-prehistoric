using System;
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class PathNotCodeCompletionDatabaseException : ApplicationException
	{
		public PathNotCodeCompletionDatabaseException(string message) : base(message){}
		public PathNotCodeCompletionDatabaseException(string message, Exception inner) : base(message, inner){}
	}
}
