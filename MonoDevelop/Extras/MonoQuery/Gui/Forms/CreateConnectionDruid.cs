
using System;

using Gnome;
using Glade;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.Gui.TreeView;
using MonoQuery.Collections;
using MonoQuery.Connection;
using MonoQuery.Services;

namespace MonoQuery.Gui
{
	public class CreateConnectionDruid
	{
		/// <summary>Window container</summary>
		[Widget]
		private Gtk.Window winAddconnectionDruid = null;
		
		/// <summary>druid widget</summary>
		[Widget]
		private Gnome.Druid druidAddconnection = null;
		
		/// <summary></summary>
		[Widget]
		private Gnome.DruidPageEdge pageAddconnectionStart = null;
		
		/// <summary></summary>
		[Widget]
		private Gnome.DruidPageStandard druidAddconnectionPage1 = null;
		
		/// <summary></summary>
		[Widget]
		private Gtk.ComboBox cmbProviders = null;
		
		/// <summary></summary>
		[Widget]
		private Gtk.TextView textview1 = null;
		
		/// <summary></summary>
		[Widget]
		private Gnome.DruidPageEdge druidpagefinish1 = null;
		
		/// <summary>
		/// Root node of the TreeView
		/// </summary>
		private MonoQueryNodeDatabaseRoot rootNode = null;
		
		/// <summary>
		/// List store for providers
		/// </summary>
		private ListStore store = null;
		
		/// <summary>
		/// Constructor. Loads our druid for display.
		/// </summary>
		public CreateConnectionDruid( MonoQuery.Gui.TreeView.MonoQueryNodeDatabaseRoot node )
		{
			this.rootNode = node;
			Glade.XML gxml = new Glade.XML( null, "monoquery.glade", "winAddconnectionDruid", null );
			gxml.Autoconnect( this );
			
			this.pageAddconnectionStart.ShowAll();
			this.druidAddconnectionPage1.ShowAll();
			this.druidpagefinish1.ShowAll();
			
			BuildProviderList();
		}
		
		#region // Private Methods
		private void OnCancelClicked( object o, EventArgs e )
		{
			this.winAddconnectionDruid.Destroy();
		}
		
		/// <summary>
		/// This method will get our list of providers and enter them into the
		/// combo box.
		/// </summary>
		private void BuildProviderList()
		{
			MonoQueryService monoQueryService = (MonoQueryService)
				ServiceManager.GetService( typeof( MonoQueryService ) );
			
			store = new ListStore( typeof( string ),
				typeof( ConnectionProviderDescriptor ) );
			
			foreach( ConnectionProviderDescriptor descriptor in monoQueryService.Providers )
			{
				store.AppendValues( descriptor.Name, descriptor );
			}
			
			CellRendererText colr = new CellRendererText();
			this.cmbProviders.Model = store;
			this.cmbProviders.PackStart( colr, true );
			this.cmbProviders.AddAttribute( colr, "text", 0 );
			
			// Set to first provider
			// This is where we could put some autolearning
			this.cmbProviders.Active = 0;
		}
		
		/// <summary>
		/// Event handler for when we should test the connection to the
		/// database.
		/// </summary>
		private void OnTestConnection( object o, EventArgs e )
		{
			MessageService service = (MessageService)ServiceManager.GetService(
				typeof( MessageService ) );
			
			bool successful = false;
			
			try
			{
				TreeIter iter;
				this.cmbProviders.GetActiveIter( out iter );
				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
					store.GetValue( iter, 1 );
				IConnection conn = CreateConnectionObject(
					descriptor.ProviderType,
					this.textview1.Buffer.Text );
				successful = conn.Open(); // returns true on success
			}
			catch ( Exception err )
			{
				successful = false;
			}
			
			if ( successful ) {
				service.ShowMessage( GettextCatalog.GetString(
					"Connection was successful." ) );
			} else {
				service.ShowError( GettextCatalog.GetString(
						"Error connecting to server." )
					+ "\n------------------------------\n"
					+ this.textview1.Buffer.Text );
			}
		}
		
		/// <summary>
		/// Creates an instance of a connection from a few settings.
		/// </summary>
		private IConnection CreateConnectionObject( System.Type type, string connString )
		{
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
			IConnection conn = (IConnection)ass.CreateInstance( type.FullName );
			try
			{
				conn.ConnectionString = connString;
			}
			catch ( Exception e ) {}
			
			return conn;
		}
		
		/// <summary>
		/// Create our node and connection objects. Add them to the root node
		/// of our treeview.
		/// </summary>
		private void OnFinishClicked( object o, FinishClickedArgs e )
		{
			TreeIter iter;
			MonoQueryNodeConnection node = null;
			
			try
			{
				this.cmbProviders.GetActiveIter( out iter );
				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
					store.GetValue( iter, 1 );
				IConnection conn = CreateConnectionObject(
					descriptor.ProviderType,
					this.textview1.Buffer.Text );
			
			
				node = new MonoQueryNodeConnection( conn );
			
				rootNode.Nodes.Add( node as MonoDevelop.Gui.Widgets.TreeNode );
				
				conn.Open();
				node.Expand();
			}
			catch ( Exception err )
			{
				MessageService service = (MessageService)ServiceManager.GetService(typeof(MessageService));
				service.ShowError( GettextCatalog.GetString("There was an error adding your connection. "
					+ "Please check your connection string.") );
			}
			finally
			{
				this.druidAddconnection.Destroy();
				this.winAddconnectionDruid.Destroy();
			}
		}
		#endregion // End Private Methods
	}
}