//
// Author: John Luke  <jluke@cfl.rr.com>
//
// Copyright 2004 John Luke
//

// support class for FdoRecentFiles.cs

using System;
using MonoDevelop.Gui.Utils;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RecentItem
	{
		// must be a valid uri ex. file://
		private string uri;
		private string mime;
		// the number of seconds sinced the Epoch when the item was added to the list.
		private string timestamp;
		// may need to change to allow for > 1
		// lets wait til it's needed though
		private string group;
		
		// these 3 are required
		public RecentItem (string uri)
		{
			this.uri = uri;
			this.mime = Vfs.GetMimeType (uri);
			// FIXME 00:00:00 UTC on January 1, 1970 (Unix Epoch)
			// Now - Epoch in seconds
			this.timestamp = DateTime.Now.ToString ();
		}

		public string Mime
		{
			get { return mime; }
		}

		public string Uri
		{
			get { return uri; }
		}

		public string Timestamp
		{
			get { return timestamp; }
		}

		public string Group
		{
			get { return group; }
		}
	}
}
