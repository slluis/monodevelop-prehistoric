
namespace MonoQuery.SchemaClass
{
	using System;
	using System.Collections;
	using System.Data;
	using System.ComponentModel;
	
	using MonoQuery.Collections;
	
	using MonoDevelop.Core.Services;
	using MonoDevelop.Services;
	using MonoDevelop.Core.AddIns.Conditions;
	using MonoDevelop.Core.AddIns.Codons;
	using MonoDevelop.Core.AddIns;
	
	using MonoQuery.Connection;
	
	public abstract class AbstractMonoQuerySchemaClass : ISchemaClass
	{			
		static StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));								
				
		protected string pCatalogName	= null;
		protected string pSchemaName	= null;
		protected string pOwnerName		= null;
		protected string pName			= null;
		
		protected IConnection pDataConnection = null;
		
		///<summary>
		/// check if there are white spaces into the string.
		/// if yes, then add "[" at the begin and "]" at the end.
		///</summary>
		internal static string CheckWhiteSpace( string str )
		{
			string returnStr = str;
			
				if ( returnStr.IndexOf(" ") > -1 )
				{
					if ( returnStr.StartsWith( "[" ) == false )
					{
						returnStr = "[" + returnStr;
					}
					if ( returnStr.EndsWith( "]" ) == false )
					{
						returnStr = returnStr + "]";
					}					
				}	
				
			return returnStr;
		}

		///<summary>remove "[" at the begin and at the end of the str</summary>
		internal static string RemoveBracket( string str )
		{
			string returnStr = str;
			if ( returnStr.StartsWith( "[" ) == true )
			{
				returnStr = returnStr.Remove(0, 1);
			}
			if ( returnStr.EndsWith( "]" ) == true )
			{
				returnStr = returnStr.Remove( returnStr.Length - 1 , 1);
			}		
			return returnStr;
		}

		
		///<summary>
		/// those, are list of the childs schema.
		/// i am using a dictionnary (<see cref="MonoQuery.Collections.MonoQueryListDictionary"></see>) because is more simplest to write 
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		protected MonoQueryListDictionary pEntities = null;
		
		public string CatalogName{
			get{
				return CheckWhiteSpace( this.pCatalogName );
			}
		}
		
		public string SchemaName{
			get{
				return CheckWhiteSpace( this.pSchemaName );
			}
		}	
		
		public string OwnerName{
			get{
				return CheckWhiteSpace( this.pOwnerName );
			}
		}				
		
		public string Name{
			get{				
				return CheckWhiteSpace( this.pName );
			}
		}

		public string InternalName
		{
			get
			{
				return RemoveBracket( this.Name );
			}
		}
		
		public virtual string NormalizedName 
		{
			get 
			{
				return CheckWhiteSpace( Name );
			}
		}	
						
		///<summary>
		/// those, are list of the childs schema.
		/// i am using a dictionnary (<see cref="MonoQuery.Collections.MonoQueryListDictionary"></see>) because is more simplest to write 
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>		
		public MonoQueryListDictionary Entities
		{
			get
			{
				return pEntities;
			}
		}
		
		public IConnection Connection
		{
			get
			{
				return this.pDataConnection;
			}
		}
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see>
//		///</summary>
//		public virtual DataObject DragObject
//		{
//			get
//			{
//				return null;
//			}
//		}						
		
		//create the entities list
		protected virtual void CreateEntitiesList()
		{
			if ( this.pEntities == null )
			{
				this.pEntities = new MonoQueryListDictionary();
			}			
		}
		
		///<summary>
		/// construtor
		/// <list type="bullet">
		///	<listheader>
		///		<term>parameters</term>
		///		<description></description>
		///	</listheader>
		///	<item>
		///		<term><code>connection</code></term>
		///		<description>connection object from wich this schema is extracted</description>
		///	</item>
		///	<item>
		///		<term><code>catalogName</code></term>
		///		<description> this is the catalog of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>schemaName</code></term>
		///		<description> this is the schema of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>ownerName</code></term>
		///		<description> this is the owner name of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>name</code></term>
		///		<description> this is the name of this schema entity </description>
		///	</item>
		/// </list>
		///</summary>							
		public AbstractMonoQuerySchemaClass( IConnection connection, string catalogName, string schemaName, string ownerName, string name ) : base()
		{
			this.pCatalogName = catalogName;
			this.pSchemaName = schemaName;	
			this.pOwnerName = ownerName;			
			this.pName = name;						
			this.pDataConnection = connection;		
			
			this.CreateEntitiesList();
		}						
			
		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected abstract void OnRefresh();
			
		public void Refresh()
		{
			this.Clear();
			this.CreateEntitiesList();
			
			if (this.Connection.IsOpen == true )
			{				
				this.OnRefresh();				
			}
		}
		
		public void Clear()
		{
			if (this.pEntities != null )
			{
				this.pEntities.Clear();
			}			
		}
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public abstract object Execute( int rows, MonoQuerySchemaClassCollection parameters );
		
		///<summary> if <see cref=".Dataconnection.CatalogName">CatalogName</see> is <code>null</code> or <code>empty</code>
		/// enumerate all catalogs from the database.
		/// Else enumerate the current catalog's properties.
		/// </summary>
		public virtual MonoQuerySchemaClassCollection GetSchemaCatalogs()
		{
			return this.pDataConnection.GetSchemaCatalogs(this);
		}
		
		///<summary> if <see cref=".Dataconnection.CatalogName">CatalogName</see> is <code>null</code> or <code>empty</code>
		/// enumerate all shcema from the database.
		/// Else enumerate schemas from the current catalog.
		/// </summary>		
		public virtual MonoQuerySchemaClassCollection GetSchemaSchemas()
		{
			return this.pDataConnection.GetSchemaSchemas(this);
		}		
		
		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s tables
		/// </summary>		
		public virtual MonoQuerySchemaClassCollection GetSchemaTables()
		{
			return this.pDataConnection.GetSchemaTables( this );		
		}
		
		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s views
		/// </summary>		
		public virtual MonoQuerySchemaClassCollection GetSchemaViews()
		{
			return this.pDataConnection.GetSchemaViews( this );		
		}
		
		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s procedures
		/// </summary>		
		public virtual MonoQuerySchemaClassCollection GetSchemaProcedures()
		{
			return this.pDataConnection.GetSchemaProcedures( this );
		}
				
		public virtual MonoQuerySchemaClassCollection GetSchemaColumns()
		{	
			MonoQuerySchemaClassCollection	list = new MonoQuerySchemaClassCollection();			
			return list;
		}
		
		public virtual MonoQuerySchemaClassCollection GetSchemaParameters()
		{
			MonoQuerySchemaClassCollection	list = new MonoQuerySchemaClassCollection();			
			return list;			
		}
	}
}
