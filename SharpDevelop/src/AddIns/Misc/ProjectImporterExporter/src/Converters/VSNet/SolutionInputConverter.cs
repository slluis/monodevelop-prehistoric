// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionConverter : AbstractInputConverter
	{
#region SolutionConverTool functions
		public static string  ProjectTitle;
		public static string  ProjectLocation;
		public static string  OutputProjectLocation;
		
		public static void WriteLine(string str)
		{
			
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			taskService.CompilerOutput += str + "\n";
			taskService.NotifyTaskChange();
		}
#endregion
		
		public override string FormatName {
			get {
				return "Visual Studio.NET 7 / 2003 Solutions";
			}
		}
				
		public override bool CanConvert(string fileName)
		{
			string upperExtension = Path.GetExtension(fileName).ToUpper();
			return upperExtension == ".SLN";
		}
		
		public override bool Convert(string inputFile, string outputPath)
		{
			ArrayList projects = ReadSolution(inputFile);
			foreach (DictionaryEntry entry in projects) {
				string projectTitle = entry.Key.ToString();
				string projectFile  = entry.Value.ToString();
				
				string inputFileName     = Path.Combine(Path.GetDirectoryName(inputFile), projectFile);
				string projectOutputPath = Path.Combine(outputPath, projectTitle);
				string projectFileName   = Path.Combine(projectOutputPath, Path.ChangeExtension(Path.GetFileName(projectFile), ".prjx"));
				
				if (!Directory.Exists(projectOutputPath)) {
					Directory.CreateDirectory(projectOutputPath);
				}
				
				ProjectTitle          = projectTitle;
				ProjectLocation       = Path.GetDirectoryName(inputFile);
				OutputProjectLocation = projectOutputPath;
				switch (Path.GetExtension(projectFile).ToUpper()) {
					case ".VBPROJ":
						ConvertProject(inputFileName, projectFileName, "VBSolutionConversion.xsl");
						break;
					case ".CSPROJ":
						ConvertProject(inputFileName, projectFileName, "CSSolutionConversion.xsl");
						break;
					default:
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError("Can't convert " + projectFile + " currently only C# and VB.NET projects are supported.");
						break;
				}
			}
			
			WriteCombine(Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(inputFile), ".cmbx")), projects);
			return true;
		}
		
		void ConvertProject(string inputFile, string outputFile, string resourceStreamFile)
		{
			XsltArgumentList xsltArgumentList = new XsltArgumentList();
			xsltArgumentList.AddExtensionObject("urn:convtool", new SolutionConversionTool());
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			ConvertXml.Convert(inputFile,
			                   new XmlTextReader(Assembly.GetCallingAssembly().GetManifestResourceStream(resourceStreamFile)),
			                   outputFile,
			                   xsltArgumentList);
			foreach (DictionaryEntry entry in SolutionConversionTool.copiedFiles) {
				string srcFile = entry.Key.ToString();
				string dstFile = entry.Value.ToString();
				if (File.Exists(srcFile)) {
					if (!Directory.Exists(Path.GetDirectoryName(dstFile))) {
						Directory.CreateDirectory(Path.GetDirectoryName(dstFile));
					}
					File.Copy(srcFile, dstFile, true);
				}
			}
			SolutionConversionTool.copiedFiles = new ArrayList();
		}
				
		ArrayList ReadSolution(string fileName)
		{
			StreamReader sr           = File.OpenText(fileName);
			Regex projectLinePattern  = new Regex("Project\\(.*\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",", RegexOptions.Compiled);
			ArrayList projects = new ArrayList();
			while (true) {
				string line = sr.ReadLine();
				if (line == null) {
					break;
				}
				Match match = projectLinePattern.Match(line);
				if (match.Success) {
					projects.Add(new DictionaryEntry(match.Result("${Title}"), match.Result("${Location}")));
				}
			}
			sr.Close();
			return projects;
		}
		
		void WriteCombine(string fileName, ArrayList projects)
		{
			StreamWriter sw = File.CreateText(fileName);
			sw.WriteLine("<Combine fileversion=\"1.0\" name=\"" + Path.GetFileNameWithoutExtension(fileName) + "\" description=\"Converted Visual Studio.NET Solution\">");
			string firstEntry = null;
			sw.WriteLine("<Entries>");
			foreach (DictionaryEntry entry in projects) {
				if (firstEntry == null) {
					firstEntry = entry.Key.ToString();
				}
				sw.WriteLine("\t<Entry filename=\".\\" + Path.Combine(entry.Key.ToString(), Path.ChangeExtension(entry.Value.ToString(), ".prjx")) + "\" />");
			}
			sw.WriteLine("</Entries>");
			sw.WriteLine("<StartMode startupentry=\"" + firstEntry + "\" single=\"True\"/>");
			sw.WriteLine("<Configurations active=\"Debug\">");
			sw.WriteLine("<Configuration name=\"Debug\">");
			foreach (DictionaryEntry entry in projects) {
				sw.WriteLine("\t<Entry name=\"" + entry.Key + "\" configurationname=\"Debug\" build=\"False\" />");
			}
			sw.WriteLine("</Configuration>");
			sw.WriteLine("<Configuration name=\"Release\">");
			foreach (DictionaryEntry entry in projects) {
				sw.WriteLine("\t<Entry name=\"" + entry.Key + "\" configurationname=\"Release\" build=\"False\" />");
			}
			sw.WriteLine("</Configuration>");
			sw.WriteLine("</Configurations>");
			sw.WriteLine("</Combine>");
			sw.Close();
		}
	}
}
