// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpUnit;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;

[TestSuiteAttribute()]
public class TextIteratorTests
{
	[TestMethodAttribute()]
	public void TestForwardTextIterator()
	{
		ITextBufferStrategy textBuffer = new GapTextBufferStrategy();
		string content = "1234567890";
		textBuffer.SetContent(content);
		for (int endOffset = 0; endOffset < content.Length; ++endOffset) {
			ITextIterator iterator = new ForwardTextIterator(textBuffer, endOffset);
			
			// test if the iterator throws an exception before MoveAhead is called
			try {
				int curIter  = iterator.Current;
				Assertion.Fail();
			} catch (Exception) {
			}
			
			// test if the iterator returns the expected results
			for (int i = 0; i < content.Length; ++i) {
				Assertion.AssertEquals(true, iterator.MoveAhead(1));
				Assertion.AssertEquals(content[(i + endOffset) % content.Length], iterator.Current);
			}
			
			// test if the iterator can move forward if it reached the end.
			Assertion.AssertEquals(false, iterator.MoveAhead(1));
			Assertion.AssertEquals(false, iterator.MoveAhead(2));
			Assertion.AssertEquals(false, iterator.MoveAhead(100));
			
			// test if the iterator throws an exception after it readched the end
			try {
				int curIter  = iterator.Current;
				Assertion.Fail();
			} catch (Exception) {
			}
		}
	}
}
