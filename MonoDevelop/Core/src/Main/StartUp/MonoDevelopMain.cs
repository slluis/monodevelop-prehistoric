// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//	 <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Mono.Posix;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui;

namespace MonoDevelop
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static string[] commandLineArgs = null;
		static Socket listen_socket = null;
		
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
		/// Starts the core of MonoDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			string socket_filename = "/tmp/md-" + Environment.GetEnvironmentVariable ("USER") + "-socket";
			listen_socket = new Socket (AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
			EndPoint ep = new UnixEndPoint (socket_filename);
			if (File.Exists (socket_filename)) {
				try {
					listen_socket.Connect (ep);
					listen_socket.Send (Encoding.UTF8.GetBytes (String.Join ("\n", args)));
					return;
				} catch {
				}
			}
			File.Delete (socket_filename);
			
			string name = Assembly.GetEntryAssembly ().GetName ().Name;
			string version = Assembly.GetEntryAssembly ().GetName ().Version.Major + + "." + Assembly.GetEntryAssembly ().GetName ().Version.Minor;

			Gnome.Program program = new Gnome.Program (name, version, Gnome.Modules.UI, args);
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
				RunMainLoop ();
			}

			bool ignoreDefaultPath = false;
			string [] addInDirs = MonoDevelop.AddInSettingsHandler.GetAddInDirectories(out ignoreDefaultPath);
			AddInTreeSingleton.SetAddInDirectories(addInDirs, ignoreDefaultPath);
			RunMainLoop ();

			ArrayList commands = null;
			try {
				ServiceManager.AddService(new IconService());
				ServiceManager.AddService(new MessageService());
				ServiceManager.AddService(new ResourceService());
				ServiceManager.InitializeServicesSubsystem("/Workspace/Services");

				commands = AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				RunMainLoop ();
				for (int i = 0; i < commands.Count - 1; ++i) {
					((ICommand)commands[i]).Run();
					RunMainLoop ();
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
			listen_socket.Bind (ep);
			listen_socket.Listen (5);
			listen_socket.BeginAccept (new AsyncCallback (ListenCallback), listen_socket);
			if (commands.Count > 0) {
				RunMainLoop ();
				((ICommand)commands[commands.Count - 1]).Run();
			}

			// unloading services
			File.Delete (socket_filename);
			ServiceManager.UnloadAllServices();
			System.Environment.Exit (0);
		}

		static string fileToOpen = String.Empty;
		
		static void RunMainLoop ()
		{
			while (Gtk.Application.EventsPending()) {
				Gtk.Application.RunIteration (false);
			}
		}

		static void ListenCallback (IAsyncResult state)
		{
			Socket client = ((Socket)state.AsyncState).EndAccept (state);
			((Socket)state.AsyncState).BeginAccept (new AsyncCallback (ListenCallback), state.AsyncState);
			byte[] buf = new byte[1024];
			client.Receive (buf);
			foreach (string filename in Encoding.UTF8.GetString (buf).Split ('\n')) {
				string trimmed = filename.Trim ();
				string file = "";
				foreach (char c in trimmed) {
					if (c == 0x0000)
						continue;
					file += c;
				}
				fileToOpen = file;
				GLib.Idle.Add (new GLib.IdleHandler (openFile));
			}
		}

		static bool openFile () 
		{
			lock (fileToOpen) {
				string file = fileToOpen;
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
				case ".CMBX":
				case ".PRJX":
					try {
						IProjectService projectService = (IProjectService)ServiceManager.GetService (typeof (IProjectService));
						projectService.OpenCombine(file);
					} catch (Exception e) {
					}
					break;
				default:
					try {
						IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
						fileService.OpenFile(file);
					} catch (Exception e) {
					}
					break;
				}
				((Gtk.Window)WorkbenchSingleton.Workbench).Present ();
				return false;
			}
		}
	}
}
