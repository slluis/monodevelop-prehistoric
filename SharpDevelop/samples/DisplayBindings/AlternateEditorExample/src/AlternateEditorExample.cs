//  AlternateEditorExample.cs 
//  Copyright (C) 2002 Mike Krueger
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

namespace AlternateEditorExample {
	
	public class AlternateEditorExample : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			AlternateEditorContent alternateEditorContent = new AlternateEditorContent();
			alternateEditorContent.LoadFile(fileName);
			return alternateEditorContent;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			AlternateEditorContent alternateEditorContent = new AlternateEditorContent();
			return alternateEditorContent;
		}
	}
}
