//
// BuildEventHandler.cs: Event handler for build events.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	public delegate void BuildEventHandler (object sender, BuildEventArgs e);
}
