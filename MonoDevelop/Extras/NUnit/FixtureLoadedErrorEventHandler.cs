using System;

namespace MonoDevelop.NUnit
{
	public delegate void FixtureLoadedErrorEventHandler (object sender, FixtureLoadedErrorEventArgs args);

	public class FixtureLoadedErrorEventArgs : EventArgs
	{
		string message, filename;

		public FixtureLoadedErrorEventArgs (string filename, Exception e)
		{
			this.filename = filename;
			this.message = e.Message;
		}

		public string Filename {
			get { return filename; }
		}

		public string Message {
			get { return message; }
		}
	}
}

