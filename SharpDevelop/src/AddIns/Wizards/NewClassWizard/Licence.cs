// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace NewClassWizard
{
	public class License {
		
		public readonly string Name = 	String.Empty;
		
		private string _text = 			String.Empty;
		private const string EMPTY = 	"<none>";
		
		public License(string name, string text) {
			Name = name;
			_text = text;
		}
		
		public override string ToString() {
			return Name;
		}	
		
		public string Text {
			get {
				//strip out leading comment characters in license headers
				return _text.Replace( "\n//", "\n" );
			}
		}
		
		public static License Empty {
			get {
				return new License( EMPTY, String.Empty );
			}
		}
		
		public static bool IsEmpty( License license ) {
			return ( license.Name == EMPTY );
		}
	}
}
