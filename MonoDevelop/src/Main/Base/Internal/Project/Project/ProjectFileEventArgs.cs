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
		IProject project;
		ProjectFile file;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public ProjectFile ProjectFile {
			get {
				return file;
			}
		}
		
		public ProjectFileEventArgs (IProject project, ProjectFile file)
		{
			this.project = project;
			this.file = file;
		}
	}
}
