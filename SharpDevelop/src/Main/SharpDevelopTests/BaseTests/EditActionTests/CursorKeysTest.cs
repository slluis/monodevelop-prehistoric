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
[TestSuiteAttribute("Test the CursorKeys")]
public class CaretLeftTests
{	
	[TestMethodAttribute("Caret at start of Dokument")]
	public void CaretLeftTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 0;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretLeft());
		
		Assertion.AssertEquals(0, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at start of Line")]
	public void CaretLeftTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 6;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretLeft());
		
		Assertion.AssertEquals(5, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(5, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret behind tab")]
	public void CaretLeftTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWo\trld!";
		LineSegment line = editActionService.Document.GetLineSegment(1);
		int col = editActionService.Document.GetViewXPos(line, 3);
		editActionService.Document.Caret.Offset = 9;
		editActionService.Document.Caret.DesiredColumn = col;
		
		editActionService.ExecuteEditAction(new CaretLeft());
		
		Assertion.AssertEquals(8, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(2, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void CaretLeftTest4()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 4;
		editActionService.Document.Caret.DesiredColumn = 6;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new CaretLeft());
		
		Assertion.AssertEquals(3, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(3, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	[TestMethodAttribute("Caret at end of Dokument (with end of line at the end of the document)")]
	public void CaretRightBug1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!\n";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength - 1;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretRight());
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	
	[TestMethodAttribute("Caret at end of Dokument")]
	public void CaretRightTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = editActionService.Document.TextLength;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretRight());
		
		LineSegment line = editActionService.Document.GetLineSegment(editActionService.Document.TotalNumberOfLines - 1);
		Assertion.AssertEquals(editActionService.Document.TextLength, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(line.Length, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret at end of Line")]
	public void CaretRightTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 5;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretRight());
		
		Assertion.AssertEquals(6, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(0, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Caret before tab")]
	public void CaretRightTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWo\trld!";
		editActionService.Document.Caret.Offset = 8;
		editActionService.Document.Caret.DesiredColumn = 8;
		
		editActionService.ExecuteEditAction(new CaretRight());
		
		LineSegment line = editActionService.Document.GetLineSegment(1);
		Assertion.AssertEquals(9, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(editActionService.Document.GetViewXPos(line, 3), editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void CaretRightTest4()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 3;
		editActionService.Document.Caret.DesiredColumn = 6;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new CaretRight());
		
		Assertion.AssertEquals(4, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(4, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	
	
	
	[TestMethodAttribute("Caret in first line")]
	public void CaretUpTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 4;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretUp());
		
		Assertion.AssertEquals(4, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(6, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Test with more tabs in upper line")]
	public void CaretUpTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "\tHello\nWorld!";
		editActionService.Document.Caret.Offset = 11;
		editActionService.Document.Caret.DesiredColumn = 3;
		
		editActionService.ExecuteEditAction(new CaretUp());
		
		LineSegment line = editActionService.Document.GetLineSegment(0);
		Assertion.AssertEquals(editActionService.Document.GetLogicalXPos(line, 3), editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(3, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Test with more tabs in lower line")]
	public void CaretUpTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\n\tWorld!";
		LineSegment line = editActionService.Document.GetLineSegment(1);
		int col = editActionService.Document.GetViewXPos(line, 2);
		editActionService.Document.Caret.Offset = 8;
		editActionService.Document.Caret.DesiredColumn = col;
		
		editActionService.ExecuteEditAction(new CaretUp());
		
		line = editActionService.Document.GetLineSegment(0);
		Assertion.AssertEquals(editActionService.Document.GetLogicalXPos(line, col), editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(col, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void CaretUpTest4()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 11;
		editActionService.Document.Caret.DesiredColumn = 3;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new CaretUp());
		
		LineSegment line = editActionService.Document.GetLineSegment(0);
		Assertion.AssertEquals(editActionService.Document.GetLogicalXPos(line, 3), editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(3, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
	
	
	[TestMethodAttribute("Caret in last line")]
	public void CaretDownTest1()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 9;
		editActionService.Document.Caret.DesiredColumn = 6;
		
		editActionService.ExecuteEditAction(new CaretDown());
		
		Assertion.AssertEquals(9, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(6, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Test with more tabs in upper line")]
	public void CaretDownTest2()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "\tHello\nWorld!srtuyhjdghujrt";
		LineSegment line = editActionService.Document.GetLineSegment(0);
		int col = editActionService.Document.GetViewXPos(line, 3);
		editActionService.Document.Caret.Offset = 3;
		editActionService.Document.Caret.DesiredColumn = col;
		
		editActionService.ExecuteEditAction(new CaretDown());
		
		line = editActionService.Document.GetLineSegment(1);
		Assertion.AssertEquals(editActionService.Document.GetLogicalXPos(line, col) + line.Offset, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(col, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Test with more tabs in lower line")]
	public void CaretDownTest3()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\n\tWorld!";
		editActionService.Document.Caret.Offset = 4;
		editActionService.Document.Caret.DesiredColumn = 4;
		
		editActionService.ExecuteEditAction(new CaretDown());
		
		LineSegment line = editActionService.Document.GetLineSegment(1);
		Assertion.AssertEquals(editActionService.Document.GetLogicalXPos(line, 4) + line.Offset, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(4, editActionService.Document.Caret.DesiredColumn);
	}
	
	[TestMethodAttribute("Text selected")]
	public void CaretDownTest4()
	{
		TestEditactionService editActionService = new TestEditactionService();
		editActionService.Document.TextContent  = "Hello\nWorld!";
		editActionService.Document.Caret.Offset = 3;
		editActionService.Document.Caret.DesiredColumn = 3;
		editActionService.Document.SelectionCollection.Add(new DefaultSelection(editActionService.Document, 3, 5));
		
		editActionService.ExecuteEditAction(new CaretDown());
		
		Assertion.AssertEquals(9, editActionService.Document.Caret.Offset);
		Assertion.AssertEquals(3, editActionService.Document.Caret.DesiredColumn);
		Assertion.Assert(!editActionService.HasSomethingSelected);
	}
}
*/
