using  System;
using  System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoQuery.Collections {
	///<summary>String dictionnary
	/// <param name='key'> this a string is defining the key</param>
	/// <param name='value'> this a string is defining the value</param>
	/// </summary>
	public class MonoQueryStringDictionary : DictionaryBase  {
	
	readonly StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
	
	public string this[ string key ]  {
	      get  {
	         return( (string) Dictionary[key] );
	      }
	      set  {
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
	
	   public void Add( string key, string value )  {
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
	
	      if ( !(value is string) )
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
	
	      if ( !(newValue is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Value Type" ), "newValue" );
	   }
	
	   protected override void OnValidate( object key, object value )  {
//		  StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));	   	
	      if ( !(key is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Key Type" ), "key" );
	   	
	      if ( !(value is string) )
	         throw new ArgumentException( GettextCatalog.GetString( "Wrong Value Type" ), "value" );
	   }
	
	}
}
