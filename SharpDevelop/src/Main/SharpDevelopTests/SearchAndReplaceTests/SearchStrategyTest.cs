// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpUnit;
using ICSharpCode.TextEditor.Document;

[TestSuiteAttribute()]
public class SearchStrategyTests
{
	void TestStrategy(ISearchStrategy searchStrategy)
	{
		IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
		
		string testText = "1234567890\n" +
		                  "123456789\n" +
		                  "12345678\n" +
		                  "1234567\n" +
		                  "123456\n" +
		                  "12345\n" +
		                  "123n\n" +
		                  "12\n" +
		                  "1\n" +
		                  "urrentOffs frefr ergf roeihore reuhfreu rehffrh FOKew(q ew jfew (ergre )()GREgrggre" + 
		                  "hallo";
		document.TextContent = testText;
		
		SearchOptions options = new SearchOptions(new DefaultProperties());
		options.SearchPattern = "123n";
		searchStrategy.CompilePattern(options);
		ISearchResult location = searchStrategy.FindNext(new ForwardTextIterator(document.TextBufferStrategy, 0), options);
		Assertion.AssertEquals(51, location.Offset);
		
		options.SearchPattern = "12";
		searchStrategy.CompilePattern(options);
		location = searchStrategy.FindNext(new ForwardTextIterator(document.TextBufferStrategy, location.Offset + location.Length), options);
		Assertion.AssertEquals(56, location.Offset);
		
		options.SearchPattern = "frefre";
		options.IgnoreCase = true;
		searchStrategy.CompilePattern(options);
		location = searchStrategy.FindNext(new ForwardTextIterator(document.TextBufferStrategy, 0), options);
		Assertion.AssertNull("Search strategy found ghost pattern", location);
		
		options.SearchPattern = "n";
		searchStrategy.CompilePattern(options);
		location = searchStrategy.FindNext(new ForwardTextIterator(document.TextBufferStrategy, 0), options);
		Assertion.AssertEquals(54, location.Offset);
		
		options.SearchPattern = "hallo12345";
		searchStrategy.CompilePattern(options);
		location = searchStrategy.FindNext(new ForwardTextIterator(document.TextBufferStrategy, 10), options);
		Assertion.AssertNull("Search strategy didn't recognize file end", location);
	}
	
	[TestMethodAttribute()]
	public void TestBasicKMPSearch()
	{
		TestStrategy(new KMPSearchStrategy());
	}
	
	[TestMethodAttribute()]
	public void TestBasicWildcardSearch()
	{
		TestStrategy(new WildcardSearchStrategy());
	}
	
	[TestMethodAttribute()]
	public void TestBasicRegExSearch()
	{
		TestStrategy(new RegExSearchStrategy());
	}
	
	[TestMethodAttribute()]
	public void TestBasicBruteForceSearch()
	{
		TestStrategy(new BruteForceSearchStrategy());
	}

}
