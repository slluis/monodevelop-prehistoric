// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA

using System;

namespace MonoQuery.Gui
{
	using Gtk;
	using Gnome;
	
	using MonoDevelop.Services;
	using MonoDevelop.Core.Services;
	
	using MonoQuery.Gui.TreeView;
	using MonoQuery.Services;
	using MonoQuery.Collections;
	using MonoQuery.Connection;
	
	public class CreateConnectionDruid
	{
		#region // Properties
		Gnome.Druid druid = null;
		Gnome.DruidPageEdge		startPage = null;
		Gnome.DruidPageEdge		endPage = null;
		Gnome.DruidPageStandard	settingsPage = null;
		Gtk.ListStore			store = null;
		Gtk.ComboBox			providers = null;
		Gtk.Button				testButton = null;
		Gtk.Entry				connStringEntry = null;
		Gtk.Entry				serverEntry = null;
		Gtk.Entry				databaseEntry = null;
		Gtk.Entry				userEntry = null;
		Gtk.Entry				passEntry = null;
		Gtk.Entry				otherEntry = null;
		MonoQueryNodeDatabaseRoot rootNode = null;
		#endregion // End Properties
		
		#region // Constructors
		public CreateConnectionDruid( MonoQueryNodeDatabaseRoot root )
		{
			rootNode = root;
			
			druid = new Druid(
					GettextCatalog.GetString( "Create Database Connection" ),
					true
				);
			druid.Cancel += new EventHandler( OnCancelClicked );
			
			startPage = new DruidPageEdge( EdgePosition.Start,
					true,
					GettextCatalog.GetString( "Create Database Connection" ),
					GettextCatalog.GetString( "This wizard will help you create"
						+ " a new database connection for use within "
						+ "MonoDevelop. You will need your connection string as"
						+ " well as a supported database." ),
					null, null, null
				);
			
			settingsPage = new DruidPageStandard();
			settingsPage.Title = GettextCatalog.GetString(
				"Connection Settings" );
			VBox vbox1 = new VBox();
			HBox hbox1 = new HBox();
			VBox vbox2 = new VBox();
			VBox vbox3 = new VBox();
			hbox1.PackStart( vbox2, false, false, 2 );
			HBox hbox4 = new HBox();
			hbox4.PackStart( new Label(GettextCatalog.GetString("Provider")), false, false, 0 );
			hbox4.PackStart( new Label() );
			vbox2.PackStart( hbox4 );
			HBox hbox5 = new HBox();
			hbox5.PackStart( new Label(GettextCatalog.GetString("Connection String")), false, false, 0 );
			hbox5.PackStart( new Label() );
			vbox2.PackStart( hbox5 );
			HBox hbox6 = new HBox();
			hbox6.PackStart( new Label(GettextCatalog.GetString("Server")), false, false, 0 );
			hbox6.PackStart( new Label() );
			vbox2.PackStart( hbox6 );
			HBox hbox7 = new HBox();
			hbox7.PackStart( new Label(GettextCatalog.GetString("Database")), false, false, 0 );
			hbox7.PackStart( new Label() );
			vbox2.PackStart( hbox7 );
			HBox hbox8 = new HBox();
			hbox8.PackStart( new Label(GettextCatalog.GetString("User ID")), false, false, 0 );
			hbox8.PackStart( new Label() );
			vbox2.PackStart( hbox8 );
			HBox hbox9 = new HBox();
			hbox9.PackStart( new Label(GettextCatalog.GetString("Password")), false, false, 0 );
			hbox9.PackStart( new Label() );
			vbox2.PackStart( hbox9 );
			HBox hbox10 = new HBox();
			hbox10.PackStart( new Label(GettextCatalog.GetString("Other")), false, false, 0 );
			vbox2.PackStart( hbox10 );
			hbox1.PackStart( vbox3, true, true, 2 );
			providers = new Gtk.ComboBox();
			connStringEntry = new Gtk.Entry();
			serverEntry = new Gtk.Entry();
			databaseEntry = new Gtk.Entry();
			userEntry = new Gtk.Entry();
			passEntry = new Gtk.Entry();
			otherEntry = new Gtk.Entry();
			serverEntry.Changed += new EventHandler( OnSettingsChanged );
			databaseEntry.Changed += new EventHandler( OnSettingsChanged );
			userEntry.Changed += new EventHandler( OnSettingsChanged );
			passEntry.Changed += new EventHandler( OnSettingsChanged );
			otherEntry.Changed += new EventHandler( OnSettingsChanged );
			vbox3.PackStart( providers, false, false, 2 );
			vbox3.PackStart( connStringEntry, false, false, 2 );
			vbox3.PackStart( serverEntry, false, false, 2 );
			vbox3.PackStart( databaseEntry, false, false, 2 );
			vbox3.PackStart( userEntry, false, false, 2 );
			vbox3.PackStart( passEntry, false, false, 2 );
			vbox3.PackStart( otherEntry, false, false, 2 );
			vbox1.PackStart( hbox1, false, false, 0 );
			testButton = new Button( GettextCatalog.GetString(
					"Test Connection" ) );
			testButton.Clicked += new EventHandler( OnTestConnectionClicked );
			HBox hbox2 = new HBox();
			hbox2.PackStart( new Label(), true, true, 0 );
			hbox2.PackStart( testButton, false, false, 0 );
			vbox1.PackStart( hbox2, false, false, 2 );
			settingsPage.AppendItem( "", vbox1, "" );
			
			endPage = new DruidPageEdge( EdgePosition.Finish,
					true,
					GettextCatalog.GetString( "Create Database Connection" ),
					GettextCatalog.GetString( "We are now ready to create your"
						+ " new connection. Press Finish to create the "
						+ "connection." ),
					null, null, null
				);
			endPage.FinishClicked += new FinishClickedHandler( OnFinishClicked );
			
			druid.AppendPage( startPage );
			druid.AppendPage( settingsPage );
			druid.AppendPage( endPage );
			
			BuildProviderList();
			
			druid.ShowAll();
		}
		#endregion // End Constructors
		
		#region // Methods
		private void OnCancelClicked( object sender, EventArgs args )
		{
			druid.Destroy();
		}
		
		private void OnTestConnectionClicked( object sender, EventArgs args )
		{
			MessageService service = (MessageService)ServiceManager.GetService(
				typeof( MessageService ) );
			
			bool successful = false;
			
			try
			{
				TreeIter iter;
				this.providers.GetActiveIter( out iter );
				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
					store.GetValue( iter, 1 );
				IConnection conn = CreateConnectionObject(
					descriptor.ProviderType,
					this.connStringEntry.Text );
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
					+ this.connStringEntry.Text );
			}
		}
		
		private void OnSettingsChanged( object sender, EventArgs args )
		{
			connStringEntry.Text = "Server=" + serverEntry.Text + ";"
				+ "Database=" + databaseEntry.Text + ";"
				+ (userEntry.Text.Equals( "" ) ? "" : "User ID=" + userEntry.Text + ";")
				+ (passEntry.Text.Equals( "" ) ? "" : "Password=" + passEntry.Text + ";")
				+ otherEntry.Text;
		}
		
		private void OnFinishClicked( object sender, FinishClickedArgs args )
		{
			MessageService service = (MessageService)ServiceManager.GetService(
				typeof( MessageService ) );
			
			bool successful = false;
			IConnection conn = null;
			
			try
			{
				TreeIter iter;
				this.providers.GetActiveIter( out iter );
				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
					store.GetValue( iter, 1 );
				conn = CreateConnectionObject(
					descriptor.ProviderType,
					this.connStringEntry.Text );
				successful = conn.Open();
			}
			catch ( Exception err )
			{
				successful = false;
			}
			finally
			{
				if ( successful ) {
					MonoQueryNodeConnection node = new MonoQueryNodeConnection( conn );
					if ( (node as MonoDevelop.Gui.Widgets.TreeNode) != null ) {
						rootNode.Nodes.Add( node as MonoDevelop.Gui.Widgets.TreeNode );
					}
				} else {
					service.ShowMessage( GettextCatalog.GetString(
						"Your connection string or provider was invalid" ) );
				}
			}
			
			druid.Destroy();
		}

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
			this.providers.Model = store;
			this.providers.PackStart( colr, true );
			this.providers.AddAttribute( colr, "text", 0 );
			
			// Set to first provider
			// This is where we could put some autolearning
			this.providers.Active = 0;
		}
		
		private IConnection CreateConnectionObject( System.Type type, string connString )
		{
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
			IConnection conn = (IConnection)ass.CreateInstance( type.FullName );
			try
			{
				conn.ConnectionString = connString;
			}
			catch ( Exception e )
			{}
			
			return conn;
		}
		#endregion // End Methods
	}
}


//
//using System;
//
//using Gnome;
//using Glade;
//using Gtk;
//
//using MonoDevelop.Core.Services;
//using MonoDevelop.Services;
//
//using MonoQuery.Gui.TreeView;
//using MonoQuery.Collections;
//using MonoQuery.Connection;
//using MonoQuery.Services;
//
//namespace MonoQuery.Gui
//{
//	public class CreateConnectionDruid
//	{
//		[Widget]
//		private Gtk.Window winAddconnectionDruid = null;
//		
//		[Widget]
//		private Druid druidAddconnection = null;
//		
//		[Widget]
//		private DruidPageEdge pageAddconnectionStart = null;
//		
//		[Widget]
//		private Gnome.DruidPageStandard druidAddconnectionPage1 = null;
//		
//		[Widget]
//		private Gtk.ComboBox comboProviders = null;
//		
//		[Widget]
//		private Gtk.Entry txtConnString = null;
//		
//		[Widget]
//		private Gtk.Entry txtServer = null;
//		
//		[Widget]
//		private Gtk.Entry txtDatabase = null;
//		
//		[Widget]
//		private Gtk.Entry txtUserID = null;
//		
//		[Widget]
//		private Gtk.Entry txtOthers = null;
//		
//		[Widget]
//		private DruidPageEdge druidpagefinish1 = null;
//
//		[Widget]
//		private Gtk.Button btnTest = null;
//
//		[Widget]
//		private Gtk.Entry txtPass = null;
//		
//		private MonoQueryNodeDatabaseRoot rootNode = null;
//		
//		private ListStore store = null;
//
//		private TreeIter cur;
//		
//		/// <summary>
//		/// Constructor. Loads our druid for display.
//		/// </summary>
//		public CreateConnectionDruid( MonoQuery.Gui.TreeView.MonoQueryNodeDatabaseRoot node )
//		{
//			this.rootNode = node;
//			Glade.XML gxml = new Glade.XML( null, "monoquery.glade", "winAddconnectionDruid", null );
//			gxml.Autoconnect( this );
//			
//			this.pageAddconnectionStart.ShowAll();
//			this.druidAddconnectionPage1.ShowAll();
//			this.druidpagefinish1.ShowAll();
//			
//			BuildProviderList();
//		}
//		
//		#region // Private Methods
//		private void OnCancelClicked( object o, EventArgs args )
//		{
//			this.winAddconnectionDruid.Destroy();
//		}
//		
//		private void OnEntryChanged( object o, EventArgs args )
//		{
//			if ( this.txtConnString != null )
//				this.txtConnString.Text = 
//					  "Server=" + this.txtServer.Text + ";"
//					+ "Database=" + this.txtDatabase.Text + ";"
//					+ "User ID=" + this.txtUserID.Text + ";"
//					+ "Password=" + this.txtPass.Text + ";"
//					+ this.txtOthers.Text;
//		}
//
//		/// <summary>
//		/// This method will get our list of providers and enter them into the
//		/// combo box.
//		/// </summary>
//		private void BuildProviderList()
//		{
//			MonoQueryService monoQueryService = (MonoQueryService)
//				ServiceManager.GetService( typeof( MonoQueryService ) );
//			
//			store = new ListStore( typeof( string ),
//				typeof( ConnectionProviderDescriptor ) );
//			
//			foreach( ConnectionProviderDescriptor descriptor in monoQueryService.Providers )
//			{
//				store.AppendValues( descriptor.Name, descriptor );
//			}
//			
//			CellRendererText colr = new CellRendererText();
//			this.comboProviders.Model = store;
//			this.comboProviders.PackStart( colr, true );
//			this.comboProviders.AddAttribute( colr, "text", 0 );
//			
//			// Set to first provider
//			// This is where we could put some autolearning
//			this.comboProviders.Active = 0;
//		}
//		
//		/// <summary>
//		/// Event handler for when we should test the connection to the
//		/// database.
//		/// </summary>
//		private void OnTestConnection( object o, EventArgs e )
//		{
//			MessageService service = (MessageService)ServiceManager.GetService(
//				typeof( MessageService ) );
//			
//			bool successful = false;
//			
//			try
//			{
//				TreeIter iter;
//				this.comboProviders.GetActiveIter( out iter );
//				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
//					store.GetValue( iter, 1 );
//				IConnection conn = CreateConnectionObject(
//					descriptor.ProviderType,
//					this.txtConnString.Text );
//				successful = conn.Open(); // returns true on success
//			}
//			catch ( Exception err )
//			{
//				successful = false;
//			}
//			
//			if ( successful ) {
//				service.ShowMessage( GettextCatalog.GetString(
//					"Connection was successful." ) );
//			} else {
//				service.ShowError( GettextCatalog.GetString(
//						"Error connecting to server." )
//					+ "\n------------------------------\n"
//					+ this.txtConnString.Text );
//			}
//		}
//		
//		/// <summary>
//		/// Creates an instance of a connection from a few settings.
//		/// </summary>
//		private IConnection CreateConnectionObject( System.Type type, string connString )
//		{
//			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
//			IConnection conn = (IConnection)ass.CreateInstance( type.FullName );
//			try
//			{
//				conn.ConnectionString = connString;
//			}
//			catch ( Exception e ) {}
//			
//			return conn;
//		}
//
//		/// <summary>
//		/// Save the provider iter so it can be used later.
//		/// </summary>
//		private void OnProvidersChanged( object o, EventArgs args )
//		{
//			this.comboProviders.GetActiveIter( out cur );
//		}
//		
//		/// <summary>
//		/// Create our node and connection objects. Add them to the root node
//		/// of our treeview.
//		/// </summary>
//		private void OnFinishClicked( object o, FinishClickedArgs e )
//		{
//			MonoQueryNodeConnection node = null;
//			
//			try
//			{
//				ConnectionProviderDescriptor descriptor = (ConnectionProviderDescriptor)
//				store.GetValue( cur, 1 );
//				IConnection conn = CreateConnectionObject(
//					descriptor.ProviderType,
//					this.txtConnString.Text );
//				conn.Open();
//			
//				node = new MonoQueryNodeConnection( conn );
//				rootNode.Nodes.Add( node );
//				node.Expand();
//			}
//			catch ( Exception err )
//			{
//				MessageService service = (MessageService)ServiceManager.GetService(typeof(MessageService));
//				service.ShowError( GettextCatalog.GetString("There was an error adding your connection. "
//					+ "Please check your connection string.") );
//				Console.WriteLine( err.Message + "\n\n" + err );
//			}
//			finally
//			{
//				this.druidAddconnection.Destroy();
//				this.winAddconnectionDruid.Destroy();
//			}
//		}
//		#endregion // End Private Methods
//	}
//}
//