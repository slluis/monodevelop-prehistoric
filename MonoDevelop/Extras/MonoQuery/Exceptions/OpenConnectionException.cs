using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.SchemaClass;


namespace MonoQuery.Exceptions
{
	public class OpenConnectionException : Exception
	{								
		public OpenConnectionException( ) : base( GettextCatalog.GetString( "Open Error" ) )
		{			
		}
		
		public OpenConnectionException( ISchemaClass schema ) : base( GettextCatalog.GetString( "Open Error" )
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.ConnectionString + ")"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.Name + ")"
		                                                               )
		{
		}		
		
		public OpenConnectionException( string message ) : base( GettextCatalog.GetString( "Open Error" ) 
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + message )
       {		                                                               	
       }
		                                                               

	}
	
}
