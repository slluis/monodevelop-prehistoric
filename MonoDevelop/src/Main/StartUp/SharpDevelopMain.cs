// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//	 <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Threading;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Dialogs;

namespace MonoDevelop
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static string[] commandLineArgs = null;
		
		public static string[] CommandLineArgs {
			get {
				return commandLineArgs;
			}
		}

		static void ShowErrorBox(object sender, ThreadExceptionEventArgs eargs)
		{
			ExceptionDialog ed;

			ed = new ExceptionDialog(eargs.Exception);
			ed.AddButtonHandler(new ButtonHandler(DialogResultHandler));
			ed.ShowAll();
		}

		static void DialogResultHandler(ExceptionDialog ed, DialogResult dr) {
			ed.Destroy();			
		}

		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			Gnome.Program program = new Gnome.Program ("MonoDevelop", "0.1", Gnome.Modules.UI, args);
			Gdk.Threads.Init();
			commandLineArgs = args;
			bool noLogo = false;
		
			SplashScreenForm.SetCommandLineArgs(args);
			
			foreach (string parameter in SplashScreenForm.GetParameterList()) {
				switch (parameter.ToUpper()) {
					case "NOLOGO":
						noLogo = true;
						break;
				}
			}

			if (!noLogo) {
				SplashScreenForm.SplashScreen.ShowAll ();
				while (Gtk.Application.EventsPending()) {
					Gtk.Application.RunIteration (false);
				}
			}

			bool ignoreDefaultPath = false;
			string [] addInDirs = MonoDevelop.AddInSettingsHandler.GetAddInDirectories(out ignoreDefaultPath);
			AddInTreeSingleton.SetAddInDirectories(addInDirs, ignoreDefaultPath);

			ArrayList commands = null;
			try {
				ServiceManager.Services.AddService(new IconService());
				ServiceManager.Services.AddService(new MessageService());
				ServiceManager.Services.AddService(new ResourceService());
				ServiceManager.Services.InitializeServicesSubsystem("/Workspace/Services");

				commands = AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				for (int i = 0; i < commands.Count - 1; ++i) {
					((ICommand)commands[i]).Run();
				}

				// We don't have yet an alternative for Application.ThreadException
				// How can we handle this?
				// Application.ThreadException += new ThreadExceptionEventHandler(ShowErrorBox);

			} catch (XmlException e) {
				Console.WriteLine("Could not load XML :\n" + e.Message);
				return;
			} catch (Exception e) {
				Console.WriteLine("Loading error, please reinstall :\n" + e.ToString());
				return;
			} finally {
				if (SplashScreenForm.SplashScreen != null) {
					SplashScreenForm.SplashScreen.Hide();
				}
			}

			// run the last autostart command, this must be the workbench starting command
			if (commands.Count > 0) {
				((ICommand)commands[commands.Count - 1]).Run();
			}

			// unloading services
			ServiceManager.Services.UnloadAllServices();
		}
	}
}
