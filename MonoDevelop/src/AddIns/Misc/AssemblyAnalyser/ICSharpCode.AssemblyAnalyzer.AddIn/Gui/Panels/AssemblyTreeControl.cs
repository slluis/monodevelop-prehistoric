// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
namespace MonoDevelop.AssemblyAnalyser
{
	/// <summary>
	/// Description of AssemblyTreeControl.	
	/// </summary>
	[ToolboxBitmap(typeof(System.Windows.Forms.TreeView))]
	public class AssemblyTreeControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView assemblyTreeView;
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
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			assemblyTreeView.ImageList = classBrowserIconService.ImageList;
			
			assembliesNode = new TreeNode(stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.AssemblyTreeControl.AssembliesNode}"));
			assembliesNode.ImageIndex = assembliesNode.SelectedImageIndex = 0;
			assemblyTreeView.Nodes.Add(assembliesNode);
			assemblyTreeView.AfterCollapse += new TreeViewEventHandler(AssemblyTreeViewAfterCollapse);
			assemblyTreeView.AfterExpand += new TreeViewEventHandler(AssemblyTreeViewAfterExpand);
			assemblyTreeView.AfterSelect += new TreeViewEventHandler(AssemblyTreeViewAfterSelect);
		}
		
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
		
		void AssemblyTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null) {
				PrintAllResolutions();
			} else {
				this.resultListControl.PrintReport((ArrayList)e.Node.Tag);
			}
		}
		
		public void PrintAllResolutions()
		{
			ArrayList allResolutions = new ArrayList();
			foreach (TreeNode node in assembliesNode.Nodes) {
				allResolutions.AddRange((ArrayList)node.Tag);
			}
			this.resultListControl.PrintReport(allResolutions);
		}
		
		public void ClearContents()
		{
			Console.WriteLine("CLEAR CONTENTS");
			assembliesNode.Nodes.Clear();
		}
		
		public void AddAssembly(string assemblyFileName, ArrayList resolutions)
		{
			TreeNode newNode = new TreeNode(Path.GetFileName(assemblyFileName));
			newNode.Tag = resolutions;
			newNode.ImageIndex = newNode.SelectedImageIndex = 2;
			assembliesNode.Nodes.Add(newNode);
			assembliesNode.Expand();
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.assemblyTreeView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// assemblyTreeView
			// 
			this.assemblyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assemblyTreeView.HideSelection = false;
			this.assemblyTreeView.ImageIndex = -1;
			this.assemblyTreeView.Location = new System.Drawing.Point(0, 0);
			this.assemblyTreeView.Name = "assemblyTreeView";
			this.assemblyTreeView.SelectedImageIndex = -1;
			this.assemblyTreeView.Size = new System.Drawing.Size(292, 266);
			this.assemblyTreeView.TabIndex = 0;
			// 
			// AssemblyTreeControl
			// 
			this.Controls.Add(this.assemblyTreeView);
			this.Name = "AssemblyTreeControl";
			this.Size = new System.Drawing.Size(292, 266);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
