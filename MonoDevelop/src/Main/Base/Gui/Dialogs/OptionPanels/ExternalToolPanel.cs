// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ExternalToolPane : AbstractOptionPanel
	{

		static string[,] argumentQuickInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemPath}",      "${ItemPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemFileName}",      "${ItemFileName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemExtension}",     "${ItemExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentLine}",   "${CurLine}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentColumn}", "${CurCol}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentText}",   "${CurText}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullTargetPath}",  "${TargetPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetExtension}", "${TargetExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectFileName}",  "${ProjectFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${CombineDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineFileName}",  "${CombineFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};
		
		static string[,] workingDirInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${CombineDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};
		
		// gtk controls
		ListStore toolListBoxStore;
		TreeView toolListBox;
		Entry titleTextBox; 
		Entry commandTextBox; 
		Entry argumentTextBox; 
		Entry workingDirTextBox; 
		CheckButton promptArgsCheckBox; 
		CheckButton useOutputPadCheckBox; 
		Label titleLabel; 
		Label argumentLabel; 
		Label commandLabel; 
		Label workingDirLabel; 
		Button browseButton; 
		Button argumentQuickInsertButton; 
		Button workingDirQuickInsertButton; 
		Button moveUpButton; 
		Button moveDownButton;
		Button addButton; 
		Button removeButton;
			
		// these are the control names which are enabled/disabled depending if tool is selected
		Widget[] dependendControls;
		
		// needed for treeview listbox
		int toolListBoxItemCount = 0;
		
		// Services
		FileUtilityService FileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		MessageService MessageService = (MessageService)ServiceManager.Services.GetService(typeof(MessageService));
// FIXME: when menu service is created
//		MenuService MenuService = (MenuService)ServiceManager.Services.GetService(typeof(MenuService));
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			SetupPanelInstance();
			
			// add each tool to the treeStore
			foreach (object o in ToolLoader.Tool) {
					toolListBoxStore.AppendValues(((ExternalTool)o).MenuCommand, (ExternalTool) o);
					toolListBoxItemCount ++;
			}
			
			toolListBox.Reorderable = false;
			toolListBox.HeadersVisible = true;
			toolListBox.Selection.Mode = SelectionMode.Multiple;
			toolListBox.Model = toolListBoxStore;
			
			toolListBox.AppendColumn (
				StringParserService.Parse("${res:Dialog.Options.ExternalTool.ToolsLabel}"), 
				new CellRendererText (), 
				"text", 
				0);
			
			
// FIXME: when menu service is created
//			MenuService.CreateQuickInsertMenu(argumentTextBox,
//			                                  argumentQuickInsertButton,
//			                                  argumentQuickInsertMenu);
			
// FIXME: when menu service is created
//			MenuService.CreateQuickInsertMenu(workingDirTextBox,
//			                                  workingDirQuickInsertButton,
//			                                  workingDirInsertMenu);
			
			toolListBox.Selection.Changed += new EventHandler(selectEvent);
			
			removeButton.Clicked   += new EventHandler(removeEvent);
			addButton.Clicked      += new EventHandler(addEvent);
			moveUpButton.Clicked   += new EventHandler(moveUpEvent);
			moveDownButton.Clicked += new EventHandler(moveDownEvent);
			
			browseButton.Clicked   += new EventHandler(browseEvent);
			
			selectEvent(this, EventArgs.Empty);
		}
		
		public override bool StorePanelContents()
		{
			ArrayList newlist = new ArrayList();
			TreeIter first;
			
			if(toolListBox.Model.GetIterFirst(out first))
			{
				TreeIter current = first;
				
				do {
				// loop through items in the tree
				
					ExternalTool tool = toolListBox.Model.GetValue(current, 1) as ExternalTool;
					if (!FileUtilityService.IsValidFileName(tool.Command)) {
						MessageService.ShowError(String.Format("The command of tool \"{0}\" is invalid.", tool.MenuCommand));
						return false;
					}
					if ((tool.InitialDirectory != "") && (!FileUtilityService.IsValidFileName(tool.InitialDirectory))) {
						MessageService.ShowError(String.Format("The working directory of tool \"{0}\" is invalid.", tool.MenuCommand));
						return false;
					}
					newlist.Add(tool);				 
				} while(toolListBox.Model.IterNext(out current));
			}
			ToolLoader.Tool = newlist;
			ToolLoader.SaveTools();
			return true;
		}
		
		void SetupPanelInstance()
		{
			// instantiate controls			
			toolListBoxStore = new ListStore(typeof(string), typeof(ExternalTool));
			toolListBox = new TreeView();
			toolListBox.SetSizeRequest(200, 150);
			titleTextBox = new Entry(); 
			commandTextBox = new Entry(); 
			argumentTextBox = new Entry(); 
			workingDirTextBox = new Entry(); 
			promptArgsCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.ExternalTool.PromptForArgsCheckBox}")); 
			useOutputPadCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.ExternalTool.UseOutputWindow}")); 
			titleLabel = new Label(StringParserService.Parse("${res:Dialog.Options.ExternalTool.TitleLabel}")); 
			argumentLabel = new Label(StringParserService.Parse("${res:Dialog.Options.ExternalTool.ArgumentLabel}")); 
			commandLabel = new Label(StringParserService.Parse("${res:Dialog.Options.ExternalTool.CommandLabel}")); 
			workingDirLabel = new Label(StringParserService.Parse("${res:Dialog.Options.ExternalTool.WorkingDirLabel}"));			
			browseButton = new Button("..."); 
			argumentQuickInsertButton = new Button(">"); 
			workingDirQuickInsertButton = new Button(">"); 
			moveUpButton = new Button(StringParserService.Parse("${res:Dialog.Options.ExternalTool.MoveUpButton}")); 
			moveDownButton = new Button(StringParserService.Parse("${res:Dialog.Options.ExternalTool.MoveDownButton}"));
			removeButton = new Button(StringParserService.Parse("${res:Global.RemoveButtonText}"));
			addButton = new Button(StringParserService.Parse("${res:Global.AddButtonText}"));
			
			dependendControls = new Widget[] {
				titleTextBox, commandTextBox, argumentTextBox, 
				workingDirTextBox, promptArgsCheckBox, useOutputPadCheckBox, 
				titleLabel, argumentLabel, commandLabel, 
				workingDirLabel, browseButton, argumentQuickInsertButton, 
				workingDirQuickInsertButton, moveUpButton, moveDownButton
			};
			
			// pack all the controls
			VBox vBox1 = new VBox(false, 2);
			vBox1.PackStart(addButton, false, false, 2);
			vBox1.PackStart(removeButton, false, false, 2);
			vBox1.PackEnd(moveDownButton, false, false, 2);
			vBox1.PackEnd(moveUpButton, false, false, 2);			
			
			HBox hBox1 = new HBox(false, 2);
			hBox1.PackStart(toolListBox, false, true, 2);
			hBox1.PackStart(vBox1, false, false, 2);
			
			Table table1 = new Table(4, 3, false);
			table1.Attach(titleLabel, 0, 1, 0, 1);
			table1.Attach(titleTextBox, 1, 3, 0, 1);
			table1.Attach(commandLabel, 0, 1, 1, 2);
			table1.Attach(commandTextBox, 1, 2, 1, 2);
			table1.Attach(browseButton, 2, 3, 1, 2);
			table1.Attach(argumentLabel, 0, 1, 2, 3);
			table1.Attach(argumentTextBox, 1, 2, 2, 3);
			table1.Attach(argumentQuickInsertButton, 2, 3, 2, 3);
			table1.Attach(workingDirLabel, 0, 1, 3, 4);
			table1.Attach(workingDirTextBox, 1, 2, 3, 4);
			table1.Attach(workingDirQuickInsertButton, 2, 3, 3, 4);
			
			VBox mainBox = new VBox(false, 2);
			mainBox.PackStart(hBox1, false, false, 2);
			mainBox.PackStart(table1, false, false, 2);
			mainBox.PackStart(promptArgsCheckBox, false, false, 2);
			mainBox.PackStart(useOutputPadCheckBox, false, false, 2);
			this.Add(mainBox);
		}
		
		void browseEvent(object sender, EventArgs e)
		{
			Gtk.FileSelection fs = new Gtk.FileSelection ("File to Open");
			int response = fs.Run ();
			string name = fs.Filename;
			fs.Destroy ();
			if (response == (int)Gtk.ResponseType.Ok) {
				commandTextBox.Text = name;
			}
		}
		
		
		void moveUpEvent(object sender, EventArgs e)
		{
			if(toolListBox.Selection.CountSelectedRows() == 1)
			{
				TreeIter selectedItem;
				TreeModel ls;				
				((ListStore)toolListBox.Model).GetIter(out selectedItem, (TreePath) toolListBox.Selection.GetSelectedRows(out ls)[0]);
				// we know we have a selected item so get it's index
				// use that to get the path of the item before it, and swap the two
				int index = GetSelectedIndex(toolListBox);
				// only swap if at the top
				if(index > 0)
				{
					TreeIter prev; 
					if(toolListBox.Model.GetIterFromString(out prev, (index - 1).ToString()))
					{
						((ListStore)ls).Swap(selectedItem, prev);
					}
				}
			}
		}
		void moveDownEvent(object sender, EventArgs e)
		{
			if(toolListBox.Selection.CountSelectedRows() == 1)
			{
				TreeIter selectedItem;
				TreeModel ls;				
				((ListStore)toolListBox.Model).GetIter(out selectedItem, (TreePath) toolListBox.Selection.GetSelectedRows(out ls)[0]);
				
				// swap it with the next one
				TreeIter toSwap = selectedItem;
				if(ls.IterNext(out toSwap))
				{
					((ListStore)ls).Swap(selectedItem, toSwap);
				}
			}
		}
		
		void setToolValues(object sender, EventArgs e)
		{
							
			ExternalTool selectedItem = null;
			if(toolListBox.Selection.CountSelectedRows() == 1)
			{
				TreeIter selectedIter;
				TreeModel lv;				
				((ListStore)toolListBox.Model).GetIter(out selectedIter, (TreePath) toolListBox.Selection.GetSelectedRows(out lv)[0]);
				
				// get the value as an external tool object
				selectedItem = lv.GetValue(selectedIter, 1) as ExternalTool;
				
				
				lv.SetValue(selectedIter, 0, titleTextBox.Text);
				selectedItem.MenuCommand        = titleTextBox.Text;
				selectedItem.Command            = commandTextBox.Text;
				selectedItem.Arguments          = argumentTextBox.Text;
				selectedItem.InitialDirectory   = workingDirTextBox.Text;
				selectedItem.PromptForArguments = promptArgsCheckBox.Active;
				selectedItem.UseOutputPad       = useOutputPadCheckBox.Active;
			}
		}
		
		void selectEvent(object sender, EventArgs e)
		{
			SetEnabledStatus(toolListBox.Selection.CountSelectedRows() > 0, removeButton);
			
			titleTextBox.Changed      -= new EventHandler(setToolValues);
			commandTextBox.Changed    -= new EventHandler(setToolValues);
			argumentTextBox.Changed   -= new EventHandler(setToolValues);
			workingDirTextBox.Changed   -= new EventHandler(setToolValues);
			promptArgsCheckBox.Toggled   -= new EventHandler(setToolValues);
			useOutputPadCheckBox.Toggled -= new EventHandler(setToolValues);
			
			if (toolListBox.Selection.CountSelectedRows() == 1) {				
				TreeIter selectedIter;
				TreeModel ls;
				((ListStore)toolListBox.Model).GetIter(out selectedIter, (TreePath) toolListBox.Selection.GetSelectedRows(out ls)[0]);				
				
				// get the value as an external tool object				
				ExternalTool selectedItem = (ExternalTool) toolListBox.Model.GetValue(selectedIter, 1);
				
				SetEnabledStatus(true, dependendControls);
				titleTextBox.Text      = selectedItem.MenuCommand;
				commandTextBox.Text    = selectedItem.Command;
				argumentTextBox.Text   = selectedItem.Arguments;
				workingDirTextBox.Text = selectedItem.InitialDirectory;
				promptArgsCheckBox.Active   = selectedItem.PromptForArguments;
				useOutputPadCheckBox.Active = selectedItem.UseOutputPad;
			} else {
				SetEnabledStatus(false, dependendControls);
				
				titleTextBox.Text      = String.Empty;
				commandTextBox.Text    = String.Empty;
				argumentTextBox.Text   = String.Empty;
				workingDirTextBox.Text = String.Empty;
				promptArgsCheckBox.Active   = false;
				useOutputPadCheckBox.Active = false;
			}
			
			titleTextBox.Changed      += new EventHandler(setToolValues);
			commandTextBox.Changed    += new EventHandler(setToolValues);
			argumentTextBox.Changed   += new EventHandler(setToolValues);
			workingDirTextBox.Changed += new EventHandler(setToolValues);
			promptArgsCheckBox.Toggled   += new EventHandler(setToolValues);
			useOutputPadCheckBox.Toggled += new EventHandler(setToolValues);
		}
		
		void removeEvent(object sender, EventArgs e)
		{
			int selectedItemCount = toolListBox.Selection.CountSelectedRows();
			if(selectedItemCount > 0) {
				int maxIndex = 0;
				// first copy the selected item paths into a temp array
				TreeIter[] selectedIters = new TreeIter[selectedItemCount];
				TreeModel lv;
				GLib.List pathList = toolListBox.Selection.GetSelectedRows(out lv);								
				for(int i = 0; i < selectedItemCount; i++) {
					TreePath path = (TreePath) pathList[i];
					((ListStore)lv).GetIter(out selectedIters[i], path);
					maxIndex = path.Indices[0];
				}
				
				// now delete each item in that list
				foreach(TreeIter toDelete in selectedIters) {
					TreeIter itr = toDelete;
					toolListBoxItemCount --;
					((ListStore)lv).Remove(out itr);
				}
				
				if (toolListBoxItemCount == 0) {
					selectEvent(this, EventArgs.Empty);
				} else {
					SetSelectedIndex(toolListBox, Math.Min(maxIndex,toolListBoxItemCount - 1));
				}
			}
		}
		
		void addEvent(object sender, EventArgs e)
		{
			TreeIter itr = toolListBoxStore.AppendValues("New Tool", new ExternalTool());
			toolListBoxItemCount ++;
			toolListBox.Selection.UnselectAll();
			toolListBox.Selection.SelectIter(itr);
		}
		
		// added this event to get the last select row index from gtk TreeView
		int GetSelectedIndex(TreeView tv)
		{
			if(toolListBox.Selection.CountSelectedRows() == 1)
			{
				TreeIter selectedIter;
				TreeModel lv;				
				((ListStore)toolListBox.Model).GetIter(out selectedIter, (TreePath) toolListBox.Selection.GetSelectedRows(out lv)[0]);
			
				// return index of first level node (since only 1 level list model)
				return lv.GetPath(selectedIter).Indices[0];
			}
			else
			{
				return -1;
			}
		}
		
		// added this event to set a specific row as selected from index
		void SetSelectedIndex(TreeView tv, int index)
		{
			//convert index to a path
			TreePath path = new TreePath(index.ToString());
			tv.Selection.UnselectAll();
			tv.Selection.SelectPath(path);
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
	}
}
