// created on 11/11/2003 at 11:20

using  System;
using  System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoQuery.Collections {
	
	///<summary>List dictionnary.
	/// <param name='key'> this a string is defining the key</param>
	/// <param name='value'> this a  <see cref=".MonoQuerySchemaClassCollection"></see> is defining the value</param>
	/// </summary>

	public class MonoQueryListDictionary : DictionaryBase
	{	
		public MonoQuerySchemaClassCollection this[ string key ]  {
	    	get  {
	    		return( (MonoQuerySchemaClassCollection) Dictionary[key] );
	    	}
	    	set {
				Dictionary[key] = value;
	    	}
	    }
	
	   public ICollection Keys  {
	      get  {
	         return( Dictionary.Keys );
	      }
	   }
	
	   public ICollection Values  {
	      get  {
	         return( Dictionary.Values );
	      }
	   }
	
	   public void Add( string key, MonoQuerySchemaClassCollection value )  {
	      Dictionary.Add( key, value );
	   }
	
	   public bool Contains( string key )  {
	      return( Dictionary.Contains( key ) );
	   }
	
	   public void Remove( string key )  {
	      Dictionary.Remove( key );
	   }
	
	   protected override void OnInsert( object key, object value )  {
//	      StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
	      if ( !(key is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Key Type" ), "key" );
	
	      if ( !(value is MonoQuerySchemaClassCollection) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Value Type" ), "value" );
	   }
	
	   protected override void OnRemove( object key, object value )  {
//		StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
	      if ( !(key is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Key Type" ), "key" );
	      }
	
	   protected override void OnSet( object key, object oldValue, object newValue )  {
//		  StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));	 
	 	  if (!(key is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Key Type" ), "key" );
	
	      if ( !(newValue is MonoQuerySchemaClassCollection) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Value Type" ), "newValue" );
	   }
	
	   protected override void OnValidate( object key, object value )  {
//		  StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));	   	
	      if ( !(key is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Key Type" ), "key" );
	   	
	      if ( !(value is MonoQuerySchemaClassCollection) )
	         throw new ArgumentException(  GettextCatalog.GetString( "Wrong Value Type" ), "value" );
	   }
	
	}
}
