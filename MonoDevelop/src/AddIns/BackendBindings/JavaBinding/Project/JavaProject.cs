// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;

namespace JavaBinding
{
	
	/// <summary>
	/// This class describes a Java project and it compilation options.
	/// </summary>
	public class JavaProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return JavaLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new JavaCompilerParameters();
		}
		
		public JavaProject()
		{
		}
		
		public JavaProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name = info.ProjectName;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				XmlElement el = projectOptions;
				
				foreach (JavaCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath;
					parameter.OutputAssembly  = Name;
					
					if (el != null) {
						if (el.Attributes["MainClass"] != null) {
							parameter.MainClass = el.Attributes["MainClass"].InnerText;
						}
						if (el.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(el.Attributes["PauseConsoleOutput"].InnerText);
						}
						if (el.Attributes["ClassPath"] != null) {
							parameter.ClassPath = el.Attributes["ClassPath"].InnerText;
						}
					}
				}
			}
		}
	}
}
