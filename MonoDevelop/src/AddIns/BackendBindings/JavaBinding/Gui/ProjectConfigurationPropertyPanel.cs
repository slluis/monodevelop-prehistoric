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
		private Label runtimeLabel = new Label ();
		private Label labelWarnings = new Label ();
		private Label labelOutput = new Label ();
		private Label titleLabel = new Label ();
		private Label labelCompiler = new Label ();
		private Label labelClasspath = new Label ();
		private Label labelMainClass = new Label ();

		private Button browseButton;
		
		private CheckButton checkDebug = new CheckButton (GettextCatalog.GetString ("Enable debug"));
		private CheckButton checkDeprecation = new CheckButton (GettextCatalog.GetString ("Deprecated"));
		private CheckButton checkOptimize = new CheckButton (GettextCatalog.GetString ("Enable optimizations"));
		private CheckButton checkWarnings = new CheckButton (GettextCatalog.GetString ("Generate Warnings"));

		// compiler chooser
		private RadioButton javac = new RadioButton ("javac");
		private RadioButton gcj;

		// runtime chooser
		private RadioButton ikvm = new RadioButton ("ikvm");
		private RadioButton mono;
		private RadioButton java;

		private Entry outputAssembly = new Entry ();
		private Entry outputDirectory = new Entry ();
		private Entry compilerPath = new Entry ();
		private Entry classPath = new Entry ();
		private Entry mainClass = new Entry ();
		
		JavaCompilerParameters compilerParameters = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null)
					return true;

				if (javac.Active)
					compilerParameters.Compiler = JavaCompiler.Javac;
				else
					compilerParameters.Compiler = JavaCompiler.Gcj;

				if (ikvm.Active)
					compilerParameters.Runtime = JavaRuntime.Ikvm;
				else if (mono.Active)
					compilerParameters.Runtime = JavaRuntime.Mono;
				else
					compilerParameters.Runtime = JavaRuntime.Java;

				compilerParameters.GenWarnings = checkWarnings.Active;			
				compilerParameters.Deprecation = checkDeprecation.Active;			
				compilerParameters.Debugmode = checkDebug.Active;			
				compilerParameters.Optimize = checkOptimize.Active;						
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
			
			if (compilerParameters.Compiler == JavaCompiler.Javac)
				javac.Active = true;
			else
				gcj.Active = true;

			switch (compilerParameters.Runtime) {
				case JavaRuntime.Ikvm:
					ikvm.Active = true;
					break;
				case JavaRuntime.Mono:
					mono.Active = true;
					break;
				case JavaRuntime.Java:
					java.Active = true;
					break;
				default:
					ikvm.Active = true;
					break;
			}

			checkOptimize.Active = compilerParameters.Optimize;
			checkDebug.Active = compilerParameters.Debugmode;
			checkDeprecation.Active = compilerParameters.Deprecation;
			checkWarnings.Active = compilerParameters.GenWarnings;
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
			HBox hboxTitle = new HBox ();
			hboxTitle.PackStart (titleLabel, false, false, 0);
			vbox.PackStart (hboxTitle);
			vbox.PackStart (outputAssembly);
			HBox hboxCompiler = new HBox ();
			hboxCompiler.PackStart (labelCompiler, false, false, 0);
			vbox.PackStart (hboxCompiler);
			HBox comps = new HBox ();
			comps.PackStart (gcj);
			comps.PackStart (javac);
			vbox.PackStart (comps);
			vbox.PackStart (compilerPath);
			HBox hboxRuntime = new HBox ();
			hboxRuntime.PackStart (runtimeLabel, false, false, 0);
			vbox.PackStart (hboxRuntime);
			HBox runtimes = new HBox ();
			runtimes.PackStart (ikvm);
			runtimes.PackStart (mono);
			runtimes.PackStart (java);
			vbox.PackStart (runtimes);
			HBox hboxClasspath = new HBox ();
			hboxClasspath.PackStart (labelClasspath, false, false, 0);
			vbox.PackStart (hboxClasspath);
			vbox.PackStart (classPath);
			HBox hboxMainClass = new HBox ();
			hboxMainClass.PackStart (labelMainClass, false, false, 0);
			vbox.PackStart (hboxMainClass);
			vbox.PackStart (mainClass);
			HBox hboxWarnings = new HBox ();
			hboxWarnings.PackStart (labelWarnings, false, false, 0);
			vbox.PackStart (hboxWarnings);
			HBox hbox = new HBox ();
			hbox.PackStart (checkDeprecation);
			hbox.PackStart (checkDebug);
			hbox.PackStart (checkOptimize);
			vbox.PackStart (hbox);
			HBox hboxOutput = new HBox ();
			hboxOutput.PackStart (labelOutput, false, false, 0);
			vbox.PackStart (hboxOutput);
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

			mono = new RadioButton (ikvm, "mono");
			mono.Sensitive = false;
			java = new RadioButton (ikvm, "java");
			java.Sensitive = false;

			runtimeLabel.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Runtime"));

			this.browseButton = new Button ("_Browse");
			this.browseButton.Clicked += new EventHandler (SelectFolder);
			labelOutput.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output path"));
			this.outputAssembly = new Entry ();
			titleLabel.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Output Assembly"));
			labelWarnings.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Warnings and Errors"));
			
			labelCompiler.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Compiler"));
			labelClasspath.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Classpath"));
			labelMainClass.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Main Class"));
		}
	}
}
