// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;
namespace ICSharpCode.SharpDevelop.Gui.Dialogs {

	/// <summary>
	/// Dialog for viewing the project options (plain treeview isn't good enough :/)
	/// </summary>
	public class ProjectOptionsDialog : TreeViewOptions
	{
		IProject  project;
		
		IAddInTreeNode configurationNode;
		Gtk.TreeIter configurationTreeNode;
		Gtk.CellRendererText textRenderer;		// used to set an editable node
		Gtk.TreeViewColumn textColumn;			// used to set an editable node
	
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof(StringParserService));
		
		public ProjectOptionsDialog(IProject project, IAddInTreeNode node, IAddInTreeNode configurationNode) : base(null, null)
		{
			this.project = project;
			this.configurationNode = configurationNode;
			this.Title = StringParserService.Parse("${res:Dialog.Options.ProjectOptions.DialogName}");
			
			//((TreeView)ControlDictionary["optionsTreeView"]).MouseUp        += new MouseEventHandler(TreeViewMouseUp);
			//((TreeView)ControlDictionary["optionsTreeView"]).AfterLabelEdit += new NodeLabelEditEventHandler(AfterLabelEdit);
			
			//((TreeView)ControlDictionary["optionsTreeView"]).Font = boldFont;
			
			properties = new DefaultProperties();
			properties.SetProperty("Project", project);
			
			AddNodes(properties, Gtk.TreeIter.Zero, node.BuildChildItems(this));			
			
			//
			// This code has to add treeview node items to the treeview. under a configuration node
			//
			AddConfigurationNodes();
			
			//TreeView.ButtonReleaseEvent += new EventHandler (OnButtonRelease);
			
			SelectFirstNode ();	
		}
		
		void AddConfigurationNodes()
		{
			configurationTreeNode = treeStore.AppendValues (StringParserService.Parse("${res:Dialog.Options.ProjectOptions.ConfigurationsNodeName}"), null);
			
			foreach (IConfiguration config in project.Configurations) {
				Gtk.TreeIter newNode ;
				
				if (config == project.ActiveConfiguration) {
					newNode = treeStore.AppendValues (configurationTreeNode, config.Name + " (Active)", config);
					//newNode.NodeFont = boldFont;
				} else {
					newNode = treeStore.AppendValues (configurationTreeNode, config.Name, config); 
					//newNode.NodeFont = plainFont;
				}
				
				DefaultProperties configNodeProperties = new DefaultProperties();
				configNodeProperties.SetProperty("Project", project);
				configNodeProperties.SetProperty("Config", config);
				AddNodes(configNodeProperties, newNode, configurationNode.BuildChildItems(this));
			}
		}
		
		// override base method so that we can make the text cell render editable 
		protected override void InitializeComponent () 
		{
			treeStore = new Gtk.TreeStore (typeof (string), typeof (IDialogPanelDescriptor));
			
			TreeView.Model = treeStore;
			// need editable text cell so we can rename configs
			textRenderer = new Gtk.CellRendererText ();			
			textRenderer.Edited += new Gtk.EditedHandler (HandleOnEdit);
			textColumn = TreeView.AppendColumn ("", textRenderer , "text", 0);
			TreeView.Selection.Changed += new EventHandler (SelectNode);
		}
		
		// handle the cell edit event
		void HandleOnEdit (object o, Gtk.EditedArgs e)
		{
			// stop editability
			textRenderer.Editable = false;			
			
			Gtk.TreeIter iter;
			if (! treeStore.GetIterFromString (out iter, e.Path)) {
				throw new Exception("Error calculating iter for path in project options dialog: " + e.Path);
			}
			
			AfterLabelEdit(iter, e.NewText);
		}
		
		#region context menu commands
		
		public void AddProjectConfiguration()
		{
			int    number  = -1;
			string name    = "New Configuration"; // don't localize this project configs should have per default an english name
			
			// make sure you have unique configuration names
			string newName = name;			
			bool duplicateNumber;
			do {
				duplicateNumber = false;
				foreach (IConfiguration config in project.Configurations) {
					newName = (number >= 0) ? name + number : name;
					if (newName == config.Name) {
						++number;
						duplicateNumber = true;
						break;
					}
				}
			} while (duplicateNumber);
			
			// append new configuration node to the configurationTreeNode
			IConfiguration newConfig = (IConfiguration) project.ActiveConfiguration.Clone();
			newConfig.Name = newName;			
			Gtk.TreeIter newNode = treeStore.AppendValues (configurationTreeNode, newConfig.Name , newConfig);
			
			// add the config to the project
			project.Configurations.Add (newConfig);
			properties.SetProperty ("Config", newConfig);
			
			// add the child nodes to this new config
			AddNodes (properties, newNode, configurationNode.BuildChildItems(newConfig));			
			
			//select new config node and set it for renaming
			Gtk.TreePath newPath = treeStore.GetPath (newNode);
			TreeView.ExpandToPath (newPath);
			TreeView.Selection.SelectPath (newPath);
			RenameProjectConfiguration();
		}
		
		public void RemoveProjectConfiguration()
		{	
			Gtk.TreeModel mdl;
			Gtk.TreeIter  iter;
			if (TreeView.Selection.GetSelected (out mdl, out iter)) {
				IConfiguration config = (IConfiguration) mdl.GetValue(iter, 1);
				if (project.Configurations.Count > 1) {
					bool newActiveConfig = project.ActiveConfiguration == config;
					project.Configurations.Remove(config);
					project.ActiveConfiguration = (IConfiguration)project.Configurations[0];
					
					if (((Gtk.TreeStore)mdl).Remove(ref iter)) {
						UpdateBoldConfigurationNode();
						SelectSpecificNode(configurationTreeNode);
					}
				}
			}
		}
		
		void UpdateBoldConfigurationNode()
		{
			if (treeStore.IterHasChild(configurationTreeNode)) {
				for(int i = 0; i < treeStore.IterNChildren(configurationTreeNode); i++) { 
					Gtk.TreeIter node;
					if (treeStore.IterNthChild(out node, configurationTreeNode, i)) {
						
						// determine if this is the active config
						IConfiguration config = (IConfiguration) treeStore.GetValue(node, 1);
						if (project.ActiveConfiguration == config) {
							treeStore.SetValue(node, 0, config.Name + " (Active)");
						} else {
							treeStore.SetValue(node, 0, config.Name);
						}
					}
				}
			}
		}
		
		public void SetSelectedConfigurationAsStartup()
		{
			Gtk.TreeModel mdl;
			Gtk.TreeIter  iter;
			if (TreeView.Selection.GetSelected (out mdl, out iter)) {					
				IConfiguration config = (IConfiguration) mdl.GetValue(iter, 1);	
				if (config != null) {
					project.ActiveConfiguration = config;
					UpdateBoldConfigurationNode();
				}
			}
		}
		
		public void RenameProjectConfiguration()
		{
			Gtk.TreeModel mdl;
			Gtk.TreeIter  iter;
			if (TreeView.Selection.GetSelected (out mdl, out iter)) {
				// make sure the node is a config node
				IConfiguration config = mdl.GetValue(iter, 1) as IConfiguration;
				if (config != null) {
					// see if it's the active columne (if so remove the active text on the end) before editing
					if (project.ActiveConfiguration == config) {
						string name = (string) mdl.GetValue(iter, 0);
						name = name.Replace(" (Active)", string.Empty);
						mdl.SetValue(iter, 0, name);
					}
					
					// make the cell editable
					textRenderer.Editable = true;
					TreeView.SetCursor (mdl.GetPath(iter), textColumn, true);
					//TreeView.GrabFocus ();
				}
			}
		}
		
		void AfterLabelEdit(Gtk.TreeIter iter, string newLabel)
		{
			// canceled edit (or empty name)
			if (newLabel == null || newLabel.Length == 0) {
				UpdateBoldConfigurationNode();
				return;
			}
			
			bool duplicateLabel = false;
			foreach (IConfiguration config in project.Configurations) {
				if (newLabel == config.Name) {
					duplicateLabel = true;
					break;
				}
			}
			
			// set the new label
			if (!duplicateLabel) {
				IConfiguration config = (IConfiguration) treeStore.GetValue(iter, 1);
				config.Name = newLabel;
				treeStore.SetValue(iter, 1, config);
				treeStore.SetValue(iter, 0, newLabel);
			}
			
			// if got this far then someone tried to edit a label, so reset the active string (in case it was removed)
			UpdateBoldConfigurationNode();
		}
		#endregion 
		
		#region context menu setup
		
		static string configNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/ConfigNodeMenu";
		static string selectConfigNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/SelectedConfigMenu";
		
		// override select node to allow config and config child nodes (braches) to be selected
		protected override void SelectNode(object sender, EventArgs e)
		{
			Gtk.TreeModel mdl;
			Gtk.TreeIter  iter;
			if (TreeView.Selection.GetSelected (out mdl, out iter)) {
				if (treeStore.IterHasChild (iter)) {
					// only interested if a row has been selected
					Gtk.TreePath path = TreeView.Model.GetPath(iter);
					Gtk.TreePath configPath = TreeView.Model.GetPath(configurationTreeNode);
					// see if the itter is a config iter or it's direct child
					// if so don't force focus onto child node just yet
					if (!iter.Equals(configurationTreeNode) &&
						!((path.Indices[0] == configPath.Indices[0]) && (path.Depth - configPath.Depth) == 1)) {
						Gtk.TreeIter new_iter;
						treeStore.IterChildren (out new_iter, iter);
						Gtk.TreePath new_path = treeStore.GetPath (new_iter);
						TreeView.ExpandToPath (new_path);
						TreeView.Selection.SelectPath (new_path);
					}
				} else {					
					SetOptionPanelTo ((IDialogPanelDescriptor)treeStore.GetValue (iter, 1));					
				}
			}
		}
		
		protected override void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs e)
		{	
			// only interested in right mouse button click
			if (e.Event.Button == 3) {
				
				// only interested if a row has been selected
				Gtk.TreeModel mdl;
				Gtk.TreeIter iter;
				Gtk.TreePath configPath = TreeView.Model.GetPath(configurationTreeNode);
				if (TreeView.Selection.GetSelected (out mdl, out iter)) {
					Gtk.TreePath path = TreeView.Model.GetPath(iter);
	
					// now see if the iter is the configuration root node iter
					if (iter.Equals(configurationTreeNode)) {							
						MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
						menuService.ShowContextMenu(this, configNodeMenu, TreeView);
					} else if (path.Indices[0] == configPath.Indices[0] && (path.Depth - configPath.Depth) == 1) {
						// now see if it's a specific configuration node (i.e. the configuration root node is it's parent
						MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
						menuService.ShowContextMenu(this, selectConfigNodeMenu, TreeView);
					}
					
				}
			}
		}
		
		#endregion
	}
}
