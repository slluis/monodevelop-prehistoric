// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;

using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Core.Properties;

using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	public class IDEOptionPanel : AbstractOptionPanel
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		Gtk.TreeView listView;
		Gtk.TreeStore listStore;
		Gtk.Label newCulture;
		Gtk.Label culture;
		Gtk.Label descr;
		
		ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
		
		string SelectedCulture {
			get {
				Gtk.TreeModel mdl;
				Gtk.TreeIter  iter;
				if (listView.Selection.GetSelected (out mdl, out iter)) {
					return (string) ((Gtk.TreeStore)mdl).GetValue (iter, 1);
				}
				return null;
			}
		}
		
		string SelectedCountry {
			get {
				Gtk.TreeModel mdl;
				Gtk.TreeIter  iter;
				if (listView.Selection.GetSelected (out mdl, out iter)) {
					return (string) ((Gtk.TreeStore)mdl).GetValue (iter, 0);
				}
				return null;
			}
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (SelectedCulture != null) {
					propertyService.SetProperty(uiLanguageProperty, SelectedCulture);
				}
			}
			return true;
		}
		
		void ChangeCulture(object sender, EventArgs e)
		{
			newCulture.Text = resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.UILanguageSetToLabel") + " " + SelectedCountry;
		}
		
		string GetCulture(string languageCode)
		{
			LanguageService languageService = (LanguageService)ServiceManager.GetService(typeof(LanguageService));
			foreach (Language language in languageService.Languages) {
				if (languageCode.StartsWith(language.Code)) {
					return language.Name;
				}
			}
			return "English";
		}
		
		public IDEOptionPanel() : base ()
		{
			LanguageService languageService = (LanguageService)ServiceManager.GetService(typeof(LanguageService));
			
			Gtk.VBox mainbox = new Gtk.VBox (false, 2);
			
			listStore = new Gtk.TreeStore (typeof (string), typeof (string));

			listView = new Gtk.TreeView (listStore);

			listView.AppendColumn ("lang", new Gtk.CellRendererText (), "text", 0);
			listView.HeadersVisible = false;

			//listView.LargeImageList = languageService.LanguageImageList;
			listView.Selection.Changed += new EventHandler(ChangeCulture);
			foreach (Language language in languageService.Languages) {
				listStore.AppendValues (language.Name, language.Code);//, language.ImageIndex);
			}			
			
			mainbox.PackStart (listView);
			
			culture = new Gtk.Label (resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.CurrentUILanguageLabel") + " " + GetCulture(propertyService.GetProperty(uiLanguageProperty, "en")));
			mainbox.PackStart (culture, false, false, 2);
			
			descr = new Gtk.Label (resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.DescriptionText"));
			
			mainbox.PackStart (descr, false, false, 2);
			

			newCulture = new Gtk.Label ("");
			mainbox.PackStart (newCulture, false, false, 2);

			this.Add (mainbox);
		}
	}
}
