//
// LoggerVerbosity.cs: enum that dictates the logger verbosity.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	public enum LoggerVerbosity
	{
		Quiet,
		Minimal,
		Normal,
		Detailed,
		Diagnostic
	}
}