using System;
using Gdk;

using MonoDevelop.Gui;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.NUnit
{
	static class CircleImage
	{
		static Pixbuf none, failure, success, notrun;
		static ResourceService res = ServiceManager.GetService (typeof (ResourceService)) as ResourceService;

		internal static Pixbuf Failure {
			get {
				if (failure == null)
					failure = res.GetIcon (Stock.NunitFailed);
				return failure;
			}
		}

		internal static Pixbuf None {
			get {
				if (none == null)
					none = res.GetIcon (Stock.NunitNone);
				return none;
			}
		}

		internal static Pixbuf NotRun {
			get {
				if (notrun == null)
					notrun = res.GetIcon (Stock.NunitNotRun);
				return notrun;
			}
		}

		internal static Pixbuf Success {
			get {
				if (success == null)
					success = res.GetIcon (Stock.NunitSuccess);
				return success;
			}
		}
	}
}

