// created on 11/11/2003 at 14:06
namespace MonoQuery.Connection
{	
	
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.OleDb;
	using System.Reflection;
	
	using MonoDevelop.Core.Services;
	using MonoDevelop.Services;
	using MonoDevelop.Core.AddIns.Conditions;
	using MonoDevelop.Core.AddIns.Codons;
	using MonoDevelop.Core.AddIns;

	using MonoQuery.Collections;
	using MonoQuery.SchemaClass;
	using MonoQuery.Codons;	
	using MonoQuery.Exceptions;
	
	///<summary>
	/// Connection properties
	///</summary>		
	public enum MonoQueryPropertyEnum
	{	
		Catalog,
		ConnectionString,
		DataSource,
		DataSourceName,
		DBMSName,
		ProviderName
	}
	
	///<summary>
	/// MonoQuery schema enumeration.
	///</summary>		
	public enum MonoQuerySchemaEnum {
		
		Asserts,
		Catalogs, 
		CharacterSets,
		CheckConstraints,
		Collations,
		ColumnPrivileges,
		Columns,
		ColumnsDomainUsage,
		ConstraintColumnUsage,
		ConstaintTableUsage,
		Cubes,
		DBInfoKeyWords,
		DBInfoLiterals,
		Dimensions,
		ForeignKeys,
		Hierarchies,
		Indexes,
		KeyColumnUsage,
		Levels,
		Measures,
		Members,
		Null, // ask for an empty list
		PrimaryKeys,
		ProcedureColumns,
		ProcedureParameters,
		Procedures,
		Properties,
		ProviderSpecific,
		ProviderTypes,
		ReferentialConstraints,
		Schemata,
		SQLLanguages,
		Statistics,
		TableConstraints,
		TablePrivileges,
		Tables,
		Tanslations,
		Trustees,
		UsagePrivileges,
		ViewColumnUsage,
		Views,
		ViewColumns,
		ViewTableUsage			
	}
	
	///<summary>
	/// this is a wrapper abstract class for connection with a database server.
	///</summary>
	public abstract class AbstractMonoQueryConnectionWrapper : IConnection
	{		
		//constants
		internal string SELECT		= "SELECT";
		internal string FROM		= "FROM";
		internal string WHERE		= "WHERE";
		internal string UPDATE		= "UPDATE";
		internal string SET			= "SET";
		internal string DELETE		= "DELETE";
		internal string INSERINTO	= "INSERT INTO";
		internal string VALUES		= "VALUES";
		internal string AND			= "AND";
				
		protected bool wrongConnectionString = false;
		static StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));		
		protected MonoQueryListDictionary pEntities = null;										
		private string name = "";

		///<summary>
		/// return <c>true</c> if the connection string is invalid.
		///</summary>
		public bool IsConnectionStringWrong
		{
			get
			{
				return this.wrongConnectionString;
			}
		}
		///<summary>return the catalog name. If there aren't a ctalog name
		/// in the <see cref=".ConnectionString">ConnectionString</see>, return "".
		/// </summary>
		public virtual string CatalogName
		{
			get
			{
				object returnValue = this.GetProperty( MonoQueryPropertyEnum.Catalog);
				
				if ( returnValue == null )
				{
					returnValue = ""; 
				}
				
				return returnValue.ToString();
			}			
		}
		
		/// <summary>
		/// Example connection string
		/// </summary>
		public virtual string ExampleConnectionString
		{
			get
			{
				return ( "" );
			}
		}
		
		public virtual string SchemaName
		{
			get
			{
				return "";//"INFORMATION_SCHEMA";
			}
		}			
		
		public virtual string Name
		{
			get { return this.name; }		
		}
		
		///<summary>return  : <see cref=".Name">Name</see>.<see cref=".ConnectionString"></see></summary>
		public string NormalizedName
		{
			get
			{
				return this.Name + "." + this.ConnectionString;
			}
		}
		
		public MonoQueryListDictionary Entities 
		{ 
			get
			{
				return this.pEntities;
			}
		}
		
		///<summary>
		///  OLEDB connection String.	
		/// i use this for speed up the code writing ...
		///</summary>
		public virtual string ConnectionString  
		{
		   get
		   {
		   	 return this.GetProperty( MonoQueryPropertyEnum.ConnectionString ).ToString();
		   }
		   
		   set
		   {		   	
		   }
		}	
				
		public virtual string Provider
		{
			get
			{
				return this.GetProperty( MonoQueryPropertyEnum.ProviderName ).ToString();
			}
		}
		
		public abstract bool IsOpen
		{
			get;
		}	
		
		
		public abstract object GetProperty( MonoQueryPropertyEnum property );
				
		/// <summary>
		/// Creates a new DataConnection object
		/// </summary>		
		public AbstractMonoQueryConnectionWrapper()
		{					
			this.pEntities = new MonoQueryListDictionary();
		}		
		
		/// <summary>
		/// Creates a new DataConnection object from a connection string
		/// </summary>		
		public AbstractMonoQueryConnectionWrapper( string connectionString ) : this()
		{				
		}						
		
		static private IConnection CreateConnectionObject( string connectionstring )
		{	
			//try
			//{
				Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
				
				IAddInTreeNode AddinNode;
				IAddInTreeNode ChildNode = null;
				
				string ClassWrapper = "";
				
				AddinNode = (IAddInTreeNode)AddInTreeSingleton.AddInTree.GetTreeNode("/MonoQuery/Connection");
				ChildNode = (IAddInTreeNode)AddinNode.ChildNodes["ConnectionWrapper"];
				ClassWrapper = ChildNode.Codon.Class;
				
				if ( (ClassWrapper != null) && (ClassWrapper != "") )
				{								
					IConnection conn = (IConnection)ass.CreateInstance( ClassWrapper,
					                                       	false,
					                                       	BindingFlags.CreateInstance,
					                                       	null,
					                                       	//new object[] {connectionstring},
										null,
					                                       	null,
					                                       	null
					                                       );
					conn.ConnectionString = connectionstring;
					return conn;
				}
				else
				{
					return null;
				}			
			//}
			//catch ( System.Exception e )
			//{
			//	throw new ConnectionStringException( e.Message );
			//}		   					
		}
		
		static public IConnection CreateFromDataConnectionLink()
		{
//				ADODB._Connection AdoConnection;
//				MSDASC.DataLinks dataLink = new MSDASC.DataLinks();			
				IConnection connection = null;
//				
//				AdoConnection = null;
//				AdoConnection = (ADODB._Connection) dataLink.PromptNew();						
//				
//				if ( ( AdoConnection != null ) && ( AdoConnection.ConnectionString != "" ) )
//				{												
//					connection = CreateConnectionObject( AdoConnection.ConnectionString );
//				}
//				
				return connection;
		}
		
		static  public IConnection UpDateFromDataConnectionLink( IConnection oldConnection )
		{
//				object AdoConnection;
//				MSDASC.DataLinks dataLink = new MSDASC.DataLinks();
				IConnection connection = null;
//				
//				AdoConnection = new ADODB.Connection();
//				(AdoConnection as ADODB.Connection).ConnectionString = oldConnection.ConnectionString;
//				
//				if ( dataLink.PromptEdit( ref AdoConnection ) )
//				{
//					connection = CreateConnectionObject( (AdoConnection as ADODB.Connection).ConnectionString );								
//				}
//				
				return connection;
		}
		
		static  public IConnection CreateFromConnectionString( string stringConnection )		
		{
				IConnection connection = null;
				
				if ( ( stringConnection != null ) && ( stringConnection != "" ) )
				{
					connection = CreateConnectionObject( stringConnection );
				}
				
				return connection;
		}
																				
		public abstract bool Open();
		public abstract void Close();
				
		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected virtual void OnRefresh()
		{
			if (this.pEntities != null )
			{
				this.pEntities.Add( "TABLES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryTables(this, this.CatalogName, this.SchemaName, this.Name,  "TABLES") } ) );
				this.pEntities.Add( "VIEWS", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryViews( this, this.CatalogName, this.SchemaName, this.Name,  "VIEWS" ) } ) );
				this.pEntities.Add( "PROCEDURES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryProcedures( this, this.CatalogName, this.SchemaName, this.Name,  "PROCEDURES" ) } ) );
			}		
		}
		
		///<summary>Refresh all dynamic properties of this connection</summary>
		public void Refresh()
		{			
			this.Clear();
						
			if ( this.IsOpen == true )
			{
				this.OnRefresh();
			}
		}

		public void Clear()
		{	
			if (this.pEntities != null )
			{
				this.pEntities.Clear();
				
				//Let do the Garbage collector to clear the MonoQuerySchmaClassCollection childs.
				// It wil be do in a thread (by the garbage collector), it will be better								
			}			
		}
				
		///<summary>
		/// Execute a SQL command
		/// <param name="SQLText">
		/// SQL command to execute
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>  
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		public abstract object ExecuteSQL( string SQLText, int rows);
		//TODO : Parameter param.
		
		///<summary>
		/// Execute a stocked procedure.
		/// <param name="schema">
		/// <see cref="MonoQuery.SchemaClass">SchemaClass</see> object.
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>  
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		public abstract object ExecuteProcedure( ISchemaClass schema, int rows, MonoQuerySchemaClassCollection parameters );
		
		///<summary>
		/// Extract Data from a Table or a View
		/// <param name="schema">
		/// <see cref="MonoQuery.SchemaClass">SchemaClass</see> object.
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>  
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		public object ExtractData( ISchemaClass schema, int rows )
		{
						
			if ( schema == null )
			{
				throw new System.ArgumentNullException("schema");
			}
			
			string SQLSelect = this.SELECT + " ";
			string SQLFrom  = this.FROM + " ";	
			MonoQuerySchemaClassCollection entitieslist = null;
			 			
			SQLFrom += schema.Name;			
			
			schema.Refresh();
			//we have only a table or view :o) //TODO : find a better way !
			foreach( DictionaryEntry DicEntry in schema.Entities )
			{
			  entitieslist = DicEntry.Value as MonoQuerySchemaClassCollection;
		      break;
			}
					
			if ( entitieslist == null )
			{
				throw new System.ArgumentNullException("entitieslist");
			}					
					
			foreach( ISchemaClass column in entitieslist )
			{
				SQLSelect += column.NormalizedName;
				SQLSelect += ",";
			}			
			
			SQLSelect = SQLSelect.TrimEnd( new Char[]{','} );
			if ( entitieslist.Count == 0) 
			{
				SQLSelect += "*";
			}
			SQLSelect += " ";
						
			return this.ExecuteSQL( SQLSelect + SQLFrom , 0);
		}
		
				
		///<summary>
		/// Update <see cref="System.Data.DataRow">row</see>'s fields into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="MonoQuery.SchemaClass.ISchema">schema</see> </param> 
		/// <remarks> it use a transaction for each row, so it's a very long process 
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void UpDateRow( ISchemaClass schema, DataRow row )
		{
			if ( schema == null )
			{
				throw new System.ArgumentNullException("schema");
			}
			
			if ( row == null )
			{
				throw new System.ArgumentNullException("row");
			}
			
			string SQLUpdate = this.UPDATE + " ";
			string SQLWhere  = this.WHERE + " ";
			string SQLValues = this.SET + " ";
			
			SQLUpdate += schema.Name;
			SQLUpdate += " ";
			
			foreach( DataColumn column in row.Table.Columns )
			{
				if ( column.ReadOnly == false 
				     && column.AutoIncrement == false				   
				   )
				{
					SQLValues += schema.Name + "." + AbstractMonoQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
					SQLValues += "=";
					if (  column.DataType.Equals( System.Type.GetType("System.String") )
					   || column.DataType.Equals( System.Type.GetType("System.Char") )
					   )
					{
						SQLValues +="'";
					}
					SQLValues += row[column.ColumnName];
					if (  column.DataType.Equals( System.Type.GetType("System.String") )
					   || column.DataType.Equals( System.Type.GetType("System.Char") )
					   )
					{
						SQLValues +="'";
					}	
					
					SQLValues += ",";
				}
				
				SQLWhere += MonoQuery.SchemaClass.AbstractMonoQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
				SQLWhere += "=";
				if (  column.DataType.Equals( System.Type.GetType("System.String") )
				   || column.DataType.Equals( System.Type.GetType("System.Char") )
				   )
				{
					SQLWhere +="'";
				}				
				SQLWhere += row[column.ColumnName, DataRowVersion.Original];
				if (  column.DataType.Equals( System.Type.GetType("System.String") )
				   || column.DataType.Equals( System.Type.GetType("System.Char") )
				   )
				{
					SQLWhere +="'";
				}				
				
				if ( row.Table.Columns.IndexOf( column ) != (row.Table.Columns.Count -1) )
				{					
					SQLWhere  += " " + this.AND + " ";					
				}		
			}
			
			SQLValues =SQLValues.TrimEnd(new Char[]{','});
						
			this.ExecuteSQL( SQLUpdate + SQLValues + SQLWhere , 0);
			row.AcceptChanges();
		}
		
		///<summary>
		/// Delete <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="MonoQuery.SchemaClass.ISchema">schema</see> </param> 
		/// <remarks> it use a transaction for each row, so it's a very long process 
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void DeleteRow( ISchemaClass schema, DataRow row )
		{		
			if ( schema == null )
			{
				throw new System.ArgumentNullException("schema");
			}
			
			if ( row == null )
			{
				throw new System.ArgumentNullException("row");
			}
			
			string SQLDelete = this.DELETE + " ";
			string SQLWhere  = this.WHERE +" ";
			string SQLFrom 	 = this.FROM +" ";
			
			SQLFrom += schema.Name;
			SQLFrom += " ";
			
			foreach( DataColumn column in row.Table.Columns )
			{
				//SQLDelete += schema.Name + "." + column.ColumnName;
				
				SQLWhere += MonoQuery.SchemaClass.AbstractMonoQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
				SQLWhere += "=";
				if (  column.DataType.Equals( System.Type.GetType("System.String") )
				   || column.DataType.Equals( System.Type.GetType("System.Char") )
				   )
				{
					SQLWhere +="'";
				}				
				SQLWhere += row[column.ColumnName, DataRowVersion.Original];
				if (  column.DataType.Equals( System.Type.GetType("System.String") )
				   || column.DataType.Equals( System.Type.GetType("System.Char") )
				   )
				{
					SQLWhere +="'";
				}				
				
				if ( row.Table.Columns.IndexOf( column ) != (row.Table.Columns.Count -1) )
				{
					//SQLDelete += ",";
					SQLWhere  += " " + this.AND + " ";					
				}		
				else
				{
					//SQLDelete += " ";
				}				
			}			
						
			this.ExecuteSQL( SQLDelete + SQLFrom + SQLWhere , 0);
			row.AcceptChanges();
		}		
		
		///<summary>
		/// Insert <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="MonoQuery.SchemaClass.ISchema">schema</see> </param> 
		/// <remarks> it use a transaction for each row, so it's a very long process 
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void InsertRow( ISchemaClass schema, DataRow row )
		{	
			if ( schema == null )
			{
				throw new System.ArgumentNullException("schema");
			}
			
			if ( row == null )
			{
				throw new System.ArgumentNullException("row");
			}			
			
			string SQLInsert = this.INSERINTO + " ";
			string SQLValues = this.VALUES +" (";
			
			SQLInsert += schema.Name;
			SQLInsert += " (";
			
			foreach( DataColumn column in row.Table.Columns )
			{				
				if ( column.ReadOnly == false 
				     && column.AutoIncrement == false				   
				   )
				{				
					SQLInsert += /*schema.Name + "." + //Full qualified name not supported by some provider*/ MonoQuery.SchemaClass.AbstractMonoQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
					
					if (  column.DataType.Equals( System.Type.GetType("System.String") )
					   || column.DataType.Equals( System.Type.GetType("System.Char") )
					   )
					{
						SQLValues +="'";
					}				
					SQLValues += row[column.ColumnName, DataRowVersion.Current ];
					if (  column.DataType.Equals( System.Type.GetType("System.String") )
					   || column.DataType.Equals( System.Type.GetType("System.Char") )
					   )
					{
						SQLValues +="'";
					}	
					
					SQLValues  += ",";
					SQLInsert += ",";
				}				
			}
			
			SQLValues = SQLValues.TrimEnd(new Char[]{','});
			SQLInsert = SQLInsert.TrimEnd(new Char[]{','});
			
			SQLInsert += ") ";	
			SQLValues += ")";

						
			this.ExecuteSQL( SQLInsert + SQLValues, 0);
			row.AcceptChanges();
		}		
		
		///<summary> throw a exception if the <seealso cref='.AbstractMonoQueryConnectionWrapper.Connection'/> is <code>null</code> </summary>
		protected abstract void CheckConnectionObject();

		///<summary> each elements of the restrictions array which are an empty string is replaced with a <code>null</code> reference</summary>		
		protected object[] NormalizeRestrictions( object[] restrictions)
		{	
			object[] newRestrictions = null;
			
			if ( restrictions != null )
			{
				newRestrictions = new object[ restrictions.Length ];
				object restriction;
				
				for( int i = 0; i < restrictions.Length; i++)
				{
					restriction =  restrictions[i];
	
					if ( restriction != null )
					{
						if ( (restriction is string) && ( (restriction as string) == "") )
						{
							restriction = null;
						}
					}
					
					newRestrictions[i] = restriction;			
				}
			}
			return newRestrictions;
		}		

		/// <summary>
		/// return a schema matching <code>restrictions</code>
		/// <param name="schema"> a <see cref=".MonoQuerySchemaEnum">MonoQuerySchemaEnum</see>.</param>
		/// <param name="restrictions"> Restrictions matching the schema</param>
		/// </summary>				
		protected abstract DataTable GetSchema( MonoQuerySchemaEnum schema, object[] restrictions );
		
		
//
// IConnection methods
//
		
		public MonoQuerySchemaClassCollection GetSchemaCatalogs( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Catalogs;
			object[] restrictions = new object[]{ schema.InternalName };
			
			try
			{
				record = this.GetSchema( schematype , restrictions );
	
				//TODO : add not supported schema code!
				
				if ( record != null )
				{					
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryCatalog( this, row["CATALOG_NAME"].ToString(), "", "", row["CATALOG_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception )
			{	
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Catalogs") );
			}
			
			return list;
		}
		
		public MonoQuerySchemaClassCollection GetSchemaSchemas( ISchemaClass schema )
		{			
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;		
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Schemata;
			object[] restrictions = new object[]{ schema.CatalogName, "", "" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQuerySchema( this, row["CATALOG_NAME"].ToString(), row["SCHEMA_NAME"].ToString(), "", row["SCHEMA_NAME"].ToString()) );
					}

				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Schemata") );
			}
			
			return list;						
		}		
		
		public MonoQuerySchemaClassCollection GetSchemaTables( ISchemaClass schema )
		{			
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Tables;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, "", "TABLE" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryTable( this, row["TABLE_CATALOG"].ToString(), row["TABLE_SCHEMA"].ToString(), "", row["TABLE_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Tables") );
			}
			
			return list;
		}
		
		public MonoQuerySchemaClassCollection GetSchemaViews( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;		
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Views;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, "", "VIEW" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryView( this, row["TABLE_CATALOG"].ToString(), row["TABLE_SCHEMA"].ToString(), "", row["TABLE_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Views") );
			}
			
			return list;
		}
		
		public MonoQuerySchemaClassCollection GetSchemaProcedures( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;		
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Procedures;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, "", ""};
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryProcedure( this, row["PROCEDURE_CATALOG"].ToString(), row["PROCEDURE_SCHEMA"].ToString(), "", row["PROCEDURE_NAME"].ToString().Split(';')[0] ) );
					}					
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Procedures") );
			}
			
			return list;			
		}
		
		public MonoQuerySchemaClassCollection GetSchemaTableColumns( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;	
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.Columns;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, schema.InternalName, "" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryColumn( this, schema.CatalogName, schema.SchemaName,schema.Name, row["COLUMN_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Columns") );
			}
			
			return list;
		}
		
		public MonoQuerySchemaClassCollection GetSchemaViewColumns( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;		
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.ViewColumns;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, schema.InternalName, "" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryColumn( this, schema.CatalogName, schema.SchemaName, schema.Name, row["COLUMN_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.Columns") );
			}
			
			return list;
		}		
		
		public MonoQuerySchemaClassCollection GetSchemaProcedureColumns( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;	
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.ProcedureColumns;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, schema.InternalName, "" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						list.Add( new MonoQueryColumn( this, schema.CatalogName, schema.SchemaName, schema.Name, row["COLUMN_NAME"].ToString()) );
					}					
				}
			}
			catch( System.Exception)
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.ProcedureColumns") );
			}
			
			return list;
		}
		
		public MonoQuerySchemaClassCollection GetSchemaProcedureParameters( ISchemaClass schema )
		{
			MonoQuerySchemaClassCollection list = new MonoQuerySchemaClassCollection( );
			DataTable record = null;		
			MonoQuerySchemaEnum schematype = MonoQuerySchemaEnum.ProcedureParameters;
			object[] restrictions = new object[]{ schema.CatalogName, schema.SchemaName, schema.InternalName, "" };
			
			try
			{
				record = this.GetSchema( schematype, restrictions );
				MonoQueryParameter par = null;
				if ( record != null )
				{
					foreach (DataRow row in record.Rows )
					{
						par = new MonoQueryParameter( this, schema.CatalogName, schema.SchemaName, schema.Name, row["PARAMETER_NAME"].ToString());
						par.DataType = StringToDbType( row["DATA_TYPE"].ToString() );
						par.Type     = StringToParamDirection( row["PARAMETER_TYPE"].ToString() );
						
						if ( par.Type != ParameterDirection.ReturnValue )
						{
							list.Add( par );
						}
					}									
				}
			}
			catch( System.Exception )
			{		
				list.Add( new MonoQueryNotSupported(this, "" , "", "", "MonoQuerySchemaEnum.ProcedureParameters") );
			}
			
			return list;
		}
		
		
		protected DbType StringToDbType( string value )
		{
			return IntToDbType( int.Parse( value ) );
		}
		
		protected DbType IntToDbType( int value )
		{
			DbType retValue;
			switch( value )
			{	
				case 129	: retValue = DbType.AnsiString; break;
				//case 1	: retValue = DbType.AnsiStringFixedLength; break;
				case 128	: retValue = DbType.Binary; break;
				case 11		: retValue = DbType.Boolean; break;
				case 17		: retValue = DbType.Byte; break;
				case 6		: retValue = DbType.Currency; break;
				case 7		:
				case 133	: retValue = DbType.Date; break;
				case 135	: retValue = DbType.DateTime; break;
				case 14		: retValue = DbType.Decimal; break;
				case 5		: retValue = DbType.Double; break;
				case 72		: retValue = DbType.Guid; break;
				case 2		: retValue = DbType.Int16; break;
				case 3		: retValue = DbType.Int32; break;
				case 20		: retValue = DbType.Int64; break;
				case 12		:
				case 132	: retValue = DbType.Object; break;
				case 16		: retValue = DbType.SByte; break;
				case 4		: retValue = DbType.Single; break;
				case 130	: retValue = DbType.String; break;
				case 8		: retValue = DbType.StringFixedLength; break;
				case 134	: retValue = DbType.Time; break;
				case 18		: retValue = DbType.UInt16; break;
				case 19		: retValue = DbType.UInt32; break;
				case 21		: retValue = DbType.UInt64; break;
				case 131	: retValue = DbType.VarNumeric; break;
				default : throw new ArgumentOutOfRangeException("value");				
			}
			
			return retValue;			
		}
		
		protected ParameterDirection StringToParamDirection( string value )
		{
			return 	IntToParamDirection( int.Parse( value ) );
		}
		
		protected ParameterDirection IntToParamDirection( int value )
		{
			ParameterDirection retValue;
			switch( value )
			{
				case 1 : retValue = ParameterDirection.Input; break;
				case 2 : retValue = ParameterDirection.InputOutput; break;
				case 3 : retValue = ParameterDirection.Output; break;
				case 4 : retValue = ParameterDirection.ReturnValue; break;
				default : throw new ArgumentOutOfRangeException("value");
				
			}
			
			return retValue;
		}
		
	}
}
