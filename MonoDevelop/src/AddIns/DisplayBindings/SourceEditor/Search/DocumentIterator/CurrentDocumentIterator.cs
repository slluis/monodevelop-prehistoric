// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

using MonoDevelop.SourceEditor.Gui;

namespace ICSharpCode.TextEditor.Document
{
	public class CurrentDocumentIterator : IDocumentIterator
	{
		bool         didRead = false;
		SourceEditor curDocument = null;
		
		public CurrentDocumentIterator() 
		{
			Reset();
		}
			
		public string CurrentFileName {
			get {
				if (!SearchReplaceUtilities.IsTextAreaSelected) {
					return null;
				}
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName == null) {
					return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.UntitledName;
				}
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
			}
		}
		
		public ProvidedDocumentInformation Current {
			get {
				if (!SearchReplaceUtilities.IsTextAreaSelected) {
					return null;
				}
				curDocument = ((SourceEditor) ((SourceEditorDisplayBindingWrapper)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).Control);
				return new ProvidedDocumentInformation(curDocument, CurrentFileName);
			}
		}
			
		public bool MoveForward() 
		{
			if (!SearchReplaceUtilities.IsTextAreaSelected) {
				return false;
			}
			if (didRead) {
				return false;
			}
			didRead = true;
			
			return true;
		}
		
		public bool MoveBackward()
		{
			return MoveForward();
		}
		
		public void Reset() 
		{
			didRead = false;
		}
	}
}