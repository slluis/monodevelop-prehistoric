// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	public class CombineStartupPanel : AbstractOptionPanel
	{
		CombineStartupPanelWidget widget;
		
		class CombineStartupPanelWidget : GladeWidgetExtract 
		{
			// Gtk Controls
			[Glade.Widget] Label ActionLabel;
 			[Glade.Widget] RadioButton singleRadioButton;
 			[Glade.Widget] RadioButton multipleRadioButton;
 			[Glade.Widget] OptionMenu singleOptionMenu;
 			[Glade.Widget] OptionMenu actionOptionMenu;
   			[Glade.Widget] Button moveUpButton;
 			[Glade.Widget] Button moveDownButton;
 			[Glade.Widget] VBox multipleBox;			
 			[Glade.Widget] Gtk.TreeView entryTreeView;
 			public ListStore store;

			// Services
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (
										typeof (StringParserService));
			static ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(
										typeof(IResourceService));
			static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(
										typeof(PropertyService));
			Combine combine;

			public  CombineStartupPanelWidget(IProperties CustomizationObject) : 
				base ("Base.glade", "CombineStartupPanel")
			{
				this.combine = (Combine)((IProperties)CustomizationObject).GetProperty("Combine");


				ActionLabel.Text = StringParserService.Parse(
					"${res:Dialog.Options.CombineOptions.Startup.ActionLabel}");

				// Setting up RadioButtons

				singleRadioButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.CombineOptions.Startup.SingleStartupRadioButton}");
				singleRadioButton.Active = combine.SingleStartupProject;
				singleRadioButton.Clicked += new EventHandler(OnSingleRadioButtonClicked);
				multipleRadioButton.Label =  StringParserService.Parse(
					"${res:Dialog.Options.CombineOptions.Startup.MultipleStartupRadioButton}");
				multipleRadioButton.Active = !combine.SingleStartupProject;
				singleRadioButton.Clicked += new EventHandler(OptionsChanged);

				// Setting up OptionMenus
				Menu singleMenu = new Menu ();
				for (int i =0;  i < combine.Entries.Count; i++)  {
					CombineEntry entry = (CombineEntry) combine.Entries[i];
					singleMenu.Append( new MenuItem(entry.Name));
						
					if (combine.SingleStartProjectName == entry.Name){
						singleMenu.SetActive ( (uint) i);
					}
				}
				singleOptionMenu.Menu = singleMenu;

				Menu actionMenu = new Menu ();
				actionMenu.Append( new MenuItem (resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.None")));
				actionMenu.Append( new MenuItem (resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.Execute")));
				actionOptionMenu.Menu = actionMenu ;
				actionOptionMenu.Changed += new EventHandler(OptionsChanged);

				// Populating entryTreeView					
				CombineExecuteDefinition edef;
 				store = new ListStore (typeof(string), typeof(string), typeof(CombineExecuteDefinition) );
				entryTreeView.Model = store;
				
				TreeIter iter = new TreeIter ();
 				string entryHeader = StringParserService.Parse(
 					"${res:Dialog.Options.CombineOptions.Startup.EntryColumnHeader}");	
 				entryTreeView.AppendColumn (entryHeader, new CellRendererText (), "text", 0);
 				string actionHeader = StringParserService.Parse(
 					"${res:Dialog.Options.CombineOptions.Startup.ActionColumnHeader}");
 				entryTreeView.AppendColumn (actionHeader, new CellRendererText (), "text", 1);
				
				// sanity check to ensure we had a proper execture definitions save last time rounf
				if(combine.CombineExecuteDefinitions.Count == combine.Entries.Count) {
					// add the previously saved execute definitions to the treeview list
					for (int n = 0; n < combine.CombineExecuteDefinitions.Count; n++) {
						edef = (CombineExecuteDefinition)combine.CombineExecuteDefinitions[n];
						string action = edef.Type == EntryExecuteType.None ? resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.None") : resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.Execute");
						iter = store.AppendValues (edef.Entry.Name, action, edef);
					}
				} else {
					// add an empty set of execute definitions
					for (int n = 0; n < combine.Entries.Count; n++) {
						edef = new CombineExecuteDefinition ((CombineEntry) combine.Entries[n],EntryExecuteType.None);
						string action = edef.Type == EntryExecuteType.None ? resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.None") : resourceService.GetString(
								"Dialog.Options.CombineOptions.Startup.Action.Execute");
						iter = store.AppendValues (edef.Entry.Name, action, edef);
					}
					
					// tell the user we encountered and worked around an issue 
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					// FIXME: il8n this
					messageService.ShowError(
						"The Combine Execute Definitions for this Combine were invalid. A new empty set of Execute Definitions has been created.");
				}
					
 				entryTreeView.Selection.Changed += new EventHandler(SelectedEntryChanged);
				entryTreeView.Selection.SelectPath(new TreePath ("0"));
				
				// Setting up Buttons
				moveUpButton.Clicked += new EventHandler(OnMoveUpButtonClicked);
				moveDownButton.Clicked += new EventHandler(OnMoveDownButtonClicked);

				OnSingleRadioButtonClicked(null, null);				
			}
						
			protected void OnMoveUpButtonClicked(object sender, EventArgs e)
			{
				if(entryTreeView.Selection.CountSelectedRows() == 1)
				{
					TreeIter selectedItem;
					TreeModel ls;				
					((ListStore)entryTreeView.Model).GetIter(
						out selectedItem, (TreePath) entryTreeView.Selection.GetSelectedRows(out ls)[0]);
					// we know we have a selected item so get it's index
					// use that to get the path of the item before it, and swap the two
					int index = GetSelectedIndex(entryTreeView);
					// only swap if at the top
					if(index > 0)
					{
						TreeIter prev; 
						if(entryTreeView.Model.GetIterFromString(out prev, (index - 1).ToString()))
						{
							((ListStore)ls).Swap(selectedItem, prev);
						}
					}
				}
			}
			

			protected void OnMoveDownButtonClicked(object sender, EventArgs e)
			{
				if(entryTreeView.Selection.CountSelectedRows() == 1)
				{
					TreeIter selectedItem;
					TreeModel ls;				
					((ListStore)entryTreeView.Model).GetIter(
						out selectedItem, (TreePath) entryTreeView.Selection.GetSelectedRows(out ls)[0]);
					// swap it with the next one
					TreeIter toSwap = selectedItem;
					if(ls.IterNext(out toSwap))
					{
						((ListStore)ls).Swap(selectedItem, toSwap);
					}
				}
			}
			
			void OnSingleRadioButtonClicked(object sender, EventArgs e)
			{
				multipleBox.Sensitive = multipleRadioButton.Active;
				singleOptionMenu.Sensitive = singleRadioButton.Active;
			}
			
  	       		void OptionsChanged(object sender, EventArgs e)
			{
				if(entryTreeView.Selection.CountSelectedRows() == 0){
					return;
				}
				TreeIter selectedItem;
				TreeModel ls;				
				((ListStore)entryTreeView.Model).GetIter(
					out selectedItem, (TreePath) entryTreeView.Selection.GetSelectedRows(out ls)[0]);
				
				int index = GetSelectedIndex(entryTreeView);
				CombineExecuteDefinition edef = (CombineExecuteDefinition) store.GetValue(selectedItem, 2);
				switch (actionOptionMenu.History) {
				case 0:
					edef.Type = EntryExecuteType.None;
					break;
				case 1:
					edef.Type = EntryExecuteType.Execute;
					break;
				default:
					break;
				}
				store.SetValue(selectedItem, 2, edef);
				string action = edef.Type == EntryExecuteType.None ? resourceService.GetString(
					"Dialog.Options.CombineOptions.Startup.Action.None") : resourceService.GetString(
						"Dialog.Options.CombineOptions.Startup.Action.Execute");
				store.SetValue(selectedItem, 1, action);

			}
			
			void SelectedEntryChanged(object sender, EventArgs e)
			{
				if(entryTreeView.Selection.CountSelectedRows() == 0){
					return;
				}
				
				TreeIter selectedItem;
				TreeModel ls;				
				
				((ListStore)entryTreeView.Model).GetIter(
					out selectedItem, (TreePath) entryTreeView.Selection.GetSelectedRows(out ls)[0]);
				
				string txt = (string) store.GetValue(selectedItem,1);
				
				if (txt == resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None")) {
				actionOptionMenu.SetHistory (0);
				} else {
					actionOptionMenu.SetHistory (1);
				}
			}
			
			// added this event to get the last select row index from gtk TreeView
			int GetSelectedIndex(Gtk.TreeView tv)
			{
				if(entryTreeView.Selection.CountSelectedRows() == 1)
				{
					TreeIter selectedIter;
					TreeModel lv;				
					((ListStore)entryTreeView.Model).GetIter(
						out selectedIter, (TreePath) entryTreeView.Selection.GetSelectedRows(out lv)[0]);
					
					// return index of first level node (since only 1 level list model)
					return lv.GetPath(selectedIter).Indices[0];
				}
				else
				{
					return -1;
				}
			}

			public bool Store()
			{
				combine.SingleStartProjectName = ((CombineEntry)combine.Entries[singleOptionMenu.History]).Name;
				combine.SingleStartupProject   = singleRadioButton.Active;
				
				// write back new combine execute definitions
				combine.CombineExecuteDefinitions.Clear();
				TreeIter first;
				store.GetIterFirst(out first);
				TreeIter current = first;
				for (int i = 0; i < store.IterNChildren() ; ++i) {
					
					CombineExecuteDefinition edef = (CombineExecuteDefinition) store.GetValue(current, 2);					
					combine.CombineExecuteDefinitions.Add(edef);
					
					store.IterNext(out current);	
				}
				return true;
			}		
		}

		public override void LoadPanelContents()
		{
			Add (widget = new  CombineStartupPanelWidget ((IProperties) CustomizationObject));
		}

		public override bool StorePanelContents()
		{
		        bool success = widget.Store ();
 			return success;			
	       	}	
				
	}
}

