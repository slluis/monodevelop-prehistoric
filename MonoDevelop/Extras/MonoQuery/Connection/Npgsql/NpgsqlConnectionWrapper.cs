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

using Npgsql;

using MonoQuery.Collections;
using MonoQuery.SchemaClass;
using MonoQuery.Exceptions;

namespace MonoQuery.Connection
{
	public class NpgsqlConnectionWrapper : AbstractMonoQueryConnectionWrapper //IConnection
	{
		#region // Private Properties
		/// <summary>
		/// This property stores whether the current connection string is
		/// wrong. This helps us determine connection errors for the
		/// monodevelop user.
		/// </summary>
		private 	bool		pIsConnectionStringWrong = false;
		
		/// <summary>
		/// Name of class providing the connection.
		/// </summary>
		protected	string		pProvider = "NpgsqlConnectionWrapper";
		
		/// <summary>
		/// Npgsql Connection object
		/// </summary>
		protected	NpgsqlConnection pConnection = null;
		
		/// <summary>
		/// Child entities of this connection.
		/// </summary>
//		protected MonoQueryListDictionary pEntities = null;	
		#endregion // End Private Properties
		
		#region // Public Properties
		/// <summary>
		/// 
		/// </summary>
		public override string Name
		{
			get
			{
				return "Npgsql: " + this.pConnection.Database;
			}
		}
		
		public override bool IsOpen
		{
			get
			{
				return (this.pConnection.State == ConnectionState.Open);
			}
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
		public override string SchemaName
		{
			get { return "public"; } // we should fix this =X
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
				catch ( NpgsqlException )
				{
					this.pIsConnectionStringWrong = true;
				}
			}
		}
		
		/// <summary>
		/// The LastSystemOID is the last internal OID used by postgresql.
		/// Since this number changes from release to release, this can
		/// can be a fucking nightmare.
		/// </summary>
		protected virtual string LastSystemOID
		{
			get
			{
				if ( IsOpen != true ) Open();
				
				string retval = "";
				String version = pConnection.ServerVersion.ToString();
				switch( version.Substring(0,3) ) { // Get Major Version
					case "7.4":
						retval = "17137";
						break;
					case "7.3":
						retval = "16974";
						break;
					case "7.2":
						retval = "16554";
						break;
					case "7.1":
						retval = "18539";
						break;
					default:
						retval = "17137";
						break;
				}
				
				return retval;
			}
		}
		#endregion // End Public Properties
		
		#region // Constructors
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NpgsqlConnectionWrapper()
		{
			this.pEntities = new MonoQueryListDictionary();
			this.pConnection = new NpgsqlConnection();
		}
		
		/// <summary>
		/// Constructor with connstring support.
		/// </summary>
		public NpgsqlConnectionWrapper( string connectionString )
			: this ()
		{
			this.ConnectionString = connectionString;
		}
		#endregion // End Constructors
		
		#region // Public Methods
		/// <summary>
		/// Open connection to the database server.
		/// </summary>
		public override bool Open()
		{
			try
			{
				if ( this.pConnection != null ) {
					this.pConnection.Open();
				}
			}
			catch  ( Exception err )
			{
				throw ( new OpenConnectionException( this.ConnectionString ) );
			}
			
			return ( this.IsOpen );
		}
		
		/// <summary>
		/// Close database connection
		/// </summary>
		public override void Close()
		{
			if ( this.pConnection != null ) {
				this.pConnection.Close();
			}
		}
		
		/// <summary>
		/// Execute a SQL Statement.
		/// <returns>System.Data.DataSet</returns>
		/// </summary>
		public override object ExecuteSQL( string SQLText, int maxRows )
		{
			NpgsqlCommand command = new NpgsqlCommand();
			DataSet returnValues = new DataSet();
			NpgsqlDataAdapter da = new NpgsqlDataAdapter();
			
			command.Connection = this.pConnection;
			command.CommandText = SQLText;
			command.CommandType = System.Data.CommandType.Text;
			
			command.Transaction = pConnection.BeginTransaction(
				System.Data.IsolationLevel.ReadCommitted );
			
			try
			{
				da.SelectCommand = command;
				if ( maxRows > 0 ) {
					da.Fill( returnValues, 0, maxRows, null );
				} else {
					da.Fill( returnValues );
				}
			}
			catch ( NpgsqlException e )
			{
				command.Transaction.Rollback();
				
				string mes = SQLText + "\n";
				
				foreach ( NpgsqlError err in e.Errors )
				{
					mes += "-----------------\n";
					mes += err.Message + "\n";
					mes += err.Hint + "\n";					
				}				
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
			
			return returnValues;
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
		#endregion // End Public Methods
		
		#region // Private Methods
		/// <summary>
		/// This method will get the tables from the server where
		/// restrictions match. This should work for all versions of postgresql
		/// that i am aware of. (7.1+)
		/// </summary>
		protected virtual DataTable GetTables( object [] restrictions )
		{
			if ( IsOpen == false ) {
				this.Open();
			}
			
			string commandText = "SELECT NULL AS TABLE_SCHEMA, c.relname AS TABLE_NAME, "
				+ "'" + this.CatalogName + "' AS TABLE_CATALOG "
				+ "FROM pg_class c "
				+ "WHERE c.relkind='r' "
				+ "AND NOT EXISTS (SELECT 1 FROM pg_rewrite r WHERE r.ev_class = c.oid AND r.ev_type = '1') "
				+ "AND c.relname NOT LIKE 'pg\\_%' "
				+ "AND c.relname NOT LIKE 'sql\\_%' "
				+ "ORDER BY relname;";
			
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
			adapter.SelectCommand = command;
			DataSet ds = new DataSet();
			adapter.Fill( ds );
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// This method will get our views from the current catalog. should work
		/// on postgres 7.1+ (havent checked 8.0 series)
		/// </summary>
		protected virtual DataTable GetViews( object [] restrictions )
		{
			if ( IsOpen == false ) {
				this.Open();
			}
			
			string commandText = "SELECT c.relname AS TABLE_NAME, "
				+ "'" + this.CatalogName + "' AS TABLE_CATALOG, "
				+ "n.nspname AS TABLE_SCHEMA "
				+ "FROM pg_catalog.pg_class c "
				+ "LEFT JOIN pg_catalog.pg_namespace n ON (n.oid = c.relnamespace) "
				+ "WHERE (c.relkind = 'v'::\"char\") "
				+ "AND n.nspname NOT IN ('information_schema', 'pg_catalog', 'pg_toast') "
				+ "ORDER BY TABLE_NAME;";
			
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
			adapter.SelectCommand = command;
			DataSet ds = new DataSet();
			adapter.Fill( ds );
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// This will get a list of procedures from the current database.
		/// It requires that LastSystemOID is properly set for server
		/// version.
		/// </summary>
		protected virtual DataTable GetProcedures( object [] restrictions )
		{
			if ( this.IsOpen == false ) {
				this.Open();
			}
			
			string commandText = "SELECT "
				+ "proname AS PROCEDURE_NAME, "
				+ "'" + this.CatalogName + "' AS PROCEDURE_CATALOG, "
				+ "'" + this.SchemaName + "' AS PROCEDURE_SCHEMA "
				+ "FROM "
				+ "pg_proc pc, pg_user pu, pg_type pt "
				+ "WHERE "
				+ "pc.proowner = pu.usesysid "
				+ "AND pc.prorettype = pt.oid "
				+ "AND pc.oid > '" + this.LastSystemOID + "'::oid "
				+ "UNION "
				+ "SELECT "
				+ "proname AS PROCEDURE_NAME, "
				+ "'" + this.CatalogName + "' AS PROCEDURE_CATALOG, "
				+ "'" + this.SchemaName + "' AS PROCEDURE_SCHEMA "
				+ "FROM "
				+ "pg_proc pc, pg_user pu, pg_type pt "
				+ "WHERE "
				+ "pc.proowner = pu.usesysid "
				+ "AND pc.prorettype = 0 "
				+ "AND pc.oid > '" + this.LastSystemOID + "'::oid "
				+ "ORDER BY "
				+ "PROCEDURE_NAME;";
			
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
			adapter.SelectCommand = command;
			DataSet ds = new DataSet();
			adapter.Fill( ds );
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// This method will get the columns for a table.
		/// </summary>
		protected virtual DataTable GetTableColumns( object [] restrictions )
		{
			if ( !IsOpen ) this.Open();
			
			DataSet ds = new DataSet();
			string commandText = "SELECT a.attname AS COLUMN_NAME "
                        + "FROM "
                        + "  pg_catalog.pg_attribute a LEFT JOIN pg_catalog.pg_attrdef adef "
                        + "  ON a.attrelid=adef.adrelid "
                        + "  AND a.attnum=adef.adnum "
                        + "  LEFT JOIN pg_catalog.pg_type t ON a.atttypid=t.oid "
                        + "WHERE "
                        + "  a.attrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname='" + restrictions[2].ToString() + "') "
                        + "  AND a.attnum > 0 AND NOT a.attisdropped "
                        + "     ORDER BY a.attnum;";
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter da = new NpgsqlDataAdapter();
			da.SelectCommand = command;
			da.Fill( ds );
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// Get the columns associated with a view.
		/// </summary>
		protected virtual DataTable GetViewColumns( object [] restrictions )
		{
			if ( IsOpen == false ) {
				this.Open();
			}
			
			DataSet ds = new DataSet();
			string commandText = "SELECT a.attname AS COLUMN_NAME "
				+ "FROM "
				+ "  pg_catalog.pg_attribute a LEFT JOIN pg_catalog.pg_attrdef adef "
				+ "  ON a.attrelid=adef.adrelid "
				+ "  AND a.attnum=adef.adnum "
				+ "  LEFT JOIN pg_catalog.pg_type t ON a.atttypid=t.oid "
				+ "WHERE "
				+ "  a.attrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname='"
				+ restrictions[2].ToString() + "') "
				+ "  AND a.attnum > 0 AND NOT a.attisdropped "
				+ "     ORDER BY a.attnum;";
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter da = new NpgsqlDataAdapter();
			da.SelectCommand = command;
			da.Fill( ds );
			
			return ds.Tables[0];
		}
		
		/// <summary>
		/// This method will get a table filled with the returning columns
		/// for a procedure.
		/// </summary>
		protected virtual DataTable GetProcedureColumns( object [] restrictions )
		{
			if ( IsOpen == false ) {
				this.Open();
			}
			
			// Get procedures OID
			// FIXME:
			int oid = 0;
			
			DataSet ds = new DataSet();
			string commandText = "SELECT "
				+ "format_type(prorettype, NULL) as COLUMN_NAME, "
				+ "FROM "
				+ "pg_catalog.pg_proc pc, pg_catalog.pg_language pl "
				+ "WHERE "
				+ "pc.oid = '" + oid + "'::oid "
				+ "AND pc.prolang = pl.oid";
			NpgsqlCommand command = new NpgsqlCommand( commandText, this.pConnection );
			NpgsqlDataAdapter da = new NpgsqlDataAdapter();
			da.SelectCommand = command;
			da.Fill( ds );
			
			return ds.Tables[0];
		}
		
		protected override void CheckConnectionObject()
		{
			if ( this.pConnection == null )
				throw new Exception("Bad connection object");
		}
		#endregion // End Private Methods
	}
}
