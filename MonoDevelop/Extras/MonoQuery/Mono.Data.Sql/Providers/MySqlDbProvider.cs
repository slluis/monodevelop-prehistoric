//
// Providers/MySqlDbProvider.cs
//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//
// Copyright (c) 2005 Christian Hergert
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

using ByteFX.Data.MySqlClient;

namespace Mono.Data.Sql
{
	[Serializable]
	public class MySqlDbProvider : DbProviderBase
	{
		protected MySqlConnection connection = null;
		protected MySqlDataAdapter adapter = new MySqlDataAdapter ();
		protected bool isConnectionStringWrong = false;
		
		public override string ProviderName {
			get {
				return "MySQL Database (Incomplete)";
			}
		}
		
		public override IDbConnection Connection {
			get {
				if (connection == null)
					connection = new MySqlConnection ();
				
				return (IDbConnection) connection;
			}
		}
		
		public override string ConnectionString {
			get {
				return Connection.ConnectionString;
			}
			set {
				if (IsOpen)
					Close ();
				
				Connection.ConnectionString = value;
				isConnectionStringWrong = false;
			}
		}
		
		public override bool IsOpen {
			get {
				return Connection.State == ConnectionState.Open;
			}
		}
		
		public override bool IsConnectionStringWrong {
			get {
				return isConnectionStringWrong;
			}
		}
		
		public override bool Open ()
		{
			try {
				Connection.Open ();
				OnOpen ();
			} catch (Exception e) {
				isConnectionStringWrong = true;
			}
			
			return IsOpen;
		}
		
		public override void Close ()
		{
			Connection.Close ();
			OnClose ();
		}
		
		public override bool SupportsSchemaType(Type type)
		{
			// FIXME: Need to check what mysql actually supports.
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
		
		public override DataTable ExecuteSQL (string SQLText)
		{
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText = SQLText;
			
			DataSet resultSet = null;
			
			lock (adapter) {
				adapter.SelectCommand = command;
				adapter.Fill (resultSet);
			}
			
			return resultSet.Tables[0];
		}
		
		public override TableSchema[] GetTables ()
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText =
				"";
			MySqlDataReader r = command.ExecuteReader ();
			
			while (r.Read ()) {
				TableSchema table = new TableSchema ();
				table.Provider = this;
				
				// TODO: Implement
				
				collection.Add (table);
			}
			
			return (TableSchema[]) collection.ToArray (typeof (TableSchema));
		}
		
		public override ViewSchema[] GetViews ()
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText =
				"";
			MySqlDataReader r = command.ExecuteReader ();
			
			while (r.Read ()) {
				ViewSchema view = new ViewSchema ();
				view.Provider = this;
				
				// TODO: Implement
				
				collection.Add (view);
			}
			
			return (ViewSchema[]) collection.ToArray (typeof (ViewSchema));
		}
		
		public override ColumnSchema[] GetTableColumns (TableSchema table)
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText =
				"";
			MySqlDataReader r = command.ExecuteReader ();
			
			while (r.Read ()) {
				ColumnSchema column = new ColumnSchema ();
				column.Provider = this;
				
				// TODO: Implement
				
				collection.Add (column);
			}
			
			return (ColumnSchema[]) collection.ToArray (typeof (ColumnSchema));
		}
		
		public override ColumnSchema[] GetViewColumns (ViewSchema table)
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText =
				"";
			MySqlDataReader r = command.ExecuteReader ();
			
			while (r.Read ()) {
				ColumnSchema column = new ColumnSchema ();
				column.Provider = this;
				
				// TODO: Implement
				
				collection.Add (column);
			}
			
			return (ColumnSchema[]) collection.ToArray (typeof (ColumnSchema));
		}
		
		public override ConstraintSchema[] GetTableConstraints (TableSchema table)
		{
			if (IsOpen == false && Open () == false)
				throw new InvalidOperationException ("Invalid connection");
			
			ArrayList collection = new ArrayList ();
			
			MySqlCommand command = new MySqlCommand ();
			command.Connection = Connection;
			command.CommandText =
				"";
			MySqlDataReader r = command.ExecuteReader ();
			
			while (r.Read ()) {
				ConstraintSchema constraint = new ConstraintSchema ();
				constraint.Provider = this;
				
				// TODO: Implement
				
				collection.Add (constraint);
			}
			
			return (ConstraintSchema[]) collection.ToArray (
				typeof (ConstraintSchema));
		}
	}
}
