using System;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class PathNotCodeCompletionDatabaseException : ApplicationException
	{
		public PathNotCodeCompletionDatabaseException(string message) : base(message){}
		public PathNotCodeCompletionDatabaseException(string message, Exception inner) : base(message, inner){}
	}
}
