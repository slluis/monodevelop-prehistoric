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
			IConfiguration newConfig = (IConfiguration)project.ActiveConfiguration.Clone();
			newConfig.Name = newName;			
			Gtk.TreeIter newNode = treeStore.AppendValues (configurationTreeNode, newConfig.Name , newConfig);
			
			// add the config to the project
			project.Configurations.Add(newConfig);
			properties.SetProperty("Config", newConfig);
			
			// add the child nodes to this new config
			AddNodes(properties, newNode, configurationNode.BuildChildItems(newConfig));			
			
			SelectSpecificNode(newNode);
			
			// FIXME: how to set the new nodes Label as editable ?
			/*
			((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = newNode;
			((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit    = true;
			*/
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
			// FIXME: have no idea if this is possible
			//((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit    = true;
			//((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode.BeginEdit();
		}
		
		/*void AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit = false;
			
			// canceled edit (or empty name)
			if (e.Label == null || e.Label.Length == 0) {
				return;
			}
			
			bool duplicateLabel       = false;
			foreach (IConfiguration config in project.Configurations) {
				if (e.Label == config.Name) {
					duplicateLabel = true;
					break;
				}
			}
			e.CancelEdit = true;
			
			if (!duplicateLabel) {
				e.Node.Text = e.Label;
				((IConfiguration)e.Node.Tag).Name = e.Label;
			}
		}*/
		
		static string configNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/ConfigNodeMenu";
		static string selectConfigNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/SelectedConfigMenu";
		
		/*void TreeViewMouseUp(object sender, MouseEventArgs e)
		{
			TreeNode clickedNode = ((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(e.X, e.Y);
			
			if (e.Button == MouseButtons.Right) {
				MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
				if (clickedNode == configurationTreeNode) {
					b = false;
					((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = clickedNode;
					b = true;
					menuService.ShowContextMenu(this, configNodeMenu, this, e.X, e.Y);
				}
				if (clickedNode.Parent == configurationTreeNode) {
					b = false;
					((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = clickedNode;
					b = true;
					menuService.ShowContextMenu(this, selectConfigNodeMenu, this, e.X, e.Y);
				}
			}
		}*/
	}
}
