// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace JScriptBinding
{
	/// <summary>
	///
	/// </summary>
	public enum CompileTarget {
		/// <summary></summary>
		Exe,
		/// <summary></summary>
		Library
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class JScriptCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		class CodeGeneration 
		{
			[XmlAttribute("includedebuginformation")]
			public bool debugmode          = true;
			
			[XmlAttribute("gernerateoverflowchecks")]
			public bool genoverflow          = true;
			
			[XmlAttribute("warninglevel")]
			public int  warninglevel        = 4;
			
			[XmlAttribute("optimize")]
			public bool optimize           = true;
			
			[XmlAttribute("mainclass")]
			public string         mainclass     = null;
			
			[XmlAttribute("definesymbols")]
			public string         definesymbols = String.Empty;
			
			[XmlAttribute("target")]
			public 
			CompileTarget  compiletarget = CompileTarget.Exe;
			
			[XmlAttribute("genwarnings")]
			public bool genwarnings = false;
			
			[XmlAttribute("unsafecodeallowed")]
			public bool unsafecode = false;
		}
		
		[XmlNodeName("Execution")]
		class Execution
		{
			[XmlAttribute("consolepause")]
			public bool    pauseconsoleoutput = true;
		}
		
		CodeGeneration codeGeneration = new CodeGeneration();
		
		Execution      execution      = new Execution();
		
		
		/// <summary>
		/// 	DefineSymbols
		/// </summary>
		
		public string DefineSymbols {
			get {
				return codeGeneration.definesymbols;
			}
			set {
				codeGeneration.definesymbols = value;
			}
		}
		
		/// <summary>
		/// 	WarningLevel
		/// </summary>
		
		public int WarningLevel {
			get {
				return codeGeneration.warninglevel;
			}
			set {
				codeGeneration.warninglevel = value;
			}
		}
		
		/// <summary>
		/// 	PauseConsoleOutput
		/// </summary>
		
		public bool PauseConsoleOutput {
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}

		public bool GenerateOverflowChecks {
			get {
				return codeGeneration.genoverflow;
			}
			set {
				codeGeneration.genoverflow = value;
			}
		}
		
		public bool UnsafeCode {
			get {
				return codeGeneration.unsafecode;
			}
			set {
				codeGeneration.unsafecode = value;
			}
		}
		
		public bool GenerateXmlDocumentation {
			get {
				return false;
			}
			set {
				
			}
		}

		/// <summary>
		/// 	DebugMode
		/// </summary>
		
		
		public bool Debugmode {
			get {
				return codeGeneration.debugmode;
			}
			set {
				codeGeneration.debugmode = value;
			}
		}
		
		/// <summary>
		/// 	Optimize
		/// </summary>
		
		public bool Optimize {
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		/// <summary>
		/// 	MainClass
		/// </summary>
		
		public string MainClass {
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		/// <summary>
		/// 	CompilerTarget
		/// </summary>
		
		public CompileTarget CompileTarget {
			get {
				return codeGeneration.compiletarget;
			}
			set {
				codeGeneration.compiletarget = value;
			}
		}
		
		/// <summary>
		///
		/// </summary>
		
		public JScriptCompilerParameters()
		{
		}
		
		/// <summary>
		///
		/// </summary>
		
		public JScriptCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
