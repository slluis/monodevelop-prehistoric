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
		public bool Fast;
		public void Generate(IProgressMonitor progress)
		{
			string path = this.CreateCodeCompletionDir();
			DefaultParserService parserService  = (DefaultParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(DefaultParserService));
			Console.WriteLine("using path " + path);
			if (Fast) {
				Console.WriteLine("Creating DB with fast process");
				parserService.GenerateCodeCompletionDatabaseFast(path, progress);
			} else {
				Console.WriteLine("Creating DB with slow process");
				parserService.GenerateEfficientCodeCompletionDatabase(path, progress);
			}
		}
	}
}
