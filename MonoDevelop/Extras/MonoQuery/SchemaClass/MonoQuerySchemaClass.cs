// created on 04/11/2003 at 18:15

using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.Connection;
using MonoQuery.Collections;

namespace MonoQuery.SchemaClass
{	
	
//
// Childs
//
			
	///<summary>
	/// Column class
	///</summary>	
	public class MonoQueryColumn : AbstractMonoQuerySchemaClass
	{			
		public override string NormalizedName 
		{
			get {
				string normalizedName = "";
								
				if ( (this.OwnerName != "") && (this.OwnerName != null) ){
					normalizedName +=  this.OwnerName +  ".";
				} 				
				
				normalizedName +=  this.Name;
				
				return normalizedName;
			}
		}
		
		protected override void CreateEntitiesList()
		{		
		}
		
		public MonoQueryColumn( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{			
		}
		
		protected override void OnRefresh()		
		{
			//nothing !
		}		

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}	
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{
//				DataObject returnValue = new DataObject();			
//				string extract = this.NormalizedName;
//				returnValue.SetData(typeof(string) , extract );
//				return returnValue;
//			}
//		}			
	}
	
	///<summary>
	/// Procedurre's parameters class
	///</summary>	
	public class MonoQueryParameter : AbstractMonoQuerySchemaClass
	{				
		private DbType _dataType;
		private object _value = null;
		private ParameterDirection _type;
		
		///<summary>
		/// Data Type of the parameter
		/// ( used only while extracting data or executing procedure )
		///</summary>
		public DbType DataType
		{
			get
			{
				return this._dataType;
			}
			set
			{
				this._dataType = value;
				switch( value )
				{
					//string type
					case DbType.AnsiString				: 
					case DbType.AnsiStringFixedLength	: 
					case DbType.String					: 
					case DbType.StringFixedLength		: this._value = new string( (char[])null ); break;
					//array type
					case DbType.Binary					: this._value = new byte[8000]; break;
					//bool type
					case DbType.Boolean					: this._value = new bool(); break;
					//interger type
					case DbType.SByte					: this._value = new sbyte(); break;
					case DbType.Byte					: this._value = new byte(); break;					 
					case DbType.Int16					: this._value = new short(); break;
					case DbType.Int32					: this._value = new int(); break;
					case DbType.Int64					: this._value = new long(); break;
					case DbType.UInt16					: this._value = new ushort(); break;
					case DbType.UInt32					: this._value = new uint(); break;
					case DbType.UInt64					: this._value = new long(); break;					
					//Date type
					case DbType.Date					: 
					case DbType.DateTime				:
					case DbType.Time					: this._value = new DateTime(); break;
					//float type
					case DbType.Decimal					: this._value = new decimal(); break;
					case DbType.Currency				: 
					case DbType.VarNumeric				: 
					case DbType.Double					: this._value = new double(); break;
					case DbType.Single					: this._value = new float(); break;
					//user defined
					case DbType.Object					: this._value = new object(); break;
					//Guid
					case DbType.Guid					: this._value = new Guid(); break;															
					default								: throw new ArgumentOutOfRangeException("value");
				}
			}
		}
		
		protected void SetValue( string value )
		{
				switch( this.DataType )
				{
					//string type
					case DbType.Object					:					
					case DbType.Binary					: 											
					case DbType.AnsiString				: 
					case DbType.AnsiStringFixedLength	: 
					case DbType.String					: 
					case DbType.StringFixedLength		: this._value = value; break;

					case DbType.Boolean					: this._value = bool.Parse( value ); break;

					case DbType.SByte					: this._value = sbyte.Parse( value ); break;
					case DbType.Byte					: this._value = byte.Parse( value ); break;					 
					case DbType.Int16					: this._value = short.Parse( value ); break;
					case DbType.Int32					: this._value = int.Parse( value ); break;
					case DbType.Int64					: this._value = long.Parse( value ); break;
					case DbType.UInt16					: this._value = ushort.Parse( value ); break;
					case DbType.UInt32					: this._value = uint.Parse( value ); break;
					case DbType.UInt64					: this._value = long.Parse( value ); break;					

					case DbType.Date					: 
					case DbType.DateTime				:
					case DbType.Time					: this._value = DateTime.Parse( value ); break;

					case DbType.Decimal					: this._value = decimal.Parse( value ); break;
					case DbType.Currency				: 
					case DbType.VarNumeric				: 
					case DbType.Double					: this._value = double.Parse( value ); break;
					case DbType.Single					: this._value = float.Parse( value );  break;

					case DbType.Guid					: this._value = new Guid( value ); break;															
					default								: throw new ArgumentOutOfRangeException("value");
				}			
		}
		
		///<summary>
		/// Value of the parameter
		/// ( used only while extracting data or executing procedure )
		///</summary>
		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this.SetValue( value.ToString() );
			}
		}
		
		///<summary>
		/// Value of the parameter
		/// ( used only while extracting data or executing procedure )
		///</summary>
		public ParameterDirection Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
		
		public override string NormalizedName 
		{
			get {
				string normalizedName = "";
								
				if ( (this.OwnerName != "") && (this.OwnerName != null) ){
					normalizedName +=  this.OwnerName +  ".";
				} 				
				
				normalizedName +=  this.Name;
				
				return normalizedName;
			}
		}
		
		protected override void CreateEntitiesList()
		{		
		}
		
		public MonoQueryParameter( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{
			this.pEntities = null;
		}			
		
		protected override void OnRefresh()		
		{
			//nothing !
		}

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see></returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}	
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{
//				DataObject returnValue = new DataObject();			
//				string extract = NormalizedName;
//				returnValue.SetData(typeof(string) , extract );
//				return returnValue;
//			}
//		}		
	}	
	
	///<summary>
	/// Table class
	///</summary>	
	public class MonoQueryProcedure : AbstractMonoQuerySchemaClass
	{		
		public override string NormalizedName 
		{
			get 
			{
				if ( (this.CatalogName != "") && (this.CatalogName != null) )
				{
					return this.CatalogName + "." + this.Name;
				}
				else
				{
					return CheckWhiteSpace(this.Connection.GetProperty( MonoQueryPropertyEnum.DataSource ).ToString()) + "." + this.Name;
				}
			}
		}
		
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("PROCEDURE_COLUMNS", new MonoQuerySchemaClassCollection() );
			this.pEntities.Add("PROCEDURE_PARAMETERS", new MonoQuerySchemaClassCollection() );
		}
				
		public MonoQueryProcedure( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{
		}					
		
		public override MonoQuerySchemaClassCollection GetSchemaParameters()
		{
			return this.pDataConnection.GetSchemaProcedureParameters( this );			
		}
		
		public override MonoQuerySchemaClassCollection GetSchemaColumns()
		{
			return this.pDataConnection.GetSchemaProcedureColumns( this );			
		}		
				
		protected override void OnRefresh()
		{
			this.Entities["PROCEDURE_COLUMNS"].AddRange( this.GetSchemaColumns() );
			this.Entities["PROCEDURE_PARAMETERS"].AddRange( this.GetSchemaParameters() );
		}	
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			return this.Connection.ExecuteProcedure( this, rows, parameters );			
		}		
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{
//				DataObject returnValue = new DataObject();			
//				string extract = "EXECUTE " + NormalizedName;
//				returnValue.SetData(typeof(string) , extract );
//				return returnValue;
//			}
//		}		
	}
	
	///<summary>
	/// Table class
	///</summary>	
	public class MonoQueryTable : AbstractMonoQuerySchemaClass
	{
		public override string NormalizedName 
		{
			get 
			{
				if ( (this.CatalogName != "") && (this.CatalogName != null) )
				{
					return this.CatalogName + "." + this.Name;
				}
				else
				{
					return CheckWhiteSpace(this.Connection.GetProperty( MonoQueryPropertyEnum.DataSource ).ToString()) + "." + this.Name;
				}
			}
		}
		
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("TABLE_COLUMNS", new MonoQuerySchemaClassCollection() );
		}		
				
		public MonoQueryTable( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{
		}		
		
		public override MonoQuerySchemaClassCollection GetSchemaColumns()
		{	
			return this.pDataConnection.GetSchemaTableColumns( this );
		}	
		
		protected override void OnRefresh()
		{
			this.Entities["TABLE_COLUMNS"].AddRange( this.GetSchemaColumns() );			
		}		

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			return this.Connection.ExtractData( this, rows );
		}	
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{		
//				string SQLSelect = "SELECT ";
//				string SQLFrom  = "FROM ";	
//				
//				MonoQuerySchemaClassCollection entitieslist = null;
//			 
//				SQLFrom += this.Name;
//			
//				this.Refresh();
//				
//				//we have only a table or view :o)
//				foreach( DictionaryEntry DicEntry in Entities )
//				{
//				  entitieslist = DicEntry.Value as MonoQuerySchemaClassCollection;
//			      break;
//				}
//						
//				if ( entitieslist == null )
//				{
//					throw new System.ArgumentNullException("entitieslist");
//				}					
//						
//				foreach( ISchemaClass column in entitieslist )
//				{
//					SQLSelect += column.NormalizedName;					
//					SQLSelect += ",";
//				}																		
//
//				SQLSelect = SQLSelect.TrimEnd( new Char[]{','} );
//				SQLSelect += " ";
//
//				DataObject returnValue = new DataObject();							
//				
//				returnValue.SetData(typeof(string) , SQLSelect + SQLFrom );
//				return returnValue;
//			}
//		}		
	}
	
	///<summary>
	/// View class
	///</summary>	
	public class MonoQueryView : AbstractMonoQuerySchemaClass
	{
		public override string NormalizedName 
		{
			get 
			{
				if ( (this.CatalogName != "") && (this.CatalogName != null) )
				{
					return this.CatalogName + "." + this.Name;
				}
				else
				{
					return CheckWhiteSpace(this.Connection.GetProperty( MonoQueryPropertyEnum.DataSource ).ToString()) + "." + this.Name;
				}
			}
		}
		
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("VIEWS_COLUMNS", new MonoQuerySchemaClassCollection() );
		}		
		
		public MonoQueryView( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{
		}		

		public override MonoQuerySchemaClassCollection GetSchemaColumns()
		{	
			return this.pDataConnection.GetSchemaViewColumns( this );
		}		

		protected override void OnRefresh()
		{
			this.Entities["VIEWS_COLUMNS"].AddRange( this.GetSchemaColumns() );			
		}	
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			return this.Connection.ExtractData( this, rows );
		}	
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{		
//				string SQLSelect = "SELECT ";
//				string SQLFrom  = "FROM ";	
//				
//				MonoQuerySchemaClassCollection entitieslist = null;
//			 
//				SQLFrom += this.Name;
//			
//				this.Refresh();
//				
//				//we have only a table or view :o)
//				foreach( DictionaryEntry DicEntry in Entities )
//				{
//				  entitieslist = DicEntry.Value as MonoQuerySchemaClassCollection;
//			      break;
//				}
//						
//				if ( entitieslist == null )
//				{
//					throw new System.ArgumentNullException("entitieslist");
//				}					
//						
//				foreach( ISchemaClass column in entitieslist )
//				{
//					SQLSelect += column.NormalizedName;					
//					SQLSelect += ",";
//				}																		
//
//				SQLSelect = SQLSelect.TrimEnd( new Char[]{','} );
//				SQLSelect += " ";
//
//				DataObject returnValue = new DataObject();							
//				
//				returnValue.SetData(typeof(string) , SQLSelect + SQLFrom );
//				return returnValue;
//			}
//		}		
	}	
	
	///<summary>
	/// Class for unsupported functions
	///</summary>	
	public class MonoQueryNotSupported : AbstractMonoQuerySchemaClass
	{		
		protected override void CreateEntitiesList()
		{
		}
		
		public MonoQueryNotSupported( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{			
//			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			this.pName += " " + GettextCatalog.GetString( "Not Supported" )  + " " + connection.Provider;			

		}
		
		protected override void OnRefresh()
		{
			//nothing !
		}	
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}	
	}	
	
	
	///<summary>
	/// Class lis of Tables
	///</summary>	
	public class MonoQueryTables : AbstractMonoQuerySchemaClass	
	{
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("TABLES", new MonoQuerySchemaClassCollection() );
		}
		
		public MonoQueryTables( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{
			this.pName = GettextCatalog.GetString( "Tables" ) ;
		}		
		
		protected override void OnRefresh()
		{
			this.Entities["TABLES"].AddRange( this.GetSchemaTables() );
		}	
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}		
	}	
	
	///<summary>
	/// Class lis of Views
	///</summary>	
	public class MonoQueryViews : AbstractMonoQuerySchemaClass
	{		
		
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("VIEWS", new MonoQuerySchemaClassCollection() );
		}
		
		public MonoQueryViews( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{			
//			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			this.pName = GettextCatalog.GetString( "Views" );
		}
		
		protected override void OnRefresh()
		{
			this.Entities["VIEWS"].AddRange( this.GetSchemaViews() );					
		}
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}		
	}
	
	///<summary>
	/// Class lis of Procedures
	///</summary>	
	public class MonoQueryProcedures : AbstractMonoQuerySchemaClass
	{			
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("PROCEDURES", new MonoQuerySchemaClassCollection() );
		}
		
		public MonoQueryProcedures( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{			
//			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			this.pName = GettextCatalog.GetString( "Procedures" );
		}
		
		protected override void OnRefresh()
		{
			this.Entities["PROCEDURES"].AddRange( this.GetSchemaProcedures() );			
		}	
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}			
	}	
	
	///<summary>
	/// Class lis of Schemas
	///</summary>	
	public class MonoQuerySchema : AbstractMonoQuerySchemaClass
	{						
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("TABLES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryTables(this.pDataConnection, this.CatalogName, this.Name, "", "") } ) );
			this.pEntities.Add("VIEWS", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryViews(this.pDataConnection, this.CatalogName, this.Name, "", "") } ) );
			this.pEntities.Add("PROCEDURES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryProcedures(this.pDataConnection, this.CatalogName, this.Name, "", "") } ) );
		}
		
		public MonoQuerySchema( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{		
		}		

		protected override void OnRefresh()
		{	
			// Nothing !
		}		
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}	
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{
//				DataObject returnValue = new DataObject();			
//				string extract = NormalizedName;
//				returnValue.SetData(typeof(string) , extract );
//				return returnValue;
//			}		
//		}
	}	
	
	///<summary>
	/// Class for a catalog
	///</summary>	
	public class MonoQueryCatalog : AbstractMonoQuerySchemaClass
	{							
		protected override void CreateEntitiesList()
		{
			base.CreateEntitiesList();
			this.pEntities.Add("SCHEMAS", new MonoQuerySchemaClassCollection() );			
		}
		
		public MonoQueryCatalog( IConnection connection, string catalogName, string schemaName, string ownerName, string name  ) : base(connection, catalogName, schemaName, ownerName, name )
		{			
		}		
		
		protected override void OnRefresh()
		{
			this.Entities["SCHEMAS"].AddRange( this.GetSchemaSchemas() );			
		}
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see> 
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public override object Execute( int rows, MonoQuerySchemaClassCollection parameters )
		{
			//nothing
			return null;
		}
		
//		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> 
//		///</summary>		
//		public override DataObject DragObject
//		{
//			get
//			{
//				DataObject returnValue = new DataObject();			
//				string extract = NormalizedName;
//				returnValue.SetData(typeof(string) , extract );
//				return returnValue;
//			}
//		}	
	}
}
