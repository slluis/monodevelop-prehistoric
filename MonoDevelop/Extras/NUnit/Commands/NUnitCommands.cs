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
		public override void Run ()
		{
			NUnitService nunitService = (NUnitService) ServiceManager.GetService (typeof (NUnitService));

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
		public override void Run ()
		{
			Console.WriteLine ("Not implemented");
		}
	}
}
