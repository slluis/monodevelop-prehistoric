using System;

using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	class QueuedSuiteFinish : QueuedEvent
    {
        SuiteFinishHandler handler;
        TestSuiteResult result;

        public QueuedSuiteFinish (SuiteFinishHandler handler, TestSuiteResult result)
        {
            this.handler = handler;
            this.result = result;
        }

        public override void DoCallback ()
        {
            handler (result);
        }
    }
}
