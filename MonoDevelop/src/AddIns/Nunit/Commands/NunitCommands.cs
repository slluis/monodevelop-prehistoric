using System;
using System.IO;
using Gtk;

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Commands
{
	public class NunitLoadAssembly : AbstractMenuCommand
	{
		public override void Run ()
		{
			NunitService nunitService = (NunitService) MonoDevelop.Core.Services.ServiceManager.Services.GetService (typeof (NunitService));

			using (FileSelection fs = new FileSelection ("Load test assembly")) {
				string defaultPath = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "MonoDevelopProjects");
				fs.Complete (defaultPath);

				if (fs.Run () == (int) Gtk.ResponseType.Ok)
				{
					nunitService.LoadAssembly (fs.Filename);
				}

				fs.Hide ();
			}
		}
	}

	public class NunitRunTests : AbstractMenuCommand
	{
		public override void Run ()
		{
			Console.WriteLine ("Not implemented");
		}
	}
}