// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace JScriptBinding
{
	public class JScriptProject : AbstractProject
	{
		public override string ProjectType {
			get {
				return JScriptLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new JScriptCompilerParameters();
		}
		
		public JScriptProject()
		{
		}
		
		public JScriptProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name              = info.ProjectName;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (JScriptCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath;
					parameter.OutputAssembly  = Name;
				}
			}
		}
	}
}
