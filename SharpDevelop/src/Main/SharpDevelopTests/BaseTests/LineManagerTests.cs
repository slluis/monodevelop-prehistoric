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
	[TestSuiteAttribute()]
	public class LineManagerTests
	{	
		[TestMethodAttribute()]
		public void TestGetLineNumberForOffsetBug1()
		{
			IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
			
			string testText = "1234567890\n";
			document.TextContent = testText;
			
			int lineNumber = document.GetLineNumberForOffset(testText.Length);
			Assertion.AssertEquals(1, lineNumber);
		}

	}
}
