// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public interface IDatabaseGenerator
	{
		void Generate(IProgressMonitor monitor);
	}
}
