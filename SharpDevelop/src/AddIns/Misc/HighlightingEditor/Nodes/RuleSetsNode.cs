// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class RuleSetsNode : AbstractNode
	{
		public RuleSetsNode(XmlElement el)
		{
			Text = ResNodeName("RuleSets");
			
			panel = new RuleSetsOptionPanel(this);
			if (el == null) return;

			XmlNodeList nodes = el.GetElementsByTagName("RuleSet");
			
			foreach (XmlElement element in nodes) {
				Nodes.Add(new RuleSetNode(element));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "\t<RuleSets>\n";
			foreach (RuleSetNode node in Nodes) {
				ret += node.ToXml();
			}
			ret += "\t</RuleSets>\n\n";
			return ret;
		}
	}
	
	class RuleSetsOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button removeBtn;
		
		public RuleSetsOptionPanel(RuleSetsNode parent) : base(parent)
		{
			SetupFromXml(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\RuleSets.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
		}
		
		public override void StoreSettings()
		{
		}
		
		public override void LoadSettings()
		{
			RuleSetsNode node = (RuleSetsNode)parent;
			listView.Items.Clear();
			
			foreach (RuleSetNode rn in node.Nodes) {
				if (rn.Name == "") continue;
				ListViewItem lv = new ListViewItem(rn.Name);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.RuleSets.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				RuleSetNode rsn = new RuleSetNode(box.TextBox.Text, "&<>~!@%^*()-+=|\\#/{}[]:;\"' ,	.?", "", false, false);
				ListViewItem lv = new ListViewItem(box.TextBox.Text);
				lv.Tag = rsn;
				parent.Nodes.Add(rsn);
				listView.Items.Add(lv);
			}
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			((TreeNode)listView.SelectedItems[0].Tag).Remove();
			listView.SelectedItems[0].Remove();
		}
	}
}
