// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Security.Permissions;

using System.Resources;
using System.Windows.Forms;

using MSjogren.GacTool.FusionNative;


using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Commands;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionConversionTool
	{
		static Hashtable GacReferences = new Hashtable();
		
		static SolutionConversionTool()
		{
			GenerateGacReferences();
		}
		
		static void GenerateGacReferences()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
				
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
									
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string[] info = sb.ToString().Split(',');
				
				string aName    = info[0];
				string aVersion = info[1].Substring(info[1].LastIndexOf('=') + 1);
				GacReferences[aName] = sb.ToString();
			}
			
		}
		static string[] commonAssemblies = new string[] {
			"mscorlib",
			"Accessibility",
			"Microsoft.Vsa",
			"System.Configuration.Install",
			"System.Data",
			"System.Design",
			"System.DirectoryServices",
			"System",
			"System.Drawing.Design",
			"System.Drawing",
			"System.EnterpriseServices",
			"System.Management",
			"System.Messaging",
			"System.Runtime.Remoting",
			"System.Runtime.Serialization.Formatters.Soap",
			"System.Security",
			"System.ServiceProcess",
			"System.Web",
			"System.Web.RegularExpressions",
			"System.Web.Services",
			"System.Windows.Forms",
			"System.XML"
		};
		
		public static string GetCurrentProjectName()
		{
			return SolutionConverter.ProjectTitle;
		}
		
		public static bool ShouldGenerateReference(bool filter, string assemblyName, string hintPath)
		{
			if (filter) {
				foreach (string reference in commonAssemblies) {
					if (reference.ToUpper() == assemblyName.ToUpper()) {
						return false;
					}
				}
			}
			
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = SolutionConverter.ProjectLocation + hintPath;
				if (File.Exists(assemblyLocation)) {
					return true;
				}
			}
			
			if (!File.Exists(SolutionConverter.ProjectLocation + assemblyName)) {
				if (GacReferences[assemblyName] != null) {
					return true;
				}
			} else {
				return true;
			}
			return false;
		}
		
		public static string GenerateReferenceType(string assemblyName, string hintPath)
		{
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = SolutionConverter.ProjectLocation + hintPath;
				if (File.Exists(assemblyLocation)) {
					return "Assembly";
				}
			}
			
			if (!File.Exists(SolutionConverter.ProjectLocation + assemblyName)) {
				if (GacReferences[assemblyName] == null) {
					SolutionConverter.WriteLine("Can't find Assembly reference " + assemblyName);
				} else {
					return "Gac";
				}
			} else {
				return "Assembly";
			}
			
			SolutionConverter.WriteLine("Can't determine reference type for " + assemblyName);
			return "Assembly";
		}
		
		public static string GenerateReference(string assemblyName, string hintPath)
		{
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = SolutionConverter.ProjectLocation + hintPath;
				if (File.Exists(assemblyLocation)) {
					return hintPath;
				}
			}
			
			if (!File.Exists(SolutionConverter.ProjectLocation + assemblyName)) {
				if (GacReferences[assemblyName] == null) {
					SolutionConverter.WriteLine("Can't find Assembly reference " + assemblyName);
				} else {
					return GacReferences[assemblyName].ToString();
				}
			} else {
				return "." + Path.DirectorySeparatorChar + assemblyName;
			}
			
			SolutionConverter.WriteLine("Created illegal, empty reference (should never happen) remove manually");
			return null;
		}
		
		public static ArrayList copiedFiles = new ArrayList();
		
		public static string VerifyFileLocation(string fileLocation)
		{
			string location        = SolutionConverter.ProjectLocation + Path.DirectorySeparatorChar + fileLocation;
				
			if (fileLocation.StartsWith("..")) {
				string correctLocation = SolutionConverter.OutputProjectLocation + Path.DirectorySeparatorChar + 
				                         "MovedFiles" + Path.DirectorySeparatorChar + Path.GetFileName(fileLocation);
//				Directory.CreateDirectory(SolutionConverter.OutputProjectLocation + Path.DirectorySeparatorChar + "MovedFiles");
				try {
					if (File.Exists(correctLocation)) {
						File.Delete(correctLocation);
					}
					SolutionConverter.WriteLine("Copy file " + location + " to " + correctLocation);
					copiedFiles.Add(new DictionaryEntry(location, correctLocation));
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't copy " + location + " to " + correctLocation +"\nCheck for write permission.");
				}
				return "." + correctLocation.Substring(SolutionConverter.OutputProjectLocation.Length);
			}
			copiedFiles.Add(new DictionaryEntry(location, SolutionConverter.OutputProjectLocation + Path.DirectorySeparatorChar + fileLocation));
			return fileLocation.StartsWith(".") ? fileLocation : "." + Path.DirectorySeparatorChar + fileLocation;
		}
		
		public static string EnsureBool(string txt)
		{
			if (txt.ToUpper() == "TRUE") {
				return true.ToString();
			}
			return false.ToString();
		}
		
		public static string ImportResource(string fileName)
		{
			SolutionConverter.WriteLine("Import resource " + fileName);
			string location = SolutionConverter.ProjectLocation + Path.DirectorySeparatorChar + fileName;
			string extension = Path.GetExtension(location).ToUpper();
			if (extension == ".RESX") {
				SolutionConverter.WriteLine("Convert resource file to .resource format : " + fileName);
				Hashtable resources = new Hashtable();
				
				// read .resx file
				try {
					Stream s              = File.OpenRead(location);
					ResXResourceReader rx = new ResXResourceReader(s);
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext()) {
						if (!resources.ContainsKey(n.Key)) {
							resources.Add(n.Key, n.Value);
						}
					}
					
					rx.Close();
					s.Close();
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't read resource file " + location +"\nCheck file existance.");
				}
				
				location = Path.ChangeExtension(location, ".resources");
				fileName = Path.ChangeExtension(fileName, ".resources");
				// write .resources file
				try {
					ResourceWriter rw = new ResourceWriter(location);
					foreach (DictionaryEntry entry in resources) {
						rw.AddResource(entry.Key.ToString(), entry.Value);
					}
					rw.Generate();
					rw.Close();
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't generate resource file " + location +"\nCheck for write permission.");
				}
			} else if (extension != ".RESOURCES") {
				fileName = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + SolutionConverter.ProjectTitle + "." + Path.GetFileName(fileName);
				string outLocation =  SolutionConverter.ProjectLocation + Path.DirectorySeparatorChar + fileName;
				SolutionConverter.WriteLine("Needed to copy file " + location + " to " + outLocation);
				try {
					File.Copy(location, outLocation, true);
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't copy " + location + " to " + outLocation +"\nCheck for write permission.");
				}
			}
			
			return (fileName.StartsWith(".") ? "" : ".\\") + fileName;
		}
	}
}

