// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.AssemblyAnalyser.Rules;
using MonoDevelop.Services;

namespace MonoDevelop.AssemblyAnalyser
{
	public class ResultListControl : TreeView
	{
		ListStore store;
		ResultDetailsView resultDetailsView = null;
		
		public ResultDetailsView ResultDetailsView {
			get {
				return resultDetailsView;
			}
			set {
				resultDetailsView = value;
			}
		}
		
		public ResultListControl()
		{
			StringParserService stringParserService = (StringParserService) ServiceManager.GetService(typeof(StringParserService));
			store = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string), typeof (string), typeof (Resolution));
			this.AppendColumn ("Level", new CellRendererText (), "text", 0);
			this.AppendColumn ("Certainty", new CellRendererText (), "text", 1);
			this.AppendColumn ("Rule", new CellRendererText (), "text", 2);
			this.AppendColumn ("Item", new CellRendererText (), "text", 3);
			this.Model = store;
			this.HeadersVisible = true;
		}
		
		public void ClearContents()
		{
			store.Clear ();
		}
		
		public void PrintReport(ArrayList resolutions)
		{
			try {
				store.Clear();
				StringParserService stringParserService = (StringParserService) ServiceManager.GetService (typeof (StringParserService));
				int cerr = 0, err = 0, cwar = 0, war = 0, inf = 0;
				foreach (Resolution resolution in resolutions) {
					string critical = String.Empty;
					string type     = String.Empty;
					Color foreColor = Color.Black;
					
					switch (resolution.FailedRule.PriorityLevel) {
						case PriorityLevel.CriticalError:
							critical = "!";
							type = "ErrorType";
							foreColor = Color.Red;
							++cerr;
							break;
						case PriorityLevel.Error:
							type = "ErrorType";
							foreColor = Color.DarkRed;
							++err;
							break;
						case PriorityLevel.CriticalWarning:
							critical = "!";
							type = "WarningType";
							foreColor = Color.Blue;
							++cwar;
							break;
						case PriorityLevel.Warning:
							type = "WarningType";
							foreColor = Color.DarkBlue;
							++war;
							break;
						case PriorityLevel.Information:
							type = "InformationType";
							++inf;
							break;
					}

					string certainity = resolution.FailedRule.Certainty.ToString() + "%";
					string text = stringParserService.Parse(resolution.FailedRule.Description);
					string item = stringParserService.Parse(resolution.Item);
					store.AppendValues (critical, type, certainity, text, item, resolution);
					
				}

				IStatusBarService statusBarService = (IStatusBarService) ServiceManager.GetService (typeof (IStatusBarService));
				if (resolutions.Count == 0) {
					statusBarService.SetMessage (GettextCatalog.GetString ("No defects found."));
				} else {
					statusBarService.SetMessage (String.Format (GettextCatalog.GetString ("Total:{0} Critical:{1} Errors:{2} Warnings:{3} Info:{4}"), resolutions.Count.ToString (), cerr.ToString (), err.ToString (), cwar.ToString (), war.ToString (), inf.ToString ()));
				}
			} catch (Exception e) {
				Console.WriteLine("Got exception : " + e.ToString());
			}
		}
		
		void ListViewSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			TreeModel model;

			if (this.Selection.GetSelected (out model, out iter))
			{
				resultDetailsView.ViewResolution ((Resolution) model.GetValue (iter, 5));
			}

			//this.GrabFocus ();
		}

		void ListViewItemActivated (object sender, RowActivatedArgs e)
		{
		//	ListViewItem item  = listView.SelectedItems[0];
		//	if (item != null && item.Tag != null) {
		//		Resolution res = (Resolution)item.Tag;
		//		IParserService parserService = (IParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		//		Position position = parserService.GetPosition(res.Item.Replace('+', '.'));
				
		//		if (position != null && position.Cu != null) {
		//			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
		//			Console.WriteLine("File name : " + position.Cu.FileName);
		//			fileService.JumpToFilePosition(position.Cu.FileName, Math.Max(0, position.Line - 1), Math.Max(0, position.Column - 1));
		//		}
		//	}
		}
	}
}
