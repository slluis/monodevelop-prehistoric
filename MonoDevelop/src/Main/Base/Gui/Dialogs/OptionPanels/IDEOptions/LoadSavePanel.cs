// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
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
		//FIXME: Add mneumonics for Window, Macintosh and Unix Radio Buttons. 
		//       Remove mneumonic from terminator label.

		class LoadSavePanelWidget : GladeWidgetExtract 
		{
			
			//
			// Gtk controls
			//
			
			[Glade.Widget] public Gtk.CheckButton loadUserDataCheckBox;
			[Glade.Widget] public Gtk.CheckButton createBackupCopyCheckBox;
			[Glade.Widget] public Gtk.RadioButton windowsRadioButton, macintoshRadioButton, unixRadioButton;
			[Glade.Widget] public Gtk.Label load, save, terminator;			
			
			public LoadSavePanelWidget () : base ("Base.glade", "LoadSavePanel")
			{
				
			}
		}		
		
		public LoadSavePanel () : base ()
		{
		}

		public override Gtk.Image Icon {
			get {
				return new Gtk.Image (Gtk.Stock.SaveAs, Gtk.IconSize.Button);
			}
		}
		
		// services needed
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		
		LoadSavePanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new LoadSavePanelWidget ());
			
 			SetupPanelInstance();
			
 			//
 			// load the data
		        //
			widget.loadUserDataCheckBox.Active = PropertyService.GetProperty ("SharpDevelop.LoadDocumentProperties", true);
			widget.createBackupCopyCheckBox.Active = PropertyService.GetProperty ("SharpDevelop.CreateBackupCopy", false);

			if (LineTerminatorStyle.Windows.Equals (
				    PropertyService.GetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix)))  {
				widget.windowsRadioButton.Active = true;}
			else if  (LineTerminatorStyle.Macintosh.Equals (
					  PropertyService.GetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix)))  {
				widget.macintoshRadioButton.Active = true;}
			else if (LineTerminatorStyle.Unix.Equals (
					 PropertyService.GetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix))) {
				widget.unixRadioButton.Active = true;}
			// Finish here
			
			// FIXME: renable all terminatore style radio buttons when they're implemented
			widget.unixRadioButton.Sensitive = false;
			widget.macintoshRadioButton.Sensitive = false;
			widget.windowsRadioButton.Sensitive = false;
			widget.terminator.Sensitive = false;

		}
		
		public override bool StorePanelContents()
		{
 			PropertyService.SetProperty ("SharpDevelop.LoadDocumentProperties", widget.loadUserDataCheckBox.Active);
 			PropertyService.SetProperty ("SharpDevelop.CreateBackupCopy",       widget.createBackupCopyCheckBox.Active);
			if (widget.windowsRadioButton.Active) {
				PropertyService.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);} 
			else if (widget.macintoshRadioButton.Active) {
				PropertyService.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Macintosh);} 
			else if (widget.unixRadioButton.Active){
				PropertyService.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix);}
			
			return true;
		}
		
		#region jba added methods
		
		private void SetupPanelInstance()
		{
 			//
 			// set up the load options
 			//
			widget.load.Markup = "<b> " + StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadLabel}") + " </b>";
 			widget.loadUserDataCheckBox.Label = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.LoadUserDataCheckBox}");
 			//
 			// setup the save options
 			//			
 			widget.save.Markup = "<b> " + StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.SaveLabel}")+ "</b>";
 			// the backup checkbox
 			widget.createBackupCopyCheckBox.Label =StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.CreateBackupCopyCheckBox}");
 			// the terminator label 
 			widget.terminator.TextWithMnemonic  = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.LineTerminatorStyleGroupBox}");
 			// the terminator radiobutton
			widget.windowsRadioButton.Label = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}");
			widget.macintoshRadioButton.Label = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}");
			widget.unixRadioButton.Label = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}");
		}
		
		#endregion
	}
}

