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
	public unsafe class ReflectionReturnTypeTests
	{
		[TestMethodAttribute()]
		public void ReflectionReturnTypeArrayDimensionsTest1()
		{
			IReturnType retType = new ReflectionReturnType(typeof(int[][,][,,,]));
			string str1 = String.Empty;
			foreach (int i in retType.ArrayDimensions) {
				str1 += i + ",";
			}
			
			string str2 = "1,2,4,";
			Assertion.AssertEquals(str2, str1);
		}
		
		[TestMethodAttribute()]
		public void ReflectionReturnTypeArrayDimensionsTest2()
		{
			IReturnType retType = new ReflectionReturnType(typeof(int));
			Assertion.AssertEquals(0, retType.PointerNestingLevel);
		}
		
		[TestMethodAttribute()]
		public unsafe void ReflectionReturnTypePointerNestingLevelTest()
		{
			IReturnType retType = new ReflectionReturnType(typeof(int***));
			Assertion.AssertEquals(3, retType.PointerNestingLevel);
		}
		[TestMethodAttribute()]
		public void ReflectionReturnTypeTest1()
		{
			IReturnType retType = new ReflectionReturnType(typeof(int[]));
			Assertion.AssertEquals(1, retType.ArrayCount);
		}
		
	}
}
