// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public interface IDatabaseGenerator
	{
		void Generate(IProgressMonitor monitor);
	}
}
