//
// FileFormatManager.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Xml;
using MonoDevelop.Internal.Serialization;
using MonoDevelop.Services;

namespace MonoDevelop.Internal.Project
{
	public class PrjxFileFormat: IFileFormat
	{
		public string Name {
			get { return "MonoDevelop Project"; }
		}
		
		public string GetValidFormatName (string fileName)
		{
			return Path.ChangeExtension (fileName, ".prjx");
		}
		
		public bool CanReadFile (string file)
		{
			return string.Compare (Path.GetExtension (file), ".prjx", true) == 0;
		}
		
		public bool CanWriteFile (object obj)
		{
			return obj is Project;
		}
		
		public void WriteFile (string file, object node)
		{
			Project project = node as Project;
			if (project == null)
				throw new InvalidOperationException ("The provided object is not a Project");

			StreamWriter sw = new StreamWriter (file);
			try {
				XmlDataSerializer ser = new XmlDataSerializer (Runtime.ProjectService.DataContext);
				ser.SerializationContext.BaseFile = file;
				ser.Serialize (sw, project, typeof(Project));
			} finally {
				sw.Close ();
			}
		}
		
		public object ReadFile (string fileName)
		{
			XmlTextReader reader = new XmlTextReader (new StreamReader (fileName));
			reader.MoveToContent ();

			string version = reader.GetAttribute ("version");
			if (version == null) version = reader.GetAttribute ("fileversion");
			
			DataSerializer serializer = new DataSerializer (Runtime.ProjectService.DataContext, fileName);
			IProjectReader projectReader = null;
			
			if (version == "1.0") {
				string tempFile = Path.GetTempFileName();
				Runtime.MessageService.ShowMessage(String.Format ("Old project file format found.\n It will be automatically converted to the current format"));
				
				ConvertXml.Convert(fileName,
				                   Runtime.Properties.DataDirectory + Path.DirectorySeparatorChar +
				                   "ConversionStyleSheets" + Path.DirectorySeparatorChar +
				                   "ConvertPrjx10to11.xsl",
				                   tempFile);
				reader.Close ();
				StreamReader sr = new StreamReader (tempFile);
				string fdata = sr.ReadToEnd ();
				sr.Close ();
				File.Delete (tempFile);
				reader = new XmlTextReader (new StringReader (fdata));
				projectReader = new ProjectReaderV1 (serializer);
			}
			else if (version == "1.1") {
				projectReader = new ProjectReaderV1 (serializer);
			}
			else if (version == "2.0") {
				projectReader = new ProjectReaderV2 (serializer);
			}
			
			try {
				if (projectReader != null)
					return projectReader.ReadProject (reader);
				else
					throw new UnknownProjectVersionException (version);
			} finally {
				reader.Close ();
			}
		}
	}
	
	interface IProjectReader {
		Project ReadProject (XmlReader reader);
	}
	
	class ProjectReaderV1: XmlConfigurationReader, IProjectReader
	{
		DotNetProject project;
		string file;
		DataSerializer serializer;

		static string [] changes = new string [] { 
			"Output/executeScript", "Execution","executeScript",
			"Output/executeBeforeBuild", "Build","executeBeforeBuild",
			"Output/executeAfterBuild", "Build","executeAfterBuild",
			"runwithwarnings", "Execution","runwithwarnings",
			"CodeGeneration/runtime", "Execution","runtime",
			"CodeGeneration/includedebuginformation", "Build","debugmode",
			"CodeGeneration/target", "Build","target",
			"CompilerOptions/compilationTarget", "Build","target",
			"CompilerOptions/includeDebugInformation", "Build","debugmode"
		};
		
		public ProjectReaderV1 (DataSerializer serializer)
		{
			this.serializer = serializer;
			this.file = serializer.SerializationContext.BaseFile;
		}

		public Project ReadProject (XmlReader reader)
		{
			string langName = reader.GetAttribute ("projecttype");
			project = new DotNetProject (langName);
			project.FileName = file;
			DataItem data = (DataItem) Read (reader);
			serializer.Deserialize (project, data);
			return project;
		}
		
		protected override DataNode ReadChild (XmlReader reader, DataItem parent)
		{
			if (reader.LocalName == "Configurations")
			{
				ILanguageBinding binding = Runtime.Languages.GetBindingPerLanguageName (project.LanguageName);
				object confObj = binding.CreateCompilationParameters (null);
				Type confType = confObj.GetType ();
				DataContext prjContext = Runtime.ProjectService.DataContext;
				
				DataItem item = base.ReadChild (reader, parent) as DataItem;
				foreach (DataNode data in item.ItemData) {
					DataItem conf = data as DataItem;
					if (conf == null) continue;
					prjContext.SetTypeInfo (conf, typeof(DotNetProjectConfiguration));
					DataItem codeGeneration = conf ["CodeGeneration"] as DataItem;
					if (codeGeneration != null)
						prjContext.SetTypeInfo (codeGeneration, confType);
					Transform (conf);
				}
				return item;
			}
			return base.ReadChild (reader, parent);
		}
		
		void Transform (DataItem conf)
		{
			for (int n=0; n<changes.Length; n+=3) {
				DataNode data = conf.ItemData.Extract (changes[n]);
				if (data != null) {
					data.Name = changes [n+2];
					conf.ItemData.Add (data, changes[n+1]);
				}
			}
		}
	}


	class ProjectReaderV2: XmlConfigurationReader, IProjectReader
	{
		DataSerializer serializer;
		
		public ProjectReaderV2 (DataSerializer serializer)
		{
			this.serializer = serializer;
		}

		public Project ReadProject (XmlReader reader)
		{
			DataNode data = Read (reader);
			Project project = (Project) serializer.Deserialize (typeof(Project), data);
			project.FileName = serializer.SerializationContext.BaseFile;
			return project;
		}
	}
}
