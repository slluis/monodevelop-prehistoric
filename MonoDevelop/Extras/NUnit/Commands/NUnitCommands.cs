using System;
using System.IO;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Commands
{
	public class NUnitLoadAssembly : AbstractMenuCommand
	{
		NUnitService nunitService = (NUnitService) ServiceManager.GetService (typeof (NUnitService));

		public override void Run ()
		{
			using (FileSelector fs = new FileSelector ("Load test assembly")) {
				//fs.DefaultPath = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Projects");

				if (fs.Run () == (int) Gtk.ResponseType.Ok)
				{
					nunitService.LoadAssembly (fs.Filename);
				}

				fs.Hide ();
			}
		}
	}

	public class NUnitRunTests : AbstractMenuCommand
	{
		NUnitService nunitService = (NUnitService) ServiceManager.GetService (typeof (NUnitService));

		public override void Run ()
		{
			nunitService.RunTests ();
		}
	}
}
