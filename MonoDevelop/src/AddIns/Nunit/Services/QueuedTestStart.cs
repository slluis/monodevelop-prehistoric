using System;

using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	class QueuedTestStart : QueuedEvent
    {
        TestStartHandler handler;
        TestCase test;

        public QueuedTestStart (TestStartHandler handler, TestCase test)
        {
            this.handler = handler;
            this.test = test;
        }

        public override void DoCallback ()
        {
            handler (test);
        }
    }
}
