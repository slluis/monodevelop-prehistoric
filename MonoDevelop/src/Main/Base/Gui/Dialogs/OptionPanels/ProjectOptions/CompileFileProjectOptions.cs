// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class CompileFileProjectOptions : AbstractOptionPanel
	{

	// FIXME 
	// - internationalize 
 	//		SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
 	//		                          @"resources\panels\CompileFileProjectOptions.xfrm"));

		class CompileFileOptionsWidget : GladeWidgetExtract 
		{
			// Gtk Controls
			[Glade.Widget] Gtk.TreeView includeTreeView;
			public ListStore store;
			
			IProject project;
                       FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			public CompileFileOptionsWidget (IProperties CustomizationObject) : 
				base ("Base.glade", "CompileFileOptionsPanel")
			{
				this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");	
				
				store = new ListStore (typeof(bool), typeof(string));
				includeTreeView.Selection.Mode = SelectionMode.None;
				includeTreeView.Model = store;
				CellRendererToggle rendererToggle = new CellRendererToggle ();
				rendererToggle.Activatable = true;
				rendererToggle.Toggled += new ToggledHandler (ItemToggled);
				includeTreeView.AppendColumn ("Choosen", rendererToggle, "active", 0);
				includeTreeView.AppendColumn ("Name", new CellRendererText (), "text", 1);
				TreeIter iter = new TreeIter ();
				
				foreach (ProjectFile info in project.ProjectFiles) {
					if (info.BuildAction == BuildAction.Nothing || info.BuildAction == BuildAction.Compile) {
						string name = fileUtilityService.AbsoluteToRelativePath(
							project.BaseDirectory, info.Name).Substring(2);
						iter = store.AppendValues (info.BuildAction == BuildAction.Compile ? true : false, name);
					}
				}
			}			
			
			private void ItemToggled (object o, ToggledArgs args)
			{
 				const int column = 0;
 				Gtk.TreeIter iter;
				
				if (store.GetIterFromString(out iter, args.Path))
 				{
 					bool val = (bool) store.GetValue(iter, column);
 					store.SetValue(iter, column, !val);
 				}
			}

			public bool Store ()
			{	
				bool success = true;
				TreeIter first;	
				store.GetIterFirst(out first);
				TreeIter current = first;
				for (int i = 0; i < store.IterNChildren() - 1 ; ++i) {
					store.IterNext(out current);
					string name = fileUtilityService.RelativeToAbsolutePath(
						project.BaseDirectory, "." + System.IO.Path.DirectorySeparatorChar + store.GetValue(current, 1));
					int j = 0;
					while (j < project.ProjectFiles.Count && project.ProjectFiles[j].Name != name) {
						++j;
					}
					if (j < project.ProjectFiles.Count) {
						project.ProjectFiles[j].BuildAction = (bool) store.GetValue(current, 0) ? BuildAction.Compile : BuildAction.Nothing;
					} else {
						string message = "File " + name + " not found in " + project.Name;
						MessageDialog dialog = new MessageDialog ((Window) WorkbenchSingleton.Workbench, 
								DialogFlags.DestroyWithParent,
								MessageType.Error, 
								ButtonsType.Close, 
								message);	
						dialog.Run ();
						dialog.Hide ();
						dialog.Dispose ();
						success = false;
					}
				}
				return true;
			}
		}
		
		CompileFileOptionsWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new  CompileFileOptionsWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			bool success = widget.Store();
 			return success;
 		}
	}
}
