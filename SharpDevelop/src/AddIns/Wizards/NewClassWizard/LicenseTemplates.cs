// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.Templates;

namespace NewClassWizard {
	
	public class LicenseTemplates  {
		
		private static TextTemplate licenseTemplates;
		private static bool hasLicense;
		
		static LicenseTemplates() {
			hasLicense = false;
			foreach( TextTemplate t in TextTemplate.TextTemplates ) {
				if ( t.Name == "Licenses" ) {
					licenseTemplates = t;
					hasLicense = true;
					break;
				}
			}
		}
		
		public static License GetLicense( string lincenseID ) {
			//need to come up with a better search - probably replace
			//the ArrayList with a HashTable but this will do for now
			//with a small number of licenses - dpk
			if (hasLicense) {
				foreach (TextTemplate.Entry entry in licenseTemplates.Entries) {
					if (entry.Display == lincenseID) {
						return new License( entry.Display, entry.Value );
					}
				}
			}
			return License.Empty;
		}
		
		public static ArrayList Licenses {
			get {
				ArrayList a = new ArrayList();
				foreach ( TextTemplate.Entry entry in licenseTemplates.Entries ) {
					a.Add( new License( entry.Display, entry.Value ) );
				}
				return a;
			}
		}		
	}
	
}
