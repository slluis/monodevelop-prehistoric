// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Reflection;

using Gtk;
using MonoDevelop.Gui;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Dialogs
{
	public class AboutMonoDevelopTabPage : VBox
	{
		Label versionLabel = new Label ();
		Label sponsorLabel = new Label ();
		Label licenseLabel = new Label ();
		Label copyrightLabel = new Label ();
		
		public AboutMonoDevelopTabPage ()
		{
			Version v = Assembly.GetEntryAssembly().GetName().Version;
			versionLabel.Markup = String.Format ("<b>{0}</b>\n    {1}", GettextCatalog.GetString("Version"), v.Major + "." + v.Minor);
			HBox hboxVersion = new HBox ();
			hboxVersion.PackStart (versionLabel, false, false, 5);
			
			HBox hboxLicense = new HBox ();
			licenseLabel.Markup = String.Format ("<b>License</b>\n    {0}", GettextCatalog.GetString ("Released under the GNU General Public license."));
			hboxLicense.PackStart (licenseLabel, false, false, 5);


			HBox hboxCopyright = new HBox ();
			copyrightLabel.Markup = "<b>Copyright</b>\n    (c) 2000-2003 by icsharpcode.net\n    (c) 2004-2005 by MonoDevelop contributors";
			hboxCopyright.PackStart (copyrightLabel, false, false, 5);

			this.PackStart (hboxVersion, false, true, 0);
			this.PackStart (hboxLicense, false, true, 5);
			this.PackStart (hboxCopyright, false, true, 5);
			this.ShowAll ();
		}
	}
	
	public class VersionInformationTabPage : VBox
	{
		private TreeView listView;
		private Button button;
		private TreeStore store;
		private Clipboard clipboard;
		
		public VersionInformationTabPage ()
		{
			TreeView listView = new TreeView ();
			listView.RulesHint = true;
			listView.AppendColumn (GettextCatalog.GetString ("Name"), new CellRendererText (), "text", 0);
			listView.AppendColumn (GettextCatalog.GetString ("Version"), new CellRendererText (), "text", 1);
			listView.AppendColumn (GettextCatalog.GetString ("Path"), new CellRendererText (), "text", 2);
			
			FillListView ();
			ScrolledWindow sw = new ScrolledWindow ();
			sw.Add (listView);
			this.PackStart (sw);
			
			HBox hbox = new HBox (false, 0);
			//button = new Button (Gtk.Stock.Copy);
			//button.Clicked += new EventHandler(CopyButtonClick);
			hbox.PackStart (new Label (), false, true, 3);
			//hbox.PackStart (button, false, false, 3);
			hbox.PackStart (new Label (), false, true, 3);
			this.PackStart (hbox, false, false, 6);
			
			listView.Model = store;
		}
		
		void FillListView()
		{
			store = new TreeStore (typeof (string), typeof (string), typeof (string));
			
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				
				string loc;
				
				try {
					loc = System.IO.Path.GetFullPath (asm.Location);
				} catch (Exception) {
					loc = GettextCatalog.GetString ("dynamic");
				}
				
				store.AppendValues (name.Name, name.Version.ToString(), loc);
			}
			
			store.SetSortColumnId (0, SortType.Ascending);
		}
		
		void CopyButtonClick(object o, EventArgs args)
		{
			StringBuilder versionInfo = new StringBuilder();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				versionInfo.Append(name.Name);
				versionInfo.Append(",");
				versionInfo.Append(name.Version.ToString());
				versionInfo.Append(",");
				try {
					versionInfo.Append(System.IO.Path.GetFullPath (asm.Location));
				} catch (Exception) {
					versionInfo.Append(GettextCatalog.GetString ("dynamic"));
				}
				
				versionInfo.Append(Environment.NewLine);
			}
			
			// set to both the X and normal clipboards
			clipboard = Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
			clipboard.SetText (versionInfo.ToString ());
			clipboard = Clipboard.Get (Gdk.Atom.Intern ("PRIMARY", false));
			clipboard.SetText (versionInfo.ToString ());
		}
	}
}
