// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

namespace NewClassWizard
{

	/// <summary>
	/// Wraps a class type object and encapsulates access to its abstract members.
	/// </summary>
	public class NewClassInfo
	{
		private bool _IsSealed		= false;
		private bool _IsAbstract	= false;
		private bool _IsPublic		= true;
		private string _name;
		private string _summary		= "TODO - Add class summary";
		private License _license	= License.Empty;

		private Type _baseType		= null;
		private InterfaceCollection _interfaces = new InterfaceCollection();

		/// <summary>
		/// Initializes all fields to default values
		/// </summary>
		public NewClassInfo():this( typeof( object ) )
		{
		}

		/// <summary>
		/// Creates a new instance that inherits from a
		/// Type other than object
		/// </summary>
		/// <param name="baseType">The base type for the new class</param>
		public NewClassInfo( Type baseType ): this( baseType, "NewClass" )
		{			
		}

		/// <summary>
		/// Creates a new instance that inherits from a
		/// Type other than object and initializes the name
		/// </summary>
		/// <param name="baseType">The base class for the new class</param>
		/// <param name="name">The name of the new class</param>
		/// <exception cref="System.NullReferenceException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		public NewClassInfo( Type baseType, string name ) 
		{
			if ( baseType == null || name == null )
				throw new NullReferenceException();

			if ( !baseType.IsClass )
				throw new  ArgumentException( "The specified type is not a class" );

			_name = name;
			_baseType = baseType;			

		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		public License License
		{
			get { return _license; }
			set { _license = value; }
		}
		
		public string Summary
		{
			get { return _summary; }
			set { _summary = value; }
		}

		public bool IsAbstract
		{
			get { return _IsAbstract; }
			set 
			{ 
				_IsAbstract = value; 
				if ( _IsAbstract )
					_IsSealed = false; 
			}
		}

		public bool IsSealed
		{
			get { return _IsSealed; }
			set 
			{ 
				_IsSealed = value; 
				if ( _IsSealed )
					_IsAbstract = false; 
			}
		}

		public bool IsPublic
		{
			get { return _IsPublic; }
			set { _IsPublic = value; }
		}

		public InterfaceCollection ImplementedInterfaces
		{
			get { return _interfaces; }
		}
		
		/// <summary>
		/// The new class's super type
		/// </summary>
		public Type BaseType
		{
			get { return _baseType;	}
			set { _baseType = value; }
		}

		public Array GetAbstractMethods()
		{
			ArrayList methods = new ArrayList();

			//add abstract super type methods
			AbstractTypeInfo abstractType = new AbstractTypeInfo( _baseType );
			methods.AddRange( abstractType.GetMethods() );

			//add methods defined on all implemented interfaces
			foreach ( Type t in this._interfaces.GetInterfaces() )
			{
				abstractType = new AbstractTypeInfo( t );
				methods.AddRange( abstractType.GetMethods() );
			}

			return methods.ToArray( typeof(MethodInfo) );
		}



		public Array GetAbstractProperties()
		{

			ArrayList props = new ArrayList();

			//add abstract properties defined on the base class
			AbstractTypeInfo abstractType = new AbstractTypeInfo( _baseType );
			props.AddRange( abstractType.GetProperties() );

			//add properties defined on all implemented _interfaces
			foreach ( Type t in this._interfaces.GetInterfaces() )
			{
				abstractType = new AbstractTypeInfo( t );
				props.AddRange( abstractType.GetProperties() );
			}

			return props.ToArray( typeof(PropertyInfo) );
	
		}

	}
}
