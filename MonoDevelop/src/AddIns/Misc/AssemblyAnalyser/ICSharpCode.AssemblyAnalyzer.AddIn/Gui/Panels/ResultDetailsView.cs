// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using Gtk;
using Gecko;

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
	// it might be better to display with some simple
	// labels, or even a DrawingArea
	public class ResultDetailsView : MozillaControl
	{
		Resolution  currentResolution;

		public ResultDetailsView()
		{
			PropertyService propertyService = (PropertyService) ServiceManager.GetService (typeof (PropertyService));
			//htmlControl.CascadingStyleSheet = propertyService.DataDirectory + Path.DirectorySeparatorChar +
			//                                  "resources" + Path.DirectorySeparatorChar +
			//                                  "css" + Path.DirectorySeparatorChar +
			//                                  "MsdnHelp.css";
			
			ClearContents();
			this.OpenUri += new OpenUriHandler (HtmlControlBeforeNavigate);
		}
		
		void HtmlControlBeforeNavigate(object sender, OpenUriArgs e)
		{
			e.RetVal = true;
			Console.WriteLine(" >{0}< ", e.AURI);
			if (e.AURI.StartsWith("help://types/")) {
				string typeName = e.AURI.Substring("help://types/".Length);
				//HelpBrowser helpBrowser = (HelpBrowser)WorkbenchSingleton.Workbench.GetPad(typeof(HelpBrowser));
				//helpBrowser.ShowHelpFromType(typeName);
			} else if (e.AURI.StartsWith("help://gotocause")) {
				GotoCurrentCause();
			}
		}
		
		public void ClearContents()
		{
			this.Html = "<HTML><BODY></BODY></HTML>";
		}
		
		void GotoCurrentCause()
		{
			IParserService parserService = (IParserService) ServiceManager.GetService (typeof (IParserService));
			//Position position = parserService.GetPosition(currentResolution.Item.Replace('+', '.'));
			
			//if (position != null && position.Cu != null) {
			//	IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			//	fileService.JumpToFilePosition(position.Cu.FileName, Math.Max(0, position.Line - 1), Math.Max(0, position.Column - 1));
			//}
		}
		
		bool CanGoto(Resolution res)
		{
			IParserService parserService = (IParserService) ServiceManager.GetService (typeof (IParserService));
			//Position position = parserService.GetPosition(res.Item.Replace('+', '.'));
			//return position != null && position.Cu != null;
			return false; //FIXME
		}
		
		public void ViewResolution (Resolution resolution)
		{
			this.currentResolution = resolution;
			StringParserService stringParserService = (StringParserService) ServiceManager.GetService (typeof (StringParserService));
			
			/*this.Html = @"<HTML><BODY ID='bodyID' CLASS='dtBODY'>
			<DIV ID='nstext'>
			<DL>" + stringParserService.Parse(resolution.FailedRule.Description)  + @"</DL>
			<H4 CLASS='dtH4'>" + stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.DescriptionLabel}") + @"</H4>
			<DL>" + stringParserService.Parse(resolution.FailedRule.Details) +  @"</DL>
			<H4 CLASS='dtH4'>" + stringParserService.Parse("${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.ResolutionLabel}") + @"</H4> 
			<DL>" + stringParserService.Parse(resolution.Text, resolution.Variables) +  @"</DL>
			" + (CanGoto(resolution) ? stringParserService.Parse("<A HREF=\"help://gotocause\">${res:MonoDevelop.AssemblyAnalyser.ResultDetailsView.JumpToSourceCodeLink}</A>") : "") + @"
			</DIV></BODY></HTML>";
			*/
		}
	}
}
