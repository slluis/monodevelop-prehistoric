// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections;

namespace NewClassWizard
{
	public class InterfaceCollection : DictionaryBase
	{
		public InterfaceCollection()
		{
		}

		public void Add( Type interfaceType )
		{
			if ( interfaceType == null )
				throw new NullReferenceException();

			if ( !interfaceType.IsInterface )
				throw new ArgumentException( "The specified type is not an interface" );

			IDictionary dict = base.Dictionary;
			string key = InterfaceCollection.InterfaceKey( interfaceType );

			if ( dict.Contains( key ) == false )
				dict.Add( key, interfaceType );

		}

		public ArrayList GetInterfaces()
		{

			return new ArrayList( base.Dictionary.Values );
			//IDictionary dict = base.Dictionary;

			//dict.Values.CopyTo(outArray, 0);
			//outArray.AddRange( base.Dictionary );
			/*
			IEnumerator interfaces = base.GetEnumerator();
			DictionaryEntry entry;

			while ( interfaces.MoveNext() )
			{
				entry = (DictionaryEntry)interfaces.Current;
				outArray.Add( (Type)entry.Value );
			}
			*/
			//return outArray.ToArray( typeof(Type) );

		}

		public void Remove( Type interfaceType )
		{
			IDictionary dict = base.Dictionary;
			dict.Remove( InterfaceCollection.InterfaceKey( interfaceType ) );
		}

		public static string InterfaceKey( Type i )
		{
			return i.Namespace + "." + i.Name;
		}
	}
	
}
