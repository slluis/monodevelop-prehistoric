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
	public class TipOfTheDayWindow
	{
 		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
 		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));

		[Glade.Widget] Label categoryLabel;
		[Glade.Widget] TextView tipTextview;
		[Glade.Widget] CheckButton noshowCheckbutton;
		[Glade.Widget] Button nextButton;
		[Glade.Widget] Button closeButton;
		[Glade.Widget] Window tipOfTheDayWindow;

		string[] tips;
		int currentTip = 0;

		public TipOfTheDayWindow ()
		{
			Glade.XML totdXml = new Glade.XML (null, "Base.glade",
							   "tipOfTheDayWindow",
							   null);
			totdXml.Autoconnect (this);
			
			noshowCheckbutton.Active = propertyService.GetProperty ("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true);
			noshowCheckbutton.Toggled += new EventHandler (OnNoshow);
			nextButton.Clicked += new EventHandler (OnNext);
			closeButton.Clicked += new EventHandler (OnClose);
			tipOfTheDayWindow.DeleteEvent += new DeleteEventHandler (OnDelete);

 			XmlDocument doc = new XmlDocument();
 			doc.Load (propertyService.DataDirectory +
				  System.IO.Path.DirectorySeparatorChar + "options" +
				  System.IO.Path.DirectorySeparatorChar + "TipsOfTheDay.xml");
			ParseTips (doc.DocumentElement);
			
			tipTextview.Buffer.Clear ();
			tipTextview.Buffer.InsertAtCursor (tips[currentTip]);
		}

		private void ParseTips (XmlElement el)
		{
 			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
 			XmlNodeList nodes = el.ChildNodes;
 			tips = new string[nodes.Count];
			
 			for (int i = 0; i < nodes.Count; i++) {
 				tips[i] = stringParserService.Parse (nodes[i].InnerText);
 			}
			
 			currentTip = (new Random ().Next ()) % nodes.Count;
		}

		public void OnNoshow (object obj, EventArgs args)
		{
			propertyService.SetProperty ("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup",
						    noshowCheckbutton.Active);
		}

		public void OnNext (object obj, EventArgs args)
		{
			tipTextview.Buffer.Clear ();
			currentTip = ++currentTip % tips.Length;
			tipTextview.Buffer.InsertAtCursor (tips[currentTip]);
		}

		public void OnClose (object obj, EventArgs args)
		{
			tipOfTheDayWindow.Destroy ();
		}

		public void OnDelete (object obj, DeleteEventArgs args)
		{
			tipOfTheDayWindow.Destroy ();
		}

		public void Show ()
		{
			tipOfTheDayWindow.TransientFor = (Window) WorkbenchSingleton.Workbench;
			tipOfTheDayWindow.WindowPosition = WindowPosition.CenterOnParent;
			tipOfTheDayWindow.ShowAll ();
		}
	}
}
