using System;
using System.IO;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class CreateDBGenerator : CreatingGenerator, IDatabaseGenerator
	{
		public bool Fast;
		public void Generate(IProgressMonitor progress)
		{
			string path = this.CreateCodeCompletionDir();
			DefaultParserService parserService  = (DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DefaultParserService));
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
