using System;
using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	class QueuedSuiteStart : QueuedEvent
    {
        SuiteStartHandler handler;
        TestSuite suite;

        public QueuedSuiteStart (SuiteStartHandler handler, TestSuite suite)
        {
            this.handler = handler;
            this.suite = suite;
        }

        public override void DoCallback ()
        {
            handler (suite);
        }
    }
}

