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
	public class UseExistingDBGenerator : IDatabaseGenerator
	{
		public string Path;
		// changed to work during GLib.Idle
		public void Generate(IProgressMonitor progress)
		{
			progress.BeginTask("Referencing existing database", 2);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			if (!fileUtilityService.IsValidFileName(Path))
				throw new PathNotCodeCompletionDatabaseException(Path + " is not a valid file name\n");
			if (!Directory.Exists(Path))
				throw new PathNotCodeCompletionDatabaseException("Directory " + Path + " does not exist");
			if (!File.Exists(fileUtilityService.GetDirectoryNameWithSeparator(Path) + "CodeCompletionProxyDataV02.bin"))
				throw new PathNotCodeCompletionDatabaseException(Path + " does not contain valid code completion data");

			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.SetProperty ("SharpDevelop.CodeCompletion.DataDirectory", Path);
			propertyService.SaveProperties ();
			progress.Worked(2, "Referenced existing database");
		}
	}
}
