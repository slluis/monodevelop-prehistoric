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
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns.Codons;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	
	public class LoadSavePanel : AbstractOptionPanel
	{
		//FIXME: Add mneumonics for Window, Macintosh and Unix Radio Buttons. 
		//       Remove mneumonic from terminator label.

		LoadSavePanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new LoadSavePanelWidget ());
		}
		
		public override bool StorePanelContents()
		{			
			bool succes = widget.Store();
			return succes;
		}
		
		class LoadSavePanelWidget : GladeWidgetExtract 
		{
			//
			// Gtk controls
			//
			[Glade.Widget] public Gnome.FileEntry projectLocationTextBox;
			[Glade.Widget] public Gtk.CheckButton loadUserDataCheckButton;
			[Glade.Widget] public Gtk.CheckButton createBackupCopyCheckButton;
			[Glade.Widget] public Gtk.CheckButton loadPrevProjectCheckButton;
			[Glade.Widget] public Gtk.RadioButton windowsRadioButton; 
			[Glade.Widget] public Gtk.RadioButton macintoshRadioButton;
			[Glade.Widget] public Gtk.RadioButton unixRadioButton;
			[Glade.Widget] public Gtk.Label loadLabel;		
			[Glade.Widget] public Gtk.Label saveLabel;
			[Glade.Widget] public Gtk.Label terminatorLabel;	
			[Glade.Widget] public Gtk.Label locationLabel;
			
			public enum LineTerminatorStyle {
				Windows,
				Macintosh,
				Unix
			}
			
			public LoadSavePanelWidget () : base ("Base.glade", "LoadSavePanel")
			{
				//
				// load the internationalized strings.
				//
				projectLocationTextBox.GtkEntry.Text = Runtime.Properties.GetProperty(
					"MonoDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", 
					System.IO.Path.Combine(System.Environment.GetEnvironmentVariable ("HOME"),
							"Projects")).ToString();
				projectLocationTextBox.DirectoryEntry = true;
				//
				// setup the properties
				//
				loadUserDataCheckButton.Active = Runtime.Properties.GetProperty (
					"SharpDevelop.LoadDocumentProperties", true);
				createBackupCopyCheckButton.Active = Runtime.Properties.GetProperty (
					"SharpDevelop.CreateBackupCopy", false);
				loadPrevProjectCheckButton.Active = (bool) Runtime.Properties.GetProperty(
					"SharpDevelop.LoadPrevProjectOnStartup", false);
				
				if (LineTerminatorStyle.Windows.Equals (
					    Runtime.Properties.GetProperty (
						    "SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix)))  {
					windowsRadioButton.Active = true;}
				else if  (LineTerminatorStyle.Macintosh.Equals (
						  Runtime.Properties.GetProperty 
						  ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix)))  {
					macintoshRadioButton.Active = true;}
				else if (LineTerminatorStyle.Unix.Equals (
						 Runtime.Properties.GetProperty (
							 "SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix))) {
					unixRadioButton.Active = true;}
				
				// FIXME: renable all terminator style radio buttons when they're implemented.
				unixRadioButton.Sensitive = false;
				macintoshRadioButton.Sensitive = false;
				windowsRadioButton.Sensitive = false;
				terminatorLabel.Sensitive = false;
			}
			
			public bool Store () 
			{
				Runtime.Properties.SetProperty("SharpDevelop.LoadPrevProjectOnStartup", loadPrevProjectCheckButton.Active);
				Runtime.Properties.SetProperty ("SharpDevelop.LoadDocumentProperties",  loadUserDataCheckButton.Active);
				Runtime.Properties.SetProperty ("SharpDevelop.CreateBackupCopy",        createBackupCopyCheckButton.Active);
				
				if (windowsRadioButton.Active) {
					Runtime.Properties.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);} 
				else if (macintoshRadioButton.Active) {
					Runtime.Properties.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Macintosh);} 
				else if (unixRadioButton.Active){
					Runtime.Properties.SetProperty ("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix);}
				
				// check for correct settings
				string projectPath = projectLocationTextBox.GtkEntry.Text;
				if (projectPath.Length > 0) {
					if (!Runtime.FileUtilityService.IsValidFileName(projectPath)) {
						Runtime.MessageService.ShowError("Invalid project path specified");
						return false;
					}
				}
				Runtime.Properties.SetProperty("MonoDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);
				
				return true;
			}
		}
	}
}

