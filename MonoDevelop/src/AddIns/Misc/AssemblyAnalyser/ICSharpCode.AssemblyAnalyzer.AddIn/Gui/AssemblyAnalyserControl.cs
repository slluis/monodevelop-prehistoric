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
using Gtk;

namespace MonoDevelop.AssemblyAnalyser
{
	public class AssemblyAnalyserControl : Frame
	{
		Notebook tabControl;
		NotebookPage assembliesTabPage;

		VSeparator splitter;
		VSeparator splitter2;

		MonoDevelop.AssemblyAnalyser.ResultListControl resultListControl;
		MonoDevelop.AssemblyAnalyser.AssemblyTreeControl assemblyTreeControl;
		MonoDevelop.AssemblyAnalyser.ResultDetailsView resultDetailsView;
		
		public AssemblyAnalyserControl()
		{
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
		
		private void InitializeComponent() {
			this.Label = "Assembly Analyser Control";
			VBox vbox = new VBox ();

			this.resultDetailsView = new MonoDevelop.AssemblyAnalyser.ResultDetailsView();
			this.assemblyTreeControl = new MonoDevelop.AssemblyAnalyser.AssemblyTreeControl();
			this.resultListControl = new MonoDevelop.AssemblyAnalyser.ResultListControl();
			vbox.PackStart (this.resultDetailsView);
			vbox.PackStart (this.assemblyTreeControl);
			vbox.PackStart (this.resultListControl);

			//this.assembliesTabPage = new NotebookPage();
			this.splitter = new VSeparator ();
			this.tabControl = new Notebook();
			this.splitter2 = new VSeparator ();
/*
			this.resultListControl.Load += new System.EventHandler(this.ResultListControlLoad);
*/
		}
		
		void ResultListControlLoad(object sender, System.EventArgs e)
		{
			
		}
		
	}
}
