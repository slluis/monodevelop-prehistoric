// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	
	public class EditHighlightingDialog : BaseSharpDevelopForm
	{
		private System.Windows.Forms.Button acceptBtn;
		private System.Windows.Forms.Panel propPanel;
		private System.Windows.Forms.Panel optionPanel;
		private System.Windows.Forms.TreeView nodeTree;

		private GradientLabel gradientLabel = new GradientLabel();
		private Label         bottomLabel   = new Label();
				
		public EditHighlightingDialog(TreeNode topNode) {
			SetupFromXml(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\EditDialog.xfrm"));
			
			acceptBtn = (Button)ControlDictionary["acceptBtn"];
			nodeTree  = (TreeView)ControlDictionary["nodeTree"];
			propPanel = (Panel)ControlDictionary["propPanel"];
			optionPanel = (Panel)ControlDictionary["optionPanel"];
			
			// Form editor does not work properly with the custom control
			this.gradientLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.gradientLabel.BorderStyle = BorderStyle.Fixed3D;
			this.gradientLabel.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.gradientLabel.Location = new System.Drawing.Point(0, 0);
			this.gradientLabel.Size = new System.Drawing.Size(propPanel.Width, 30);
			this.gradientLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.propPanel.Controls.Add(gradientLabel);

			this.bottomLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.bottomLabel.BorderStyle = BorderStyle.Fixed3D;
			this.bottomLabel.Location = new System.Drawing.Point(0, propPanel.Height - 2);
			this.bottomLabel.Size = new System.Drawing.Size(propPanel.Width, 2);
			this.propPanel.Controls.Add(bottomLabel);
			
			this.ClientSize = new Size(660, 530);
			this.acceptBtn.Click += new EventHandler(acceptClick);
			
			this.nodeTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.NodeTreeAfterSelect);
			this.nodeTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.NodeTreeBeforeSelect);
			
			nodeTree.Nodes.Add(topNode);
			nodeTree.ExpandAll();
			
			nodeTree.SelectedNode = topNode;
		}
		
		void acceptClick(object sender, EventArgs e)
		{
			if (currentPanel != null) {
				if (!currentPanel.ValidateSettings()) {
					return;
				}
				currentPanel.StoreSettings();
				currentPanel.ParentNode.UpdateNodeText();
			}
			
			nodeTree.Nodes.Clear();
			
			DialogResult = DialogResult.OK;
		}
				
		public class GradientLabel : Label
		{
			protected override void OnPaintBackground(PaintEventArgs pe)
			{
//				base.OnPaintBackground(pe);
				Graphics g = pe.Graphics;
				g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                        SystemColors.ControlLightLight,
			                                        SystemColors.Control),
			                                        new Rectangle(0, 0, Width, Height));
			}
			
			public GradientLabel() : base()
			{
				UseMnemonic = false;
			}
		}
		
		private NodeOptionPanel currentPanel;
		
		void NodeTreeBeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (currentPanel != null) {
				if (!currentPanel.ValidateSettings()) {
					e.Cancel = true;
					return;
				}
			}
		}
		
		void NodeTreeAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (currentPanel != null) {
				currentPanel.StoreSettings();
				currentPanel.ParentNode.UpdateNodeText();
			}
			
			optionPanel.Controls.Clear();
			NodeOptionPanel control = ((AbstractNode)e.Node).OptionPanel;
			if (control != null) {
				optionPanel.Controls.Add(control);
				currentPanel = control;
				currentPanel.LoadSettings();
			}
			
			gradientLabel.Text = " " + e.Node.Text;
			
		}
		
	}
	
}
