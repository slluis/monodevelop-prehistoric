// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpUnit;

namespace SharpDevelopTests
{ 
	/// <summary>
	/// This class contains test cases for the file utility class of SharpDevelop. 
	/// </summary>
	[TestSuiteAttribute("Test the FileUtility class")]
	public class FileUtilityTestSuite
	{
		FileUtilityService fileUtilityService = new FileUtilityService();
		
		[TestMethodAttribute("Tests AbsoluteToRelativePath method.")]
		public void TestAbsoluteToRelativePathTest() 
		{
			Assertion.AssertEquals(@".\b", fileUtilityService.AbsoluteToRelativePath(@"C:\test\a", @"C:\test\a\b"));
			Assertion.AssertEquals(@"..\b", fileUtilityService.AbsoluteToRelativePath(@"C:\test\a", @"C:\test\b"));
			Assertion.AssertEquals(@"D:\b", fileUtilityService.AbsoluteToRelativePath(@"C:\test\a", @"D:\b"));
			Assertion.AssertEquals(@".\testme.exe", fileUtilityService.AbsoluteToRelativePath(@"C:\test\a", @"C:\test\a\testme.exe"));
		}
		
		[TestMethodAttribute("Tests RelativeToAbsolutePath method.")]
		public void TestRelativeToAbsolutePathTest() 
		{
			Assertion.AssertEquals(@"C:\test\a\b", fileUtilityService.RelativeToAbsolutePath(@"C:\test\a", @".\b"));
			Assertion.AssertEquals(@"C:\test\b", fileUtilityService.RelativeToAbsolutePath(@"C:\test\a", @"..\b"));
			Assertion.AssertEquals(@"D:\b", fileUtilityService.RelativeToAbsolutePath(@"C:\test\a", @"D:\b"));
			Assertion.AssertEquals(@"C:\test\a\testme.exe", fileUtilityService.RelativeToAbsolutePath(@"C:\test\a", @".\testme.exe"));
		}
	}
}
