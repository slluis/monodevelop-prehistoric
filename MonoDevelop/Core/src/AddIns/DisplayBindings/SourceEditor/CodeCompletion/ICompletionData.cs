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

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.SourceEditor.CodeCompletion
{
	public interface ICompletionData
	{
		string Image {
			get;
		}
		
		string[] Text {
			get;
		}
		
		string Description {
			get;
		}

		string CompletionString 
		{
			get;
		}
		
		void InsertAction(SourceEditorView control);
	}
	
	public interface ICompletionDataWithMarkup : ICompletionData
	{
		string DescriptionPango {
			get;
		}
	}
}
