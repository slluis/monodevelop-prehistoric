using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;

namespace NemerleBinding
{
	public class NemerleProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return NemerleLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new NemerleParameters();
		}
		
		public NemerleProject()
		{
		}
		
		public NemerleProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null)
			{
				Name = info.ProjectName;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (NemerleParameters p in Configurations)
				{
					p.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + p.Name;
					p.OutputAssembly = Name;
					p.RunWithWarnings = true;
				}				
			}
		}
	}
}
