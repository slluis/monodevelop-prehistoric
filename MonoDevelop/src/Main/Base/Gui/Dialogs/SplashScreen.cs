using System;
using System.Collections;
//using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Resources;
using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	
	public class SplashScreenForm : Gtk.Window
	{
		static SplashScreenForm splashScreen = new SplashScreenForm();
		static ArrayList requestedFileList = new ArrayList();
		static ArrayList parameterList = new ArrayList();
		
		public static SplashScreenForm SplashScreen {
			get {
				return splashScreen;
			}
		}		
		
		public SplashScreenForm() : base ("Splash")
		{
#if !DEBUG
			//TopMost         = true;
#endif
			this.Decorated = false;
			this.WindowPosition = WindowPosition.Center;
			this.TypeHint = Gdk.WindowTypeHint.Splashscreen;
			//ShowInTaskbar   = false;
			Gdk.Pixbuf bitmap = new Gdk.Pixbuf(Assembly.GetEntryAssembly(), "SplashScreen.png");
			DefaultWidth = bitmap.Width;
			DefaultHeight = bitmap.Height;
			Gtk.Image image = new Gtk.Image (bitmap);
			image.Show ();
			this.Add (image);
		}

		public static string[] GetParameterList()
		{
			return GetStringArray(parameterList);
		}
		
		public static string[] GetRequestedFileList()
		{
			return GetStringArray(requestedFileList);
		}
		
		static string[] GetStringArray(ArrayList list)
		{
			return (string[])list.ToArray(typeof(string));
		}
		
		public static void SetCommandLineArgs(string[] args)
		{
			requestedFileList.Clear();
			parameterList.Clear();
			
			foreach (string arg in args)
			{
				if (arg[0] == '-' || arg[0] == '/') {
					int markerLength = 1;
					
					if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-') {
						markerLength = 2;
					}
					
					parameterList.Add(arg.Substring(markerLength));
				}
				else {
					requestedFileList.Add(arg);
				}
			}
		}
	}
}
