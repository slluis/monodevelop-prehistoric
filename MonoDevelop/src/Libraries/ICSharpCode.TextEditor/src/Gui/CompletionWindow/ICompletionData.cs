// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public interface ICompletionData
	{
		int ImageIndex {
			get;
		}
		
		string[] Text {
			get;
		}
		
		string Description {
			get;
		}
		
		void InsertAction(TextEditorControl control);
	}
	
	public interface ICompletionDataWithMarkup : ICompletionData
	{
		string DescriptionPango {
			get;
		}
	}
}
