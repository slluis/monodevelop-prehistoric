using System;

namespace MonoDevelop.NUnit
{
	delegate void FixtureAddedEventHandler (object sender, FixtureAddedEventArgs args);

	public class FixtureAddedEventArgs : EventArgs
	{
		int total, current;

		public FixtureAddedEventArgs (int current, int total)
		{
			this.total = total;
			this.current = current;
		}

		public int Current {
			get { return current; }
		}

		public int Total {
			get { return total; }
		}
	}
}

