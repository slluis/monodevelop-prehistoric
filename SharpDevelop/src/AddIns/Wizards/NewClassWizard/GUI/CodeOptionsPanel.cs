// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace NewClassWizard {
		
	public class CodeOptionsPanel : MyPanelBase {
		
		private NewClassWizard.CodeOptionsControl codeOptions;
		private System.Windows.Forms.TextBox txtClassSummary;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label10;				
		private System.Windows.Forms.ComboBox cboLicense;
		
		public CodeOptionsPanel(){
			InitializeComponent();
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message) {
			
			if (message == DialogMessage.Finish) {
				codeOptions.SaveOptions();
				if ( cboLicense.SelectedItem != null )
					CodeFactoryOptions.WriteOption( 
		                               CodeFactoryOptions.RVALUE_LICENSE, 
		                               ((License)cboLicense.SelectedItem).Name );
			}
			return true;
		}			
		
		public override object CustomizationObject {
			get {
				return base.CustomizationObject;
			}
			set {	
				base.CustomizationObject = value;
				//put the code options object in the property bag
				//so it can be accessed by the panel that does the
				//code generation (ClassPropertiesPanel)
				base.CodeOptions = codeOptions.Options;		

				//set the license to the stored option
				newClassInfo.License = LicenseTemplates.GetLicense(
						(string)CodeFactoryOptions.ReadOption( 
						          CodeFactoryOptions.RVALUE_LICENSE, 
						          License.Empty.Name
							)
					);				
				
			}

		}

		private void cboLicense_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( cboLicense.SelectedItem != null )
				newClassInfo.License = (License)cboLicense.SelectedItem;
		}

		private void txtClassSummary_TextChanged(object sender, System.EventArgs e)
		{
			newClassInfo.Summary = txtClassSummary.Text;
		}		
		
		private void Panel_Load(object sender, System.EventArgs e)
		{
			try {

				cboLicense.Items.AddRange( LicenseTemplates.Licenses.ToArray() );
				cboLicense.Items.Add( License.Empty ); 
				
				cboLicense.SelectedIndex = cboLicense.FindStringExact( newClassInfo.License.Name );

			} catch ( Exception ) {
				
			}			
			
			cboLicense.Enabled = ( cboLicense.Items.Count > 0 );
			
		}			
		
		private void InitializeComponent(){
			this.codeOptions = new NewClassWizard.CodeOptionsControl();
			this.txtClassSummary = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cboLicense = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();			
			
			this.SuspendLayout();		

			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(285, 205);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(136, 15);
			this.label10.TabIndex = 2;
			this.label10.Text = "Insert License:";	
			// 
			// cboLicense
			// 
			this.cboLicense.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLicense.DropDownWidth = 184;
			this.cboLicense.Location = new System.Drawing.Point(285, 225);
			this.cboLicense.Name = "cboLicense";
			this.cboLicense.Size = new System.Drawing.Size(140, 20);
			this.cboLicense.Sorted = true;
			this.cboLicense.TabIndex = 9;
			this.cboLicense.Enabled = false;
			this.cboLicense.SelectedIndexChanged += new System.EventHandler(this.cboLicense_SelectedIndexChanged);
			
			// 
			// txtClassSummary
			// 
			this.txtClassSummary.AcceptsReturn = true;
			this.txtClassSummary.AcceptsTab = true;
			this.txtClassSummary.Location = new System.Drawing.Point(5, 225);
			this.txtClassSummary.Multiline = true;
			this.txtClassSummary.Name = "txtClassSummary";
			this.txtClassSummary.Size = new System.Drawing.Size(275, 140);
			this.txtClassSummary.ScrollBars = ScrollBars.Vertical;
			this.txtClassSummary.WordWrap = true;
			this.txtClassSummary.TabIndex = 0;
			this.txtClassSummary.Text = "";
			this.txtClassSummary.TextChanged += new System.EventHandler(this.txtClassSummary_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 205);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 48);
			this.label1.TabIndex = 4;
			this.label1.Text = "Class Description";
			//
			// codeOptions
			// 
			this.codeOptions.Location = new System.Drawing.Point(5, 5);
			this.codeOptions.Name = "codeOptions";
			this.codeOptions.Size = new System.Drawing.Size(700, 200);
			this.codeOptions.TabIndex = 2;
			
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																	this.codeOptions,
																	this.txtClassSummary,
																	this.label1,
																	this.label10,
																	this.cboLicense																	
																	});

			
			this.Load += new System.EventHandler(this.Panel_Load);
			this.ResumeLayout(false);			
			
		}			
	}
}
