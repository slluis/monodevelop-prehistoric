// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace MonoDevelop.AssemblyAnalyser
{
	/// <summary>
	/// Description of AssemblyAnalyserControl.	
	/// </summary>
	public class AssemblyAnalyserControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TabControl tabControl;
		private MonoDevelop.AssemblyAnalyser.ResultListControl resultListControl;
		private System.Windows.Forms.Splitter splitter;
		private MonoDevelop.AssemblyAnalyser.AssemblyTreeControl assemblyTreeControl;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.TabPage assembliesTabPage;
		private MonoDevelop.AssemblyAnalyser.ResultDetailsView resultDetailsView;
		
		public AssemblyAnalyserControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			
			InitializeComponent();
			resultListControl.ResultDetailsView = resultDetailsView;
			assemblyTreeControl.ResultListControl = resultListControl;
			
		}
		
		public void ClearContents()
		{
			resultListControl.ClearContents();
			assemblyTreeControl.ClearContents();
		}
		
		public void AnalyzeAssembly(AssemblyAnalyser analyser, string fileName)
		{
			if (File.Exists(fileName)) {
				analyser.Analyse(fileName);
				assemblyTreeControl.AddAssembly(fileName, analyser.Resolutions);
			}
		}
		
		public void PrintAllResolutions()
		{
			assemblyTreeControl.PrintAllResolutions();
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.resultDetailsView = new MonoDevelop.AssemblyAnalyser.ResultDetailsView();
			this.assembliesTabPage = new System.Windows.Forms.TabPage();
			this.panel = new System.Windows.Forms.Panel();
			this.assemblyTreeControl = new MonoDevelop.AssemblyAnalyser.AssemblyTreeControl();
			this.splitter = new System.Windows.Forms.Splitter();
			this.resultListControl = new MonoDevelop.AssemblyAnalyser.ResultListControl();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.assembliesTabPage.SuspendLayout();
			this.panel.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// resultDetailsView
			// 
			this.resultDetailsView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.resultDetailsView.Location = new System.Drawing.Point(0, 304);
			this.resultDetailsView.Name = "resultDetailsView";
			this.resultDetailsView.Size = new System.Drawing.Size(544, 200);
			this.resultDetailsView.TabIndex = 1;
			// 
			// assembliesTabPage
			// 
			this.assembliesTabPage.Controls.Add(this.assemblyTreeControl);
			this.assembliesTabPage.Location = new System.Drawing.Point(4, 22);
			this.assembliesTabPage.Name = "assembliesTabPage";
			this.assembliesTabPage.Size = new System.Drawing.Size(192, 478);
			this.assembliesTabPage.TabIndex = 0;
			this.assembliesTabPage.Text = "Assemblies";
			// 
			// panel
			// 
			this.panel.Controls.Add(this.resultListControl);
			this.panel.Controls.Add(this.splitter2);
			this.panel.Controls.Add(this.resultDetailsView);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(204, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(544, 504);
			this.panel.TabIndex = 2;
			// 
			// assemblyTreeControl
			// 
			this.assemblyTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assemblyTreeControl.Location = new System.Drawing.Point(0, 0);
			this.assemblyTreeControl.Name = "assemblyTreeControl";
			this.assemblyTreeControl.Size = new System.Drawing.Size(192, 478);
			this.assemblyTreeControl.TabIndex = 0;
			// 
			// splitter
			// 
			this.splitter.Location = new System.Drawing.Point(200, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(4, 504);
			this.splitter.TabIndex = 1;
			this.splitter.TabStop = false;
			// 
			// resultListControl
			// 
			this.resultListControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultListControl.Location = new System.Drawing.Point(0, 0);
			this.resultListControl.Name = "resultListControl";
			this.resultListControl.Size = new System.Drawing.Size(544, 300);
			this.resultListControl.TabIndex = 3;
			this.resultListControl.Load += new System.EventHandler(this.ResultListControlLoad);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.assembliesTabPage);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(200, 504);
			this.tabControl.TabIndex = 0;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 300);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(544, 4);
			this.splitter2.TabIndex = 2;
			this.splitter2.TabStop = false;
			// 
			// AssemblyAnalyserControl
			// 
			this.Controls.Add(this.panel);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.tabControl);
			this.Name = "AssemblyAnalyserControl";
			this.Size = new System.Drawing.Size(748, 504);
			this.assembliesTabPage.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		
		void ResultListControlLoad(object sender, System.EventArgs e)
		{
			
		}
		
	}
}
