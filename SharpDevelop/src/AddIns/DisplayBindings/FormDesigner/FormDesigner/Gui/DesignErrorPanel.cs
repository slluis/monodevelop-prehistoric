// created on 22.08.2003 at 15:59
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	
	public class DesignErrorPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.RichTextBox richTextBox;
		
		public DesignErrorPanel(string errors)
		{
			InitializeComponent();
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			pictureBox.Image = resourceService.GetBitmap("Icons.32x32.Error");
			richTextBox.Text = errors;
			Dock = DockStyle.Fill;
			this.richTextBox.Text = errors;
			this.label.Text = resourceService.GetString("FormsDesigner.DesignErrorPanel.ErrorText");
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void InitializeComponent() {
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// richTextBox
			// 
			this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.richTextBox.Location = new System.Drawing.Point(8, 48);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ReadOnly = true;
			this.richTextBox.Size = new System.Drawing.Size(448, 304);
			this.richTextBox.TabIndex = 2;
			this.richTextBox.Text = "";
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(8, 8);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(36, 36);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.label.Location = new System.Drawing.Point(48, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(408, 40);
			this.label.TabIndex = 1;
			this.label.Text = "Please correct all source code errors first:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// DesignErrorPanel
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.label);
			this.Controls.Add(this.pictureBox);
			this.Name = "DesignErrorPanel";
			this.Size = new System.Drawing.Size(464, 360);
			this.ResumeLayout(false);
		}
	}
}
