using System;
using System.IO;
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class CreateDBGenerator : CreatingGenerator, IDatabaseGenerator
	{
		public bool Cancelable { get { return true; } }
		public bool Fast;
		public void Generate(IProgressMonitor progress)
		{
			string path = this.CreateCodeCompletionDir();
			DefaultParserService parserService  = (DefaultParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(DefaultParserService));
			if (Fast) {
				parserService.GenerateCodeCompletionDatabase (path, progress);
			} else {
				parserService.GenerateCodeCompletionDatabase (path, progress);
			}
		}
	}
}
