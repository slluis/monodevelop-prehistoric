// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
//using ICSharpCode.XmlForms;
//using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ReplaceDialog : Gtk.Window
	{
		bool replaceMode;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		
		Gtk.Combo searchPatternComboBox;
		Gtk.Combo replacePatternComboBox;
		Gtk.Button findHelpButton;
		Gtk.Button findButton;
		Gtk.Button markAllButton;
		Gtk.Button closeButton;
		Gtk.Button replaceButton;
		Gtk.Button replaceAllButton;
		Gtk.Button replaceHelpButton;
		Gtk.CheckButton ignoreCaseCheckBox;
		Gtk.CheckButton searchWholeWordOnlyCheckBox;
		Gtk.CheckButton useSpecialSearchStrategyCheckBox;
		Gtk.OptionMenu specialSearchStrategyComboBox;
		Gtk.OptionMenu searchLocationComboBox;
		
		void InitDialog ()
		{
			Gtk.Label findWhat = new Gtk.Label (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.FindWhat}"));
			Gtk.Label searchIn = new Gtk.Label (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.SearchIn}"));
			searchPatternComboBox = new Gtk.Combo ();
			findHelpButton = new Gtk.Button (">");
			findButton = new Gtk.Button (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.FindNextButton}"));
			markAllButton = new Gtk.Button (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.MarkAllButton}"));
			closeButton = new Gtk.Button (stringParserService.Parse ("${res:Global.CloseButtonText}"));
			ignoreCaseCheckBox = new Gtk.CheckButton (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.CaseSensitive}"));
			searchWholeWordOnlyCheckBox = new Gtk.CheckButton (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.WholeWord}"));
			useSpecialSearchStrategyCheckBox = new Gtk.CheckButton (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.UseMethodLabel}"));
			specialSearchStrategyComboBox = new Gtk.OptionMenu ();
			searchLocationComboBox = new Gtk.OptionMenu ();

			if (replaceMode) {
				findButton = new Gtk.Button (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.FindNextButton}"));
				replacePatternComboBox = new Gtk.Combo ();
				replaceButton = new Gtk.Button (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.ReplaceButton}"));
				replaceAllButton = new Gtk.Button (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.ReplaceAllButton}"));
				replaceHelpButton = new Gtk.Button (">");
			}
			
			Gtk.HBox mainbox = new Gtk.HBox (false, 2);
			Gtk.VButtonBox btnbox = new Gtk.VButtonBox ();
			btnbox.PackStart (findButton);
			if (replaceMode) {
				btnbox.PackStart (replaceButton);
				btnbox.PackStart (replaceAllButton);
			} else {
				btnbox.PackStart (markAllButton);
			}
			btnbox.PackEnd (closeButton);

			Gtk.VBox controlbox = new Gtk.VBox (false, 2);
			Gtk.HBox findbox = new Gtk.HBox (false, 2);
			findbox.PackStart (findWhat, false, false, 2);
			findbox.PackStart (searchPatternComboBox);
			findbox.PackStart (findHelpButton, false, false, 2);
			controlbox.PackStart (findbox);

			if (replaceMode) {
				Gtk.HBox replaceBox = new Gtk.HBox (false, 2);
				replaceBox.PackStart (new Gtk.Label (stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.ReplaceWith}")) , false, false, 2);
				replaceBox.PackStart (replacePatternComboBox);
				replaceHelpButton.Sensitive = false;
				replaceBox.PackStart (replaceHelpButton, false, false, 2);
				controlbox.PackStart (replaceBox);
			}

			Gtk.HBox optionbox = new Gtk.HBox (false, 2);
			Gtk.VBox checkbox = new Gtk.VBox (false, 2);

			checkbox.PackStart (ignoreCaseCheckBox);
			checkbox.PackStart (searchWholeWordOnlyCheckBox);
			optionbox.PackStart (checkbox);

			Gtk.VBox searchInBox = new Gtk.VBox (false, 2);
			searchInBox.PackStart (searchIn);
			searchInBox.PackStart (searchLocationComboBox);
			optionbox.PackStart (searchInBox);

			controlbox.PackStart (optionbox);

			Gtk.HBox strategyBox = new Gtk.HBox (false, 2);
			strategyBox.PackStart (useSpecialSearchStrategyCheckBox);
			strategyBox.PackStart (specialSearchStrategyComboBox);
			controlbox.PackStart (strategyBox);

			mainbox.PackStart (controlbox);
			mainbox.PackStart (btnbox, false, false, 2);

			this.Add (mainbox);
		}
		
		public ReplaceDialog(bool replaceMode) : base ("Find")
		{
			this.replaceMode = replaceMode;
			InitDialog ();
			/*if (replaceMode) {
				//this.SetupFromXml(Path.Combine(propertyService.DataDirectory, @"resources\dialogs\ReplaceDialog.xfrm"));
				//ControlDictionary["replaceHelpButton"].Enabled = false;
				InitDialogForReplace ();
			} else {
				InitDialogForFind ();
				//this.SetupFromXml(Path.Combine(propertyService.DataDirectory, @"resources\dialogs\FindDialog.xfrm"));
			}*/
			
			findHelpButton.Sensitive = false;
			//AcceptButton = (Button)ControlDictionary["findButton"];
			//CancelButton = (Button)ControlDictionary["closeButton"];
			
			ignoreCaseCheckBox.Active = !SearchReplaceManager.SearchOptions.IgnoreCase;
			searchWholeWordOnlyCheckBox.Active = SearchReplaceManager.SearchOptions.SearchWholeWordOnly;
			
			useSpecialSearchStrategyCheckBox.Active  = SearchReplaceManager.SearchOptions.SearchStrategyType != SearchStrategyType.Normal;
			useSpecialSearchStrategyCheckBox.Toggled += new EventHandler(SpecialSearchStrategyCheckBoxChangedEvent);
			
			Gtk.MenuItem tmpItem = new Gtk.MenuItem ("Wildcards");
			Gtk.Menu stratMenu = new Gtk.Menu ();
			stratMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (resourceService.GetString("Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch"));
			stratMenu.Append (tmpItem);
			specialSearchStrategyComboBox.Menu = stratMenu;
		
			uint index = 0;
			switch (SearchReplaceManager.SearchOptions.SearchStrategyType) {
				case SearchStrategyType.Normal:
				case SearchStrategyType.Wildcard:
					break;
				case SearchStrategyType.RegEx:
					index = 1;
					break;
			}
			specialSearchStrategyComboBox.SetHistory (index);
			
			Gtk.Menu locMenu = new Gtk.Menu ();
			tmpItem = new Gtk.MenuItem (resourceService.GetString("Global.Location.currentfile"));
			locMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (resourceService.GetString("Global.Location.allopenfiles"));
			locMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (resourceService.GetString("Global.Location.wholeproject"));
			locMenu.Append (tmpItem);
			
			searchLocationComboBox.Menu = locMenu;
			
			index = 0;
			switch (SearchReplaceManager.SearchOptions.DocumentIteratorType) {
				case DocumentIteratorType.AllOpenFiles:
					index = 1;
					break;
				case DocumentIteratorType.WholeCombine:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
					break;
			}
			searchLocationComboBox.SetHistory (index);
			
			searchPatternComboBox.Entry.Text  = SearchReplaceManager.SearchOptions.SearchPattern;
			
			// insert event handlers
			findButton.Clicked  += new EventHandler(FindNextEvent);
			closeButton.Clicked += new EventHandler(CloseDialogEvent);
			DeleteEvent += new GtkSharp.DeleteEventHandler (OnDeleted);
			
			if (replaceMode) {
				this.Title = resourceService.GetString("Dialog.NewProject.SearchReplace.ReplaceDialogName");
				replaceButton.Clicked    += new EventHandler(ReplaceEvent);
				replaceAllButton.Clicked += new EventHandler(ReplaceAllEvent);
				replacePatternComboBox.Entry.Text = SearchReplaceManager.SearchOptions.ReplacePattern;
			} else {
				this.Title = resourceService.GetString("Dialog.NewProject.SearchReplace.FindDialogName");
				markAllButton.Clicked    += new EventHandler(MarkAllEvent);
			}
			
				//ControlDictionary["replacePatternComboBox"].Visible = false;
				//ControlDictionary["replaceAllButton"].Visible       = false;
				//ControlDictionary["replacePatternLabel"].Visible    = false;
				//ControlDictionary["replacePatternButton"].Visible   = false;
				//ControlDictionary["replaceButton"].Text             = resourceService.GetString("Dialog.NewProject.SearchReplace.ToggleReplaceModeButton");
				//ClientSize = new Size(ClientSize.Width, ClientSize.Height - 32);
			
			SpecialSearchStrategyCheckBoxChangedEvent(null, null);
			SearchReplaceManager.ReplaceDialog     = this;
		}
		
		protected void OnClosed(EventArgs e)
		{
			//base.OnClosed(e);
			SearchReplaceManager.ReplaceDialog     = null;
		}
		
		void OnDeleted (object o, GtkSharp.DeleteEventArgs args)
		{
			SearchReplaceManager.ReplaceDialog = null;
		}

		public void SetSearchPattern(string pattern)
		{
			searchPatternComboBox.Entry.Text  = pattern;
		}
		
		void SetupSearchReplaceManager()
		{
			SearchReplaceManager.SearchOptions.SearchPattern  = searchPatternComboBox.Entry.Text;
			if (replaceMode) {
				SearchReplaceManager.SearchOptions.ReplacePattern = replacePatternComboBox.Entry.Text;
			}
			
			SearchReplaceManager.SearchOptions.IgnoreCase          = !ignoreCaseCheckBox.Active;
			SearchReplaceManager.SearchOptions.SearchWholeWordOnly = searchWholeWordOnlyCheckBox.Active;
			
			if (useSpecialSearchStrategyCheckBox.Active) {
				switch (specialSearchStrategyComboBox.History) {
					case 0:
						SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.Wildcard;
						break;
					case 1:
						SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.RegEx;
						break;
				}
			} else {
				SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.Normal;
			}
			
			switch (searchLocationComboBox.History) {
				case 0:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
					break;
				case 1:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.AllOpenFiles;
					break;
				case 2:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.WholeCombine;
					break;
			}
		}
		
		void FindNextEvent(object sender, EventArgs e)
		{
			if (searchPatternComboBox.Entry.Text.Length == 0) {
				return;
			}
			
			try {
				//Cursor = Cursors.WaitCursor;
				SetupSearchReplaceManager();
				SearchReplaceManager.FindNext();
				//this.Focus();
			}
			finally {
				//Cursor = Cursors.Default;
			}
		}
		
		void ReplaceEvent(object sender, EventArgs e)
		{
			if (searchPatternComboBox.Entry.Text.Length == 0) {
				return;
			}
			
			try {
				//Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.Replace();
			}
			finally {
				//Cursor = Cursors.Default;
			}
		}
		
		void ReplaceAllEvent(object sender, EventArgs e)
		{
			if (searchPatternComboBox.Entry.Text.Length == 0) {
				return;
			}
			
			try {
				//Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.ReplaceAll();
			} finally {
				//Cursor = Cursors.Default;
			}
		}
		
		void MarkAllEvent(object sender, EventArgs e)
		{
			if (searchPatternComboBox.Entry.Text.Length == 0) {
				return;
			}
			
			try {
				//Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.MarkAll();			
			} finally {
				//Cursor = Cursors.Default;
			}
		}
		
		void CloseDialogEvent(object sender, EventArgs e)
		{
			Hide();
			OnClosed (null);
		}
		
		void SpecialSearchStrategyCheckBoxChangedEvent(object sender, EventArgs e)
		{
			if (useSpecialSearchStrategyCheckBox != null) {
				specialSearchStrategyComboBox.Sensitive = useSpecialSearchStrategyCheckBox.Active;
			}
		}
	}
}
