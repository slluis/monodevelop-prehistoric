// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Undo;
using System.Drawing.Printing;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return fileName.StartsWith("http") || fileName.StartsWith("ftp");
		}

		public bool CanCreateContentForMimeType (string mimetype)
		{
			switch (mimetype) {
				case "text/html":
					return true;
				default:
					return false;
			}
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			BrowserPane browserPane = new BrowserPane();
			browserPane.Load (fileName);
			return browserPane;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content, string new_file_name)
		{
			return null;
		}
	}
}
