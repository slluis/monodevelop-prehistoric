// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels 
{
	public class CodeTemplatePane : AbstractOptionPanel
	{
		class CodeTemplatePanelWidget : GladeWidgetExtract {
			// members
			ArrayList templateGroups;
			int       currentSelectedGroup = -1;
			
			// Services
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			MessageService MessageService = (MessageService)ServiceManager.Services.GetService(typeof(MessageService));					
			
			// Gtk widgets
			[Glade.Widget] Label extensionLabel;
			[Glade.Widget] Gtk.TreeView templateListView;
			ListStore templateListViewStore = new ListStore(typeof(CodeTemplate));
			[Glade.Widget] Button removeButton;
			[Glade.Widget] Button addButton;
			[Glade.Widget] Button editButton;
			[Glade.Widget] Button addGroupButton;
			[Glade.Widget] Button editGroupButton;
			[Glade.Widget] Button removeGroupButton;
			TextBuffer templateTextBuffer = new TextBuffer(null);
			[Glade.Widget] Gtk.TextView templateTextView;
			[Glade.Widget] OptionMenu groupOptionMenu;
			Menu groupMenu;
			
			public CodeTemplateGroup CurrentTemplateGroup {
				get {
					if (currentSelectedGroup < 0 || currentSelectedGroup >= templateGroups.Count) {
						return null;
					}
					return (CodeTemplateGroup)templateGroups[currentSelectedGroup];
				}
			}
			
			public ArrayList TemplateGroups {
				get {
					return this.templateGroups;
				}
			}
			
			public CodeTemplatePanelWidget (ArrayList templateGroups) : base ("texteditoraddin.glade", "CodeTemplatePane")
			{
				this.templateGroups = templateGroups;
				
				SetLabels();
				
				// set up the treeview
				templateListView.Reorderable = false;
				templateListView.HeadersVisible = true;
				templateListView.Selection.Mode = SelectionMode.Multiple;
				templateListView.Model = templateListViewStore;
				
				// set up the text view
				templateTextView.Buffer = templateTextBuffer;
				//templateTextView.Font = new System.Drawing.Font("Courier New", 10f);
				
				// wire in the events
				removeButton.Clicked += new System.EventHandler(RemoveEvent);
				addButton.Clicked    += new System.EventHandler(AddEvent);
				editButton.Clicked   += new System.EventHandler(EditEvent);
				
				addGroupButton.Clicked    += new System.EventHandler(AddGroupEvent);
				editGroupButton.Clicked += new System.EventHandler(EditGroupEvent);
				removeGroupButton.Clicked += new System.EventHandler(RemoveGroupEvent);
				templateListView.RowActivated		+= new GtkSharp.RowActivatedHandler(RowActivatedEvent);
				templateListView.Selection.Changed 	+= new EventHandler(IndexChange);
				templateTextBuffer.Changed += new EventHandler(TextChange);
				
				if (templateGroups.Count > 0) {
					currentSelectedGroup = 0;
				}
				
				FillGroupOptionMenu();
				BuildListView();
				IndexChange(null, null);
				SetEnabledStatus();
			}
			
			void SetLabels()
			{
				extensionLabel.TextWithMnemonic = StringParserService.Parse("${res:Dialog.Options.CodeTemplate.ExtensionsLabel}");
				removeButton.Label = StringParserService.Parse("${res:Global.RemoveButtonText}");
				addButton.Label = StringParserService.Parse("${res:Global.AddButtonText}");
				editButton.Label = StringParserService.Parse("${res:Global.EditButtonText}");
				addGroupButton.Label = StringParserService.Parse("${res:Dialog.Options.CodeTemplate.AddGroupLabel}");
				// FIXME: make this use the resource file for the label
				editGroupButton.Label = "Ed_it Group";
				removeGroupButton.Label = StringParserService.Parse("${res:Dialog.Options.CodeTemplate.RemoveGroupLabel}");
				
				CellRendererText textRenderer = new CellRendererText ();
				
				// and listview columns 
				templateListView.AppendColumn (
					StringParserService.Parse("${res:Dialog.Options.CodeTemplate.Template}"), 
					textRenderer,  
					new TreeCellDataFunc(TemplateListViewCellDataFunc));
				templateListView.AppendColumn (
					StringParserService.Parse("${res:Dialog.Options.CodeTemplate.Description}"), 
					textRenderer, 
					new TreeCellDataFunc(TemplateListViewCellDataFunc));
			}
			
			// function to render the cell
			void TemplateListViewCellDataFunc(TreeViewColumn column, CellRenderer renderer, TreeModel model, TreeIter iter)
			{
				string toWrite = string.Empty;
				
				CodeTemplate codeTemplate = ((ListStore)model).GetValue(iter, 0) as CodeTemplate;
				
				if(column.Title == StringParserService.Parse("${res:Dialog.Options.CodeTemplate.Template}"))
				{
					// first column
					((CellRendererText)renderer).Text = codeTemplate.Shortcut;
				}
				else
				{
					// second column
					((CellRendererText)renderer).Text = codeTemplate.Description;
				}
			}
			
			void SetEnabledStatus()
			{
				bool groupSelected = CurrentTemplateGroup != null;
				bool groupsEmpty   = templateGroups.Count != 0;
				
				SetEnabledStatus(groupSelected, addButton, editButton, removeButton, templateListView, templateTextView);
				SetEnabledStatus(groupsEmpty, groupOptionMenu, extensionLabel);
				if (groupSelected) {
					bool oneItemSelected = templateListView.Selection.CountSelectedRows() == 1;
					bool isItemSelected  = templateListView.Selection.CountSelectedRows() > 0;
					SetEnabledStatus(oneItemSelected, editButton, templateTextView);
					SetEnabledStatus(isItemSelected, removeButton);
				}
			}
			
			// disables or enables (sets sensitivty) a specified array of widgets
			public void SetEnabledStatus(bool enabled, params Widget[] controls)
			{
				foreach (Widget control in controls) {				
					if (control == null) {
						MessageService.ShowError("Control not found!");
					} else {
						control.Sensitive = enabled;
					}
				}
			}
			
	#region GroupComboBox event handler
			void SetGroupSelection(object sender, EventArgs e)
			{
				currentSelectedGroup = groupOptionMenu.History;
				BuildListView();
			}		
			
	#endregion
			
	#region Group Button events
			void AddGroupEvent(object sender, EventArgs e)
			{
				CodeTemplateGroup templateGroup = new CodeTemplateGroup(".???");
				if(ShowEditTemplateGroupDialog(ref templateGroup, "New ")) {
					templateGroups.Add(templateGroup);
					FillGroupOptionMenu();
					groupOptionMenu.SetHistory((uint) templateGroups.Count - 1);
					SetEnabledStatus();
				}
			}
			
			void EditGroupEvent(object sender, EventArgs e)
			{
				
				int index = groupOptionMenu.History;
				CodeTemplateGroup templateGroup = (CodeTemplateGroup) templateGroups[index];
				if(ShowEditTemplateGroupDialog(ref templateGroup, "Edit ")) {
					templateGroups[index] = templateGroup;
					FillGroupOptionMenu();
					groupOptionMenu.SetHistory((uint)index);
					SetEnabledStatus();
				}
			}
			
			void RemoveGroupEvent(object sender, EventArgs e)
			{
				if (CurrentTemplateGroup != null) {
					templateGroups.RemoveAt(currentSelectedGroup);
					if (templateGroups.Count == 0) {
						currentSelectedGroup = -1;
					} else {
						groupOptionMenu.SetHistory((uint) Math.Min(currentSelectedGroup, templateGroups.Count - 1));
					}
					FillGroupOptionMenu();
					BuildListView();
					SetEnabledStatus();
				}
			}
			
			bool ShowEditTemplateGroupDialog(ref CodeTemplateGroup templateGroup, string title)
			{
				using (EditTemplateGroupDialog etgd = new EditTemplateGroupDialog(templateGroup, title)) {
					return (etgd.Run() == (int) Gtk.ResponseType.Ok);
				}
			}
	#endregion
			
	#region Template Button events
			void RemoveEvent(object sender, System.EventArgs e)
			{
				// first copy the selected item paths into a temp array
				int maxIndex = -1;				
				int selectedItemCount = templateListView.Selection.CountSelectedRows();
				TreeIter[] selectedIters = new TreeIter[selectedItemCount];
				TreeModel lv;
				GLib.List pathList = templateListView.Selection.GetSelectedRows(out lv);								
				for(int i = 0; i < selectedItemCount; i++) {
					TreePath path = (TreePath) pathList[i];
					((ListStore)lv).GetIter(out selectedIters[i], path);
					maxIndex = path.Indices[0];
				}
				
				// now delete each item in that list
				foreach(TreeIter toDelete in selectedIters) {
					TreeIter itr = toDelete;					
					((ListStore)lv).Remove(out itr);
				}
				
				StoreTemplateGroup();
				
				// try and select the next item after the one removed
				if(maxIndex > -1) {
					templateListView.Selection.UnselectAll();
					TreeIter nextIter;
					maxIndex += 1- selectedItemCount;
					if(templateListViewStore.GetIterFromString(out nextIter, maxIndex.ToString())) {				
						// select the next one
						templateListView.Selection.SelectIter(nextIter);
					} else {
						// select the very last one
						maxIndex = templateListViewStore.IterNChildren() - 1;
						if(templateListViewStore.GetIterFromString(out nextIter, (maxIndex).ToString())) {
							templateListView.Selection.SelectIter(nextIter);
						}
					}
				}
			}
			
			void AddEvent(object sender, System.EventArgs e)
			{
				CodeTemplate newTemplate = new CodeTemplate();
				using (EditTemplateDialog etd = new EditTemplateDialog(newTemplate)) {
					if (etd.Run() == (int) Gtk.ResponseType.Ok) {
						CurrentTemplateGroup.Templates.Add(newTemplate);						
						templateListView.Selection.UnselectAll();
						BuildListView();
						
						// select the newly added last one
						TreeIter nextIter;
						int maxIndex = templateListViewStore.IterNChildren() - 1;
						if(templateListViewStore.GetIterFromString(out nextIter, (maxIndex).ToString())) {
							templateListView.Selection.SelectIter(nextIter);
						}
					}
				}
			}
			
			void EditEvent(object sender, System.EventArgs e)
			{
				TreeIter selectedIter;
				TreeModel ls;				
				if(((ListStore)templateListView.Model).GetIter(out selectedIter, (TreePath) templateListView.Selection.GetSelectedRows(out ls)[0])) {
					CodeTemplate template = ls.GetValue(selectedIter, 0) as CodeTemplate;
					
					using (EditTemplateDialog etd = new EditTemplateDialog(template)) {
						if (etd.Run() == (int) Gtk.ResponseType.Ok) {
							ls.SetValue(selectedIter, 0, template);
							StoreTemplateGroup();
						}
					}					
					
					// select the newly edited item
					templateListView.Selection.SelectIter(selectedIter);
				}				
			}
			
			// raised when a treeview row is double clicked on
			void RowActivatedEvent(object sender, GtkSharp.RowActivatedArgs ra)
			{
				EditEvent(sender, System.EventArgs.Empty);
			}
	#endregion
			
			void FillGroupOptionMenu()
			{
				groupOptionMenu.Changed -= new EventHandler(SetGroupSelection);
				
				// remove the menu items
				groupOptionMenu.RemoveMenu();
				groupMenu = new Menu();
				
				
				foreach (CodeTemplateGroup templateGroup in templateGroups) {					
					groupMenu.Append(Gtk.MenuItem.NewWithLabel(String.Join(";", templateGroup.ExtensionStrings)));					
				}
				
				groupMenu.ShowAll();
				groupOptionMenu.Menu = groupMenu;
				if (currentSelectedGroup >= 0) {
					groupOptionMenu.SetHistory((uint)currentSelectedGroup);
				}
				
				groupOptionMenu.Changed += new EventHandler(SetGroupSelection);
			}
			
			void IndexChange(object sender, System.EventArgs e)
			{
				if(templateListView.Selection.CountSelectedRows() == 1) {
					TreeIter selectedIter;
					TreeModel listStore;				
					((ListStore)templateListView.Model).GetIter(out selectedIter, (TreePath) templateListView.Selection.GetSelectedRows(out listStore)[0]);
					templateTextBuffer.Text    = ((CodeTemplate)listStore.GetValue(selectedIter, 0)).Text;
				} else {
					templateTextBuffer.Text    = String.Empty;
				}
				SetEnabledStatus();
			}
			
			void TextChange(object sender, EventArgs e)
			{
				if(templateListView.Selection.CountSelectedRows() == 1) {
					TreeIter selectedIter;
					TreeModel listStore;				
					((ListStore)templateListView.Model).GetIter(out selectedIter, (TreePath) templateListView.Selection.GetSelectedRows(out listStore)[0]);
					((CodeTemplate)listStore.GetValue(selectedIter, 0)).Text = templateTextBuffer.Text;
				}
			}
			
			void StoreTemplateGroup()
			{
				if (CurrentTemplateGroup != null) {
					CurrentTemplateGroup.Templates.Clear();
						
					TreeIter first;			
					if(templateListView.Model.GetIterFirst(out first)) {
						TreeIter current = first;
						
						// loop through items in the tree, adding each item to the template group
						do {
							CurrentTemplateGroup.Templates.Add(templateListView.Model.GetValue(current, 0)); 
						} while(templateListView.Model.IterNext(out current));
					}
				}
			}
			
			void BuildListView()
			{
				ListStore listStore = templateListView.Model as ListStore;
				listStore.Clear();
				if (CurrentTemplateGroup != null) {
					foreach (CodeTemplate template in CurrentTemplateGroup.Templates) {
						listStore.AppendValues(template);					
					}
				}
				IndexChange(this, EventArgs.Empty);
			}
		}
		
		CodeTemplatePanelWidget widget;
		
		public override void LoadPanelContents()
		{
			// create a new CodeTemplatePanelWidget using glade files and add it
			// pass in the template groups that it needs
			this.Add (widget = new CodeTemplatePanelWidget (CodeTemplateLoader.TemplateGroups));
		}
			
		public override bool StorePanelContents()
		{
			// get template groups from widhet and save them
			CodeTemplateLoader.TemplateGroups = widget.TemplateGroups;
			CodeTemplateLoader.SaveTemplates();
			return true;
		}		
	}
}
