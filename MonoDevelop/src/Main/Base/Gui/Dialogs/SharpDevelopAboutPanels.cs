// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using Gtk;

using System.Resources;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project.Collections;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class AboutMonoDevelopTabPage : VBox
	{
		static GLib.GType type;
		Label      buildLabel   = new Label ();
		Label    buildTextBox = new Label ();
		Label      versionLabel   = new Label ();
		Label    versionTextBox = new Label ();
		Label      sponsorLabel   = new Label ();
		
		static AboutMonoDevelopTabPage ()
		{
			type = RegisterGType (typeof (AboutMonoDevelopTabPage));
		}
		
		public AboutMonoDevelopTabPage() : base (type)
		{
			HBox hbox = new HBox (false, 0);
			Version v = Assembly.GetEntryAssembly().GetName().Version;
			versionTextBox.Text = v.Major + "." + v.Minor;
			buildTextBox.Text   = v.Revision + "." + v.Build;
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			versionLabel.Text = resourceService.GetString("Dialog.About.label1Text");
			
			//versionLabel.TabIndex = 1;
			hbox.PackStart (versionLabel, false, false, 10);
			
			//versionTextBox.TabIndex = 4;
			hbox.PackStart (versionTextBox, false, false, 10);
			
			buildLabel.Text = resourceService.GetString("Dialog.About.label2Text");
			//buildLabel.TabIndex = 2;
			hbox.PackStart (buildLabel, false, false, 10);
			
			//buildTextBox.TabIndex = 3;
			hbox.PackStart (buildTextBox, false, false, 10);
			this.PackStart (hbox, false, false, 5);
			
			sponsorLabel.Text = "Released under the GNU General Public license.";
				               // "Sponsored by AlphaSierraPapa\n" +
			                   // "                   http://www.AlphaSierraPapa.com";
			//sponsorLabel.TabIndex = 8;
			this.PackStart (sponsorLabel, false, false, 5);
			this.ShowAll ();
		}
	}
	
	public class AuthorAboutTabPage : ICSharpCode.SharpDevelop.Gui.HtmlControl.HtmlControl
	{
		public AuthorAboutTabPage()
		{
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string html = ConvertXml.ConvertToString(fileUtilityService.SharpDevelopRootPath +
				                   System.IO.Path.DirectorySeparatorChar + "doc" +
				                   System.IO.Path.DirectorySeparatorChar + "AUTHORS.xml",
				                   
				                   propertyService.DataDirectory +
				                   System.IO.Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   System.IO.Path.DirectorySeparatorChar + "ShowAuthors.xsl");
				
				
				base.CascadingStyleSheet = propertyService.DataDirectory + System.IO.Path.DirectorySeparatorChar +
				                           "resources" + System.IO.Path.DirectorySeparatorChar +
				                           "css" + System.IO.Path.DirectorySeparatorChar +
				                           "SharpDevelopStandard.css";
				base.Html = html;
			} catch (Exception e) {
				IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
			}
		}
	}
	
	public class ChangeLogTabPage : ICSharpCode.SharpDevelop.Gui.HtmlControl.HtmlControl
	{
		public ChangeLogTabPage()
		{
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string html = ConvertXml.ConvertToString(fileUtilityService.SharpDevelopRootPath +
				                   System.IO.Path.DirectorySeparatorChar + "doc" +
				                   System.IO.Path.DirectorySeparatorChar + "ChangeLog.xml",
				                   
				                   propertyService.DataDirectory +
				                   System.IO.Path.DirectorySeparatorChar + "ConversionStyleSheets" + 
				                   System.IO.Path.DirectorySeparatorChar + "ShowChangeLog.xsl");
				
				base.CascadingStyleSheet = propertyService.DataDirectory + System.IO.Path.DirectorySeparatorChar +
				                           "resources" + System.IO.Path.DirectorySeparatorChar +
				                           "css" + System.IO.Path.DirectorySeparatorChar +
				                           "SharpDevelopStandard.css";
				//base.Html = html;
				
				// feel free to add your name and email here
				// if you contributed to the port
				// FIXME: make real port credits
				base.Html = "<html><body><h3>MonoDevelop port</h3>"
				+ "<p>This is a port of SharpDevelop 0.98 to Gtk#.</p>"
				+ "<p>by:"
				+ "<ul><li>Todd Berman</li>"
				+ "<li>Pedro Abelleira Seco</li>"
				+ "<li>John Luke</li>"
				+ "<li>dkor</li>"
				+ "<li>orph</li>"
				+ "<li>nricciar</li>"
				+ "<li>jba</li>"
				+ "</ul></body></html>";
				
			} catch (Exception e) {
				IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
			}
		}
	}
	
	
	public class VersionInformationTabPage : VBox
	{
		private static GLib.GType type;
		private TreeView listView;
		private Button button;
		private TreeStore store;
		
		static VersionInformationTabPage ()
		{
			type = RegisterGType (typeof (VersionInformationTabPage));
		}

		public VersionInformationTabPage() : base (type)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			TreeView listView = new TreeView ();
			listView.RulesHint = true;
			listView.AppendColumn (resourceService.GetString("Dialog.About.VersionInfoTabName.NameColumn"), new CellRendererText (), "text", 0);
			listView.AppendColumn (resourceService.GetString("Dialog.About.VersionInfoTabName.VersionColumn"), new CellRendererText (), "text", 1);
			listView.AppendColumn (resourceService.GetString("Dialog.About.VersionInfoTabName.PathColumn"), new CellRendererText (), "text", 2);
			
			FillListView ();
			ScrolledWindow sw = new ScrolledWindow ();
			sw.Add (listView);
			this.PackStart (sw);
			
			HBox hbox = new HBox (false, 0);
			button = new Button (Gtk.Stock.Copy);
			button.Clicked += new EventHandler(CopyButtonClick);
			hbox.PackStart (new Label (), false, true, 3);
			hbox.PackStart (button, false, false, 3);
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
					loc = asm.Location;
				} catch (Exception) {
					loc = "dynamic";
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
					versionInfo.Append(asm.Location);
				} catch (Exception) {
					versionInfo.Append("dynamic");
				}
				
				versionInfo.Append(Environment.NewLine);
			}
			
			//Clipboard.SetDataObject(new DataObject(System.Windows.Forms.DataFormats.Text, versionInfo.ToString()), true);
		}
	}
}
