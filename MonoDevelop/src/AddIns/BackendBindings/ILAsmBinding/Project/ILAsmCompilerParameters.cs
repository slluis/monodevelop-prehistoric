// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;

namespace ILAsmBinding
{
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class ILAsmCompilerParameters : AbstractProjectConfiguration
	{
		CompilerOptions compilerOptions = new CompilerOptions();
		
		[Browsable(false)]
		public CompilerOptions CurrentCompilerOptions {
			get {
				return compilerOptions;
			}
		}
		
		[LocalizedProperty("Output path",
			                   Description = "The path where the assembly is created.")]
		public string OutputPath {
			get {
				return OutputDirectory;
			}
			set {
				OutputDirectory = value;
			}
		}
		
		[LocalizedProperty("Output assembly",
		                   Description = "The assembly name.")]
		public string AssemblyName {
			get {
				return OutputAssembly;
			}
			set {
				OutputAssembly = value;
			}
		}
		
		[DefaultValue(CompilationTarget.Exe)]
		[LocalizedProperty("Compilation Target",
		                   Description = "The compilation target of the source code. (/DLL, /EXE)")]
		public CompilationTarget CompilationTarget {
			get {
				return compilerOptions.compilationTarget;
			}
			set {
				compilerOptions.compilationTarget = value;
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("Include debug information",
		                   Description = "Specifies if debug information should be omited. (/DEBUG)")]
		public bool IncludeDebugInformation {
			get {
				return compilerOptions.includeDebugInformation;
			}
			set {
				compilerOptions.includeDebugInformation = value;
			}
		}
		
		public ILAsmCompilerParameters()
		{
		}
		
		public ILAsmCompilerParameters(string name)
		{
			this.name = name;
		}
		
		[XmlNodeName("CompilerOptions")]
		public class CompilerOptions
		{
			[XmlAttribute("compilationTarget")]
			internal CompilationTarget compilationTarget = CompilationTarget.Exe;
			
			[XmlAttribute("includeDebugInformation")]
			internal bool includeDebugInformation = false;
			
			public string GenerateOptions()
			{
				StringBuilder options = new StringBuilder();
				switch (compilationTarget) {
					//case CompilationTarget.Dll:
					//	options.Append("/DLL ");
					//	break;
					//case CompilationTarget.Exe:
					//	options.Append("/EXE ");
					//	break;
					default:
						throw new System.NotSupportedException("Unsupported compilation target : " + compilationTarget);
				}
				
				if (includeDebugInformation) {
					options.Append("/DEBUG ");
				}
				
				return options.ToString();
			}
		}
	}
}
