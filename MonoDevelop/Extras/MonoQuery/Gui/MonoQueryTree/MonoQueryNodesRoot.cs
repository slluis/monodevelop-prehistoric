// created on 04/11/2003 at 17:29

using System;
using System.Xml;
using System.Reflection;
using System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui;

using MonoQuery.SchemaClass;
using MonoQuery.Collections;
using MonoQuery.Connection;
using MonoQuery.Exceptions;

namespace MonoQuery.Gui.TreeView
{	
	///<summary>
	/// this is the root of all others nodes!
	///</summary>	
	public class MonoQueryNodeDatabaseRoot :  AbstractMonoQueryNode
	{										
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/DatabaseRoot";
			}
		}						
					
		public override string entityName
		{
			get
			{
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				return GettextCatalog.GetString( "Database Connections" ); 
			}
		}
		
		public MonoQueryNodeDatabaseRoot() : base()
		{
			this.Text = this.entityName;
			this.Image = "md-network";
			
//			this.ImageIndex = 0;
//			this.SelectedImageIndex = 0;
		}		

		public override void Refresh()
		{			
			this.Text = this.entityName;
			
			foreach( IMonoQueryNode node in this.Nodes )
			{
				node.Refresh();
			}
		}		
		
		public override void Clear()
		{		
			foreach( IMonoQueryNode node in this.Nodes )
			{
				node.Clear();
			}
		}
						
		public override void BuildsChilds()
		{
			// We now do this from a druid
			/*			
			IConnection connection = null;
			IMonoQueryNode node = null;
			
			try
			{			
				connection = AbstractMonoQueryConnectionWrapper.CreateFromDataConnectionLink();
				
				if (connection != null)
				{										
					string ChildClass = "";
					
					if ( MonoQueryTree.SchemaClassDict.Contains( connection.GetType().FullName ) == true )
					{
						ChildClass = MonoQueryTree.SchemaClassDict[ connection.GetType().FullName ];
					}
					
					if ( (ChildClass != null) && (ChildClass != "" ) )
					{					
						node = (IMonoQueryNode)ass.CreateInstance(ChildClass, false, BindingFlags.CreateInstance, null, new object[] {connection}, null, null);				
					}
					else
					{
						node = new MonoQueryNodeNotSupported( new MonoQueryNotSupported( connection, "", "", "", connection.GetType().FullName ) );
					}
					

					//TODO : do an interface for the node connection!
					(node as MonoQueryNodeConnection).Connect();
					this.Nodes.Add( node as TreeNode );
					node.Refresh();
					
					if ( node.Connection.IsConnectionStringWrong == true )
					{
						this.Nodes.Remove( node as TreeNode);	
					}
					else
					{
						this.Expand();
					}					
				}
			}
			catch( ConnectionStringException e )
			{
				if ( this.Nodes.Contains( node as TreeNode) == true )
				{
					this.Nodes.Remove( node as TreeNode);
				}
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError( e.Message );															
			}*/	
		}
		
		/// <summary>
		/// This will show a window so that the user can select their settings
		/// for the new connection.
		/// </summary>
		public void AddConnection()
		{
			new CreateConnectionDruid( this );
		}
	}

	///<summary>
	/// Root nodes for a connection to a database
	///</summary>		
	public class MonoQueryNodeConnection : AbstractMonoQueryNode
	{			
		IConnection pConnection = null;
		
		public override string entityNormalizedName
		{
			get
			{
				if ( this.pConnection != null )
				{
					return this.pConnection.NormalizedName;
				}
				else
				{
					return "";
				}
			}
		}
		
		public override string entityName
		{
			get
			{
				if ( this.pConnection != null )
				{
					return this.pConnection.Name;
				}
				else
				{
					return "";
				}				
			}
		}		
		
		public override IConnection Connection{ 
			get
			{
				if ( this.pConnection != null )
				{
					return this.pConnection;
				}
				else
				{
					return null;
				}
			}
		}	
		
		public override MonoQueryListDictionary Entities { 
			get{
				if ( this.Connection != null )
				{
					return this.Connection.Entities;
				}
				else
				{
					return null;
				}
			}
		}		
		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/DatabaseConnection";
			}
		}
						
		public MonoQueryNodeConnection( IConnection dataConnection ) : base( null )
		{								
			this.pConnection = dataConnection;
			this.Image = "md-mono-query-database";
			this.Text = this.pConnection.Name;
			
			this.Nodes.Add( "" ); // dummy
			
//			this.ImageIndex = 1;
//			this.SelectedImageIndex = 1;				
		}			
		
		public bool IsConnected
		{
			get 
			{
				return this.Connection.IsOpen;
			}
		}
		
		public void Disconnect()
		{
			if (this.IsConnected == true)
			{
//				this.Collapse();
				this.Nodes.Clear();
				this.Clear();
				this.Connection.Close();
//				this.ImageIndex = 1;
//				this.SelectedImageIndex = 1;
			}
		}
		
		public void Connect()
		{
			try
			{
				if ( this.IsConnected == false )
				{				
					if ( this.Connection.Open() )
					{
						this.Refresh();
//						this.ImageIndex = 2;
//						this.SelectedImageIndex = 2;																				
					}				
				}
			}
			catch( Exception e )
			{
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError( e.Message );
			}			
		}
		
		protected override void OnRefresh()
		{
			if ( this.IsConnected != true )
				this.Connect();

			this.Clear();
			this.Connection.Refresh();
		}
		
		public override void Clear()
		{			
			if ( this.Connection != null )
			{
				this.Connection.Clear();
			}
			
			base.Clear();
		}
				
		public void RemoveConnection()
		{
			this.Disconnect();
			this.pConnection = null;
			
			this.Parent.Nodes.Remove( this );
		}		
		
		public void ModifyConnection()
		{	
			IConnection Oldconnection = this.pConnection;
			bool error = false;
			try
			{
				IConnection connection = null;
				
				connection = AbstractMonoQueryConnectionWrapper.UpDateFromDataConnectionLink( this.Connection );
				
				if ( connection != null )
				{
					this.Disconnect();
					this.pConnection = connection;
					this.Refresh();	
					error = this.pConnection.IsConnectionStringWrong;
				}				
			}
			catch( ConnectionStringException e )
			{				
				error = true;
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError( e.Message );
			}	
			finally
			{
				if ( error == true )
				{
					this.pConnection = Oldconnection;
					this.Connect();
					this.Refresh();
				}
			}
		}
	}
					
	
	///<summary>
	/// Tables Root Node
	///</summary>	
	public class MonoQueryNodeTableRoot : AbstractMonoQueryNode
	{						
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>	
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/TablesRoot";
			}
		}
				
		public MonoQueryNodeTableRoot( AbstractMonoQuerySchemaClass databaseclass) : base( databaseclass )
		{
			this.Image = "md-mono-query-tables";
			
//			this.ImageIndex = 3;
//			this.SelectedImageIndex = 3;
		}
						
	}

	///<summary>
	/// Views Root Node
	///</summary>	
	public class MonoQueryNodeViewRoot : AbstractMonoQueryNode
	{					
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>	
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/ViewsRoot";
			}
		}

		public MonoQueryNodeViewRoot( AbstractMonoQuerySchemaClass databaseclass) : base( databaseclass )
		{
			this.Image = "md-mono-query-tables";
			
//			this.ImageIndex = 4;
//			this.SelectedImageIndex = 4;	
		}				
	}
		
	///<summary>
	/// Procedure Root Node
	///</summary>	
	public class MonoQueryNodeProcedureRoot : AbstractMonoQueryNode
	{							
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>	
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/ProceduresRoot";
			}
		}
		
		public MonoQueryNodeProcedureRoot( AbstractMonoQuerySchemaClass databaseclass) : base( databaseclass )
		{
			this.Image = "md-mono-query-procedure";
			
//			this.ImageIndex = 5;
//			this.SelectedImageIndex = 5;	
		}						
	}
	
	
}
