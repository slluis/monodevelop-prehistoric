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

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class RuleSetNode : AbstractNode
	{
		bool noEscapeSequences = false;
		bool ignoreCase   = false;
		bool isRoot       = false;
		string name       = String.Empty;
		string delimiters = String.Empty;
		string reference  = String.Empty;
		
		KeywordListsNode keywordNode;
		SpansNode spansNode;
		MarkersNode prevMarkerNode;
		MarkersNode nextMarkerNode;
		
		public RuleSetNode(XmlElement el)
		{
			Text = ResNodeName("RuleSet");
			
			panel = new RuleSetOptionPanel(this);
			
			if (el == null) return;
			
			if (el.Attributes["name"] != null) {
				name = el.Attributes["name"].InnerText;
				Text = name;
				isRoot = false;
			}
			
			if (name == "") {
				Text = ResNodeName("RootRuleSet");
				isRoot = true;
			}
			
			if (el.Attributes["noescapesequences"] != null) {
				noEscapeSequences = Boolean.Parse(el.Attributes["noescapesequences"].InnerText);
			}
			
			if (el.Attributes["reference"] != null) {
				reference = el.Attributes["reference"].InnerText;
			}
			
			if (el.Attributes["ignorecase"] != null) {
				ignoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
			}
			
			if (el["Delimiters"] != null) {
				delimiters = el["Delimiters"].InnerText;
			}
			
			keywordNode = new KeywordListsNode(el);
			spansNode   = new SpansNode(el);
			prevMarkerNode = new MarkersNode(el, true);  // Previous
			nextMarkerNode = new MarkersNode(el, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);			
	
		}
		
		public RuleSetNode(string Name, string Delim, string Ref, bool noEsc, bool noCase)
		{
			name = Name;
			Text = Name;
			delimiters = Delim;
			reference = Ref;
			noEscapeSequences = noEsc;
			ignoreCase = noCase;
			
			keywordNode = new KeywordListsNode(null);
			spansNode   = new SpansNode(null);
			prevMarkerNode = new MarkersNode(null, true);  // Previous
			nextMarkerNode = new MarkersNode(null, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);	
			
			panel = new RuleSetOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "" && !isRoot) {
				Text = name;
			}
		}
		
		public override string ToXml()
		{
			if (reference != "")   return "\t\t<RuleSet name=\"" + ReplaceXmlChars(name) + "\" reference=\"" + ReplaceXmlChars(reference) + "\"></RuleSet>\n\n";
			
			string ret = "\t\t<RuleSet ignorecase=\"" + ignoreCase.ToString().ToLower() + "\" ";
			if (noEscapeSequences) ret += "noescapesequences=\"true\" ";
			if (!isRoot)           ret += "name=\"" + ReplaceXmlChars(name) + "\" ";
			ret += ">\n";
			
			ret += "\t\t\t<Delimiters>" + ReplaceXmlChars(delimiters) + "</Delimiters>\n\n";
			
			ret += spansNode.ToXml();
			ret += prevMarkerNode.ToXml();
			ret += nextMarkerNode.ToXml();
			ret += keywordNode.ToXml();
			
			ret += "\t\t</RuleSet>\n\n";
			
			return ret;
		}
		
		public string Delimiters {
			get {
				return delimiters;
			}
			set {
				delimiters = value;
			}
		}
		
		public bool NoEscapeSequences {
			get {
				return noEscapeSequences;
			}
			set {
				noEscapeSequences = value;
			}
		}
		
		public bool IgnoreCase {
			get {
				return ignoreCase;
			}
			set {
				ignoreCase = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Reference {
			get {
				return reference;
			}
			set {
				reference = value;
			}
		}
		
		public bool IsRoot {
			get {
				return isRoot;
			}
		}
		
	}
	
	class RuleSetOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.CheckBox igcaseBox;
		private System.Windows.Forms.CheckBox noEscBox;
		private System.Windows.Forms.TextBox refBox;
		private System.Windows.Forms.TextBox delimBox;
		private System.Windows.Forms.TextBox nameBox;
		
		public RuleSetOptionPanel(RuleSetNode parent) : base(parent)
		{
			SetupFromXml(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\RuleSet.xfrm"));
			
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			refBox   = (TextBox)ControlDictionary["refBox"];
			delimBox = (TextBox)ControlDictionary["delimBox"];
			
			igcaseBox = (CheckBox)ControlDictionary["igcaseBox"];
			noEscBox  = (CheckBox)ControlDictionary["noEscBox"];
		}
		
		public override void StoreSettings()
		{
			RuleSetNode node = (RuleSetNode)parent;
			if (!node.IsRoot) node.Name = nameBox.Text;
			node.Reference = refBox.Text;
			node.Delimiters = delimBox.Text;
			node.NoEscapeSequences = noEscBox.Checked;
			node.IgnoreCase = igcaseBox.Checked;
		}
		
		public override void LoadSettings()
		{
			RuleSetNode node = (RuleSetNode)parent;
			
			nameBox.Text = node.Name;
			
			if (node.IsRoot) {
				nameBox.Text = ResourceService.GetString("Dialog.HighlightingEditor.TreeView.RootRuleSet");
				nameBox.Enabled = false;
			}
			
			refBox.Text = node.Reference;
			delimBox.Text = node.Delimiters;
			
			noEscBox.Checked = node.NoEscapeSequences;
			igcaseBox.Checked = node.IgnoreCase;
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.RuleSet.NameEmpty"));
				return false;
			}
			
			return true;
		}

	}
}
