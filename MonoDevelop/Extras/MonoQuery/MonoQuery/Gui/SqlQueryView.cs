//
// SqlQueryView.cs
//
// Author:
//   Christian Hergert <chris@mosaix.net>
//
// Copyright (C) 2005 Christian Hergert
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Data;

using Gtk;
using GtkSourceView;

using Mono.Data.Sql;

using MonoDevelop.Gui;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Widgets;

namespace MonoQuery
{
	public class SqlQueryView : AbstractViewContent
	{
		protected Frame control;
		protected SourceView sourceView;
		protected ComboBox providers;
		
		protected ListStore model;
		
		protected EventHandler changedHandler;
		
		protected MonoQueryService service;
		
		public SqlQueryView () : base ()
		{
			control = new Frame ();
			control.Show ();
			
			VBox vbox = new VBox ();
			vbox.Show ();
			
			Tooltips tips = new Tooltips ();
			
			Toolbar toolbar = new Toolbar ();
			vbox.PackStart 	(toolbar, false, true, 0);
			toolbar.Show ();
			
			Image image = new Image ();
			image.Pixbuf = Gdk.Pixbuf.LoadFromResource ("MonoQuery.Execute");
			image.Show ();
			
			Button execute = new Button (image);
			execute.Clicked += new EventHandler (OnExecute);
			execute.Relief = ReliefStyle.None;
			tips.SetTip (execute, "Execute", "");
			toolbar.Add (execute);
			execute.Show ();
			
			image = new Image ();
			image.Pixbuf = Gdk.Pixbuf.LoadFromResource ("MonoQuery.RunFromCursor");
			image.Show ();
			
			Button run = new Button (image);
			run.Clicked += new EventHandler (OnRunFromCursor);
			run.Relief = ReliefStyle.None;
			tips.SetTip (run, "Run from cursor", "");
			toolbar.Add (run);
			run.Show ();
			
			image = new Image ();
			image.Pixbuf = Gdk.Pixbuf.LoadFromResource ("MonoQuery.Explain");
			image.Show ();
			
			Button explain = new Button (image);
			explain.Clicked += new EventHandler (OnExplain);
			explain.Relief = ReliefStyle.None;
			tips.SetTip (explain, "Explain query", "");
			toolbar.Add (explain);
			explain.Show ();
			
			image = new Image ();
			image.Pixbuf = Gdk.Pixbuf.LoadFromResource ("MonoQuery.Stop");
			image.Show ();
			
			Button stop = new Button (image);
			stop.Clicked += new EventHandler (OnStop);
			stop.Relief = ReliefStyle.None;
			stop.Sensitive = false;
			tips.SetTip (stop, "Stop", "");
			toolbar.Add (stop);
			stop.Show ();

			VSeparator sep = new VSeparator ();
			toolbar.Add (sep);
			sep.Show ();
			
			model = new ListStore (typeof (string), typeof (DbProviderBase));
			
			providers = new ComboBox ();
			providers.Model = model;
			CellRendererText ctext = new CellRendererText ();
			providers.PackStart (ctext, true);
			providers.AddAttribute (ctext, "text", 0);
			toolbar.Add (providers);
			providers.Show ();
			
			SourceLanguagesManager lm = new SourceLanguagesManager ();
			SourceLanguage lang = lm.GetLanguageFromMimeType ("text/x-sql");
			SourceBuffer buf = new SourceBuffer (lang);
			buf.Highlight = true;
			sourceView = new SourceView (buf);
			sourceView.ShowLineNumbers = true;
			sourceView.Show ();
			
			ScrolledWindow scroller = new ScrolledWindow ();
			scroller.Add (sourceView);
			scroller.Show ();
			vbox.PackStart (scroller, true, true, 0);
			
			control.Add (vbox);
			
			service = (MonoQueryService)
				ServiceManager.GetService (typeof (MonoQueryService));
			changedHandler
				 = (EventHandler) Runtime.DispatchService.GuiDispatch (
					new EventHandler (OnProvidersChanged));
			service.Providers.Changed += changedHandler;
			
			foreach (DbProviderBase p in service.Providers) {
				model.AppendValues (p.Name, p);
			}
		}
		
		public string Text {
			get {
				return sourceView.Buffer.Text;
			}
			set {
				sourceView.Buffer.Text = value;
			}
		}
		
		public override Gtk.Widget Control {
			get {
				return control;
			}
		}
		
		public override string UntitledName {
			get {
				return "SQL Query";
			}
		}
		
		public override void Dispose ()
		{
			service.Providers.Changed -= changedHandler;
		}
		
		public override void Load (string filename)
		{
		}
		
		public DbProviderBase Connection {
			set {
				int i = 0;
				foreach (object[] row in model) {
					if (row[1] == value)
						providers.Active = i;
					i++;
				}
			}
			get {
				TreeIter iter;
				providers.GetActiveIter (out iter);
				return (DbProviderBase) model.GetValue (iter, 1);
			}
		}
		
		void OnExecute (object sender, EventArgs args)
		{
			Runtime.Gui.StatusBar.BeginProgress (
				GettextCatalog.GetString("Execuing sql query on")
				+ String.Format (" {0}", Connection.Name));
			Runtime.Gui.StatusBar.SetProgressFraction (0.1);
			
			string query = sourceView.Buffer.Text;
			SQLCallback callback = (SQLCallback)
				Runtime.DispatchService.GuiDispatch (
				new SQLCallback (OnExecuteReturn));
			
			Runtime.Gui.StatusBar.SetMessage (
				GettextCatalog.GetString ("Query sent, waiting for response."));
			Runtime.Gui.StatusBar.SetProgressFraction (0.5);
			
			Connection.ExecuteSQL (query, callback);
		}
		
		void OnExecuteReturn (object sender, object results)
		{
			Runtime.Gui.StatusBar.SetMessage (
				GettextCatalog.GetString ("Query results received"));
			Runtime.Gui.StatusBar.SetProgressFraction (0.9);
			
			if (results == null) {
				Runtime.Gui.StatusBar.ShowErrorMessage (
					GettextCatalog.GetString ("Invalid select query"));
			} else {
				DataGridView dataView = new DataGridView (results as DataTable);
				Runtime.Gui.Workbench.ShowView (dataView, true);
			}
			
			Runtime.Gui.StatusBar.EndProgress ();
		}
		
		void OnRunFromCursor (object sender, EventArgs args)
		{
			Runtime.Gui.StatusBar.BeginProgress (
				GettextCatalog.GetString("Execuing sql query on")
				+ String.Format (" {0}", Connection.Name));
			Runtime.Gui.StatusBar.SetProgressFraction (0.1);
			
			string query = sourceView.Buffer.GetSlice (
				sourceView.Buffer.GetIterAtMark (sourceView.Buffer.InsertMark),
				sourceView.Buffer.EndIter, false);
			SQLCallback callback = (SQLCallback)
				Runtime.DispatchService.GuiDispatch (
				new SQLCallback (OnExecuteReturn));
			
			Runtime.Gui.StatusBar.SetMessage (
				GettextCatalog.GetString ("Query sent, waiting for response."));
			Runtime.Gui.StatusBar.SetProgressFraction (0.5);
			
			Connection.ExecuteSQL (query, callback);
		}
		
		void OnExplain (object sender, EventArgs args)
		{
			Runtime.Gui.StatusBar.BeginProgress (
				GettextCatalog.GetString("Execuing sql query on")
				+ String.Format (" {0}", Connection.Name));
			Runtime.Gui.StatusBar.SetProgressFraction (0.1);
			
			string query = sourceView.Buffer.Text;
			SQLCallback callback = (SQLCallback)
				Runtime.DispatchService.GuiDispatch (
				new SQLCallback (OnExecuteReturn));
			
			Runtime.Gui.StatusBar.SetMessage (
				GettextCatalog.GetString ("Query sent, waiting for response."));
			Runtime.Gui.StatusBar.SetProgressFraction (0.5);
			
			Connection.ExplainSQL (query, callback);
		}
		
		void OnStop (object sender, EventArgs args)
		{
		}
		
		void OnProvidersChanged (object sender, EventArgs args)
		{
			DbProviderBase current = Connection;
			model.Clear ();
			
			foreach (DbProviderBase p in service.Providers) {
				TreeIter cur = model.AppendValues (p.Name, p);
				if (p == current)
					providers.SetActiveIter (cur);
			}
		}
	}
}