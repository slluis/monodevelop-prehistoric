//
// ProjectFolder.cs
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
using System.Collections;

using MonoDevelop.Internal.Project;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Pads.ProjectPad
{
	public class ProjectFolder: IDisposable
	{
		string absolutePath;
		Project project;
		
		public ProjectFolder (string absolutePath, Project project)
		{
			this.project = project;
			this.absolutePath = absolutePath;
			Runtime.FileService.FileRenamed += new FileEventHandler (OnFileRenamed);
			Runtime.FileService.FileRemoved += new FileEventHandler (OnFileRemoved);
		}
		
		public string Path {
			get { return absolutePath; }
		}
		
		public string Name {
			get { return System.IO.Path.GetFileName (absolutePath); }
		}
		
		public Project Project {
			get { return project; }
		}
		
		public void Dispose ()
		{
			Runtime.FileService.FileRenamed -= new FileEventHandler (OnFileRenamed);
			Runtime.FileService.FileRemoved -= new FileEventHandler (OnFileRemoved);
		}
		
		void OnFileRemoved (object sender, FileEventArgs e)
		{
			if (!e.IsDirectory || e.TargetFile != absolutePath) return;
			
			if (FolderRemoved != null) {
				FolderRemoved(this, e);
			}
		}

		void OnFileRenamed (object sender, FileEventArgs e)
		{
			if (!e.IsDirectory || e.SourceFile != absolutePath) return;

			absolutePath = e.TargetFile;
			if (FolderRenamed != null) {
				FolderRenamed(this, e);
			}
		}

		public event FileEventHandler FolderRenamed;
		public event FileEventHandler FolderRemoved;
	}
}
