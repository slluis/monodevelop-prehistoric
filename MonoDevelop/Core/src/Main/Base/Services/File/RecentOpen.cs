// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.IO;

using MonoDevelop.Core.Properties;
using MonoDevelop.Gui.Utils;
using MonoDevelop.Services;
using Freedesktop.RecentFiles;

namespace MonoDevelop.Services
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of MonoDevelop
	/// </summary>
	public class RecentOpen
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		const int MAX_LENGTH = 10;
		
		RecentItem[] lastfile;
		RecentItem[] lastproject;
		RecentFiles recentFiles;
		
		public event EventHandler RecentFileChanged;
		public event EventHandler RecentProjectChanged;
		
		public RecentItem[] RecentFile {
			get {
				Debug.Assert(lastfile != null, "RecentOpen : set string[] LastFile (value == null)");
				return lastfile;
			}
		}

		public RecentItem[] RecentProject {
			get {
				Debug.Assert(lastproject != null, "RecentOpen : set string[] LastProject (value == null)");
				return lastproject;
			}
		}
		
		void OnRecentFileChange()
		{
			if (RecentFileChanged != null) {
				RecentFileChanged(this, null);
			}
		}
		
		void OnRecentProjectChange()
		{
			if (RecentProjectChanged != null) {
				RecentProjectChanged(this, null);
			}
		}

		public RecentOpen()
		{
			recentFiles = RecentFiles.GetInstance ();
			UpdateLastFile ();
			UpdateLastProject ();
		}
		
		public void AddLastFile (string name)
		{
			if (lastfile != null && lastfile.Length >= MAX_LENGTH)
			{
				RecentItem oldestItem = lastfile[0];
				for (int i = 1; i < lastfile.Length - 1; i ++)
				{
					// the lowest number is the oldest
					if (lastfile[i].Timestamp < oldestItem.Timestamp)
						oldestItem = lastfile[i];
				}
				recentFiles.RemoveItem (oldestItem);
			}

			recentFiles.AddItem (new RecentItem (new Uri (name), Vfs.GetMimeType (name), "MonoDevelop Files"));
			UpdateLastFile ();
		}
		
		public void ClearRecentFiles()
		{
			lastfile = null;
			recentFiles.ClearGroup ("MonoDevelop Files");
			OnRecentFileChange();
		}
		
		public void ClearRecentProjects()
		{
			lastproject = null;
			recentFiles.ClearGroup ("MonoDevelop Projects");
			OnRecentProjectChange();
		}
		
		public void AddLastProject (string name)
		{
			if (lastproject != null && lastproject.Length >= MAX_LENGTH)
			{
				RecentItem oldestItem = lastproject[0];
				for (int i = 1; i < lastproject.Length; i ++)
				{
					// the lowest number is the oldest
					if (lastproject[i].Timestamp < oldestItem.Timestamp)
						oldestItem = lastproject[i];
				}
				recentFiles.RemoveItem (oldestItem);
			}

			recentFiles.AddItem (new RecentItem (new Uri (name), Vfs.GetMimeType (name), "MonoDevelop Projects"));
			UpdateLastProject ();
		}
		
		public void FileRemoved(object sender, FileEventArgs e)
		{
			recentFiles.RemoveItem (new Uri (e.FileName));
			UpdateLastFile ();
		}
		
		public void FileRenamed(object sender, FileEventArgs e)
		{
			recentFiles.RenameItem (new Uri (e.FileName), new Uri (e.TargetFile));
			UpdateLastFile ();
		}

		void UpdateLastFile ()
		{
			lastfile = recentFiles.GetItemsInGroup ("MonoDevelop Files");
			OnRecentFileChange();
		}

		void UpdateLastProject ()
		{
			lastproject = recentFiles.GetItemsInGroup ("MonoDevelop Projects");
			OnRecentFileChange();
		}
	}
}

