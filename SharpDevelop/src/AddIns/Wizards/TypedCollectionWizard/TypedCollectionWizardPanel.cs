// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace TypedCollectionGenerator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TypedCollectionWizardPanel : AbstractWizardPanel
	{
		private System.Windows.Forms.GroupBox typedCollectionGroupBox;
		private System.Windows.Forms.Label    typeNameLabel;
		private System.Windows.Forms.TextBox  typeNameTextBox;
		private System.Windows.Forms.CheckBox generateCommentsCheckBox;
		private System.Windows.Forms.CheckBox generateNestedEnumeratorCheckBox;
		private System.Windows.Forms.Label    namespaceLabel;
		private System.Windows.Forms.TextBox  namespaceTextBox;
		private System.Windows.Forms.CheckBox addValidationCheckBox;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		IProperties customizer = null;
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (IProperties)value;
			}
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			Debug.Assert(customizer != null);
			if (message == DialogMessage.Finish) {
				FileTemplate    template = (FileTemplate)customizer.GetProperty("Template");
				INewFileCreator creator  = (INewFileCreator)customizer.GetProperty("Creator");
				TypedCollectionGenerator generator = new TypedCollectionGenerator();
				
				string filename = "TypedVBCollection.vb";
				Language language = Language.VisualBasic;
				
				if (template.LanguageName != "VBNET") {
					language = Language.CSharp;
					filename = "TypedCSharpCollection.cs";
				}

				creator.SaveFile(filename, generator.Generate(language, 
				                                              typeNameTextBox.Text, 
				                                              namespaceTextBox.Text),
				                                  			  template.LanguageName, 
				                                  			  true);
			}
			return true;
		}
		
		void ChangedEvent(object sender, EventArgs e)
		{
			EnableFinish = typeNameTextBox.Text.Length > 0 && namespaceTextBox.Text.Length > 0;
		}
			
		
		public TypedCollectionWizardPanel()
		{
			InitializeComponent();
			EnableFinish = false;
			
			typeNameTextBox.TextChanged += new EventHandler(ChangedEvent);
			namespaceTextBox.TextChanged += new EventHandler(ChangedEvent);
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = true;
			
			this.typedCollectionGroupBox = new System.Windows.Forms.GroupBox();
			this.typeNameLabel = new System.Windows.Forms.Label();
			this.typeNameTextBox = new System.Windows.Forms.TextBox();
			this.generateCommentsCheckBox = new System.Windows.Forms.CheckBox();
			this.generateNestedEnumeratorCheckBox = new System.Windows.Forms.CheckBox();
			this.namespaceLabel = new System.Windows.Forms.Label();
			this.namespaceTextBox = new System.Windows.Forms.TextBox();
			this.addValidationCheckBox = new System.Windows.Forms.CheckBox();
			this.typedCollectionGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// typedCollectionGroupBox
			// 
			this.typedCollectionGroupBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.typedCollectionGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.addValidationCheckBox,
																					this.namespaceTextBox,
																					this.namespaceLabel,
																					this.generateCommentsCheckBox,
																					this.typeNameTextBox,
																					this.typeNameLabel,
																					this.generateNestedEnumeratorCheckBox});
			this.typedCollectionGroupBox.Location = new System.Drawing.Point(8, 8);
			this.typedCollectionGroupBox.Name = "typedCollectionGroupBox";
			this.typedCollectionGroupBox.Size = new System.Drawing.Size(346, 174);
			this.typedCollectionGroupBox.TabIndex = 0;
			this.typedCollectionGroupBox.TabStop = false;
			this.typedCollectionGroupBox.Text = "Typed Collection Properties";
			typedCollectionGroupBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// typeNameLabel
			// 
			this.typeNameLabel.Location = new System.Drawing.Point(8, 24);
			this.typeNameLabel.Name = "typeNameLabel";
			this.typeNameLabel.Size = new System.Drawing.Size(72, 16);
			this.typeNameLabel.TabIndex = 0;
			this.typeNameLabel.Text = "TypeName";
			// 
			// typeNameTextBox
			// 
			this.typeNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.typeNameTextBox.Location = new System.Drawing.Point(88, 24);
			this.typeNameTextBox.Name = "typeNameTextBox";
			this.typeNameTextBox.Size = new System.Drawing.Size(250, 20);
			this.typeNameTextBox.TabIndex = 0;
			this.typeNameTextBox.Text = "";
			typeNameTextBox.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			// 
			// generateCommentsCheckBox
			// 
			this.generateCommentsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.generateCommentsCheckBox.Checked = true;
			this.generateCommentsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.generateCommentsCheckBox.Location = new System.Drawing.Point(8, 80);
			this.generateCommentsCheckBox.Name = "generateCommentsCheckBox";
			this.generateCommentsCheckBox.Size = new System.Drawing.Size(330, 24);
			this.generateCommentsCheckBox.TabIndex = 2;
			this.generateCommentsCheckBox.Text = "Generate Comments";
			generateCommentsCheckBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// generateNestedEnumeratorCheckBox
			// 
			this.generateNestedEnumeratorCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.generateNestedEnumeratorCheckBox.Checked = true;
			this.generateNestedEnumeratorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.generateNestedEnumeratorCheckBox.Location = new System.Drawing.Point(8, 104);
			this.generateNestedEnumeratorCheckBox.Name = "generateNestedEnumeratorCheckBox";
			this.generateNestedEnumeratorCheckBox.Size = new System.Drawing.Size(330, 24);
			this.generateNestedEnumeratorCheckBox.TabIndex = 4;
			this.generateNestedEnumeratorCheckBox.Text = "Generate nested enumerator";
			generateNestedEnumeratorCheckBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// namespaceLabel
			// 
			this.namespaceLabel.Location = new System.Drawing.Point(8, 48);
			this.namespaceLabel.Name = "namespaceLabel";
			this.namespaceLabel.Size = new System.Drawing.Size(72, 24);
			this.namespaceLabel.TabIndex = 3;
			this.namespaceLabel.Text = "Namespace";
			// 
			// namespaceTextBox
			// 
			this.namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.namespaceTextBox.Location = new System.Drawing.Point(88, 48);
			this.namespaceTextBox.Name = "namespaceTextBox";
			this.namespaceTextBox.Size = new System.Drawing.Size(250, 20);
			this.namespaceTextBox.TabIndex = 1;
			this.namespaceTextBox.Text = "";
			namespaceTextBox.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			// 
			// addValidationCheckBox
			// 
			this.addValidationCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.addValidationCheckBox.Location = new System.Drawing.Point(8, 128);
			this.addValidationCheckBox.Name = "addValidationCheckBox";
			this.addValidationCheckBox.Size = new System.Drawing.Size(330, 24);
			this.addValidationCheckBox.TabIndex = 5;
			this.addValidationCheckBox.Text = "Add validation";
			addValidationCheckBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// Form1
			//
			this.ClientSize = new System.Drawing.Size(362, 207);
			this.Controls.AddRange(new System.Windows.Forms.Control[] { this.typedCollectionGroupBox});
			this.Text = "Collection Generator Wizard";
			this.typedCollectionGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
