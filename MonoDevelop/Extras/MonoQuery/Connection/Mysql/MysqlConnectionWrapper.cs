//  MonoQuery - SharpQuery port to MonoDevelop (+ More)
//  Copyright (C) Christian Hergert <chris@mosaix.net>
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Data;

using ByteFX.Data.MySqlClient;

using MonoQuery.Collections;
using MonoQuery.SchemaClass;
using MonoQuery.Exceptions;

namespace MonoQuery.Connection
{
	/// <summary>
	/// Mysql connection wrapper using the ByteFX mysql driver.
	/// </summary>
	public class MysqlConnectionWrapper : AbstractMonoQueryConnectionWrapper
	{
		#region // Private Properties
		/// <summary>
		/// This property stores whether the current connection string is
		/// wrong. This helps us determine connection errors for the
		/// monodevelop user.
		/// </summary>
		private		bool		pIsConnectionStringWrong = false;
		
		/// <summary>
		/// Name of class providing the connection. This isnt really used
		/// and is legacy from the SharpQuery.
		/// </summary>
		protected	string		pProvider = "MysqlConnectionWrapper";
		
		/// <summary>
		/// Mysql connection object
		/// </summary>
		protected	MySqlConnection		pConnection = null;
		#endregion // End Private Properties
		
		#region // Public Properties
		/// <summary>
		/// Name of database
		/// </summary>
		public override string Name
		{
			get { return "MySQL: " + this.pConnection.Database; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string CatalogName
		{
			get
			{
				if ( IsOpen == false ) {
					this.Open();
				}
				
				return this.pConnection.Database;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string SchemaName {
			get { return ""; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override bool IsOpen
		{
			get
			{
				try
				{
					return ( this.pConnection.State == ConnectionState.Open );
				}
				catch ( Exception e )
				{
					return false;
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string ConnectionString
		{
			get { return this.pConnection.ConnectionString; }
			set
			{
				if ( IsOpen == true ) {
					pConnection.Close();
				}
				
				try
				{
					this.pConnection.ConnectionString = value;
					this.pIsConnectionStringWrong = false;
				}
				catch ( MySqlException e )
				{
					this.pIsConnectionStringWrong = true;
				}
			}
		}
		#endregion // End Public Properties
		
		#region // Constructors
		/// <summary>
		/// Default constructor.
		/// </summary>
		public MysqlConnectionWrapper()
		{
			this.pEntities = new MonoQueryListDictionary();
			this.pConnection = new MySqlConnection();
		}
		
		/// <summary>
		/// Constructor with connection string
		/// </summary>
		public MysqlConnectionWrapper( string connString ) : this()
		{
			this.ConnectionString = connString;
		}
		#endregion // End Constructors
		
		#region // Public Methods
		/// <summary>
		/// 
		/// </summary>
		public override bool Open()
		{
			this.pConnection.Open();
			return IsOpen;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void Close()
		{
			this.pConnection.Close();
		}
		
		/// <summary>
		/// Execute a SQL Statement.
		/// <returns>System.Data.DataSet</returns>
		/// </summary>
		public override object ExecuteSQL( string SQLText, int maxRows )
		{
			MySqlCommand command = new MySqlCommand();
			DataSet ds = new DataSet();
			MySqlDataAdapter da = new MySqlDataAdapter();
			
			command.Connection = this.pConnection;
			command.CommandText = SQLText;
			command.CommandType = System.Data.CommandType.Text;
			
			command.Transaction = pConnection.BeginTransaction(
				System.Data.IsolationLevel.ReadCommitted );
			
			try
			{
				da.SelectCommand = command;
				if ( maxRows > 0 ) {
					da.Fill( ds, 0, maxRows, null );
				} else {
					da.Fill( ds );
				}
			}
			catch ( MySqlException e )
			{
				command.Transaction.Rollback();
				
				string mes = SQLText + "\n";
							
				throw new ExecuteSQLException( mes );
			}
			catch ( Exception e )
			{
				command.Transaction.Rollback();
				throw new ExecuteSQLException( SQLText );
			}
			finally
			{
				command.Transaction.Commit();
			}
			
			return ds;
		}
		
		/// <summary>
		/// Return settings on this connection.
		/// </summary>
		public override object GetProperty( MonoQueryPropertyEnum property )
		{
			object returnValues = null;
			
			switch( property ) {
				case MonoQueryPropertyEnum.ProviderName:
					returnValues = this.Provider;
					break;
				case MonoQueryPropertyEnum.Catalog:
					returnValues = this.CatalogName;
					break;
				case MonoQueryPropertyEnum.ConnectionString:
					returnValues = this.ConnectionString;
					break;
				case MonoQueryPropertyEnum.DataSource:
					returnValues = this.pConnection.DataSource;
					break;
				default:
					break;
			}
			
			return returnValues;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override object ExecuteProcedure( ISchemaClass schema, int rows,
			MonoQuerySchemaClassCollection parameters )
		{
			return (object) null;
		}
		
		/// <summary>
		/// This will call the proper method depending on the schema
		/// that is being asked for.
		/// </summary>
		protected override DataTable GetSchema( MonoQuerySchemaEnum schema, object [] restrictions )
		{
			DataTable returnValues = new DataTable();
			
			switch( schema ) {
				case MonoQuerySchemaEnum.Tables:
					returnValues = this.GetTables( restrictions );
					break;
				case MonoQuerySchemaEnum.Columns:
					returnValues = this.GetTableColumns( restrictions );
					break;
				case MonoQuerySchemaEnum.Views:
					returnValues = this.GetViews( restrictions );
					break;
				case MonoQuerySchemaEnum.ViewColumns:
					returnValues = this.GetViewColumns( restrictions );
					break;
				case MonoQuerySchemaEnum.Procedures:
					returnValues = this.GetProcedures( restrictions );
					break;
				default:
					break;
			}
			
			return returnValues;
		}
		#endregion // End Public Methods
		
		#region // Private Methods
		/// <summary>
		/// Retrieve the tables for the currently connected database.
		/// </summary>
		protected virtual DataTable GetTables( object [] restrictions )
		{
			if ( IsOpen == false )
				this.Open();
			
			MySqlCommand command = new MySqlCommand();
			command.CommandText = "SHOW TABLES";
			command.Connection = this.pConnection;
			
			MySqlDataAdapter da = new MySqlDataAdapter();
			da.SelectCommand = command;
			DataSet ds = new DataSet();
			da.Fill( ds );
			
			ds.Tables[0].Columns[0].ColumnName = "TABLE_NAME";
			
			// Hack to get around there only being one of the columns in the
			// select statement.
			ds.Tables[0].Columns.Add( new DataColumn("TABLE_SCHEMA", typeof(string)) );
			ds.Tables[0].Columns.Add( new DataColumn("TABLE_CATALOG", typeof(string)) );
			foreach( DataRow row in ds.Tables[0].Rows ) {
				row.ItemArray[1] = this.SchemaName;
				row.ItemArray[2] = this.CatalogName;
			}
			// End hack
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// MySQL does not support this yet (will in 5.0)
		/// </summary>
		protected virtual DataTable GetViews( object [] restrictions )
		{
			return new DataTable();
		}
		
		/// <summary>
		/// MySQL does not support this yet (will in 5.0)
		/// </summary>
		protected virtual DataTable GetProcedures( object [] restrictions )
		{
			return new DataTable();
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual DataTable GetTableColumns( object [] restrictions )
		{
			if ( IsOpen == false )
				this.Open();
			
			MySqlCommand command = new MySqlCommand();
			command.CommandText = "DESCRIBE " + restrictions[2];
			command.Connection = this.pConnection;
			
			MySqlDataAdapter da = new MySqlDataAdapter();
			da.SelectCommand = command;
			DataSet ds = new DataSet();
			da.Fill( ds );
			
			// Hack: the Collection bullshit requires it to be *named*.
			ds.Tables[0].Columns[0].ColumnName = "COLUMN_NAME";
			// End Hack
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// MySQL does not support views yet (will in 5.0)
		/// </summary>
		protected virtual DataTable GetViewColumns( object [] restrictions )
		{
			return new DataTable();
		}
		
		/// <summary>
		/// MySQL does not support procs yet (will in 5.0)
		/// </summary>
		protected virtual DataTable GetProcedureColumns( object [] restrictions )
		{
			return new DataTable();
		}
		
		/// <summary>
		/// Overridable method for extending class to control what happens on
		/// a connection refresh.
		/// </summary>
		protected override void OnRefresh()
		{
			if (this.pEntities != null )
			{
				this.pEntities.Add( "TABLES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryTables(this, this.CatalogName, this.SchemaName, this.Name,  "TABLES") } ) );
				
				// Not yet supported
				//this.pEntities.Add( "VIEWS", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryViews( this, this.CatalogName, this.SchemaName, this.Name,  "VIEWS" ) } ) );
				//this.pEntities.Add( "PROCEDURES", new MonoQuerySchemaClassCollection( new ISchemaClass[] { new MonoQueryProcedures( this, this.CatalogName, this.SchemaName, this.Name,  "PROCEDURES" ) } ) );
			}
		}
		
		protected override void CheckConnectionObject()
		{
			if ( this.pConnection == null )
				throw new Exception("Bad connection object");
		}
		#endregion // End Private Methods
	}
}