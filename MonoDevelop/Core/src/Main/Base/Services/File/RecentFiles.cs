/*
Copyright (c) 2004 John Luke

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

// implementation of the freedesktop.org Recent Files spec
// http://freedesktop.org/Standards/recent-file-spec/recent-file-spec-0.2.html

namespace Freedesktop.RecentFiles
{
	// FIXME: make sure locking is ok
	// ex. should we survive ctl-c in middle of read/write
	// do we fail gracefully if someone else has a lock
    public class RecentFiles
	{
		private static XmlSerializer serializer;

		// expose this so consumers can watch it with FileSystemWatcher for changes
		public static readonly string RecentFileStore = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".recently-used");

		static RecentFiles ()
		{
			serializer = new XmlSerializer (typeof (RecentFiles));
		}

		// required by serializer
		public RecentFiles ()
		{
		}

		// currently only emits on our changes
		// we should probably use FSW and send those events here
		// for now you have to do it on your own
		public event EventHandler Changed;

        [XmlElement ("RecentItem")]
        public RecentItem[] RecentItems;

		// FIXME: maybe not write until Save at the End
		public void AddItem (RecentItem item)
		{
			if (RecentItems == null)
			{
				RecentItems = new RecentItem [] {item};
				Save ();
				return;
			}

			// check for already existing URI
			// if it does update timestamp and return unchanged;
			// FIXME: but, what about new Groups? private changing?
			foreach (RecentItem ri in RecentItems)
			{
				if (item.Uri == ri.Uri)
				{
					ri.Timestamp = item.Timestamp;
					Save ();
					return;
				}
			}

			int count = RecentItems.Length;
			RecentItem[] newItems;
			if (count < 500)
			{
				newItems = new RecentItem[count + 1];
				RecentItems.CopyTo (newItems, 0);
				newItems[count + 1] = item;
			}
			else
			{
				// FIXME: crashes
				newItems = new RecentItem[500];
				// here we chop off the beginning (oldest)
				Array.Copy (RecentItems, count - 500, newItems, 0, 499);
				newItems[500] = item;
			}

			RecentItems = newItems;
			Save ();
		}

		public void Clear ()
		{
			RecentItems = new RecentItem [0];
			Save ();
		}

		public void ClearGroup (string group)
		{
			if (this.RecentItems == null)
				return;

			ArrayList list = new ArrayList ();
			foreach (RecentItem ri in RecentItems)
			{
				// FIXME: needs to Handle 2 groups becoming 1 group
				foreach (string g in ri.Groups)
				{
					if (g != group)
						list.Add (ri);
				}
			}

			RecentItem[] items = new RecentItem [list.Count];
			list.CopyTo (items);
			RecentItems = items;
			Save ();
		}

		private void ClearMissing ()
		{
			ArrayList list = new ArrayList ();
			foreach (RecentItem ri in RecentItems)
			{
				// we cant test !file:// Uris can we?
				if (!ri.Uri.StartsWith ("file://"))
					list.Add (ri);
				else if (File.Exists (ri.Uri.Substring (7)))
					list.Add (ri);
			}

			RecentItem[] items = new RecentItem [list.Count];
			list.CopyTo (items);
			RecentItems = items;
			Save ();
		}

		private void EmitChangedEvent ()
		{
			if (Changed != null)
				Changed (this, EventArgs.Empty);	
		}

		public static RecentFiles GetInstance ()
		{
			try
			{
				XmlTextReader reader = new XmlTextReader (RecentFileStore);
				RecentFiles rf = (RecentFiles) serializer.Deserialize (reader);
				reader.Close ();
				return rf;
			}
			catch (IOException e)
			{
				// FIXME: this is wrong, because if we later save it, we blow away what was there
				// somehow we should ask for the lock or wait for it...
				return new RecentFiles ();
			}
			catch
			{
				// FIXME: this is wrong, because if we later save it, we blow away what was there
				return new RecentFiles ();
			}
		}

		public RecentItem[] GetItemsInGroup (string group)
		{
			if (RecentItems == null)
				return null;

			ArrayList list = new ArrayList ();
			// disable for now
			//ClearMissing ();

			foreach (RecentItem ri in RecentItems)
			{
				foreach (string g in ri.Groups)
				{
					if (g == group)
						list.Add (ri);
				}
			}

			RecentItem[] items = new RecentItem [list.Count];
			list.CopyTo (items);
			return items;
		}

		public void RemoveItem (string uri)
		{
			ArrayList list = new ArrayList ();
			foreach (RecentItem ri in RecentItems)
			{
				if (ri.Uri != uri)
					list.Add (ri);
			}

			RecentItem[] items = new RecentItem [list.Count];
			list.CopyTo (items);
			RecentItems = items;
			Save ();
		}

		public void RenameItem (string oldUri, string newUri)
		{
			foreach (RecentItem ri in RecentItems)
			{
				if (ri.Uri == oldUri)
				{
					ri.Uri = newUri;
					ri.Timestamp = RecentItem.NewTimestamp;
					Save ();
					return;
				}
			}
		}

		// Save implies EmitChangedEvent (otherwise why would we save?)
		private void Save ()
		{
			// if we specifically set Encoding UTF 8 here it writes the BOM
			// which confuses others (egg-recent-files) I guess
			XmlTextWriter writer = new XmlTextWriter (new StreamWriter (RecentFileStore));
			writer.Formatting = Formatting.Indented;
			serializer.Serialize (writer, this);
			writer.Close ();
			EmitChangedEvent ();
		}

		public override string ToString ()
		{
			if (this.RecentItems == null)
				return "0 recent files";

			StringBuilder sb = new StringBuilder ();
			foreach (RecentItem ri in this.RecentItems)
			{
				sb.Append (ri.Uri);
				sb.Append (" ");
				sb.Append (ri.MimeType);
				sb.Append (" ");
				sb.Append (ri.Timestamp);
				sb.Append ("\n");
			}
			sb.Append (RecentItems.Length);
			sb.Append (" total recent files\n");
			return sb.ToString ();
		}
    }

    public class RecentItem
	{
        [XmlElement ("URI")]
        public string Uri;

        [XmlElement ("Mime-Type")]
        public string MimeType;

        public string Timestamp;

        public string Private;

        [System.Xml.Serialization.XmlArrayItem(ElementName="Group",IsNullable=false)]
        public string[] Groups;

		// required by serialization
		public RecentItem ()
		{
		}

		public RecentItem (string uri, string mimetype) : this (uri, mimetype, null)
		{
		}

		public RecentItem (string uri, string mimetype, string group)
		{
			Uri = uri;
			MimeType = mimetype;
			Timestamp = NewTimestamp;

			if (group != null)
			{
				this.Groups = new string[] {group};
			}
		}

		public void AddGroup (string group)
		{
			if (this.Groups == null)
			{
				Groups = new string[] {group};
				return;
			}

			// if it already has this group no need to add it
			foreach (string g in Groups)
			{
				if (g == group)
					return;
			}

			int length = this.Groups.Length + 1;
			string[] groups = new string [length];
			this.Groups.CopyTo (groups, 0);
			groups[length] = group;
		}

		public static string NewTimestamp
		{
			get {
				// from the unix epoch
				return ((int) (DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString ();
			}
		}

		// FIXME
		public void RemoveGroup (string group)
		{
			if (this.Groups == null)
				return;

			ArrayList groups = new ArrayList ();
			foreach (string g in Groups)
			{
				if (g != group)
					groups.Add (g);
			}

			string[] newGroups = new string [groups.Count];
			groups.CopyTo (newGroups, 0);
			this.Groups = newGroups;
		}

		// some apps might depend on this, even though they shouldn't
		public override string ToString ()
		{
			return this.Uri;
		}
    }
}

