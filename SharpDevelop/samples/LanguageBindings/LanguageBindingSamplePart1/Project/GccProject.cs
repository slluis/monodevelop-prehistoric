// GccProject.cs
// Copyright (C) 2002 Mike Krueger
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.CombineProjectLayer.Project;
using ICSharpCode.SharpDevelop.CombineProjectLayer.Attributes;

namespace GccBinding {
	
	/// <summary>
	/// This class implements a gcc project
	/// </summary>
	public class GccProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return GccLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new GccCompilerParameters();
		}
		
		
		public GccProject()
		{
		}
		
		public GccProject(ProjectCreateInformation info)
		{
			if (info != null) {
				Description       = info.Description;
				Name              = info.Name;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (GccCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory   = info.Location;
					parameter.OutputAssembly    = Name;
				}
			}
		}
	}
}
