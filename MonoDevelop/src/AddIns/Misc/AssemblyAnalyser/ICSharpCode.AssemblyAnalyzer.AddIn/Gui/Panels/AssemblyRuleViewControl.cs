// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonoDevelop.AssemblyAnalyser
{
	/// <summary>
	/// Description of AssemblyRuleViewControl.	
	/// </summary>
	[ToolboxBitmap(typeof(System.Windows.Forms.TreeView))]
	public class AssemblyRuleViewControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView ruleTreeView;
		public AssemblyRuleViewControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.ruleTreeView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// ruleTreeView
			// 
			this.ruleTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ruleTreeView.ImageIndex = -1;
			this.ruleTreeView.Location = new System.Drawing.Point(0, 0);
			this.ruleTreeView.Name = "ruleTreeView";
			this.ruleTreeView.SelectedImageIndex = -1;
			this.ruleTreeView.Size = new System.Drawing.Size(292, 266);
			this.ruleTreeView.TabIndex = 0;
			// 
			// AssemblyRuleViewControl
			// 
			this.Controls.Add(this.ruleTreeView);
			this.Name = "AssemblyRuleViewControl";
			this.Size = new System.Drawing.Size(292, 266);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
