// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.CodeDom.Compiler;
using Microsoft.Win32;

namespace NewClassWizard
{
	public enum BracingStyleEnum { bsCStyle, bsBlock }

	public class CodeFactoryOptions
	{
		private const string RKEY_OPTIONS = 
			"SOFTWARE\\IT Software Engineering\\SharpDevelop\\AddIns\\NewClassWizard\\Options" ;
	
		public static string RVALUE_LICENSE 		= "LicenseHeaderID";
		public static string RVALUE_STUBS 			= "AutoGenerateStubs";
		public static string RVALUE_COMMENTS 		= "AutoGenerateComments";
		public static string RVALUE_BLANKLINES		= "BlankLinesBetweenMembers";
		public static string RVALUE_BRACINGSTYLE 	= "BracingStyle";
		
		private BracingStyleEnum _bracingStyle	= BracingStyleEnum.bsBlock;
		private bool _BlankLinesBetweenMembers	= true;
		private bool _AutoGenerateComments		= true;
		private bool _AutoGenerateStubs			= true;
		
		//the license header id corresponds to a license element in the
		//licenses.xml resource file 
		private string _LicenseHeaderID			= String.Empty;

		public CodeFactoryOptions()
		{
		}

			 
		public CodeGeneratorOptions GetGeneratorOptions()
		{
			CodeGeneratorOptions ret = new CodeGeneratorOptions();
			ret.BlankLinesBetweenMembers = _BlankLinesBetweenMembers;
			ret.BracingStyle = BracingStyleString();
			ret.IndentString = "\t";
			return ret;
		}
		
		public bool AutoGenerateStubs {
			get	{
				return _AutoGenerateStubs;
			}
			set	{
				_AutoGenerateStubs = value;
			}
		}
		
		public bool AutoGenerateComments {
			get	{
				return _AutoGenerateComments;
			}
			set	{
				_AutoGenerateComments = value;
			}
		}
		
		public BracingStyleEnum BracingStyle {
			get	{
				return _bracingStyle;
			}
			set
			{
				_bracingStyle = value;
			}
		}
		
		public bool BlankLinesBetweenMembers {
			get	{
				return _BlankLinesBetweenMembers;
			}
			set	{
				_BlankLinesBetweenMembers = value;
			}
		}

		private string BracingStyleString()	{
			switch ( _bracingStyle )
			{
				case BracingStyleEnum.bsBlock :
					return "Block";
					
				case BracingStyleEnum.bsCStyle :
					return "C";

				default:
					return "Block";
			}
		}
		
		public static BracingStyleEnum BracingStyleFromString( string bracingStyle )	{
			switch ( bracingStyle )
			{
				case "Block" :
					return BracingStyleEnum.bsBlock;
					
				case "C" :
					return BracingStyleEnum.bsCStyle;

				default:
					return BracingStyleEnum.bsBlock;
			}
		}
		
		public static object ReadOption( string key, object defaultValue ) {
			RegistryKey Key = null;

			try
			{
				Key = Registry.CurrentUser.OpenSubKey( RKEY_OPTIONS );	
				return Key.GetValue( key, defaultValue );
			} catch ( Exception ) {
				return defaultValue;
			}
		}
		
		public static void WriteOption( string key, object Value ) {
			RegistryKey Key = null;
			try
			{
				Key = Registry.CurrentUser.CreateSubKey( RKEY_OPTIONS ) ;
				Key.SetValue( key, Convert.ToString( Value ) );
			}			
			catch ( Exception )	{
			}
			finally	{
				if ( Key != null )
					Key.Close();
			}
		}		
		
		public static CodeFactoryOptions LoadFromStorage() {
			RegistryKey Key = null;

			try
			{
				Key = Registry.CurrentUser.OpenSubKey( RKEY_OPTIONS ) ;
				CodeFactoryOptions opt = new CodeFactoryOptions();
				
				if ( Key != null ) {
					opt.AutoGenerateStubs = Convert.ToBoolean( Key.GetValue ( RVALUE_STUBS, true ) );
					opt.AutoGenerateComments = Convert.ToBoolean( Key.GetValue ( RVALUE_COMMENTS, true ) );
					opt.BlankLinesBetweenMembers = Convert.ToBoolean( Key.GetValue ( RVALUE_BLANKLINES, true ) );
					opt.BracingStyle = BracingStyleFromString( (string)Key.GetValue( RVALUE_BRACINGSTYLE, "bsBlock" ) );

				}

				return opt;
				
			} catch ( Exception ) {
				return new CodeFactoryOptions();
			}
			finally	{
				if ( Key != null )
					Key.Close();
			}
		}
		public static void SaveToStorage( CodeFactoryOptions o ) {
			if ( o == null )
				throw new NullReferenceException();
			
			RegistryKey Key = null;
			try
			{
				Key = Registry.CurrentUser.CreateSubKey( RKEY_OPTIONS ) ;

				Key.SetValue( RVALUE_STUBS, Convert.ToString( o.AutoGenerateStubs ) );
				Key.SetValue( RVALUE_COMMENTS, Convert.ToString( o.AutoGenerateComments ) );
				Key.SetValue( RVALUE_BLANKLINES, Convert.ToString( o.BlankLinesBetweenMembers ) );
				Key.SetValue( RVALUE_BRACINGSTYLE, o.BracingStyle );
			}
			catch ( Exception )	{
			}
			finally	{
				if ( Key != null )
					Key.Close();
			}
		}


		#region Can't get this object to serialize to XML under Beta 2
//		public static CodeFactoryOptions LoadFromStorage()
//		{
//			XmlReader reader = null;
//			try
//			{
//				string url = GetStorageLocation();
//
//				if ( File.Exists( url ) )
//				{
//					XmlSerializer serializer = new XmlSerializer(typeof(CodeFactoryOptions));
//
//					reader = new XmlTextReader( url );
//					object opt = serializer.Deserialize( reader );
//					return (CodeFactoryOptions)opt;
//				}
//				else
//				{
//					return new CodeFactoryOptions();
//				}
//			}
//			catch ( Exception )
//			{
//				//deserialization failed just return default options
//				return new CodeFactoryOptions();
//			}
//			finally
//			{
//				if ( reader != null )
//					reader.Close();
//			}
//		}
//
//		public static void SaveToStorage( CodeFactoryOptions o )
//		{
//			if ( o == null )
//				throw new NullReferenceException();
//
//			XmlSerializer serializer = new XmlSerializer(typeof(CodeFactoryOptions));
//	
//			// Create an XmlTextWriter using a FileStream.
//			Stream fs = new FileStream( GetStorageLocation(), FileMode.Create, FileAccess.Write );
//			XmlWriter writer = new XmlTextWriter( fs, Encoding.UTF8 );
//			// Serialize using the XmlTextWriter.
//			serializer.Serialize( writer, o );
//			writer.Close();
//
//		}
//
//		private static string GetStorageLocation()
//		{
//			string fileName = Environment.GetFolderPath( System.Environment.SpecialFolder.LocalApplicationData );
//			
//			fileName = Path.Combine( fileName, "IT Software Engineering\\VisualStudio\\7.0\\AddIns\\New Class Wizard" );
//			
//			if ( Directory.Exists( fileName ) == false )
//				Directory.CreateDirectory( fileName );
//
//			return Path.Combine( fileName, "CodeOptions.xml" );
//		}
		#endregion
	}
}
