// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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

namespace VBBinding
{
	/// <summary>
	/// This class describes a VB.NET project and it compilation options.
	/// </summary>
	public class VBProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return VBLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new VBCompilerParameters();
		}
		
		public VBProject()
		{
		}
		
		public VBProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name              = info.ProjectName;
				
				//VBCompilerParameters debug = (VBCompilerParameters)CreateConfiguration("Debug");
				//debug.Optimize = false;
				//release.Debugmode=false;
				//Configurations.Add(debug);
				
				VBCompilerParameters release = (VBCompilerParameters)CreateConfiguration("Release");
				//release.Optimize = true;
				//release.Debugmode = false;
				//release.GenerateOverflowChecks = false;
				//release.TreatWarningsAsErrors = false;
				Configurations.Add(release);

				XmlElement el = projectOptions;
				
				foreach (VBCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
					
					if (el != null) {
						System.Console.WriteLine("ProjectOptions " + el.OuterXml);
						if (el.Attributes["Target"] != null) {
							parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), el.Attributes["Target"].InnerText);
						}
						if (el.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(el.Attributes["PauseConsoleOutput"].InnerText);
						}
					}else{
						System.Console.WriteLine("ProjectOptions XML is NULL!");
					}
				}
			}else{
				System.Console.WriteLine("NULL Projectinfo!");
			}
		}
	}
}
