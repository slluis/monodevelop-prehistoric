// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;

using ICSharpCode.SharpUnit;

using ICSharpCode.TextEditor.Document;

namespace SharpDevelopTests
{ 
	[TestSuiteAttribute("Test the IDocumentAggregator interface")]
	public class DocumentTestSuite
	{
		[TestMethodAttribute()]
		public void TestDocumentGenerationTest()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
		}
		
		[TestMethodAttribute()]
		public void TestDocumentStoreTest()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string testText = "1234567890\n" +
			"12345678\n" +
			"1234567\n" +
			"123456\n" +
			"12345\n" +
			"1234\n" +
			"123\n" +
			"12\n" +
			"1\n" +
			"\n";
			document.TextContent = testText;
			
			Assertion.AssertEquals(testText, document.TextContent);
			Assertion.AssertEquals(11, document.TotalNumberOfLines);
			Assertion.AssertEquals(testText.Length, document.TextLength);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentInsertTest()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string top      = "1234567890\n";
			string testText =
			"12345678\n" +
			"1234567\n" +
			"123456\n" +
			"12345\n" +
			"1234\n" +
			"123\n" +
			"12\n" +
			"1\n" +
			"\n";
			
			document.TextContent = top;
			document.Insert(top.Length, testText);
			Assertion.AssertEquals(top + testText, document.TextContent);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentRemoveStoreTest()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string top      = "1234567890\n";
			string testText =
			"12345678\n" +
			"1234567\n" +
			"123456\n" +
			"12345\n" +
			"1234\n" +
			"123\n" +
			"12\n" +
			"1\n" +
			"\n";
			document.TextContent = top + testText;
			document.Remove(0, top.Length);
			Assertion.AssertEquals(document.TextContent, testText);
			
			document.Remove(0, document.TextLength);
			LineSegment line = document.GetLineSegment(0);
			Assertion.AssertEquals(0, line.Offset);
			Assertion.AssertEquals(0, line.Length);
			Assertion.AssertEquals(0, document.TextLength);
			Assertion.AssertEquals(1, document.TotalNumberOfLines);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentBug1Test()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string top      = "1234567890";
			document.TextContent = top;
			
			Assertion.AssertEquals(document.GetLineSegment(0).Length, document.TextLength);
			
			document.Remove(0, document.TextLength);
			
			LineSegment line = document.GetLineSegment(0);
			Assertion.AssertEquals(0, line.Offset);
			Assertion.AssertEquals(0, line.Length);
			Assertion.AssertEquals(0, document.TextLength);
			Assertion.AssertEquals(1, document.TotalNumberOfLines);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentBug2Test()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string top      = "123\n456\n789\n0";
			string testText = "Hello World!";
			
			document.TextContent = top;
			
			document.Insert(top.Length, testText);
			
			LineSegment line = document.GetLineSegment(document.TotalNumberOfLines - 1);
			
			Assertion.AssertEquals(top.Length - 1, line.Offset);
			Assertion.AssertEquals(testText.Length + 1, line.Length);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentBug3Test()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string testText = "123\r\n";
			
			for (int i = 0; i < 5; ++i) {
				document.Insert(document.TextLength, testText);
			}
			
			document.Caret.Offset = testText.Length * 2 + 1;
			document.Remove(testText.Length * 2, 2);
			
			Assertion.AssertEquals(testText.Length * 2, document.Caret.Offset);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentBug4Test()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string testText = "123\r\n";
			
			for (int i = 0; i < 5; ++i) {
				document.Insert(document.TextLength, testText);
			}
			
			document.Caret.Offset = testText.Length * 2 + 1;
			document.Replace(testText.Length * 2, 2, "");
			
			Assertion.AssertEquals(testText.Length * 2, document.Caret.Offset);
		}
		
		[TestMethodAttribute()]
		public void TestDocumentBug5Test()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			for (int i = 3; i <= 5; ++i) {
				document.TextContent  = "abcdefgh";
				document.Caret.Offset = i;
				
				document.Replace(2, 3, "Hello");
				
				Assertion.AssertEquals(i, document.Caret.Offset);
			}
		}
	}
}
