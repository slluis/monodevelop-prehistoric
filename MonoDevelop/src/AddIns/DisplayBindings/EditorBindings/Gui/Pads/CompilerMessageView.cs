// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace MonoDevelop.EditorBindings.Gui.Pads
{
	// Note: I moved the pads to this assembly, because I want no cyclic dll dependency
	// on the ICSharpCode.TextEditor assembly.
	
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warnig / error
	/// </summary>
	public class CompilerMessageView : IPadContent
	{
		Gtk.TextBuffer buffer;
		Gtk.TextView textEditorControl;
		Gtk.ScrolledWindow scroller;
		//Panel       myPanel = new Panel();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public Gtk.Widget Control {
			get {
				return scroller;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.OutputWindow");
			}
		}
		
		public string Icon {
			get {
				return "Icons.16x16.OutputIcon";
			}
		}
		
		public void Dispose()
		{
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		public CompilerMessageView()
		{
			buffer = new Gtk.TextBuffer (new Gtk.TextTagTable ());
			textEditorControl = new Gtk.TextView (buffer);
			textEditorControl.Editable = false;
			scroller = new Gtk.ScrolledWindow ();
			scroller.Add (textEditorControl);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			TaskService     taskService    = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			
			taskService.CompilerOutputChanged += new EventHandler(SetOutput);
		}
		
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			buffer.Text = String.Empty;
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			buffer.Text = String.Empty;
		}
		
		void SelectMessageView(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
			} else { 
				if ((bool)propertyService.GetProperty("SharpDevelop.ShowOutputWindowAtBuild", true)) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
					WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
				}
			}
			
		}
		
		void SetOutput2()
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			try {
				buffer.Text = taskService.CompilerOutput;
				UpdateTextArea();
			} catch (Exception) {}
			
			System.Threading.Thread.Sleep(100);
		}
		
		void UpdateTextArea()
		{
			buffer.MoveMark (buffer.InsertMark, buffer.EndIter);
			textEditorControl.ScrollMarkOnscreen (buffer.InsertMark);
		}
		
		string outputText = null;
		void SetOutput(object sender, EventArgs e)
		{
			//Console.WriteLine("Create CompilerMessage View Handle:" + textEditorControl.Handle);
			//throw new Exception("Trace me...");
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				SetOutput2();
				outputText = null;
			} else {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				outputText = taskService.CompilerOutput;
				UpdateTextArea();
			}
		}
		
		void ActivateTextBox(object sender, EventArgs e)
		{
			if (outputText != null && textEditorControl.Visible) {
				buffer.Text = outputText;
				UpdateTextArea();
				outputText = null;
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		public void BringToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}

	}
}
