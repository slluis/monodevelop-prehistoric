// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Internal.Project
{
	public class AssemblyDeploy  : IDeploymentStrategy
	{
		static string[] extensions = {
			"",
			".exe",
			".dll"
		};
		
		public void DeployProject(IProject project)
		{
			if (project.DeployInformation.DeployTarget.Length == 0) {
				Runtime.MessageService.ShowError(GettextCatalog.GetString ("Can't deploy: no deployment target set"));
				return;
			}
			try {
				AbstractProjectConfiguration config = (AbstractProjectConfiguration)project.ActiveConfiguration;
				FileUtilityService fileUtilityService = Runtime.FileUtilityService;
				string assembly = fileUtilityService.GetDirectoryNameWithSeparator(config.OutputDirectory) + config.OutputAssembly;
				
				foreach (string  ext in extensions) {
					if (File.Exists(assembly + ext)) {
						File.Copy(assembly + ext, fileUtilityService.GetDirectoryNameWithSeparator(project.DeployInformation.DeployTarget) + config.OutputAssembly + ext, true);
						return;
					}
				}
				throw new Exception("Assembly not found.");
			} catch (Exception e) {
				Runtime.MessageService.ShowError(e);
			}
		}
	}
}
