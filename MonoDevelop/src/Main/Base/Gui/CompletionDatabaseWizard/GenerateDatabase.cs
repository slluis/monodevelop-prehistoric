using System;
using Gtk;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	class ProgressMonitorBar : Gtk.ProgressBar, IProgressMonitor
	{
		private bool canceled;
		public void BeginTask(string name, int totalWork)
		{
			this.DiscreteBlocks = (uint)totalWork;
		}
				
		public void Worked(double work, string status)
		{
			double f = work / this.DiscreteBlocks;
			//Console.WriteLine("% is {0}", f);
			this.Fraction = f;
		}
		public void Done()
		{
			this.Fraction = 1;
		}
		public bool Canceled {
			get { return canceled;  }
			set { canceled = value; }
		}
		public string TaskName {
			get { return ""; }
			set {  ;}
		}
	}

	class GeneratorProgress : Gtk.Window
	{
		Gtk.Button cancel;
		IDatabaseGenerator generator;
		ProgressMonitorBar progress;
		
		public GeneratorProgress (IDatabaseGenerator generator) : base(GettextCatalog.GetString ("Code completion database generator"))
		{
			try {
				this.generator = generator;
			
				Gtk.VBox vb = new Gtk.VBox(false, 6);
				this.Add(vb);
				
				vb.Add(new Gtk.Label(GettextCatalog.GetString ("Creating database...")));

				progress = new ProgressMonitorBar();
				vb.Add(progress);

				cancel = new Gtk.Button(GettextCatalog.GetString ("Cancel"));
				cancel.Clicked += new EventHandler(DoCancel);
				if (!generator.Cancelable) {
					cancel.Sensitive = false;
					Tooltips t = new Tooltips();
					t.SetTip(cancel, GettextCatalog.GetString ("Cancelling not available"),
							GettextCatalog.GetString ("This type of code completion database generator can not be canceled"));
				}
				vb.Add(cancel);
				this.ShowAll();
				while (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();
		
				generator.Generate(progress);
			} finally {
				this.Destroy();
			}
		}

		private void DoCancel(object sender, EventArgs args)
		{
			((Button)sender).Sensitive = false;
			progress.Canceled = true;
		}
	}
	
	class GenerateDatabase {
		Window druidHost;
		
		public void Start()
		{
			CodeCompletionDruid druid = new CodeCompletionDruid();
			druidHost = new Gtk.Window(GettextCatalog.GetString ("Code completion database druid"));
			druidHost.Add(druid);
			druid.Finished += new DruidFinished(GotDruidData);
			druid.Canceled += new DruidCanceled(DruidCanceled);
			druidHost.ShowAll();

		}

		void GotDruidData(object sender, IDatabaseGenerator gen)
		{
			GeneratorProgress gp = null;
			try {
				druidHost.Destroy();

				gp = new GeneratorProgress(gen);
			} catch (Exception e) {
				IMessageService messageService = (IMessageService) ServiceManager.Services.GetService (typeof (IMessageService));
				string message = e.Message;
				if (e.InnerException != null)
					message += ": " + e.InnerException.Message;
				messageService.ShowError(message);
				if (gp != null)
					gp.Destroy();
				Start();
				return;
			}
			// restart  & exit 
			ServiceManager.Services.UnloadAllServices();
			// FIXME: handle this elegantly
			Console.WriteLine ("******************************************************************************");
			Console.WriteLine (GettextCatalog.GetString ("Attempting to restart MonoDevelop, if you get any exceptions, restart manually"));
			Console.WriteLine ("******************************************************************************");
			// not everyone can run .exe's directly
			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo ("mono", "MonoDevelop.exe");
			psi.UseShellExecute = false;
			System.Diagnostics.Process.Start (psi);
			Gtk.Application.Quit ();

		}

		public void DruidCanceled(object o)
		{
			druidHost.Destroy();
		}
	}
}
