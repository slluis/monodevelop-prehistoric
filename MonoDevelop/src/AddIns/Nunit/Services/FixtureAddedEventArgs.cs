using System;

namespace MonoDevelop.Services.Nunit
{
	class FixtureAddedEventArgs : EventArgs
	{
		int total;
		int current;

		public FixtureAddedEventArgs (int current, int total)
		{
			this.total = total;
			this.current = current;
		}

		public int Total {
			get { return total; }
		}

		public int Current {
			get { return current; }
		}
	}
}
