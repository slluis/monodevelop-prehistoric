// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Reflection;

using ICSharpCode.SharpUnit;
using SharpDevelop.Internal.Parser;

namespace SharpDevelopTests
{ 
	[TestSuiteAttribute()]
	public class DefaultRegionTests
	{
		[TestMethodAttribute()]
		public void SimpleRegionTest()
		{
			DefaultRegion region = new DefaultRegion(10, 10, 15, 15);
			for (int x = 0; x < 10; ++x) {
				Assertion.AssertEquals(false, region.IsInside(10, x));
			}
			for (int x = 10; x < 100; ++x) {
				Assertion.AssertEquals(true, region.IsInside(10, x));
			}
			
			for (int x = 0; x < 100; ++x) {
				Assertion.AssertEquals(true, region.IsInside(14, x));
			}
			
			for (int x = 0; x <= 15; ++x) {
				Assertion.AssertEquals(true, region.IsInside(15, x));
			}
			
			for (int x = 16; x < 100; ++x) {
				Assertion.AssertEquals(false, region.IsInside(15, x));
			}
		}
		
		[TestMethodAttribute()]
		public void UnknownLineEndTest()
		{
			DefaultRegion region = new DefaultRegion(10, 0, -1, 5);
			int y = 0;
			for (int x = 0; x < 10; ++x) {
				Assertion.AssertEquals(false, region.IsInside(x, ++y));
			}
			for (int x = 10; x < 100; ++x) {
				Assertion.AssertEquals(true, region.IsInside(x, ++y));
			}
		}
		
	}
}
