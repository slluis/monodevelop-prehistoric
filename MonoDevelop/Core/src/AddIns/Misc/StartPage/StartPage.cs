using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Xml;
using MonoDevelop;
using MonoDevelop.Gui;
using MonoDevelop.Core;
using MonoDevelop.Services;
using MonoDevelop.BrowserDisplayBinding;
using MonoDevelop.Gui.ErrorHandlers;
using MonoDevelop.Gui.HtmlControl;
using MonoDevelop.Core.Services;

using Gecko;

namespace MonoDevelop.StartPage 
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
		MonoDevelopPage page = new MonoDevelopPage();
		
		// Default constructor: Initialize controls and display recent projects.
		public StartPageView()
		{
			htmlControl = new MozillaControl ();
			//htmlControl.Css = Runtime.PropertyService.DataDirectory + Path.DirectorySeparatorChar +
			//                                  "resources" + Path.DirectorySeparatorChar +
			//                                  "startpage" + Path.DirectorySeparatorChar +
			//                                  "Layout" + Path.DirectorySeparatorChar +
			//                                  "default.css";
			
			htmlControl.Html = page.Render(curSection);
			htmlControl.ShowAll ();
			htmlControl.OpenUri += new OpenUriHandler (HtmlControlBeforeNavigate);
			
			// Description of the tab shown in #develop
			ContentName = Runtime.StringParserService.Parse("${res:StartPage.StartPageContentName}");
			
			Runtime.ProjectService.CombineOpened += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler(HandleCombineOpened));
		}
		
		public void DelayedInitialize (string base_uri)
		{
			htmlControl.InitializeWithBase (base_uri);
		}
		
		void HandleCombineOpened(object sender, CombineEventArgs e)
		{
			WorkbenchWindow.CloseWindow(true, false, 0);
		}
		
		void HtmlControlBeforeNavigate (object sender, OpenUriArgs e)
		{
			Console.WriteLine (e.AURI);
			e.RetVal = true;
			if (e.AURI.StartsWith("project://")) {
				try {
					object recentOpenObj = Runtime.Properties.GetProperty("MonoDevelop.Gui.MainWindow.RecentOpen");
					if (recentOpenObj is MonoDevelop.Services.RecentOpen) {
						MonoDevelop.Services.RecentOpen recOpen = (MonoDevelop.Services.RecentOpen)recentOpenObj;
						
						string prjNumber = e.AURI.Substring("project://".Length);
						// wrong (jluke)
						//prjNumber = prjNumber.Substring(0, prjNumber.Length - 1);
			
						string projectFile = page.projectFiles[int.Parse(prjNumber)];
			
						try {
							Runtime.ProjectService.OpenCombine(projectFile);
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
				MonoDevelop.Commands.OpenCombine cmd = new MonoDevelop.Commands.OpenCombine();
				cmd.Run();
			} catch (Exception ex) {
				//MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void NewBtnClicked(object sender, EventArgs e) 
		{
			try {
				MonoDevelop.Commands.CreateNewProject cmd = new MonoDevelop.Commands.CreateNewProject();
				cmd.Run();
			} catch (Exception ex) {
				//MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
