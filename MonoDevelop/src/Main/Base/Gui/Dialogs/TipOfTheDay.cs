// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Resources;
using System.Xml;
using System.IO;

using Gtk;
using GtkSharp;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class TipOfTheDayView : Frame
	{

		TextBuffer buffer;
		string[] tips;
		int curtip = 0;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public TipOfTheDayView(XmlElement el) : base ()
		{

 			XmlNodeList nodes = el.ChildNodes;
 			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
 			tips = new string[nodes.Count];
			
 			for (int i = 0; i < nodes.Count; ++i) {
 				tips[i] = stringParserService.Parse(nodes[i].InnerText);
 			}
			
 			curtip = (new Random().Next()) % nodes.Count;

			TextView view = new TextView ();
			view.WrapMode = WrapMode.Word;
			view.Editable = false;
			buffer = view.Buffer;
			buffer.InsertAtCursor(tips[curtip]);

			this.Add(view);
			
		}
		
		public void NextTip()
		{
			buffer.Clear();
 			curtip = (curtip + 1) % tips.Length;
			buffer.InsertAtCursor(tips[curtip]);
		}
	}
	
	public class TipOfTheDayDialog  : MessageDialog
	{

		enum UserDefinedResponseType {Next=1, Show}

 		CheckButton viewTipsAtStartCheckBox;
 		Button   closeButton;
 		Button   nextTipButton;
		
 		TipOfTheDayView tipview;
 		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
 		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));

		public TipOfTheDayDialog() :  base ((Window) WorkbenchSingleton.Workbench,
				                    DialogFlags.DestroyWithParent,
				                    MessageType.Info,
				                    ButtonsType.None,
				                    "")
		{

			this.Modal = false;

			this.Title = resourceService.GetString ("Dialog.TipOfTheDay.DidYouKnowText");
		
			this.SetDefaultSize (320, 240);

			viewTipsAtStartCheckBox = new CheckButton("Show tips at startup");
 			viewTipsAtStartCheckBox.Active = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true);
			this.AddActionWidget(viewTipsAtStartCheckBox, (int) UserDefinedResponseType.Show);

			nextTipButton = (Button) this.AddButton ("_Next Tip", (int) UserDefinedResponseType.Next);
			closeButton = (Button) this.AddButton (Stock.Close, (int) ResponseType.Close);
			
 			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
 			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
 			XmlDocument doc = new XmlDocument();
 			doc.Load(propertyService.DataDirectory +
 			         System.IO.Path.DirectorySeparatorChar + "options" +
 			         System.IO.Path.DirectorySeparatorChar + "TipsOfTheDay.xml" );
 			tipview = new TipOfTheDayView(doc.DocumentElement);

			this.Response += new ResponseHandler (HandleResponse);

 			this.VBox.PackStart(tipview);
			this.ShowAll();
		}

		public void HandleResponse (object sender, ResponseArgs e)
		{
			switch (e.ResponseId) 
			{
			case (int) ResponseType.Close : 
				this.Hide ();
				break;
			case (int) UserDefinedResponseType.Next : 
				tipview.NextTip();
				break;
			case (int) UserDefinedResponseType.Show : 
				propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", viewTipsAtStartCheckBox.Active);
				break;
			}
		}
		
	}
}
