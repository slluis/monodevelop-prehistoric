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

using ICSharpCode.TextEditor.Document;

using Gdk;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public interface ICompletionDataProvider
	{
		Pixbuf[] ImageList {
			get;
		}
		
		ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
	}
}
