using System;

using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	public abstract class QueuedEvent
    {
        public abstract void DoCallback ();
    }
}
