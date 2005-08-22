//
// Provider/SybaseDbProvider.cs
//
// Authors:
//   Christian Hergert	<chris@mosaix.net>
//   Daniel Morgan <danielmorgan@verizon.net>
//
// Copyright (C) 2005 Mosaix Communications, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

using Mono.Data.SybaseClient;

namespace Mono.Data.Sql
{
	/// <summary>
	/// Mono.Data.Sql provider for PostgreSQL databases.
	/// </summary>
	[Serializable]
	public class SybaseDbProvider : DbProviderBase
	{
		protected SybaseConnection connection = null;
		protected SybaseDataAdapter adapter = new SybaseDataAdapter();
		protected bool isConnectionStringWrong = false;
		
		/// <summary>
		/// Default Constructor
		/// </summary>
		public SybaseDbProvider () : base ()
		{
		}
		
		public override string ProviderName {
			get {
				return "Sybase ASE Database";
			}
		}
		
		/// <summary>
		/// Constructor with ADO.NET Sql connection.
		/// </summary>
		public SybaseDbProvider (SybaseConnection conn)
		{
			connection = conn;
		}
		
		/// <summary>
		/// ADO.NET Connection
		/// </summary>
		public override IDbConnection Connection {
			get {
				if (connection == null)
					connection = new SybaseConnection();
				
				return (IDbConnection) connection;
			}
		}
		
		/// <summary>
		/// Connection String
		/// </summary>
		public override string ConnectionString {
			get {
				return Connection.ConnectionString;
			}
			set {
				if (IsOpen == true)
					Close();
				
				Connection.ConnectionString = value;
				isConnectionStringWrong = false;
			}
		}
		
		/// <summary>
		/// Is the connection open
		/// </summary>
		public override bool IsOpen {
			get {
				return Connection.State == ConnectionState.Open;
			}
		}
		
		/// <summary>
		/// Is the last used connection string wrong
		/// </summary>
		public override bool IsConnectionStringWrong {
			get {
				return isConnectionStringWrong;
			}
		}
		
		/// <summary>
		/// Open the connection. Returns true on success.
		/// </summary>
		public override bool Open()
		{
			try {
				Connection.Open();
			} catch {
				isConnectionStringWrong = true;
			}
			OnOpen ();
			return IsOpen;
		}
		
		/// <summary>
		/// Close the database connection.
		/// </summary>
		public override void Close()
		{
			Connection.Close();
			OnClose();
		}
		
		/// <summary>
		/// Do we support the passed schema type
		/// </summary>
		public override bool SupportsSchemaType(Type type)
		{
			if (type == typeof(TableSchema))
				return true;
			else if (type == typeof(ViewSchema))
				return true;
			else if (type == typeof(ProcedureSchema))
				return true;
			else if (type == typeof(AggregateSchema))
				return true;
			else if (type == typeof(GroupSchema))
				return true;
			else if (type == typeof(UserSchema))
				return true;
			else if (type == typeof(LanguageSchema))
				return true;
			else if (type == typeof(OperatorSchema))
				return true;
			else if (type == typeof(RoleSchema))
				return true;
			else if (type == typeof(SequenceSchema))
				return true;
			else if (type == typeof(DataTypeSchema))
				return true;
			else if (type == typeof(TriggerSchema))
				return true;
			else if (type == typeof(RuleSchema))
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// Thread safe SQL execution.
		/// </summary>
		public override DataTable ExecuteSQL(string SQLText)
		{
			try {
				SybaseCommand command = new SybaseCommand();
				command.Connection = connection;
				command.CommandText = SQLText;

				DataSet resultSet = new DataSet ();

				lock(adapter) {
					adapter.SelectCommand = command;
					adapter.Fill(resultSet);
				}

				return resultSet.Tables[0];
			} catch {
				return null;
			}
		}
		
		/// <summary>
		/// Get a list of tables in the system.
		/// </summary>
		public override TableSchema[] GetTables()
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			SybaseCommand command = new SybaseCommand();
			command.Connection = connection;
			command.CommandText =
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.type as table_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE type IN ('S','U') " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2";
			SybaseDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				TableSchema table = new TableSchema();
				table.Provider = this;
				table.Name = r.GetString(1);

				table.IsSystemTable = r.GetString(4) == "S" ? true : false;
				
				table.SchemaName = r.GetString(0);
				table.OwnerName = r.GetString(0);
				table.Comment = "";
				
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat ("-- Table: {0}\n", table.Name);
				sb.AppendFormat ("-- DROP TABLE {0};\n\n", table.Name);
				sb.AppendFormat ("CREATE TABLE {0} (\n", table.Name);
				
				ColumnSchema[] columns = table.Columns;
				string[] parts = new string[columns.Length];
				for (int i = 0; i < parts.Length; i++) {
					parts[i] = "\t" + columns[i].Definition;
				}
				sb.Append (String.Join (",\n", parts));
				
				ConstraintSchema[] cons = table.Constraints;
				parts = new string[cons.Length];
				if (cons.Length > 0)
					sb.Append (",\n");
				for (int i = 0; i < parts.Length; i++) {
					parts[i] = "\t" + cons[i].Definition;
				}
				sb.Append (String.Join (",\n", parts));
				
				sb.Append ("\n);\n");
				//sb.AppendFormat ("COMMENT ON TABLE {0} IS '{1}';", table.Name, table.Comment);
				table.Definition = sb.ToString();
				collection.Add (table);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
			
			return (TableSchema[]) collection.ToArray (typeof (TableSchema));
		}

		/// <summary>
		/// Get columns for a table.
		/// </summary>
		public override ColumnSchema[] GetTableColumns(TableSchema table)
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList();
			SybaseConnection con2 = (SybaseConnection) (((ICloneable) connection).Clone ());
			if (con2.State == ConnectionState.Closed)
				con2.Open();
			SybaseCommand command = con2.CreateCommand ();
			
			command.CommandText = 
				"select su.name as owner, so.name as table_name, sc.name as column_name,  " +
					" st.name as date_type, sc.length as column_length,  " +
					" sc.prec as data_preceision, sc.scale as data_scale, " +
					" 0 as isnullable, sc.colid as column_id " +
					"from dbo.syscolumns sc, dbo.sysobjects so, " +
					"     dbo.systypes st, dbo.sysusers su " +
					"where sc.id = so.id " +
					"and so.type in ('U','S') " +
					"and so.name = '" + table.Name + "' " + 
					"and su.name = '" + table.OwnerName + "' " + 
					"and su.uid = so.uid " +
					"and sc.usertype = st.usertype " +
					"order by sc.colid";

			SybaseDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				ColumnSchema column = new ColumnSchema();
				
				try { column.Name = r.GetString(2); } catch {}
				column.Provider = this;
				try { column.DataTypeName = r.GetString(3); } catch {}
				try { column.Default = ""; } catch {}
				column.Comment = "";
				column.OwnerName = table.OwnerName;
				column.SchemaName = table.OwnerName;
				try { column.NotNull = r.GetValue(7).ToString() == "0" ? true : false;  } catch {}
				try { column.Length = r.GetInt32(4); } catch {}
				//try { column.Precision = GetInt(r, 5)); } catch {}
				//try { column.Scale = GetIn(r, 6); } catch {}
				
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("{0} {1}{2}",
					column.Name,
					column.DataTypeName,
					(column.Length > 0) ? ("(" + column.Length + ")") : "");
				sb.AppendFormat(" {0}", column.NotNull ? "NOT NULL" : "NULL");
				//if (column.Default.Length > 0)
				//	sb.AppendFormat(" DEFAULT {0}", column.Default);
				column.Definition = sb.ToString();
				
				collection.Add(column);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
			con2.Close ();
			con2 = null;
			
			return (ColumnSchema[]) collection.ToArray(typeof(ColumnSchema));
		}

		private string GetSource (string objectName) 
		{
			string sql = String.Format ("EXEC [master].[dbo].[sp_helptext] '{0}', null", objectName);
			SybaseConnection con2 = (SybaseConnection) (((ICloneable) connection).Clone ());
			if (con2.State == ConnectionState.Closed)
				con2.Open();
			SybaseCommand cmd = con2.CreateCommand ();
			cmd.CommandText = sql;
			IDataReader reader = cmd.ExecuteReader ();

			StringBuilder sb = new StringBuilder ();

			while (reader.Read ()) {
				string text = reader.GetString (0);
				sb.Append (text);
			}

			reader.Close ();
			reader = null;
			cmd.Dispose ();
			cmd = null;
			con2.Close ();
			con2 = null;

			return sb.ToString ();
		}

		/// <summary>
		/// Get a collection of views from the system.
		/// </summary>
		public override ViewSchema[] GetViews()
		{
			ArrayList collection = new ArrayList();
			
			SybaseCommand command = new SybaseCommand();
			command.Connection = connection;
			command.CommandText =
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.type as table_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE type = 'V' " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2";
			SybaseDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				ViewSchema view = new ViewSchema();
				view.Provider = this;
				
				try {
					view.Name = r.GetString(1);
					view.SchemaName = r.GetString(0);
					view.OwnerName = r.GetString(0);
					
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat ("-- View: {0}\n", view.Name);
					sb.AppendFormat ("-- DROP VIEW {0};\n\n", view.Name);
					string source = GetSource(view.Owner + "." + view.Name);
					sb.AppendFormat ("  {0}\n);", source);
					view.Definition = sb.ToString ();
					//view.Comment = r.GetString(5);
				} catch (Exception e) {
				}
				
				collection.Add(view);
			}
			r.Close ();
			r = null;
			command.Dispose();
			command = null;
			
			return (ViewSchema[]) collection.ToArray (typeof (ViewSchema));
		}
		
		/// <summary>
		/// Get a collection of columns within a view
		/// </summary>
		public override ColumnSchema[] GetViewColumns(ViewSchema view) 
		{
			if (IsOpen == false && Open() == false)
				throw new Exception ("No connection to database");
			
			ArrayList collection = new ArrayList();
			
			SybaseConnection con2 = (SybaseConnection) (((ICloneable) connection).Clone ());
			if (con2.State == ConnectionState.Closed)
				con2.Open();
			SybaseCommand command = con2.CreateCommand ();
			command.CommandText =
				"SELECT * " +
				" FROM " + view.Name +
				" WHERE 1 = 0";
			SybaseDataReader r = command.ExecuteReader();

			for (int i = 0; i < r.FieldCount; i++) {
				ColumnSchema column = new ColumnSchema();
				
				column.Name = r.GetName(i);
				column.DataTypeName = r.GetDataTypeName(i);
				column.Default = "";
				column.Definition = "";
				column.OwnerName = view.OwnerName;
				column.SchemaName = view.OwnerName;
				
				collection.Add(column);
			}

			command.Dispose ();
			command = null;
			con2.Close ();
			con2 = null;

			return (ColumnSchema[]) collection.ToArray (typeof(ColumnSchema));
		}
		
		/// <summary>
		/// Get a collection of constraints within a a table.
		/// </summary>
		public override ConstraintSchema[] GetTableConstraints (TableSchema table)
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
//			ArrayList collection = new ArrayList ();
//			
//			SybaseConnection con2 = (SybaseConnection) (((ICloneable) connection).Clone ());
//			con2.Open();
//			SybaseCommand command = con2.CreateCommand ();

			// TODO: get constraints
			return new ConstraintSchema[0];
		}
		
		public override UserSchema[] GetUsers ()
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			return (UserSchema[]) collection.ToArray (typeof (UserSchema));
		}
	}
}
