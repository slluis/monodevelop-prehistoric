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
		// The document should be stored in "~/.recently-used",

		// and it should contain no more than 500 items.
		int totalMaxLength = 500;

		// MD only wants to save last 10 in its group
		int maxLength = 10;                                            
        ArrayList lastfile = new ArrayList();
        ArrayList lastproject = new ArrayList();

		XmlDocument doc;

		public event EventHandler RecentFileChanged;
        public event EventHandler RecentProjectChanged;

		public FdoRecentFiles ()
		{
			string recentFile = Environment.GetEnvironmentVariable ("HOME");
			recentFile = Path.Combine (recentFile, ".recently_used");
			Console.WriteLine (recentFile);

			if (File.Exists (recentFile))
			{
				// use POSIX lockf ()
				doc = new XmlDocument ();
				doc.Load (recentFile);

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

			FileSystemWatcher watcher = new FileSystemWatcher (recentFile);
			watcher.Changed += new FileSystemEventHandler (OnWatcherChanged);
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
				return lastfile;
            }
        }
                                                                       
        public ArrayList RecentProjects
		{
            get
			{
                return lastproject;
            }
        }

		// new entries seem to go on top
		// but it is not explicitly stated as so
		public void AddFile (string file_uri)
		{
			// uri must be unique
			// or just update timestamp and group
			RecentItem ri = new RecentItem (file_uri);
		}

		public void AddProject (string file_uri)
		{
			// uri must be unique
			// or just update timestamp and group
			RecentItem ri = new RecentItem (file_uri);
		}

		// spec doesn't mention removal
		public void ClearFiles ()
		{
		}

		// spec doesn't mention removal
		public void ClearProjects ()
		{
		}
	}
}
