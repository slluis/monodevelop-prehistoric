// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using System.CodeDom.Compiler;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Services;

using ICSharpCode.SharpRefactory.PrettyPrinter;
using ICSharpCode.SharpRefactory.Parser;

namespace MonoDevelop.Commands
{
	public class VBConvertProject : AbstractMenuCommand
	{
		void ConvertFile(string fileName, string outFile)
		{
//			Errors.count = 0;
//			Scanner.Init(fileName);
//			Parser.Parse();
//			
//			if (Errors.count > 0) {
//				MessageBox.Show("Correct source code errors in " + fileName + " first (only compileable C# source code would convert).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
//				return;
//			}
//			VBNetVisitor vbv = new VBNetVisitor();
//			vbv.Visit(Parser.compilationUnit, null);
//			StreamWriter sw = new StreamWriter(outFile);
//			sw.Write(vbv.SourceText.ToString());
//			sw.Close();
		}
		
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject != null) {
				foreach (ProjectFile file in projectService.CurrentSelectedProject.ProjectFiles) {
					ConvertFile(file.Name, @"C:\\vbout\\" + Path.GetFileName(file.Name));
				}
			}
		}
	}
}
