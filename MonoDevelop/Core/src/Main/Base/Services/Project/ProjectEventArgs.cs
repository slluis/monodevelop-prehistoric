// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Services
{
	public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);
	
	public class ProjectEventArgs : EventArgs
	{
		IProject project;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public ProjectEventArgs(IProject project)
		{
			this.project = project;
		}
	}
}
