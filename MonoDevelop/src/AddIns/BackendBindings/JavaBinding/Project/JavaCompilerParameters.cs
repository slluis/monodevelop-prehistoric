// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;

using MonoDevelop.Internal.Project;

namespace JavaBinding
{
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class JavaCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		class CodeGeneration 
		{
			[XmlAttribute("includedebuginformation")]
			public bool debugmode          = true;
			
			[XmlAttribute("deprecation")]
			public bool deprecation        = true;
			
			[XmlAttribute("optimize")]
			public bool optimize           = true;
			
			[XmlAttribute("mainclass")]
			public string         mainclass     = null;
			
			[XmlAttribute("definesymbols")]
			public string         definesymbols = String.Empty;
			
			[XmlAttribute("classpath")]
			public string         classpath = String.Empty;
			
			[XmlAttribute("compilerpath")]
			public string         compilerpath  = "javac.exe";		
			
			[XmlAttribute("genwarnings")]
			public bool genwarnings = false;
		}
		
		[XmlNodeName("Execution")]
		class Execution
		{
			[XmlAttribute("consolepause")]
			public bool    pauseconsoleoutput = true;
		}
		
		CodeGeneration codeGeneration = new CodeGeneration();
		
		Execution      execution      = new Execution();
		
		public bool GenWarnings {
			get {
				return codeGeneration.genwarnings;
			}
			set {
				codeGeneration.genwarnings = value;
			}
		}
		
		public string ClassPath {
			get {
				return codeGeneration.classpath;
			}
			set {
				codeGeneration.classpath = value;
			}
		}
		
		public string CompilerPath {
			get {
				return codeGeneration.compilerpath;
			}
			set {
				codeGeneration.compilerpath = value;
			}
		}
		
		public bool Debugmode {
			get {
				return codeGeneration.debugmode;
			}
			set {
				codeGeneration.debugmode = value;
			}
		}
		
		public bool Deprecation {
			get {
				return codeGeneration.deprecation;
			}
			set {
				codeGeneration.deprecation = value;
			}
		}
		
		public bool Optimize {
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		public string MainClass {
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		public bool PauseConsoleOutput {
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}
		
		public JavaCompilerParameters()
		{
		}
		public JavaCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
