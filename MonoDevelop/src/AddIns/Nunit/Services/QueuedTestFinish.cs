using System;

using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	class QueuedTestFinish : QueuedEvent
    {
        TestFinishHandler handler;
        TestCaseResult result;

        public QueuedTestFinish (TestFinishHandler handler, TestCaseResult result)
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
