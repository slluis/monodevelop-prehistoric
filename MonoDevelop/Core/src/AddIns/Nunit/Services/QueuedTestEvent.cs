using System;

using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	class QueuedTestEvent : QueuedEvent
    {
        Delegate te;
        object arg;

        public QueuedTestEvent (TestCaseResultHandler te, object arg)
        {
            this.te = te;
            this.arg = arg;
        }

        public override void DoCallback ()
        {
            te.DynamicInvoke (new object [] {arg});
        }
    }
}
