// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using MonoDevelop.Gui;
using MonoDevelop.Core;
using MonoDevelop.Services;
using MonoDevelop.BrowserDisplayBinding;
using MonoDevelop.Gui.HtmlControl;
using MonoDevelop.Core.Services;
using MonoDevelop.AssemblyAnalyser.Rules;
using MonoDevelop.Gui.Pads;

namespace MonoDevelop.AssemblyAnalyser
{
	/// <summary>
	/// Description of ResultDetailsView.	
	/// </summary>
	[ToolboxBitmap(typeof(System.Windows.Forms.RichTextBox))]
	public class ResultDetailsView : System.Windows.Forms.UserControl
	{
		HtmlControl htmlControl;
		Resolution  currentResolution;
		public ResultDetailsView()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			htmlControl = new HtmlControl();
			htmlControl.Dock = DockStyle.Fill;
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			htmlControl.CascadingStyleSheet = propertyService.DataDirectory + Path.DirectorySeparatorChar +
			                                  "resources" + Path.DirectorySeparatorChar +
			                                  "css" + Path.DirectorySeparatorChar +
			                                  "MsdnHelp.css";
			
			ClearContents();
			htmlControl.BeforeNavigate += new BrowserNavigateEventHandler(HtmlControlBeforeNavigate);
			this.Controls.Add(htmlControl);
		}
		
		void HtmlControlBeforeNavigate(object sender, BrowserNavigateEventArgs e)
		{
			e.Cancel = true;
			Console.WriteLine(" >{0}< ", e.Url);
			if (e.Url.StartsWith("help://types/")) {
				string typeName = e.Url.Substring("help://types/".Length);
				HelpBrowser helpBrowser = (HelpBrowser)WorkbenchSingleton.Workbench.GetPad(typeof(HelpBrowser));
				helpBrowser.ShowHelpFromType(typeName);
			} else if (e.Url.StartsWith("help://gotocause")) {
				GotoCurrentCause();
			}
		}
		
		public void ClearContents()
		{
			htmlControl.Html = "<HTML><BODY></BODY></HTML>";
		}
		
		void GotoCurrentCause()
		{
			IParserService parserService = (IParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			Position position = parserService.GetPosition(currentResolution.Item.Replace('+', '.'));
			
			if (position != null && position.Cu != null) {
				IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.JumpToFilePosition(position.Cu.FileName, Math.Max(0, position.Line - 1), Math.Max(0, position.Column - 1));
			}
		}
		
		bool CanGoto(Resolution res)
		{
			IParserService parserService = (IParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			Position position = parserService.GetPosition(res.Item.Replace('+', '.'));
			return position != null && position.Cu != null;
		}
		
		public void ViewResolution(Resolution resolution)
		{
			this.currentResolution = resolution;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			htmlControl.Html = @"<HTML><BODY ID='bodyID' CLASS='dtBODY'>
			<DIV ID='nstext'>
			<DL>" + stringParserService.Parse(resolution.FailedRule.Description)  + @"</DL>
			<H4 CLASS='dtH4'>" + stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.DescriptionLabel}") + @"</H4>
			<DL>" + stringParserService.Parse(resolution.FailedRule.Details) +  @"</DL>
			<H4 CLASS='dtH4'>" + stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.ResolutionLabel}") + @"</H4> 
			<DL>" + stringParserService.Parse(resolution.Text, resolution.Variables) +  @"</DL>
			" + (CanGoto(resolution) ? stringParserService.Parse("<A HREF=\"help://gotocause\">${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.JumpToSourceCodeLink}</A>") : "") + @"
			</DIV></BODY></HTML>";
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			// 
			// ResultDetailsView
			// 
			this.Name = "ResultDetailsView";
			this.Size = new System.Drawing.Size(292, 266);
		}
		#endregion
	}
}
