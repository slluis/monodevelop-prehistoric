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
	public abstract class CreatingGenerator
	{
		public string CreateCodeCompletionDir()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string path = propertyService.ConfigDirectory + System.IO.Path.DirectorySeparatorChar + "CodeCompletionData";
			if (!Directory.Exists (path))
				Directory.CreateDirectory (path);
			propertyService.SetProperty ("SharpDevelop.CodeCompletion.DataDirectory", path);
			propertyService.SaveProperties ();
			return fileUtilityService.GetDirectoryNameWithSeparator (path);
		}
	}
}
