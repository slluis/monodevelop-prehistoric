using System;

//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

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