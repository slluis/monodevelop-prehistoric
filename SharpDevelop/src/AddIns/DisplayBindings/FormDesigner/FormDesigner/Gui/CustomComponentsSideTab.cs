// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using System.Security.Policy;
using System.Runtime.Remoting;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;

namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	
	public class CustomComponentsSideTab : SideTabDesigner
	{
		ArrayList projectAssemblies = new ArrayList();
		
		///<summary>Load an assembly's controls</summary>
		public CustomComponentsSideTab(AxSideBar sideTab, string name, IToolboxService toolboxService) : base(sideTab,name, toolboxService)
		{
			ScanProjectAssemblies();
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.EndBuild += new EventHandler(ReloadProjectAssemblies);
		}
		
		string loadingPath;
		
		byte[] GetBytes(string fileName)
		{
			FileStream fs = File.OpenRead(fileName);
			long size = fs.Length;
			byte[] outArray = new byte[size];
			fs.Read(outArray, 0, (int)size);
			fs.Close();
			return outArray;
		}
		
		Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			if (File.Exists(loadingPath + args.Name + ".exe")) {
				return Assembly.Load(GetBytes(loadingPath + args.Name + ".exe"));
			} 
			if (File.Exists(loadingPath + args.Name + ".dll")) {
				return Assembly.Load(GetBytes(loadingPath + args.Name + ".dll"));
			} 
			return null;
		}
		
		void ReloadProjectAssemblies(object sender, EventArgs e)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
			try {
				Items.Clear();
				AddDefaultItem();
				foreach (string assemblyName in projectAssemblies) {
					if ((assemblyName.EndsWith("exe") || assemblyName.EndsWith("dll")) && File.Exists(assemblyName)) {
						loadingPath = Path.GetDirectoryName(assemblyName) + Path.DirectorySeparatorChar;
						Assembly asm = Assembly.Load(Path.GetFileNameWithoutExtension(assemblyName));
						BuildToolboxFromAssembly(asm);
					}
				}
			} finally {
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				FormDesignerDisplayBindingBase formDesigner = content as FormDesignerDisplayBindingBase;
				if (formDesigner != null) {
					formDesigner.Control.Invoke(new ThreadStart(formDesigner.Reload), null);
				}
			}
		}
		
		void ScanProjectAssemblies()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			// custom user controls don't need custom images
			loadImages                     = false;
			
			ITypeResolutionService typeResolutionService = ToolboxProvider.TypeResolutionService;
			if (projectService.CurrentOpenCombine != null) {
				ArrayList projects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				foreach (ProjectCombineEntry projectEntry in projects) {
					string assemblyName = projectService.GetOutputAssemblyName(projectEntry.Project);
					projectAssemblies.Add(assemblyName);
					if ((assemblyName.EndsWith("exe") || assemblyName.EndsWith("dll")) && File.Exists(assemblyName)) {
						string shadowDir = Path.GetDirectoryName(assemblyName);
						AppDomain.CurrentDomain.SetShadowCopyFiles();
						AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true";
						AppDomain.CurrentDomain.SetupInformation.ShadowCopyDirectories = shadowDir;
						AppDomain.CurrentDomain.SetShadowCopyPath(shadowDir);
						Assembly asm = Assembly.LoadFrom(assemblyName);
						if (asm != null) {
							BuildToolboxFromAssembly(asm);
						}
					}
				}
			}
		}
	}
}
