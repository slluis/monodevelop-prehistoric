
using System;
using System.Data;

using Gtk;
using GtkSourceView;

using MonoQuery.Connection;
using MonoDevelop;
using MonoDevelop.Gui;
using MonoDevelop.Services;

namespace MonoQuery.Gui.SqlQueryView
{
	/// <summary>
	/// SQL Query View to execute sql queries from a MonoQuery IConnection.
	/// </summary>
	public class SqlQueryView : AbstractViewContent
	{
		#region // Private Properties
		
		private IConnection connection = null;
		private VPaned vpaned = null;
		private SourceView sourceView = null;
		private SourceBuffer sourceBuffer = null;
		private ScrolledWindow sourceScroller = null;
		private VBox vbox1 = null;
		private Notebook notebook = null;
		private TextView logView = null;
		private ScrolledWindow logScroller = null;
		private MonoDevelop.Gui.Widgets.DataGrid outputView = null;
		private SqlQueryViewToolbar toolbar = null; 
		
		#endregion // End Private Properties
		
		/// <summary>
		/// Default constructor. Setup our gui widgets and IConnection
		/// </summary>
		public SqlQueryView( IConnection conn ) : base()
		{
			this.connection = conn;
			this.UntitledName = Connection.Name;
			
			this.vpaned = new VPaned();
			
			this.vbox1 = new VBox();
			this.toolbar = new SqlQueryViewToolbar();
			this.toolbar.Run += new EventHandler( OnQueryRun );
			this.vbox1.PackStart( this.toolbar, false, false, 0 );
			
			SourceLanguagesManager lm = new SourceLanguagesManager();
			this.sourceBuffer = new SourceBuffer(
				lm.GetLanguageFromMimeType("text/x-sql") );
			this.sourceBuffer.Highlight = true;
			
			this.sourceView = new SourceView( sourceBuffer );
			this.sourceScroller = new ScrolledWindow();
			this.sourceScroller.Add( sourceView );
			this.vbox1.PackStart( this.sourceScroller, true, true, 0 );
			this.vpaned.Pack1( vbox1, true, true );
			
			this.notebook = new Notebook();
			this.notebook.TabPos = PositionType.Bottom;
			this.logView = new TextView();
			this.logView.Editable = false;
			this.logScroller = new ScrolledWindow();
			this.logScroller.Add( logView );
			this.outputView = new MonoDevelop.Gui.Widgets.DataGrid();
			this.notebook.AppendPage( logScroller, new Label("Log") );
			this.notebook.AppendPage( outputView, new Label("Output") );
			
			this.vpaned.Pack2( notebook, true, true );
			
			this.vpaned.ShowAll();
		}
		
		public override Gtk.Widget Control {
			get { return this.vpaned; }
		}
		
		public override void Load( string fileName )
		{
			// Load contents of file into the sourceView
			// TODO:
		}
		
		public IConnection Connection {
			get { return this.connection; }
		}
		
		public void OnQueryRun( object o, EventArgs e )
		{
			DateTime start = DateTime.Now;
			
			try
			{
				this.logView.Buffer.Text +=
					  "\n\n-------------------------------------------"
					+ "\n Attempting Query\n                     ";
				DataSet ds = (DataSet)this.Connection.ExecuteSQL(
					this.sourceView.Buffer.Text, 0 );
				this.outputView.DataSource = ds;
				this.outputView.DataBind();
				this.notebook.Page = 1;
			}
			catch ( Exception err )
			{
				this.logView.Buffer.Text +=
					  "\n Exception caught during query."
					+ "\n " + err.StackTrace;
				this.notebook.Page = 0;
			}
			finally
			{
				this.logView.Buffer.Text +=
					  "\n Query completed in " + (DateTime.Now - start)
					+ "\n-------------------------------------------";
				this.logView.ScrollToMark( this.logView.Buffer.InsertMark, 0.4,
					true, 0.0, 1.0);
			}
		}
	}
	
	public class SqlQueryViewToolbar : Gtk.Toolbar
	{
		public event EventHandler Run;
		
		public SqlQueryViewToolbar() : base()
		{
			this.ToolbarStyle = ToolbarStyle.BothHoriz;
			Gtk.ToolButton execute = new Gtk.ToolButton( Gtk.Stock.Execute );
			
			execute.Clicked += new EventHandler( OnExecuteClicked );
			Insert( execute, 0 );
		}
		
		private void OnExecuteClicked( object sender, EventArgs args )
		{
			this.Run( this, new EventArgs() );
		}
	}
}
