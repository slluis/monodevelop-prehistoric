using System;
using MonoDevelop.Core.AddIns.Codons;
using AddInManager;

namespace MonoDevelop.Commands
{
	public class ShowAddInManager : AbstractMenuCommand
	{
		public override void Run ()
		{
			Console.WriteLine ("asdfsdfgfh");
			using (AddInManagerDialog d = new AddInManagerDialog ()) {
				d.Run ();
				d.Hide ();
			}
		}
	}
}
