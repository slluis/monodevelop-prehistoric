using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.SchemaClass;

namespace MonoQuery.Exceptions
{
	public class ExecuteProcedureException : Exception
	{								
		public ExecuteProcedureException( ) : base( GettextCatalog.GetString( "Procedure Exception" ) )
		{			
		}
		
		public ExecuteProcedureException( ISchemaClass schema ) : base( GettextCatalog.GetString( "Procedure Exception" ) 
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.ConnectionString + ")"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.Name + ")"
		                                                               )
		{
		}		
		
		public ExecuteProcedureException( string message ) : base( GettextCatalog.GetString( "Procedure Exception" ) 
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + message )			
       {		                                                               	
       }
	}
	
}
