//
// Author: John Luke  <jluke@cfl.rr.com>
//
// Copyright 2004 John Luke
//

using System;
using System.Reflection;

using NUnit.Core;
using MonoDevelop.Services.Nunit;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{
	public class NunitService : AbstractService
	{
		event EventHandler FixtureAdded;
		AssemblyStore assemblyStore;

		public NunitService () : base ()
		{
		}

		public void LoadAssembly (string path)
		{
			assemblyStore = new AssemblyStore (path);
			if (FixtureAdded != null)
				FixtureAdded (this, EventArgs.Empty);
		}

		public AssemblyStore AssemblyStore
		{
			get { return assemblyStore; }
		}
	}
}

