using System;
using System.IO;
using System.Collections;
using System.Reflection;
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
			this.Decorated = false;
			this.WindowPosition = WindowPosition.Center;
			this.TypeHint = Gdk.WindowTypeHint.Splashscreen;
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
				string a = arg;
				// this does not yet work with relative paths
				if (a[0] == '~')
				{
					a = System.IO.Path.Combine (Environment.GetEnvironmentVariable ("HOME"), a.Substring (1));
				}
				
				if (File.Exists (a))
				{
					requestedFileList.Add (a);
					return;
				}
	
				if (a[0] == '-' || a[0] == '/') {
					int markerLength = 1;
					
					if (a.Length >= 2 && a[0] == '-' && a[1] == '-') {
						markerLength = 2;
					}
					
					parameterList.Add(a.Substring (markerLength));
				}
			}
		}
	}
}
