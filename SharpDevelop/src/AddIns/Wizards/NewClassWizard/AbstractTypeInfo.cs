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
	/// Summary description for AbtractTypeInfo.
	/// </summary>
	internal class AbstractTypeInfo
	{
		public readonly Type type;

		public AbstractTypeInfo( Type t )
		{
			if ( t == null )
				throw new NullReferenceException();

			type = t;
		}

		public Array GetMethods()
		{
			ArrayList methods = new ArrayList();

			methods.AddRange( GetMethodsFromType( type ) );

			//if the type is an interface get all the methods from
			//inherited interfaces 
			//this is not neseccary if the type is a class because
			//the clas will have had to have implemented interface members
			if ( type.IsInterface )
			{
				foreach ( Type t in type.GetInterfaces() )
				{
					methods.AddRange( GetMethodsFromType( t ) );
				}
			}

			return methods.ToArray( typeof(MethodInfo) );
		}

		private Array GetMethodsFromType( Type t )
		{
			ArrayList methods = new ArrayList();

			//the flags used will get both protected and public members
			//(they will get private members as well but they get filtered because
			//they cannot be abstract)
			foreach ( MethodInfo m in t.GetMethods( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) )
			{
				//filter out property getter and setter methods
				if ( m.IsAbstract && !m.IsSpecialName )
				{
					methods.Add( m );
				}
			}

			return methods.ToArray( typeof(MethodInfo) );

		}

		public Array GetProperties()
		{

			ArrayList props = new ArrayList();

			props.AddRange( GetPropertiesFromType( type ) );

			//if the type is an interface get all the Properties from
			//inherited interfaces 
			//this is not neseccary if the type is a class because
			//the clas will have had to have implemented interface members
			if ( type.IsInterface )
			{
				foreach ( Type t in type.GetInterfaces() )
				{
					props.AddRange( GetPropertiesFromType( t ) );
				}
			}

			return props.ToArray( typeof(PropertyInfo) );
	
		}

		private Array GetPropertiesFromType( Type t )
		{
			ArrayList props = new ArrayList();

			//these flags used will get both protected and public members
			//(they will get private members as well but they get filtered because
			//they cannot be abstract)
			foreach ( PropertyInfo p in t.GetProperties( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public  ) )
			{
				//ASSUMPTION
				//a property is abstract when either its getter or setter method is abstract
				MethodInfo getter = p.GetGetMethod( true );
				MethodInfo setter = p.GetSetMethod( true );
				if ( ( getter != null && getter.IsAbstract ) ||	( setter != null && setter.IsAbstract ) )
				{
					props.Add( p );
				}
			}

			return props.ToArray( typeof(PropertyInfo) );
		}
	}
}
