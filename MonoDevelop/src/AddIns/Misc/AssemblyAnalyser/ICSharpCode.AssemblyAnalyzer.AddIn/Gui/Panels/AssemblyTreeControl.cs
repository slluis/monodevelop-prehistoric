// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.AssemblyAnalyser
{
	public class AssemblyTreeControl : TreeView
	{
		TreeStore assembliesStore;
		TreeNode assembliesNode;
		ResultListControl resultListControl;
		
		public ResultListControl ResultListControl {
			get {
				return resultListControl;
			}
			set {
				resultListControl = value;
			}
		}
		
		public AssemblyTreeControl()
		{
			StringParserService stringParserService = (StringParserService) ServiceManager.GetService (typeof (StringParserService));
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService) ServiceManager.GetService (typeof (ClassBrowserIconsService));
			assembliesStore = new TreeStore (typeof (string));
			//assemblyTreeView.ImageList = classBrowserIconService.ImageList;
			
			//assembliesNode = new TreeNode(stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.AssemblyTreeControl.AssembliesNode}"));
			//assembliesNode.ImageIndex = assembliesNode.SelectedImageIndex = 0;
			//assemblyTreeView.Nodes.Add(assembliesNode);
			//assemblyTreeView.AfterCollapse += new TreeViewEventHandler(AssemblyTreeViewAfterCollapse);
			//assemblyTreeView.AfterExpand += new TreeViewEventHandler(AssemblyTreeViewAfterExpand);
			//assemblyTreeView.AfterSelect += new TreeViewEventHandler(AssemblyTreeViewAfterSelect);
			this.Selection.Changed += AssemblyTreeViewSelectionChanged;
		}
		
/*		
		void AssemblyTreeViewAfterCollapse(object sender, TreeViewEventArgs e)
		{
			if (e.Node == assembliesNode) {
				assembliesNode.ImageIndex = assembliesNode.SelectedImageIndex = 0;
			}
		}

		void AssemblyTreeViewAfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Node == assembliesNode) {
				assembliesNode.ImageIndex = assembliesNode.SelectedImageIndex = 1;
			}
		}
*/
		
		void AssemblyTreeViewSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			TreeModel model;

			if (this.Selection.GetSelected (out model, out iter))
			{
				this.resultListControl.PrintReport ((ArrayList) model.GetValue (iter, 1));
			}
			else
			{
				PrintAllResolutions ();
			}
		}
		
		public void PrintAllResolutions ()
		{
			ArrayList allResolutions = new ArrayList ();
			//foreach (TreeNode node in assembliesNode.Nodes) {
			//	allResolutions.AddRange((ArrayList)node.Tag);
			//}
			this.resultListControl.PrintReport (allResolutions);
		}
		
		public void ClearContents()
		{
			//Console.WriteLine("CLEAR CONTENTS");
			assembliesStore.Clear ();
		}
		
		public void AnalyzeAssembly (AssemblyAnalyser current, string output)
		{
		}
		
		public void AddAssembly (string assemblyFileName, ArrayList resolutions)
		{
			assembliesStore.AppendValues (System.IO.Path.GetFileName (assemblyFileName), resolutions);
			//newNode.ImageIndex = newNode.SelectedImageIndex = 2;
			//assembliesNode.Expand();
		}
	}
}
