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

using MonoDevelop.Gui.Utils;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserDisplayBinding : IDisplayBinding, ISecondaryDisplayBinding
	{

		public bool CanCreateContentForFile(string fileName)
		{
			return fileName.StartsWith("http") || fileName.StartsWith("ftp");
		}

		public bool CanCreateContentForMimeType (string mimetype)
		{
			/*switch (mimetype) {
				case "text/html":
					return true;
				default:
					return false;
			}*/
			return false;
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

		public bool CanAttachTo (IViewContent parent)
		{
			string filename = parent.ContentName;
			string mimetype = Vfs.GetMimeType (filename);
			if (mimetype == "text/html")
				return true;
			return false;
		}

		public ISecondaryViewContent CreateSecondaryViewContent (IViewContent parent)
		{
			return new BrowserPane (false, parent);
		}
	}
}
