using System;

namespace MonoDevelop.Services.Nunit
{
	class FixtureLoadErrorEventArgs : EventArgs
    {
        string message;
        string filename;

        public FixtureLoadErrorEventArgs (string filename, Exception e)
        {
            this.filename = filename;
            message = e.Message;
        }

        public string FileName {
            get { return filename; }
        }

        public string Message {
            get { return message; }
        }
    }
}
