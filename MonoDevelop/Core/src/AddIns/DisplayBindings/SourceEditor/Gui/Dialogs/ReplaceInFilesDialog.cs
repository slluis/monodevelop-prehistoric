// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Kr�ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;

using MonoDevelop.Gui;
using MonoDevelop.TextEditor.Document;
using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.TextEditor;

using MonoDevelop.Gui.Widgets;

using Glade;
using Gtk;

namespace MonoDevelop.Gui.Dialogs
{
	public class ReplaceInFilesDialog
	{
		IMessageService messageService  = (IMessageService)ServiceManager.GetService(typeof(IMessageService));
		StringParserService stringParserService = (StringParserService)ServiceManager.GetService (typeof (StringParserService));
		static PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
		public bool replaceMode;

		[Glade.Widget] Gnome.Entry searchPatternEntry;
		[Glade.Widget] Gnome.Entry replacePatternEntry;
		[Glade.Widget] Gtk.Button findHelpButton;
		[Glade.Widget] Gtk.Button findButton;
		[Glade.Widget] Gtk.Button markAllButton;
		[Glade.Widget] Gtk.Button closeButton;
		[Glade.Widget] Gtk.Button replaceButton;
		[Glade.Widget] Gtk.Button replaceAllButton;
		[Glade.Widget] Gtk.Button replaceHelpButton;
		[Glade.Widget] Gtk.CheckButton ignoreCaseCheckBox;
		[Glade.Widget] Gtk.CheckButton searchWholeWordOnlyCheckBox;
		[Glade.Widget] Gtk.CheckButton useSpecialSearchStrategyCheckBox;
		[Glade.Widget] Gtk.OptionMenu specialSearchStrategyComboBox;
		[Glade.Widget] Gtk.OptionMenu searchLocationComboBox;
		[Glade.Widget] Gtk.Label label1;
		[Glade.Widget] Gtk.Label label2;
		[Glade.Widget] Gtk.Label searchLocationLabel;
		[Glade.Widget] Gtk.Dialog FindInFilesDialogWidget;
		[Glade.Widget] Gtk.Dialog ReplaceInFilesDialogWidget;

		[Glade.Widget] Gtk.CheckButton includeSubdirectoriesCheckBox;
		[Glade.Widget] Gtk.Entry fileMaskTextBox;
		[Glade.Widget] Gtk.Entry directoryTextBox;
		[Glade.Widget] Gtk.Button browseButton;
		[Glade.Widget] Gtk.Label label6;
		[Glade.Widget] Gtk.Label label7;
		
		Gtk.Dialog ReplaceDialogPointer;
		
		void InitDialog ()
		{
			findButton.UseUnderline = true;			
			closeButton.UseUnderline = true;
			
			//set up the size groups
			SizeGroup labels = new SizeGroup(SizeGroupMode.Horizontal);
			SizeGroup combos = new SizeGroup(SizeGroupMode.Horizontal);
			SizeGroup options = new SizeGroup(SizeGroupMode.Horizontal);
			SizeGroup helpButtons = new SizeGroup(SizeGroupMode.Horizontal);
			SizeGroup checkButtons = new SizeGroup(SizeGroupMode.Horizontal);
			labels.AddWidget(label1);
			labels.AddWidget(label6);
			labels.AddWidget(label7);
			combos.AddWidget(searchPatternEntry);
			combos.AddWidget(directoryTextBox);
			combos.AddWidget(fileMaskTextBox);
			helpButtons.AddWidget(findHelpButton);
			helpButtons.AddWidget(browseButton);
			checkButtons.AddWidget (includeSubdirectoriesCheckBox);
			checkButtons.AddWidget(ignoreCaseCheckBox);
			checkButtons.AddWidget(searchWholeWordOnlyCheckBox);
			checkButtons.AddWidget(useSpecialSearchStrategyCheckBox);
			checkButtons.AddWidget(searchLocationLabel);
			options.AddWidget(specialSearchStrategyComboBox);
			options.AddWidget(searchLocationComboBox);
			
			// set button sensitivity
			findHelpButton.Sensitive = false;
			
			// set replace dialog properties 
			if(replaceMode)
			{
				// set the label properties
				label2.Text = GettextCatalog.GetString ("Replace in Files");
				//replaceButton.Label = stringParserService.Parse ("${res:Dialog.NewProject.SearchReplace.ReplaceButton}");
				//replaceButton.UseUnderline = true;
				
				// set te size groups to include the replace dialog
				labels.AddWidget(label2);
				combos.AddWidget(replacePatternEntry);
				helpButtons.AddWidget(replaceHelpButton);
				
				replaceHelpButton.Sensitive = false;
				ReplaceDialogPointer = this.ReplaceInFilesDialogWidget;
			}
			else
			{
				ReplaceDialogPointer = this.FindInFilesDialogWidget;
			}
			ReplaceDialogPointer.TransientFor = (Gtk.Window)WorkbenchSingleton.Workbench;
		}

		protected void OnClosed()
		{
			//SaveHistoryValues();
		}
		
		void OnDeleted (object o, DeleteEventArgs args)
		{
			// perform the standard closing windows event
			OnClosed();
			SearchReplaceInFilesManager.ReplaceDialog = null;
		}

		public void Present ()
		{
			ReplaceDialogPointer.Present ();
		}

		public void Destroy ()
		{
			ReplaceDialogPointer.Destroy ();
		}

		void CloseDialogEvent(object sender, EventArgs e)
		{
			ReplaceDialogPointer.Hide();
			OnClosed ();
		}

		public void ShowAll ()
		{
			ReplaceDialogPointer.ShowAll ();
			SearchReplaceInFilesManager.ReplaceDialog = this;
			searchPatternEntry.GtkEntry.SelectRegion (0, searchPatternEntry.GtkEntry.Text.Length);
		}

		public ReplaceInFilesDialog(bool replaceMode)
		{
			this.replaceMode = replaceMode;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
			string dialogName = (replaceMode) ? "ReplaceInFilesDialogWidget" : "FindInFilesDialogWidget";
			Glade.XML glade = new XML (null, "texteditoraddin.glade", dialogName, null);
			glade.Autoconnect (this);
			InitDialog ();
			/*
			if (replaceMode) {
				this.SetupFromXml(propertyService.DataDirectory + @"\resources\dialogs\ReplaceInFilesDialog.xfrm");
				ControlDictionary["replacePatternEntry"].Text = SearchReplaceInFilesManager.SearchOptions.ReplacePattern;
				ControlDictionary["replaceHelpButton"].Enabled = false;
			} else {
				this.SetupFromXml(propertyService.DataDirectory + @"\resources\dialogs\FindInFilesDialog.xfrm");
			}*/
			
			//ControlDictionary["findHelpButton"].Enabled = false;
			//ControlDictionary["searchPatternEntry"].Text = SearchReplaceInFilesManager.SearchOptions.SearchPattern;
			
			//AcceptButton = (Button)ControlDictionary["findButton"];
			//CancelButton = (Button)ControlDictionary["closeButton"];
			Gtk.MenuItem tmpItem;
			Gtk.Menu stratMenu = new Gtk.Menu ();
			tmpItem = new Gtk.MenuItem (GettextCatalog.GetString ("Wildcards"));
			stratMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (GettextCatalog.GetString ("Regular Expressions"));
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
			tmpItem = new Gtk.MenuItem (GettextCatalog.GetString ("Directories"));
			locMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (GettextCatalog.GetString ("All open files"));
			locMenu.Append (tmpItem);
			tmpItem = new Gtk.MenuItem (GettextCatalog.GetString ("Whole project"));
			locMenu.Append (tmpItem);
			searchLocationComboBox.Menu = locMenu;
						
			index = 0;
			switch (SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType) {
				case DocumentIteratorType.AllOpenFiles:
					index = 1;
					break;
				case DocumentIteratorType.WholeCombine:
					index = 2;
					break;
			}
			
			searchLocationComboBox.SetHistory (index);
			
			searchLocationComboBox.Changed += new EventHandler(SearchLocationCheckBoxChangedEvent);
			
			useSpecialSearchStrategyCheckBox.Toggled += new EventHandler(SpecialSearchStrategyCheckBoxChangedEvent);
			
			directoryTextBox.Text = SearchReplaceInFilesManager.SearchOptions.SearchDirectory;
			fileMaskTextBox.Text = SearchReplaceInFilesManager.SearchOptions.FileMask;
			includeSubdirectoriesCheckBox.Active = SearchReplaceInFilesManager.SearchOptions.SearchSubdirectories;
			
			browseButton.Clicked += new EventHandler(BrowseDirectoryEvent);
			findButton.Clicked += new EventHandler(FindEvent);
			
			searchPatternEntry.GtkEntry.Text = SearchReplaceInFilesManager.SearchOptions.SearchPattern;
			
			if (replaceMode) {
				replaceAllButton.Clicked += new EventHandler(ReplaceEvent);
				replacePatternEntry.GtkEntry.Text = SearchReplaceInFilesManager.SearchOptions.ReplacePattern;
			}
			
			ReplaceDialogPointer.Close += new EventHandler (CloseDialogEvent);
			closeButton.Clicked += new EventHandler (CloseDialogEvent);
			ReplaceDialogPointer.DeleteEvent += new DeleteEventHandler (OnDeleted);
			
			SearchLocationCheckBoxChangedEvent(null, null);
			SpecialSearchStrategyCheckBoxChangedEvent(null, null);
		}
		
		void FindEvent(object sender, EventArgs e)
		{
			if (SetupSearchReplaceInFilesManager()) {
				SearchReplaceInFilesManager.FindAll();
			}
		}
		
		void ReplaceEvent(object sender, EventArgs e)
		{
			if (SetupSearchReplaceInFilesManager()) {
				SearchReplaceInFilesManager.ReplaceAll();
			}
		}
		
		void BrowseDirectoryEvent(object sender, EventArgs e)
		{
			PropertyService PropertyService = (PropertyService)ServiceManager.GetService (typeof (PropertyService));			
			FolderDialog fd = new FolderDialog(GettextCatalog.GetString ("Select directory"));

			// set up the dialog to point to currently selected folder, or the default projects folder
			string defaultFolder = this.directoryTextBox.Text;	
			if (defaultFolder == string.Empty || defaultFolder == null) {
				// only use the bew project default path if there is no path set
				defaultFolder =	PropertyService.GetProperty (
						"MonoDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", 
						System.IO.Path.Combine (
							System.Environment.GetEnvironmentVariable ("HOME"),
							"Projects")).ToString ();
			}

			if (fd.Run() == (int)Gtk.ResponseType.Ok)
			{
				directoryTextBox.Text = fd.Filename;
			}
			fd.Hide ();
		}
		
		void SearchLocationCheckBoxChangedEvent(object sender, EventArgs e)
		{
			bool enableDirectorySearch = searchLocationComboBox.History == 0;
			fileMaskTextBox.Sensitive = enableDirectorySearch;
			directoryTextBox.Sensitive = enableDirectorySearch;
			browseButton.Sensitive = enableDirectorySearch;
			includeSubdirectoriesCheckBox.Sensitive = enableDirectorySearch;
		}
		
		void SpecialSearchStrategyCheckBoxChangedEvent(object sender, EventArgs e)
		{
			//CheckBox cb = (CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"];
			//if (cb != null) {
				specialSearchStrategyComboBox.Sensitive = useSpecialSearchStrategyCheckBox.Active;
			//}
		}
		
		bool SetupSearchReplaceInFilesManager()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
			
			string directoryName = directoryTextBox.Text;
			string fileMask      = fileMaskTextBox.Text;
			if (fileMask == null || fileMask.Length == 0) {
				fileMask = "*";
			}
			
			if (SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType == DocumentIteratorType.Directory) {
				
				if (!fileUtilityService.IsValidFileName(directoryName)) {
					messageService.ShowErrorFormatted(GettextCatalog.GetString ("Invalid directory name: {0}"), directoryName);
					return false;
				}
				
				if (!Directory.Exists(directoryName)) {
					messageService.ShowErrorFormatted (GettextCatalog.GetString ("Invalid directory name: {0}"), directoryName);
					return false;
				}
				
				if (!fileUtilityService.IsValidFileName(fileMask) || fileMask.IndexOf('\\') >= 0) {
					messageService.ShowErrorFormatted(GettextCatalog.GetString ("Invalid file mask: {0}"), fileMask);
					return false;
				}
			}
			if (fileMask == null || fileMask.Length == 0) {
				SearchReplaceInFilesManager.SearchOptions.FileMask = "*";
			} else {
				SearchReplaceInFilesManager.SearchOptions.FileMask        = fileMask;
			}
			SearchReplaceInFilesManager.SearchOptions.SearchDirectory = directoryName;
			SearchReplaceInFilesManager.SearchOptions.SearchSubdirectories = includeSubdirectoriesCheckBox.Active;
			
			SearchReplaceInFilesManager.SearchOptions.SearchPattern  = searchPatternEntry.GtkEntry.Text;
			if (replaceMode) {
				SearchReplaceInFilesManager.SearchOptions.ReplacePattern = replacePatternEntry.GtkEntry.Text;
			}
			
			SearchReplaceInFilesManager.SearchOptions.IgnoreCase          = !ignoreCaseCheckBox.Active;
			SearchReplaceInFilesManager.SearchOptions.SearchWholeWordOnly = searchWholeWordOnlyCheckBox.Active;
			
			if (useSpecialSearchStrategyCheckBox.Active) {
				switch (specialSearchStrategyComboBox.History) {
					case 0:
						SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.Wildcard;
						break;
					case 1:
						SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.RegEx;
						break;
				}
			} else {
				SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.Normal;
			}
			
			switch (searchLocationComboBox.History) {
				case 0:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.Directory;
					break;
				case 1:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.AllOpenFiles;
					break;
				case 2:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.WholeCombine;
					break;
			}
			return true;
		}
	}
}
