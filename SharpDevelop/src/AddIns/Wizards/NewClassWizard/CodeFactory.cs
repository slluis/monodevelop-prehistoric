// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace NewClassWizard {

	/// <summary>
	/// Generates a CodeDOM object graph that implements a specific abstract class.
	/// </summary>
	internal class CodeFactory {

		private CodeNamespace _codeNamespace	= new CodeNamespace();

		public readonly CodeFactoryOptions Options;

		public CodeFactory() : this( new CodeFactoryOptions() )	{
		}

		public CodeFactory( CodeFactoryOptions opt ) {
			if ( opt == null )
				opt = new CodeFactoryOptions();

			Options = opt;	

			//Always add the System namespace
			addImport( "System" );
		}

		public string Namespace {
			get	{
				return _codeNamespace.Name;
			}
			set	{
				_codeNamespace.Name = value;
			}
		}

		public void GenerateCode( Language language, TextWriter tw ) {
			//create a compile unit for the namespace
			CodeCompileUnit ccu	= new CodeCompileUnit();
			ccu.Namespaces.Add( _codeNamespace );
			
			CreateProjectCode( ccu, language, tw, this.Options.GetGeneratorOptions() );
		}

		public void AddClass( NewClassInfo classInfo ) {
			addImport( classInfo.BaseType.Namespace );			

			//get the type declaration
			CodeTypeDeclaration theClass = CreateClass( classInfo, Options.AutoGenerateStubs, Options.AutoGenerateComments );
			_codeNamespace.Types.Add(theClass);		
			if ( License.IsEmpty( classInfo.License ) == false )
				addDocComment( _codeNamespace.Comments, classInfo.License.Text, "license" );
		}

		private void addImport( string ns ) {
			//if the namespace isn't already imported create an import statement
			if ( importExists( ns ) == false ) {
				CodeNamespaceImport nsImport = new CodeNamespaceImport( ns );
				_codeNamespace.Imports.Add( nsImport );
			}
		}

		private bool importExists( string ns ) {

			foreach ( CodeNamespaceImport nsImport in _codeNamespace.Imports ) {
				if ( nsImport.Namespace == ns )
					return true;
			}

			return false;

		}


		#region static methods

		#region CreateProjectCode overrides
		public static void CreateProjectCode( CodeCompileUnit codeObject, Language language, TextWriter tw, CodeGeneratorOptions options )
		{
			ICodeGenerator generator = CodeProviderChooser.chooseCodeGenerator( language );
			//generate code using the generator
			generator.GenerateCodeFromCompileUnit( codeObject, tw, options );
		}
		public static void CreateProjectCode( CodeExpression codeObject, Language language, TextWriter tw, CodeGeneratorOptions options )
		{
			ICodeGenerator generator = CodeProviderChooser.chooseCodeGenerator( language );
			//generate code using the generator
			generator.GenerateCodeFromExpression( codeObject, tw, options );
		}
		public static void CreateProjectCode( CodeNamespace codeObject, Language language, TextWriter tw, CodeGeneratorOptions options )
		{
			ICodeGenerator generator = CodeProviderChooser.chooseCodeGenerator( language );
			//generate code using the generator
			generator.GenerateCodeFromNamespace( codeObject, tw, options );
		}
		public static void CreateProjectCode( CodeStatement codeObject, Language language, TextWriter tw, CodeGeneratorOptions options )
		{
			ICodeGenerator generator = CodeProviderChooser.chooseCodeGenerator( language );
			//generate code using the generator
			generator.GenerateCodeFromStatement( codeObject, tw, options );

		}
		public static void CreateProjectCode( CodeTypeDeclaration codeObject, Language language, TextWriter tw, CodeGeneratorOptions options )
		{
			ICodeGenerator generator = CodeProviderChooser.chooseCodeGenerator( language );
			//generate code using the generator
			generator.GenerateCodeFromType( codeObject, tw, options );

		}
		#endregion

		/// <summary>
		/// adds implmentations for all abstract methods
		/// </summary>
		private static void implementMethods( NewClassInfo classInfo, CodeTypeDeclaration c, bool GenerateComments )
		{
			foreach ( MethodInfo m in classInfo.GetAbstractMethods() )
			{
				addMethod( c, m, GenerateComments );
			}
		}

		/// <summary>
		/// adds implmentations for all abstract properties
		/// </summary>
		private static void implementProperties( NewClassInfo classInfo, CodeTypeDeclaration c, bool GenerateComments )
		{
			foreach ( PropertyInfo p in classInfo.GetAbstractProperties() )
			{
				addProperty( c, p, GenerateComments );
			}
		}


		public static CodeMemberMethod CreateMethod( MethodInfo m, bool GenerateComments )
		{
			string remarks = String.Empty;

			CodeMemberMethod member = new CodeMemberMethod();
			member.Name = m.Name;
			member.ReturnType = new CodeTypeReference( m.ReturnType );

			//all interface members are public
			if ( m.DeclaringType.IsInterface ) 
			{
				remarks += "Interface method from " + m.DeclaringType.Name + "\n";

				member.Attributes = MemberAttributes.Public;  
			}
			else
			{
				remarks += "Inherited method from base class " + m.DeclaringType.Name + "\n";

				//this is a public abstract member
				if ( m.IsPublic )
				{
					member.Attributes = MemberAttributes.Public | MemberAttributes.Override;
				}
					//non public abstract members are protected
				else
				{
					member.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				}

			}

			addParameters( member.Parameters, m.GetParameters() );

			addImpl( member.Statements );

			if ( GenerateComments )
			{
				addDocComment( member.Comments, "TODO - add method description", "summary" );
				addDocComment( member.Comments, remarks, "remarks" );
				addParameterComments( member.Parameters, member.Comments );
			}

			return member;
		}


		/// <summary>
		/// creates and returns a class object graph
		/// </summary>
		public static CodeTypeDeclaration CreateClass( NewClassInfo classInfo, bool AutoGenerateStubs, bool GenerateComments )  
		{
		
			//create the type declaration
			CodeTypeDeclaration theClass = new CodeTypeDeclaration( classInfo.Name );	
			theClass.IsClass = true;
		
			//add the base class type
			theClass.BaseTypes.Add( classInfo.BaseType );

			//add interface base types
			foreach ( Type t in classInfo.ImplementedInterfaces.GetInterfaces() )
			{
				theClass.BaseTypes.Add( t );
			}	
		
			//add the summary comment
			if ( classInfo.Summary.Length > 0 )
			{
				addDocComment( theClass.Comments, classInfo.Summary, "summary" );
			}

			//add some meta data if option set
			if ( GenerateComments ) 
			{
				string s = "\tcreated by - " + Environment.UserName + "\n";
				s += "\tcreated on - " + DateTime.Now;
				addDocComment( theClass.Comments, s, "remarks" );
			}


			if ( classInfo.IsAbstract )
				theClass.TypeAttributes = theClass.TypeAttributes | TypeAttributes.Abstract;

			if ( classInfo.IsSealed )
				theClass.TypeAttributes = theClass.TypeAttributes | TypeAttributes.Sealed;

			//set class visiblity
			if ( classInfo.IsPublic )
				theClass.TypeAttributes = theClass.TypeAttributes | TypeAttributes.Public;
			else
				theClass.TypeAttributes = theClass.TypeAttributes | TypeAttributes.NestedFamily;
			
			if ( AutoGenerateStubs ) {
				//add a constructor to the type if it's not abstract
				if ( classInfo.IsAbstract == false )
				{
					CodeConstructor cc = new CodeConstructor();			
					cc.Attributes = MemberAttributes.Public;
	
					//comment the contructor
					if ( GenerateComments )
						addDocComment( cc.Comments, "Default constructor - initializes all fields to default values", "summary" );
	
					theClass.Members.Add( cc );
				}
	
				implementMethods( classInfo, theClass, GenerateComments );
				implementProperties( classInfo, theClass, GenerateComments );
			}
			
			return theClass;

		}

		/// <summary>
		/// adds a single method corresponding to the MethodInfo
		/// to the CodeTypeDeclaration
		/// </summary>
		private static void addMethod( CodeTypeDeclaration c, MethodInfo m, bool GenerateComments )
		{
			c.Members.Add( CodeFactory.CreateMethod( m, GenerateComments ) );
		}


		public static CodeMemberProperty CreateProperty( PropertyInfo p, bool GenerateComments )
		{
			string remarks = String.Empty;

			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Name = p.Name;
			prop.HasGet = p.CanRead;
			prop.HasSet = p.CanWrite;
			prop.Type = new CodeTypeReference( p.PropertyType );

			if ( p.DeclaringType.IsInterface ) 
			{
				remarks += "Interface property from " + p.DeclaringType.Name + "\n";

				prop.Attributes = MemberAttributes.Public;
			}
			else
			{
				remarks += "Inherited property from base class " + p.DeclaringType.Name + "\n";

				if ( ( p.GetGetMethod() != null && p.GetGetMethod().IsAbstract ) || ( p.GetSetMethod() != null && p.GetSetMethod().IsAbstract ) )
				{
					prop.Attributes = MemberAttributes.Public | MemberAttributes.Override;
				}
				else
				{
					prop.Attributes = MemberAttributes.Family | MemberAttributes.Override;
				}
			}

			addParameters( prop.Parameters, p.GetIndexParameters() );

			if ( prop.HasGet && prop.HasSet ) 
			{
				remarks += "\t- read/write\n";
				addImpl( prop.SetStatements );
				addImpl( prop.GetStatements );
			}
			else if ( prop.HasGet )
			{
				remarks += "\t- read only\n";
				addImpl( prop.GetStatements );
			}
			else
			{
				remarks += "\t- write only\n";
				addImpl( prop.SetStatements );
			}

			//generate comments if option set
			if ( GenerateComments ) 
			{
				addDocComment( prop.Comments, "TODO - add property description", "summary" );
				addDocComment( prop.Comments, remarks, "remarks" );
				addParameterComments( prop.Parameters, prop.Comments );
			}

			return prop;
		}


		/// <summary>
		/// Adds a single property corresponding to the PropertyInfo object
		/// to the CodeTypeDeclarion's properites collection
		/// </summary>
		private static void addProperty( CodeTypeDeclaration c, PropertyInfo p, bool GenerateComments )
		{
			//add the memeber to the class
			c.Members.Add( CodeFactory.CreateProperty( p, GenerateComments ) );
		}

		/// <summary>
		/// Iterates over the ParameterInfo items in the paramArray
		/// adding each one to the parameter collection as a new CodeParameterDeclaration
		/// </summary>
		private static void addParameters( CodeParameterDeclarationExpressionCollection paramCol, ParameterInfo[] paramArray )
		{
			foreach ( ParameterInfo pInfo in paramArray )
			{
				CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression( pInfo.ParameterType, pInfo.Name );
				paramCol.Add( param );
			}
		}

		/// <summary>
		/// Adds a default implementation to the referenced statement collection.
		/// </summary>
		/// <remarks>The default implementation is to throw a NotImplementedException</remarks>
		private static void addImpl( CodeStatementCollection statements ) 
		{
			//make the "new NotImplementedException()" expression
			CodeObjectCreateExpression createExp = new CodeObjectCreateExpression();
			createExp.CreateType = new CodeTypeReference( typeof(NotImplementedException) );

			//make the "throw" expression
			CodeThrowExceptionStatement throwExp = new CodeThrowExceptionStatement( createExp );
			statements.Add(throwExp);
		}

		private static void addDocComment( CodeCommentStatementCollection comments, string content, string docType )
		{
			//open the doc comment
			comments.Add( new CodeCommentStatement( "<" + docType + ">", true ) );

			//create a doc comment for every line in the string
			string[] lines = content.Split( '\n' );
			foreach ( string s in lines )
			{
				//strip off the carriage return
				comments.Add( new CodeCommentStatement( s.Replace( "\r", String.Empty ), true ) );
			}
			//close the doc comment
			comments.Add( new CodeCommentStatement( "</" + docType + ">", true ) );
		}

		private static void addParameterComments( CodeParameterDeclarationExpressionCollection paramCollection, CodeCommentStatementCollection comments )
		{
			foreach ( CodeParameterDeclarationExpression p in paramCollection )
			{
				comments.Add( new CodeCommentStatement( "<param name='" + p.Name + "'>TODO - add parameter description</param>", true ) );
			}
		}
		#endregion
	}
}
