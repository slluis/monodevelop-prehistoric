using System;

//
// ILogger.cs: Interface for a logger.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	public interface ILogger
	{
		void Initialize (IEventSource eventSource);
		void Shutdown ();

		string Parameters
		{
			get;
			set;
		}

		LoggerVerbosity Verbosity
		{
			get;
			set;
		}
	}
}