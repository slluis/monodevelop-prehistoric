using System;

using Monodoc;

using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{

	public class MonodocService : AbstractService
	{

		RootTree helpTree;

		public MonodocService () : base ()
		{
			helpTree = RootTree.LoadTree ();
		}

		public RootTree HelpTree {
			get { return helpTree; }
		}

	}

}
