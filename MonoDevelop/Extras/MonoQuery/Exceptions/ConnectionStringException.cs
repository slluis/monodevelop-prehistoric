using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoQuery.SchemaClass;

namespace MonoQuery.Exceptions
{
	public class ConnectionStringException : Exception
	{				
		public ConnectionStringException( ) : base( GettextCatalog.GetString( "Wrong Connection String" ) )
		{			
		}
		
		public ConnectionStringException( ISchemaClass schema ) : base( GettextCatalog.GetString( "Wrong Connection String" )
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.ConnectionString + ")"
		                                                               + "\n\r"
		                                                               + "(" + schema.Connection.Name + ")"
		                                                               )
		{
		}		
		
		public ConnectionStringException( string message ) : base( GettextCatalog.GetString( "Wrong Connection String" )
		                                                               + "\n\r"
		                                                               + "-----------------"
		                                                               + "\n\r"
		                                                               + message )
		{
		}					
	}
	
}
