//  MonoQuery - SharpQuery port to MonoDevelop (+ More)
//  Copyright (C) Christian Hergert <chris@mosaix.net>
//                Ankit Jain <radical@gamil.com>
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

using Mono.Data.SqliteClient;

using MonoQuery.Collections;
using MonoQuery.SchemaClass;
using MonoQuery.Exceptions;

namespace MonoQuery.Connection
{
	/// <summary>
	/// Sqlite connection wrapper using the Mono.Data.SqliteClient.
	/// </summary>
	public class SqliteConnectionWrapper : AbstractMonoQueryConnectionWrapper
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
		protected	string		pProvider = "SqliteConnectionWrapper";
		
		/// <summary>
		/// Sqlite connection object
		/// </summary>
		protected	SqliteConnection		pConnection = null;
		#endregion // End Private Properties
		
		#region // Public Properties
		/// <summary>
		/// Name of database
		/// </summary>
		public override string Name
		{
			get { return "SQLite: " + this.pConnection.Database; }
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
				catch ( InvalidOperationException e )
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
		public SqliteConnectionWrapper()
		{
			this.pEntities = new MonoQueryListDictionary();
			this.pConnection = new SqliteConnection();
		}
		
		/// <summary>
		/// Constructor with connection string
		/// </summary>
		public SqliteConnectionWrapper( string connString ) : this()
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
			SqliteCommand command = new SqliteCommand();
			DataSet ds = new DataSet();
			SqliteDataAdapter da = new SqliteDataAdapter();
			
			command.Connection = this.pConnection;
			command.CommandText = SQLText;
			command.CommandType = System.Data.CommandType.Text;
			
			/*command.Transaction = pConnection.BeginTransaction(
			System.Data.IsolationLevel.ReadCommitted );*/

			//Mono.Data.SqliteClient returns doesnt implement BeginTransaction
			//when specified with an isolation level!
			command.Transaction = pConnection.BeginTransaction();
			
			try
			{
				da.SelectCommand = command;
				if ( maxRows > 0 ) {
					da.Fill( ds, 0, maxRows, null );
				} else {
					da.Fill( ds );
				}
			}
			/* AJ catch ( SqliteException e )
			{
				command.Transaction.Rollback();
				
				string mes = SQLText + "\n";
							
				throw new ExecuteSQLException( mes );
			}*/
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
					//returnValues = this.pConnection.DataSource;
					returnValues = this.pConnection.Database;
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
			
			SqliteCommand command = new SqliteCommand();
			command.CommandText = "select * from sqlite_master";
			command.Connection = this.pConnection;
			
			SqliteDataAdapter da = new SqliteDataAdapter();
			da.SelectCommand = command;
			DataSet ds = new DataSet();
			da.Fill( ds );
			
			if(ds.Tables[0].Columns.Count ==0){
				return null;
			}
				
			ds.Tables[0].Columns[1].ColumnName = "TABLE_NAME";
			
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
			
			SqliteCommand command = new SqliteCommand();
			command.CommandText = "PRAGMA table_info('" + restrictions[2] + "')";
			command.Connection = this.pConnection;

			SqliteDataAdapter da = new SqliteDataAdapter();
			da.SelectCommand = command;

			DataSet ds = new DataSet();
			da.Fill(ds);
			/*
			FIXME: What exception should be thrown in this case??

			if (ds.Tables[0].Columns.Count==0)
				//No schema info for this table??
				return null;
			*/
			
	        ds.Tables[0].Columns[1].ColumnName = "COLUMN_NAME";

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
