﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//   license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Runtime.InteropServices;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public class ImageButton : Gtk.Button
	{
		public ImageButton (string stock, string label)
		{
			Gtk.HBox hbox1 = new Gtk.HBox(false,0);
			hbox1.PackStart(new Gtk.Image(stock, Gtk.IconSize.Button), false, true, 0);
			hbox1.PackStart(new Gtk.Label(label), true, true, 0);
			this.Add(hbox1);
		}
	}
	public class ResourceService : AbstractService, IResourceService
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		
		readonly static string stringResources  = "StringResources";
		readonly static string imageResources   = "BitmapResources";
		
		static string resourceDirctory;
		
		static ResourceService()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			resourceDirctory = propertyService.DataDirectory + Path.DirectorySeparatorChar + "resources";

			iconFactory = new Gtk.IconFactory ();

			// FIXME: remove this when all MonoDevelop is using Gtk+
			// stock icons
			stockMappings = new Hashtable ();
			MonoDevelop.Gui.Stock.Init ();
			iconFactory.AddDefault ();
		}
		
		Hashtable userStrings = null;
		Hashtable userIcons   = null;
		
		ResourceManager strings = null;
		ResourceManager icon    = null;
		
		Hashtable localStrings = null;
		Hashtable localIcons   = null;
		
		void ChangeProperty(object sender, PropertyEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.OldValue != e.NewValue) {
				LoadLanguageResources();
			} 
		}
		void LoadLanguageResources()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string language = propertyService.GetProperty(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			
			localStrings = Load(stringResources, language);
			if (localStrings == null && language.IndexOf('-') > 0) {
				localStrings = Load(stringResources, language.Split(new char[] {'-'})[0]);
			}
			
			localIcons = Load(imageResources, language);
			if (localIcons == null && language.IndexOf('-') > 0) {
				localIcons = Load(imageResources, language.Split(new char[] {'-'})[0]);
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.PropertyChanged += new PropertyEventHandler(ChangeProperty);
			
			LoadLanguageResources();
		}
		
		// core service : Can't use Initialize, because all other stuff needs this service before initialize is called.
		public ResourceService()
		{
			strings = new ResourceManager(stringResources, Assembly.GetCallingAssembly());
			icon    = new ResourceManager(imageResources,  Assembly.GetCallingAssembly());
			
			if (System.Configuration.ConfigurationSettings.AppSettings["UserStrings"] != null) {
				userStrings = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserStrings"]);
			}
			if (System.Configuration.ConfigurationSettings.AppSettings["UserIcons"] != null) {
				userIcons   = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserIcons"]);
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size)
		{
			return LoadFont(fontName, size, FontStyle.Regular);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size, FontStyle style)
		{
			try {
				return new Font(fontName, size, style);
			} catch (Exception) {
				//return SystemInformation.MenuFont;
				return null;
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size, GraphicsUnit unit)
		{
			return LoadFont(fontName, size, FontStyle.Regular, unit);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>

		//FIXME: Convert to Pango.FontDescription
		public Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit)
		{
			//try {
				return new Font(fontName, size, style);
			//} catch (Exception) {
				//return new Gtk.Label ("-").Style.FontDescription;
			//}
		}
		
		Hashtable Load(string fileName)
		{
			if (File.Exists(fileName)) {
				Hashtable resources = new Hashtable();
				ResourceReader rr = new ResourceReader(fileName);
				foreach (DictionaryEntry entry in rr) {
					resources.Add(entry.Key, entry.Value);
				}
				rr.Close();
				return resources;
			}
			return null;
		}
		Hashtable Load(string name, string language)
		{
			return Load(resourceDirctory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
			
		}
		
		/// <summary>
		/// Returns a string from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <returns>
		/// The string in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested resource.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public string GetString(string name)
		{
			if (name.StartsWith ("${")) {
				name = name.Substring (6);
				name = name.Substring (0, name.Length - 1);
			}
			if (this.userStrings != null && this.userStrings[name] != null) {
				return userStrings[name].ToString();
			}
			if (localStrings != null && localStrings[name] != null) {
				return localStrings[name].ToString();
			}
			
			string s = strings.GetString(name);
			
			if (s == null) {
				throw new ResourceNotFoundException("string >" + name + "<");
			}
			
			return s;
		}
		
		// use P/Invoke to be able to pass some NULL parameters
		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern IntPtr
		gtk_icon_set_render_icon (IntPtr raw, IntPtr style, int direction,
		                          int state, int size, IntPtr widget,
		                          string detail);

		/// <summary>
		/// Returns a icon from the resource database, it handles localization
		/// transparent for the user. In the resource database can be a bitmap
		/// instead of an icon in the dabase. It is converted automatically.
		/// </summary>
		/// <returns>
		/// The icon in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested icon.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Gdk.Pixbuf GetIcon(string name)
		{
			string stockid = GetStockId (name);
			if (stockid != null) {
				Gtk.IconSet iconset = Gtk.IconFactory.LookupDefault (stockid);
				if (iconset != null) {
					// use P/Invoke to be able to pass some NULL parameters
					IntPtr raw_ret = gtk_icon_set_render_icon
						(iconset.Handle,
						 Gtk.Widget.DefaultStyle.Handle,
						 (int) Gtk.TextDirection.None,
						 (int) Gtk.StateType.Normal,
						 (int) Gtk.IconSize.Button,
						 IntPtr.Zero, null);
					return (Gdk.Pixbuf) GLib.Object.GetObject(raw_ret);
				}
			}
			
			// throw GLib.GException as the old code?
			return null;
		}
		
		/// <summary>
		/// Returns a bitmap from the resource database, it handles localization
		/// transparent for the user. 
		/// </summary>
		/// <returns>
		/// The bitmap in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Gdk.Pixbuf GetBitmap(string name)
		{
			// Try stock icons first
			Gdk.Pixbuf pix = GetIcon (name);
			if (pix == null) {
				// Try loading directly from disk then
				pix = new Gdk.Pixbuf("../data/resources/icons/" + name);
			}
			return pix;
		}

		public Gtk.Image GetImage (string name, Gtk.IconSize size)
		{
			string stock = GetStockId (name);
			if (stock != null)
				return new Gtk.Image (stock, size);
			return new Gtk.Image (GetBitmap (name));
		}
		
		static Gtk.IconFactory iconFactory = null;
		static Hashtable stockMappings = null;

		internal static void AddToIconFactory (string stockId,
		                                       string filename,
						       Gtk.IconSize iconSize)
		{
			try {
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf ("../data/resources/icons/" + filename);

				Gtk.IconSet iconSet = iconFactory.Lookup (stockId);
				if (iconSet == null) {
					iconSet = new Gtk.IconSet ();
					iconFactory.Add (stockId, iconSet);
				}

				Gtk.IconSource source = new Gtk.IconSource ();
				source.Pixbuf = pixbuf;
				source.Size = iconSize;
				iconSet.AddSource (source);

				// FIXME: temporary hack to retrieve the correct icon
				// from the filename
				stockMappings.Add (filename, stockId);
			}
			catch (GLib.GException ex) {
				// just discard the exception, the icon simply can't be
				// loaded
				Console.WriteLine ("Warning: can't load " + filename +
				                   " icon file");
			}
		}

		internal static void AddToIconFactory (string stockId, string filename)
		{
			AddToIconFactory (stockId, filename, Gtk.IconSize.Invalid);
		}
		
		internal static void AddDefaultStockMapping (string stockFile, string nativeStock)
		{
			stockMappings.Add (stockFile, nativeStock);
		}

		public static string GetStockId (string filename)
		{
			return (string) stockMappings [filename];
		}
	}
}
