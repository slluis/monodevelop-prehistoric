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
[TestSuiteAttribute("Test the home and end key")]
public class HomeEndKeyTests
{	
	[TestMethodAttribute("Caret at end of Dokument")]
	public void MoveToStartTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		new MoveToStart().Execute(editActionService);
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at start of dokument")]
	public void MoveToStartTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 0;
		editActionService.Document.Caret.DesiredColumn = 0;
		
		new MoveToStart().Execute(editActionService);
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void MoveToStartTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 5;
		editActionService.Document.Caret.DesiredColumn = 2;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new MoveToStart());
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	
	[TestMethodAttribute("Caret at end of Dokument")]
	public void MoveToEndTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		new MoveToEnd().Execute(editActionService);
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at start of dokument")]
	public void MoveToEndTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 0;
		editActionService.Document.Caret.DesiredColumn = 0;
		
		new MoveToEnd().Execute(editActionService);
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void MoveToEndTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 5;
		editActionService.Document.Caret.DesiredColumn = 2;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new MoveToEnd());
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	
	[TestMethodAttribute("Caret at end of Dokument")]
	public void HomeTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new Home());
		
		Assertion.AssertEquals(6, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at start of dokument")]
	public void HomeTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 0;
		editActionService.Document.Caret.DesiredColumn = 0;
		
		editActionService.ExecuteEditAction(new Home());
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void HomeTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 3;
		editActionService.Document.Caret.DesiredColumn = 6;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new Home());
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	
	[TestMethodAttribute("Caret at end of Dokument")]
	public void EndTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new End());
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at start of dokument")]
	public void EndTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 0;
		editActionService.Document.Caret.DesiredColumn = 0;
		
		editActionService.ExecuteEditAction(new End());
		
		LineSegment line = editActionService.Document.GetLineSegment(0);
		Assertion.AssertEquals(line.Offset + line.Length, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void EndTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 9;
		editActionService.Document.Caret.DesiredColumn = 6;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new End());
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
}
*/
