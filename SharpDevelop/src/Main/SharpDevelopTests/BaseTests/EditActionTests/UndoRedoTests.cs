// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpUnit;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
/*
[TestSuiteAttribute("Test the undo/redo keys")]
public class UndoRedoTests
{	
	[TestMethodAttribute("Tests if redo clears the selections")]
	public void RedoBug1Test()
	{
		TestEditactionService editActionService = new TestEditactionService();
		string str = "Hello\nWorld!";
		editActionService.Document.TextContent  = str;
		editActionService.Document.Remove(0, str.Length);
		new Undo().Execute(editActionService);
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 0, 3));
		new Redo().Execute(editActionService);
		Assertion.AssertEquals(0, editActionService.Document.SelectionCollection.Count);
	}
	
	[TestMethodAttribute("Tests if undo clears the selections")]
	public void UndoBug1Test()
	{
		TestEditactionService editActionService = new TestEditactionService();
		string str = "Hello\nWorld!";
		editActionService.Document.TextContent  = str;
		editActionService.Document.Insert(0, str);
		
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 0, 3));
		new Undo().Execute(editActionService);
		Assertion.AssertEquals(0, editActionService.Document.SelectionCollection.Count);
	}
	
}
*/
