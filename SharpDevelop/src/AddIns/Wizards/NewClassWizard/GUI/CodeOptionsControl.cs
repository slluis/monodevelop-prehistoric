// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace NewClassWizard
{	

	/// <summary>
	/// Summary description for CodeOptionsControl.
	/// </summary>
	public class CodeOptionsControl : System.Windows.Forms.UserControl
	{
		public delegate void OptionChangedEventHandler(object sender, OptionChangedEventArgs e);   // delegate declaration

		public class OptionChangedEventArgs : EventArgs {
			
			public readonly string Option = String.Empty;
			
			internal OptionChangedEventArgs( string option ) {
				Option = option;
			}
			
		}

		public event OptionChangedEventHandler OptionChanged;

		#region User Interface member variables
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton optBlock;
		private System.Windows.Forms.RadioButton optCStyle;
		private System.Windows.Forms.CheckBox chkBlankLine;
		private System.Windows.Forms.CheckBox chkMemberComments;
		private System.Windows.Forms.CheckBox chkGenerateStubs;
		
		#endregion
		
		private bool initComplete = false;		

		CodeFactoryOptions _factoryOptions = CodeFactoryOptions.LoadFromStorage();

		public CodeOptionsControl()
		{
			InitializeComponent();
		}

		public CodeFactoryOptions Options
		{
			get
			{
				return _factoryOptions;
			}
		}

		public void SaveOptions()
		{
			CodeFactoryOptions.SaveToStorage( _factoryOptions );
		}

		#region User Interface control event handlers
		
		private void chkGenerateStubs_CheckedChanged(object sender, System.EventArgs e)
		{
			_factoryOptions.AutoGenerateStubs = chkGenerateStubs.Checked;
			chkMemberComments.Enabled = chkGenerateStubs.Checked;
			OnOptionChanged( "AutoGenerateStubs" );
		}
		
		private void chkMemberComments_CheckedChanged(object sender, System.EventArgs e)
		{
			_factoryOptions.AutoGenerateComments = chkMemberComments.Checked;
			OnOptionChanged( "AutoGenerateComments" );
		}

		private void chkBlankLine_CheckedChanged(object sender, System.EventArgs e)
		{
			_factoryOptions.BlankLinesBetweenMembers = chkBlankLine.Checked;			
			OnOptionChanged( "BlankLinesBetweenMembers" );
		}		

		private void optCStyle_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( optCStyle.Checked )
				_factoryOptions.BracingStyle = BracingStyleEnum.bsCStyle;
			OnOptionChanged( "BracingStyle" );
		}

		private void optBlock_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( optBlock.Checked )
				_factoryOptions.BracingStyle = BracingStyleEnum.bsBlock;
			OnOptionChanged( "BracingStyle" );
		}

		public void OnOptionChanged( string option ) {
			if ( initComplete ) {
				try {
					OptionChanged( this, new OptionChangedEventArgs( option ) );
				} catch ( NullReferenceException ) {
					//Not sure if it's a bug in .NET or what
					//but if no one is listening to an event
					//a NullReferenceException occurs
				}
			}
		}
		
		#endregion

		private void UserControl_Load(object sender, System.EventArgs e)
		{
			chkMemberComments.Checked = _factoryOptions.AutoGenerateComments;
			chkGenerateStubs.Checked = _factoryOptions.AutoGenerateStubs;
			chkBlankLine.Checked = _factoryOptions.BlankLinesBetweenMembers;
			
			switch ( _factoryOptions.BracingStyle )
			{
				case BracingStyleEnum.bsBlock:
					optBlock.Checked = true;
					break;

				case BracingStyleEnum.bsCStyle :
					optCStyle.Checked = true;
					break;
			}

			initComplete = true;
		}

		#region Disposer
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkBlankLine = new System.Windows.Forms.CheckBox();
			this.chkMemberComments = new System.Windows.Forms.CheckBox();
			this.chkGenerateStubs = new System.Windows.Forms.CheckBox();
			this.optBlock = new System.Windows.Forms.RadioButton();
			this.optCStyle = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();			
			
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkGenerateStubs
			// 
			this.chkGenerateStubs.Location = new System.Drawing.Point(260, 20);
			this.chkGenerateStubs.Name = "chkGenerateStubs";
			this.chkGenerateStubs.Size = new System.Drawing.Size(150, 45);
			this.chkGenerateStubs.TabIndex = 2;
			this.chkGenerateStubs.Text = "Auto generate abstract inherited members";
			this.chkGenerateStubs.CheckedChanged += new System.EventHandler(this.chkGenerateStubs_CheckedChanged);
			// 
			// chkMemberComments
			// 
			this.chkMemberComments.Location = new System.Drawing.Point(260, 80);
			this.chkMemberComments.Name = "chkMemberComments";
			this.chkMemberComments.Size = new System.Drawing.Size(150, 45);
			this.chkMemberComments.TabIndex = 4;
			this.chkMemberComments.Text = "Auto generate member comments";
			this.chkMemberComments.CheckedChanged += new System.EventHandler(this.chkMemberComments_CheckedChanged);
			// 
			// chkBlankLine
			// 
			this.chkBlankLine.Location = new System.Drawing.Point(260, 140);
			this.chkBlankLine.Name = "chkBlankLine";
			this.chkBlankLine.Size = new System.Drawing.Size(150, 45);
			this.chkBlankLine.TabIndex = 2;
			this.chkBlankLine.Text = "Blank line between members";
			this.chkBlankLine.CheckedChanged += new System.EventHandler(this.chkBlankLine_CheckedChanged);				

			//
			// optBlock
			// 
			this.optBlock.Location = new System.Drawing.Point(16, 128);
			this.optBlock.Name = "optBlock";
			this.optBlock.Size = new System.Drawing.Size(72, 24);
			this.optBlock.TabIndex = 1;
			this.optBlock.Text = "Block";
			this.optBlock.CheckedChanged += new System.EventHandler(this.optBlock_CheckedChanged);
			// 
			// optCStyle
			// 
			this.optCStyle.Checked = true;
			this.optCStyle.Location = new System.Drawing.Point(16, 48);
			this.optCStyle.Name = "optCStyle";
			this.optCStyle.Size = new System.Drawing.Size(72, 24);
			this.optCStyle.TabIndex = 0;
			this.optCStyle.TabStop = true;
			this.optCStyle.Text = "C Style";
			this.optCStyle.CheckedChanged += new System.EventHandler(this.optCStyle_CheckedChanged);

			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																			this.label9,
																			this.label8,
																			this.optBlock,
																			this.optCStyle});
			this.groupBox2.Location = new System.Drawing.Point(10, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 176);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "C# Bracing Style";
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.Color.White;
			this.label8.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(88, 24);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(136, 64);
			this.label8.TabIndex = 2;
			this.label8.Text = "if ( true )\n{\n   //statements\n}";
			//
			// label9
			// 
			this.label9.BackColor = System.Drawing.Color.White;
			this.label9.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label9.Location = new System.Drawing.Point(88, 112);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(136, 48);
			this.label9.TabIndex = 2;
			this.label9.Text = "if ( true ) {\n   //statements\n}";		
			// 
			// CodeOptionsControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
															this.chkGenerateStubs,
															this.chkBlankLine,
															this.chkMemberComments,	
															this.groupBox2
															});
			this.Name = "CodeOptionsControl";
			this.Size = new System.Drawing.Size(312, 344);
			this.Load += new System.EventHandler(this.UserControl_Load);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
