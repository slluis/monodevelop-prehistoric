// created on 10/10/2002 at 16:13

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	/// <summary>
	/// BaseImlementation of IHelpService
	/// </summary>
	/// <remarks>
	/// 	created by - Niv
	/// 	created on - 10/10/2002 11:44:46
	/// </remarks>
	public class HelpService : IHelpService
	{
		Hashtable LocalContexts     = new Hashtable();
		ArrayList ContextAttributes = new ArrayList();
		string HelpPrefix  = "ms-help://MS.NETFrameworkSDK/cpref/html/frlrf";
		string HelpPostfix = "Topic.htm";
		
		string f1Keyword      = null;
		string generalKeyword = null;
		
		public HelpService()
		{
		}
		
		public void AddContextAttribute(string name, string value, HelpKeywordType keywordType)
		{
			switch (keywordType) {
				case HelpKeywordType.F1Keyword:
					f1Keyword = value;
					return;
				case HelpKeywordType.GeneralKeyword:
					generalKeyword = value;
					return;
			}
		}
		
		public void ClearContextAttributes()
		{
		}
		
		public IHelpService CreateLocalContext(HelpContextType contextType)
		{
			return this;
		}
		
		public void RemoveContextAttribute(string name, string value)
		{
//			System.Console.WriteLine("child removeing {0} : {1}",name,value);
//			object att = helpGUI.RemoveContextAttributeFromView(name,value);
//			ContextAttributes.Remove(att);;
		}
		
		public void RemoveLocalContext(IHelpService localContext)
		{
			LocalContexts.Remove(LocalContexts);
		}
		
		protected string GetHelpString(string word)
		{
			int i = 0;
			while ((i = word.IndexOf('.')) != -1) {
				word = word.Remove(i,1);
			}
			return word;
		}
		
		public void ShowHelpFromKeyword(string helpKeyword)
		{
			string url = string.Concat(GetHelpString(generalKeyword), "Class");
			if (helpKeyword == generalKeyword) {
				ShowHelpFromUrl(HelpPrefix + url + HelpPostfix);
				return;
			}
			if (helpKeyword == f1Keyword) {
				if (helpKeyword.StartsWith(generalKeyword))
					helpKeyword = helpKeyword.Remove(0,generalKeyword.Length+1);
				else
					url = "";
				url = string.Concat(url,GetHelpString(helpKeyword));
				ShowHelpFromUrl(HelpPrefix + url + HelpPostfix);
			}
		}
		
		public void ShowHelp()
		{
			ShowHelpFromKeyword(generalKeyword);
		}
		
		public void ShowHelpFromUrl(string helpURL)
		{
			HelpBrowser helpBrowser = (HelpBrowser)WorkbenchSingleton.Workbench.GetPad(typeof(HelpBrowser));
			helpBrowser.ShowHelpBrowser(helpURL);
		}
	}
}
