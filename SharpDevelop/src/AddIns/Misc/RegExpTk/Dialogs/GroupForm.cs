// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.XmlForms;

namespace Plugins.RegExpTk {

	public class GroupForm : XmlForm
	{
		public GroupForm(Match match) : base(Application.StartupPath + @"\..\data\resources\dialogs\RegExpTkGroupForm.xfrm")
		{
			ListView groupsListView = (ListView)ControlDictionary["GroupsListView"];
			((Button)ControlDictionary["CloseButton"]).Click += new EventHandler(CloseButton_Click);
			foreach(Group group in match.Groups)
			{
				ListViewItem groupItem = groupsListView.Items.Add(group.Value);
				groupItem.SubItems.Add(group.Index.ToString());
				groupItem.SubItems.Add((group.Index + group.Length).ToString());
				groupItem.SubItems.Add(group.Length.ToString());
			}
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}

}
