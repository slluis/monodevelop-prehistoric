// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

namespace JavaBinding
{
	public class ProjectConfigurationPropertyPanel : AbstractOptionPanel
	{
		private Label label4;
		private Label label5;
		private Label titleLabel;
		private Entry outputAssembly;

		private Entry textBox3;				
		private Button button1;
		
		private CheckButton checkBox3;
		private CheckButton checkBox5;
		private CheckButton checkBox6;
		private CheckButton checkBox7;
		
		private Label label6;
		private Label label7;
		private Label label8;

		private Entry textBox5;	//Compiler Path
		private Entry textBox6;	//Classpath
		private Entry textBox7;	//MainClass
		
		ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService (typeof (IResourceService));
		JavaCompilerParameters compilerParameters = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null)
					return true;
				compilerParameters.GenWarnings = checkBox7.Active;			
				compilerParameters.Deprecation = checkBox6.Active;			
				compilerParameters.Debugmode = checkBox5.Active;			
				compilerParameters.Optimize = checkBox3.Active;						
				compilerParameters.OutputAssembly = outputAssembly.Text;
				compilerParameters.OutputDirectory = textBox3.Text;
				
				compilerParameters.CompilerPath = textBox5.Text;
				compilerParameters.ClassPath = textBox6.Text;
				compilerParameters.MainClass = textBox7.Text;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (JavaCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			checkBox3.Active = compilerParameters.Optimize;
			checkBox5.Active = compilerParameters.Debugmode;
			checkBox6.Active = compilerParameters.Deprecation;
			checkBox7.Active = compilerParameters.GenWarnings;
			outputAssembly.Text = compilerParameters.OutputAssembly;
			textBox3.Text = compilerParameters.OutputDirectory;				
			
			textBox5.Text = compilerParameters.CompilerPath;
			textBox6.Text = compilerParameters.ClassPath;				
			textBox7.Text = compilerParameters.MainClass;				
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			using (FolderDialog fdiag = new FolderDialog (GettextCatalog.GetString ("Browse"))) {
			
				if (fdiag.Run () == (int) ResponseType.Ok) {
					//textBox3.Text = fdiag.Path;
				}
				fdiag.Hide ();
			}
		}
		
		public ProjectConfigurationPropertyPanel ()
		{
			InitializeComponent ();						
			VBox vbox = new VBox ();
			vbox.PackStart (titleLabel);
			vbox.PackStart (outputAssembly);
			vbox.PackStart (label6);
			vbox.PackStart (textBox5);
			vbox.PackStart (label7);
			vbox.PackStart (textBox6);
			vbox.PackStart (label8);
			vbox.PackStart (textBox7);
			vbox.PackStart (label4);
			HBox hbox = new HBox ();
			hbox.PackStart (checkBox5);
			hbox.PackStart (checkBox6);
			hbox.PackStart (checkBox3);
			vbox.PackStart (hbox);
			vbox.PackStart (label5);
			vbox.PackStart (textBox3);
			this.Add (vbox);
			CustomizationObjectChanged += new EventHandler (SetValues);
		}
		
		private void InitializeComponent()
		{
			this.checkBox6 = new CheckButton (GettextCatalog.GetString ("Deprecation"));
			this.checkBox5 = new CheckButton (GettextCatalog.GetString ("Debug Info"));
			this.checkBox3 = new CheckButton (GettextCatalog.GetString ("Optimize"));
			
			this.button1 = new Button ("...");
			this.button1.Clicked += new EventHandler (SelectFolder);
			this.textBox3 = new Entry ();
			this.label5 = new Label ();
			label5.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output path"));
			this.titleLabel = new Label ();
			this.outputAssembly = new Entry ();
			titleLabel.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output Assembly"));
			this.label4 = new Label ();
			label4.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Warnings and Errors"));
			
			this.checkBox7 = new CheckButton (GettextCatalog.GetString ("Generate Warnings"));
			this.textBox5 = new Entry ();
			this.textBox6 = new Entry ();
			this.textBox7 = new Entry ();
			
			this.label6 = new Label ();
			label6.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Compiler path"));
			this.label7 = new Label ();
			label7.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Classpath"));
			this.label8 = new Label ();
			label8.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Main Class"));
		}
	}
}
