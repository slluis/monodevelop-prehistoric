//
// Author: John Luke  <jluke@cfl.rr.com>
//
// Copyright 2004 John Luke
//

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;

// implementation of the freedesktop.org Recent Files spec
// http://freedesktop.org/Standards/recent-file-spec/recent-file-spec-0.2.html

namespace MonoDevelop.Services
{
	public class FdoRecentFiles
	{
		// MD only wants to save last 10 in its group
        ArrayList lastFiles = new ArrayList (10); // max 10
        ArrayList lastProjects = new ArrayList (10); // max 10
        ArrayList allRecents = new ArrayList (10); // max 500

		XPathDocument doc;

		public event EventHandler RecentFileChanged;
        public event EventHandler RecentProjectChanged;

		public FdoRecentFiles ()
		{
			// The document should be stored in "~/.recently-used",
			string recentFile = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), ".recently_used");
			//Console.WriteLine (recentFile);

			if (File.Exists (recentFile))
			{
				// use POSIX lockf ()
				doc = new XPathDocument (recentFile);

				XPathNavigator nav = doc.CreateNavigator ();
				XPathNodeIterator xni = nav.Select ("/RecentFiles/RecentItem");
				Console.WriteLine ("Total Items {0}", xni.Count);
			}
			else
			{
				// use POSIX lockf ()
				Console.WriteLine ("{0} does not exist.", recentFile);
				// create it
			}

			//FileSystemWatcher watcher = new FileSystemWatcher (recentFile);
			//watcher.Changed += new FileSystemEventHandler (OnWatcherChanged);
		}

		void OnWatcherChanged (object o, FileSystemEventArgs args)
		{
			// TODO
			// decide if projects or files changed or both
			Console.WriteLine ("on watcher changed");
		}

		void OnRecentFileChange ()
        {
            if (RecentFileChanged != null)
			{
                RecentFileChanged (this, null);
            }
        }
                                                                       
        void OnRecentProjectChange ()
        {
            if (RecentProjectChanged != null)
			{
                RecentProjectChanged (this, null);
            }
        }

		public ArrayList RecentFiles
		{
            get
			{
				return lastFiles;
            }
        }
                                                                       
        public ArrayList RecentProjects
		{
            get
			{
                return lastProjects;
            }
        }

		// new entries seem to go on top
		// but it is not explicitly stated as so
		public void AddFile (string file_uri)
		{
			// uri must be unique
			// or just update timestamp and group
			foreach (RecentItem recentItem in allRecents)
			{
				if (recentItem.Uri == file_uri)
				{
					recentItem.Update (false);
					lastFiles.Add (recentItem);
					return;
				}
			}

			RecentItem ri = new RecentItem (file_uri);
			ri.Group = "MonoDevelop Files";
			lastFiles.Add (ri);
		}

		public void AddProject (string file_uri)
		{
			// uri must be unique
			// or just update timestamp and group
			foreach (RecentItem recentItem in allRecents)
			{
				if (recentItem.Uri == file_uri)
				{
					recentItem.Update (true);
					lastProjects.Add (recentItem);
					return;
				}
			}

			RecentItem ri = new RecentItem (file_uri);
			ri.Group = "MonoDevelop Projects";
			lastProjects.Add (ri);
		}

		// spec doesn't mention removal
		public void ClearFiles ()
		{
			lastFiles.Clear ();
			// remove from allRecents
			// write the file
		}

		// spec doesn't mention removal
		public void ClearProjects ()
		{
			lastProjects.Clear ();
			// remove from allRecents
			// write the file
		}
	}
}
