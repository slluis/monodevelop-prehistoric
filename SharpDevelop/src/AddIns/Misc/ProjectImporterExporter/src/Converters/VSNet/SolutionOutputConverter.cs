// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionOutputConverter : AbstractOutputConverter
	{
		public override string FormatName {
			get {
				return "Visual Studio.NET 2003 Solutions";
			}
		}
		
		public override void ConvertCombine(string inputCombine, string outputPath)
		{
			Console.WriteLine("Convert combine: " + inputCombine);
		}
		
		public override void ConvertProject(string inputProject, string outputPath)
		{
			Console.WriteLine("Convert project: " + inputProject);
		}
	}
}
