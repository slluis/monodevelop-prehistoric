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
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class DigitsNode : AbstractNode
	{
		EditorHighlightColor color;
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}

		public DigitsNode(XmlElement el)
		{
			if (el != null) {
				color = new EditorHighlightColor(el);
			} else {
				color = new EditorHighlightColor();
			}
			
			Text = ResNodeName("DigitsColor");
			
			panel = new DigitsOptionPanel(this);
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			return "\t<Digits name=\"Digits\" " + color.ToXml() + "/>\n\n";
		}
	}
	
	class DigitsOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.Label sampleLabel;
		
		EditorHighlightColor color = new EditorHighlightColor();
		
		public DigitsOptionPanel(DigitsNode parent) : base(parent)
		{
			SetupFromXml(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Digits.xfrm"));
			
			button = (Button)ControlDictionary["button"];
			button.Click += new EventHandler(EditButtonClicked);
			sampleLabel  = (Label)ControlDictionary["sampleLabel"];
		}

		public override void StoreSettings()
		{
			DigitsNode node = (DigitsNode)parent;
			
			node.Color = color;
		}
		
		public override void LoadSettings()
		{
			DigitsNode node = (DigitsNode)parent;
			
			IProperties properties = ((IProperties)PropertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			sampleLabel.Font = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));

			color = node.Color;
			PreviewUpdate(sampleLabel, color);
		}
		
		void EditButtonClicked(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(sampleLabel, color);
				}
			}
		}
	}
}
