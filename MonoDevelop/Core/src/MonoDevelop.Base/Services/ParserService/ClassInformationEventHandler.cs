// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using MonoDevelop.Core.AddIns;

using MonoDevelop.Internal.Parser;
using MonoDevelop.Internal.Project;

using MonoDevelop.Gui;

namespace MonoDevelop.Services
{
	public delegate void ClassInformationEventHandler(object sender, ClassInformationEventArgs e);
	
	public class ClassInformationEventArgs : EventArgs
	{
		string fileName;
		ClassUpdateInformation classInformation;
				
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public ClassUpdateInformation ClassInformation {
			get {
				return classInformation;
			}
		}
		
		public ClassInformationEventArgs(string fileName, ClassUpdateInformation classInformation)
		{
			this.fileName         = fileName;
			this.classInformation = classInformation;
		}
	}
}
