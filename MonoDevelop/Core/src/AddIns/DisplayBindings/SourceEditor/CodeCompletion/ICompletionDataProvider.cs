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
using MonoDevelop.Internal.Project;

using Gdk;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.SourceEditor.CodeCompletion {
	public interface ICompletionDataProvider {
		ICompletionData[] GenerateCompletionData(IProject project, string fileName, SourceEditorView textArea, char charTyped, Gtk.TextMark mark);
	}
}
