// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Internal.Project
{
	public class ScriptDeploy : IDeploymentStrategy
	{
		public void DeployProject(IProject project)
		{
			if (project.DeployInformation.DeployScript.Length == 0) {
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError(GettextCatalog.GetString ("Can't deploy: you forgot to specify a deployment script"));
				return;
			}
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
				if (fileUtilityService.TestFileExists(project.DeployInformation.DeployScript)) {
					ProcessStartInfo pInfo = new ProcessStartInfo(project.DeployInformation.DeployScript);
					pInfo.WorkingDirectory = Path.GetDirectoryName(project.DeployInformation.DeployScript);
					Process.Start(pInfo);
				}
			} catch (Exception e) {
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError(e, GettextCatalog.GetString ("Error while executing deploy script"));
			}
		}
	}
}
