// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
//using System.Windows.Forms;
using Gtk;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public enum LineTerminatorStyle {
		Windows,
		Macintosh,
		Unix
	}
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoadSavePanel : AbstractOptionPanel
	{
		//FIXME: Hashtables are wrong here.
		//FIXME: Yes, this is a dirty hack.
		//FIXME: Lets use something else.
		Hashtable MenuToValue = new Hashtable ();	
		
		//
		// Gtk controls
		//
		Gtk.CheckButton loadUserDataCheckBox;
		Gtk.CheckButton createBackupCopyCheckBox;
		Gtk.Menu lineTerminatorStyleComboBoxMenu;
		Gtk.OptionMenu lineTerminatorStyleComboBox;
		
		// services needed
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		
		public override void LoadPanelContents()
		{
			SetupPanelInstance();
			
			//
			// load the data
			//
			loadUserDataCheckBox.Active = PropertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true);
			createBackupCopyCheckBox.Active = PropertyService.GetProperty("SharpDevelop.CreateBackupCopy", false);
		
			lineTerminatorStyleComboBoxMenu.Append(
				Gtk.MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}")));
			MenuToValue[0] = StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}");
			
			lineTerminatorStyleComboBoxMenu.Append(
				Gtk.MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}")));
			MenuToValue[1] = StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}");
			
			lineTerminatorStyleComboBoxMenu.Append(
				Gtk.MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}")));
			MenuToValue[2] = StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}");
			
			//FIXME: need Gtk# fix here for mapping menu item to index
			string selectedItem = Enum.GetName(typeof(LineTerminatorStyle), PropertyService.GetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix));			
			for(int i = 0; i < MenuToValue.Count; i++)
			{
				if(MenuToValue[i].ToString().Equals(selectedItem))
				{				
					lineTerminatorStyleComboBox.SetHistory((uint)i);
					break;
				}
			}
		
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("SharpDevelop.LoadDocumentProperties", loadUserDataCheckBox.Active);
			PropertyService.SetProperty("SharpDevelop.CreateBackupCopy",       createBackupCopyCheckBox.Active);
			PropertyService.SetProperty("SharpDevelop.LineTerminatorStyle",    (LineTerminatorStyle)lineTerminatorStyleComboBox.History);
			
			return true;
		}
		
		#region jba added methods
		
		private void SetupPanelInstance()
		{
			//
			// set up the load options
			//
			Gtk.Frame loadContainer = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadLabel}"));
			loadUserDataCheckBox = Gtk.CheckButton.NewWithLabel (StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadUserDataCheckBox}"));
			loadContainer.Add(loadUserDataCheckBox);
			
			//
			// setup the save options
			//			
			Gtk.Frame saveContainer = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.SaveLabel}"));
			Gtk.VBox saveVBox = new Gtk.VBox(false, 2);
			
			// the backup checkbox
			createBackupCopyCheckBox = Gtk.CheckButton.NewWithLabel (StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.CreateBackupCopyCheckBox}"));
			saveVBox.PackStart(createBackupCopyCheckBox, false, false, 2);
			
			// the terminator label 
			Gtk.Label label1 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.LineTerminatorStyleGroupBox}"));
			saveVBox.PackStart(label1, false, false, 2);
			
			// the terminator menu
			lineTerminatorStyleComboBoxMenu = new Gtk.Menu();
			lineTerminatorStyleComboBox = new Gtk.OptionMenu();
			lineTerminatorStyleComboBox.Menu = lineTerminatorStyleComboBoxMenu;
			saveVBox.PackStart(lineTerminatorStyleComboBox, false, false, 2);
			
			// add the vbox
			saveContainer.Add(saveVBox);
			
			// create the main box
			Gtk.VBox mainBox = new Gtk.VBox(false, 2);
			mainBox.PackStart(loadContainer, false, false, 2);
			mainBox.PackStart(saveContainer, false, false, 2);
			
			this.Add(mainBox);
		}
		
		#endregion
	}
}

