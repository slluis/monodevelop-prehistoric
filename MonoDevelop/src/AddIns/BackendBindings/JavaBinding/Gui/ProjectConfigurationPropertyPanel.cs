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
		private Label label4 = new Label ();
		private Label label5 = new Label ();
		private Label titleLabel = new Label ();
		private Label label6 = new Label ();
		private Label label7 = new Label ();
		private Label label8 = new Label ();

		private Button button1;
		
		private CheckButton checkBox3;
		private CheckButton checkBox5;
		private CheckButton checkBox6;
		private CheckButton checkBox7;

		private RadioButton javac = new RadioButton ("javac");
		private RadioButton gcj;

		private Entry outputAssembly = new Entry ();
		private Entry outputDirectory = new Entry ();
		private Entry compilerPath = new Entry ();
		private Entry classPath = new Entry ();
		private Entry mainClass = new Entry ();
		
		ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService (typeof (IResourceService));
		JavaCompilerParameters compilerParameters = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null)
					return true;
				if (javac.Active)
					compilerParameters.Compiler = "javac";
				else
					compilerParameters.Compiler = "gcj";
				compilerParameters.GenWarnings = checkBox7.Active;			
				compilerParameters.Deprecation = checkBox6.Active;			
				compilerParameters.Debugmode = checkBox5.Active;			
				compilerParameters.Optimize = checkBox3.Active;						
				compilerParameters.OutputAssembly = outputAssembly.Text;
				compilerParameters.OutputDirectory = outputDirectory.Text;
				
				compilerParameters.CompilerPath = compilerPath.Text;
				compilerParameters.ClassPath = classPath.Text;
				compilerParameters.MainClass = mainClass.Text;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (JavaCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			if (compilerParameters.Compiler == "javac")
				javac.Active = true;
			else
				gcj.Active = true;
			checkBox3.Active = compilerParameters.Optimize;
			checkBox5.Active = compilerParameters.Debugmode;
			checkBox6.Active = compilerParameters.Deprecation;
			checkBox7.Active = compilerParameters.GenWarnings;
			outputAssembly.Text = compilerParameters.OutputAssembly;
			outputDirectory.Text = compilerParameters.OutputDirectory;				
			
			compilerPath.Text = compilerParameters.CompilerPath;
			classPath.Text = compilerParameters.ClassPath;				
			mainClass.Text = compilerParameters.MainClass;				
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
			HBox comps = new HBox ();
			comps.PackStart (javac);
			comps.PackStart (gcj);
			vbox.PackStart (comps);
			vbox.PackStart (compilerPath);
			vbox.PackStart (label7);
			vbox.PackStart (classPath);
			vbox.PackStart (label8);
			vbox.PackStart (mainClass);
			vbox.PackStart (label4);
			HBox hbox = new HBox ();
			hbox.PackStart (checkBox5);
			hbox.PackStart (checkBox6);
			hbox.PackStart (checkBox3);
			vbox.PackStart (hbox);
			vbox.PackStart (label5);
			vbox.PackStart (outputDirectory);
			this.Add (vbox);
			CustomizationObjectChanged += new EventHandler (SetValues);
		}

		void OnCompilerToggled (object o, EventArgs args)
		{
			if (javac.Active)
				compilerPath.Text = "javac";
			else
				compilerPath.Text = "gcj";
		}
		
		private void InitializeComponent()
		{
			gcj = new RadioButton (javac, "gcj");
			gcj.Toggled += OnCompilerToggled;
			javac.Toggled += OnCompilerToggled;

			this.checkBox6 = new CheckButton (GettextCatalog.GetString ("Deprecation"));
			this.checkBox5 = new CheckButton (GettextCatalog.GetString ("Debug Info"));
			this.checkBox3 = new CheckButton (GettextCatalog.GetString ("Optimize"));
			
			this.button1 = new Button ("...");
			this.button1.Clicked += new EventHandler (SelectFolder);
			label5.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output path"));
			this.outputAssembly = new Entry ();
			titleLabel.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output Assembly"));
			label4.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Warnings and Errors"));
			
			this.checkBox7 = new CheckButton (GettextCatalog.GetString ("Generate Warnings"));
			label6.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Compiler"));
			label7.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Classpath"));
			label8.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Main Class"));
		}
	}
}
