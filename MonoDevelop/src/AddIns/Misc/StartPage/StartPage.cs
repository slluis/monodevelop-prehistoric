using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Xml;
using ICSharpCode;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Gui.ErrorHandlers;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;
using ICSharpCode.Core.Services;

using GtkMozEmbedSharp;

namespace ICSharpCode.StartPage 
{
	/// <summary>
	/// This is the ViewContent implementation for the Start Page.
	/// </summary>
	public class StartPageView : AbstractViewContent
	{
		// defining the control variables used
		MozillaControl htmlControl;
		
		// return the panel that contains all of our controls
		public override Gtk.Widget Control {
			get {
				return htmlControl;
			}
		}
		
		// the content cannot be modified
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		// these methods are unused in this view
		public override void Save(string fileName) 
		{}
		public override void Load(string fileName) 
		{}
		
		// the redraw should get new add-in tree information
		// and update the view, the language or layout manager
		// may have changed.
		public override void RedrawContent()
		{
		}
		
		// Dispose all controls contained in this panel
		public override void Dispose()
		{
			try {
				htmlControl.Dispose();
			} catch {}
		}
		
		string curSection = "Start";
		ICSharpCodePage page = new ICSharpCodePage();
		
		// Default constructor: Initialize controls and display recent projects.
		public StartPageView()
		{
			htmlControl = new MozillaControl ();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			//htmlControl.Css = propertyService.DataDirectory + Path.DirectorySeparatorChar +
			//                                  "resources" + Path.DirectorySeparatorChar +
			//                                  "startpage" + Path.DirectorySeparatorChar +
			//                                  "Layout" + Path.DirectorySeparatorChar +
			//                                  "default.css";
			
			htmlControl.Html = page.Render(curSection);
			htmlControl.ShowAll ();
			htmlControl.OpenUri += new OpenUriHandler (HtmlControlBeforeNavigate);
			
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			// Description of the tab shown in #develop
			ContentName = stringParserService.Parse("${res:StartPage.StartPageContentName}");
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineOpened += new CombineEventHandler(HandleCombineOpened);
		}
		
		public void DelayedInitialize (string base_uri)
		{
			htmlControl.InitializeWithBase (base_uri);
		}
		
		void HandleCombineOpened(object sender, CombineEventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		void HtmlControlBeforeNavigate (object sender, OpenUriArgs e)
		{
			Console.WriteLine (e.AURI);
			e.RetVal = true;
			if (e.AURI.StartsWith("project://")) {
				try {
					Core.Properties.DefaultProperties svc = (Core.Properties.DefaultProperties)Core.Services.ServiceManager.Services.GetService(typeof(Core.Services.PropertyService));
					object recentOpenObj = svc.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
					if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) {
						ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
						
						IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
						
						string prjNumber = e.AURI.Substring("project://".Length);
						// wrong (jluke)
						//prjNumber = prjNumber.Substring(0, prjNumber.Length - 1);
			
						string projectFile = page.projectFiles[int.Parse(prjNumber)];
			
						try {
							projectService.OpenCombine(projectFile);
						} catch (Exception ex) {
							CombineLoadError.HandleError(ex, projectFile);
						}
					}
				} catch (Exception ex) {
					//MessageBox.Show("Could not access project service or load project:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine (ex.ToString ());
				}
			} else if (e.AURI.EndsWith("/opencombine")) {
				OpenBtnClicked(this, EventArgs.Empty);
			} else if (e.AURI.EndsWith("/newcombine")) {
				NewBtnClicked(this, EventArgs.Empty);
			} else if (e.AURI.EndsWith("/newcombine")) {
				NewBtnClicked(this, EventArgs.Empty);
			} else if (e.AURI.EndsWith("/opensection")) {
				Regex section = new Regex(@"/(?<section>.+)/opensection", RegexOptions.Compiled);
				Match match = section.Match(e.AURI);
				if (match.Success) {
					curSection = match.Result("${section}");
					htmlControl.Html = page.Render(curSection);
				}
			} else {
				//System.Diagnostics.Process.Start(e.AURI);
			}
		}
		
		public void OpenBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Commands.OpenCombine cmd = new ICSharpCode.SharpDevelop.Commands.OpenCombine();
				cmd.Run();
			} catch (Exception ex) {
				//MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void NewBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Commands.CreateNewProject cmd = new ICSharpCode.SharpDevelop.Commands.CreateNewProject();
				cmd.Run();
			} catch (Exception ex) {
				//MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
