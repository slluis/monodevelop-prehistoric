using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.SchemaClass;

namespace MonoQuery.Exceptions
{
	public class ExecuteSQLException : Exception
	{		
		
		public ExecuteSQLException( ) : base( GettextCatalog.GetString( "SQL Exception" ) )
		{			
		}
		
		public ExecuteSQLException( ISchemaClass schema ) : base( GettextCatalog.GetString( "SQL Exception" ) 
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.ConnectionString + ")"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.Name + ")"
		                                                               )
		{
		}		
		
		public ExecuteSQLException( string message ) : base( GettextCatalog.GetString( "SQL Exception" ) 
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + message )	
       {		                                                               	
       }
		                                                               
	}
	
}
