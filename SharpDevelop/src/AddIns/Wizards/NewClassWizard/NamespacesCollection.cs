// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;

namespace NewClassWizard
{
	/// <summary>
	/// Summary description for NamespacesCollection.
	/// </summary>
	internal class NamespacesCollection : DictionaryBase
	{

		internal NamespacesCollection()
		{
		}

		public void Add( Namespace ns )
		{
			IDictionary dict = base.Dictionary;
			dict.Add( ns.Name, ns );
		}

		public Namespace this[ string key ]
		{
			get
			{
				System.Collections.Hashtable ht = base.InnerHashtable;
				return (Namespace)ht[key];
			}
		}

		public Namespace this[ int index ]
		{
			get
			{
				System.Collections.Hashtable ht = base.InnerHashtable;
				return (Namespace)ht[index];
			}
		}
		public bool Contains( string key )
		{
			IDictionary dict = base.Dictionary;
			return dict.Contains( key );
		}

	}

	internal class Namespace {
		
		public readonly string Name;
		public readonly NamespacesCollection Namespaces = new NamespacesCollection();
		
		private ArrayList types = new ArrayList();

		public Namespace( string name )	{
			Name = name;
		}


		public Array GetTypes()	{
			return types.ToArray( typeof(Type) );
		}

		private void AddType( Type t ) {
			types.Add( t );
		}
		 
		public static Namespace CreateNamespaceTree( Assembly a ) {
			Namespace ns = new Namespace( a.GetName().Name );

			foreach ( Type t in a.GetExportedTypes() ) {
				string nsName = t.Namespace + "." + t.Name;
				buildNSTree( nsName.Substring( nsName.IndexOf( '.' ) + 1 ), ns, t );
			}

			return ns;
		}


		private static void buildNSTree( string ns, Namespace parentNamespace, Type t )	{	
			
			int dotPos = ns.IndexOf( '.' );

			//if there is not '.' in the ns Name then it 
			//that it corresponds to type t
			//just add the Type to the parent
			if ( dotPos == -1 )	{
				parentNamespace.AddType( t );
			}
			//otherwise it's another ndoe in the namespace hierarchy
			else {
				Namespace newNs = null;
				string name = ns.Substring( 0, dotPos );  //get the front node

				//if the parent doesn't contain the namespace create it
				if ( parentNamespace.Namespaces.Contains( name ) == false )	{
					newNs = new Namespace( name );
					parentNamespace.Namespaces.Add( newNs );
				}
				else {
					newNs = parentNamespace.Namespaces[name];
				}

				//recurse by stripping off the front node
				buildNSTree( ns.Substring( dotPos + 1 ), newNs, t );

			}

		}
	}

}
