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
