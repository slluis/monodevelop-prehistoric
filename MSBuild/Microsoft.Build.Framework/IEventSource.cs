//
// IEventSource.cs: Interface for an event source.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	public interface IEventSource
	{
		event BuildEventHandler BuildEvent;
		event BuildEventHandler BuildFinishedEvent;
		event BuildEventHandler BuildStartedEvent;
		event BuildEventHandler CommentEvent;
		event BuildEventHandler CustomEvent;
		event BuildEventHandler ErrorEvent;
		event BuildEventHandler ProjectFinishedEvent;
		event BuildEventHandler ProjectStartedEvent;
		event BuildEventHandler StatusEvent;
		event BuildEventHandler TargetFinishedEvent;
		event BuildEventHandler TargetStartedEvent;
		event BuildEventHandler TaskFinishedEvent;
		event BuildEventHandler TaskStartedEvent;
		event BuildEventHandler WarningEvent;
	}
}
