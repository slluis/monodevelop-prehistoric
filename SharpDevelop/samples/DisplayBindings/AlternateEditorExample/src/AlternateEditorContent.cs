//  AlternateEditorContent.cs 
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

namespace AlternateEditorExample {
	
	public class AlternateEditorContent : AbstractViewContent
	{
		RichTextBox rtb = new RichTextBox();
		
		public override Control Control {
			get {
				return rtb;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return rtb.ReadOnly;
			}
		}
		
		public AlternateEditorContent()
		{
			rtb.Dock = DockStyle.Fill;
		}
		
		public override void RedrawContent()
		{
			rtb.Refresh();
		}
		
		public override void Dispose()
		{
			rtb.Dispose();
		}
		
		public override void SaveFile(string fileName)
		{
			rtb.SaveFile(fileName, RichTextBoxStreamType.PlainText);
			ContentName = fileName;
			IsDirty     = false;
		}
		
		public override void LoadFile(string fileName)
		{
			rtb.LoadFile(fileName, RichTextBoxStreamType.PlainText);
			ContentName = fileName;
			IsDirty     = false;
		}
	}
}
