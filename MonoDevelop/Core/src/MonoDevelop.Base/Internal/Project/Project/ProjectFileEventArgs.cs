// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Internal.Project
{
	public delegate void ProjectFileEventHandler(object sender, ProjectFileEventArgs e);
	
	public class ProjectFileEventArgs : EventArgs
	{
		Project project;
		ProjectFile file;
		
		public Project Project {
			get {
				return project;
			}
		}
		
		public ProjectFile ProjectFile {
			get {
				return file;
			}
		}
		
		public ProjectFileEventArgs (Project project, ProjectFile file)
		{
			this.project = project;
			this.file = file;
		}
	}
}
