using System;
using System.IO;
using MonoDevelop.Gui;

class T
{
	static void Main ()
	{
		Vfs.Init ();
		Console.WriteLine (Vfs.Initialized);
		string test_file = Path.Combine (Environment.CurrentDirectory, "test.txt");
		if (File.Exists (test_file))
		{
			string mt = Vfs.GetMimeType (test_file);
			Console.WriteLine (Vfs.IsKnownType (mt));
			string icon = Vfs.GetIcon (mt);
			Console.WriteLine (mt);
			Console.WriteLine (icon);
		}
		else
		{
			Console.WriteLine (test_file + " does not exist");
		}
		Vfs.Shutdown ();
	}
}
