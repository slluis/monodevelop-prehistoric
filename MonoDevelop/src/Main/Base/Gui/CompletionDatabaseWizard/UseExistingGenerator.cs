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
	public class UseExistingDBGenerator : IDatabaseGenerator
	{
		public bool Cancelable { get { return true; } }
		public string Path;
		// changed to work during GLib.Idle
		public void Generate(IProgressMonitor progress)
		{
			progress.BeginTask(GettextCatalog.GetString ("Referencing existing database"), 2);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			if (!fileUtilityService.IsValidFileName(Path))
				throw new PathNotCodeCompletionDatabaseException(Path + " is not a valid file name\n");
			if (!Directory.Exists(Path))
				throw new PathNotCodeCompletionDatabaseException("Directory " + Path + " does not exist");
			if (!System.IO.File.Exists(fileUtilityService.GetDirectoryNameWithSeparator(Path) + "CodeCompletionProxyDataV02.bin"))
				throw new PathNotCodeCompletionDatabaseException(Path + " does not contain valid code completion data");

			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.SetProperty ("SharpDevelop.CodeCompletion.DataDirectory", Path);
			propertyService.SaveProperties ();
			progress.Worked(2, GettextCatalog.GetString ("Referenced existing database"));
		}
	}
}
