// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

using MonoDevelop.Internal.Project;

namespace VBBinding {
	
	public enum CompileTarget
	{
		Exe,
		WinExe,
		Library,
		Module
	};
	
	public enum VBCompiler {
		Vbc,
		Mbas
	};
	
	public enum NetRuntime {
		Mono,
		MonoInterpreter,
		MsNet
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class VBCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		class CodeGeneration 
		{
			[XmlAttribute("compilerversion")]
			public string vbCompilerVersion = String.Empty;
			
			[XmlAttribute("runtime")]
			public NetRuntime netRuntime         = NetRuntime.Mono;
			
			[XmlAttribute("compiler")]
			public VBCompiler vbCompiler = VBCompiler.Mbas;
			
			[XmlAttribute("warninglevel")]
			public int  warninglevel       = 4;
			
			[XmlAttribute("nowarn")]
			public string noWarnings      = String.Empty;
			
			//[XmlAttribute("includedebuginformation")]
			//public bool debugmode = false;
			
			[XmlAttribute("optimize")]
			public bool optimize = true;
			
			[XmlAttribute("unsafecodeallowed")]
			public bool unsafecode         = false;
			
			[XmlAttribute("generateoverflowchecks")]
			public bool generateOverflowChecks = true;
			
			[XmlAttribute("rootnamespace")]
			public string rootnamespace = String.Empty;
			
			[XmlAttribute("mainclass")]
			public string mainclass = null;
			
			[XmlAttribute("target")]
			public CompileTarget  compiletarget = CompileTarget.Exe;
			
			[XmlAttribute("definesymbols")]
			public string definesymbols = String.Empty;
			
			[XmlAttribute("generatexmldocumentation")]
			public bool generateXmlDocumentation = false;
			
			[XmlAttribute("optionexplicit")]
			public bool optionExplicit = true;
			
			[XmlAttribute("optionstrict")]
			public bool optionStrict = false;
			
			[ConvertToRelativePathAttribute()]
			[XmlAttribute("win32Icon")]
			public string win32Icon = String.Empty;
			
			[XmlAttribute("imports")]
			public string imports = String.Empty;
		}
		
		[XmlNodeName("Execution")]
		class Execution
		{
			[XmlAttribute("consolepause")]
			public bool pauseconsoleoutput = true;
			
			[XmlAttribute("commandlineparameters")]
			public string commandLineParameters = String.Empty;
			
		}
		
		[XmlNodeName("VBDOC")]
		class VBDOC
		{
			[XmlAttribute("outputfile")]
			[ConvertToRelativePathAttribute()]
			public string outputfile = String.Empty;
			
			[XmlAttribute("filestoparse")]
			public string filestoparse = String.Empty;
			
			[XmlAttribute("commentprefix")]
			public string commentprefix = "'";
		}
		
		CodeGeneration codeGeneration = new CodeGeneration();
		VBDOC		   vbdoc		  = new VBDOC();
		Execution      execution      = new Execution();
		
		[Browsable(false)]
		public string VBCompilerVersion
		{
			get {
				return codeGeneration.vbCompilerVersion;
			}
			set {
				codeGeneration.vbCompilerVersion = value;
			}
		} 
		
		[Browsable(false)]
		public VBCompiler VBCompiler {
			get {
				return codeGeneration.vbCompiler;
			}
			set {
				codeGeneration.vbCompiler = value;
			}
		}
		
		[Browsable(false)]
		public NetRuntime NetRuntime {
			get {
				return codeGeneration.netRuntime;
			}
			set {
				codeGeneration.netRuntime = value;
			}
		}
		
		public string CommandLineParameters
		{
			get {
				return execution.commandLineParameters;
			}
			set {
				execution.commandLineParameters = value;
			}
		}
		public bool GenerateOverflowChecks
		{
			get {
				return codeGeneration.generateOverflowChecks;
			}
			set {
				codeGeneration.generateOverflowChecks = value;
			}
		}
		
		[DefaultValue(false)]
//		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.UnsafeCode}",
//		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
//		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.UnsafeCode.Description}")]
		public bool UnsafeCode {
			get {
				return codeGeneration.unsafecode;
			}
			set {
				codeGeneration.unsafecode = value;
			}
		}
		
		[DefaultValue(false)]
//		[LocalizedProperty("${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateXmlDocumentation}",
//		                   Category    = "${res:BackendBindings.CompilerOptions.CodeGeneration}",
//		                   Description = "${res:BackendBindings.CompilerOptions.CodeGeneration.GenerateXmlDocumentation.Description}")]
		public bool GenerateXmlDocumentation {
			get {
				return codeGeneration.generateXmlDocumentation;
			}
			set {
				codeGeneration.generateXmlDocumentation = value;
			}
		}
		
		
		[DefaultValue(4)]
//		[LocalizedProperty("${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.WarningLevel}",
//		                   Category    = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory}",
//		                   Description = "${res:BackendBindings.CompilerOptions.WarningAndErrorCategory.WarningLevel.Description}")]
		public int WarningLevel {
			get {
				return codeGeneration.warninglevel;
			}
			set {
				codeGeneration.warninglevel = value;
			}
		}
		
		public string Imports
		{
			get {
				return codeGeneration.imports;
			}
			set {
				codeGeneration.imports = value;
			}
		}
		
		public string Win32Icon
		{
			get {
				return codeGeneration.win32Icon;
			}
			set {
				codeGeneration.win32Icon = value;
			}
		}
		
		public string RootNamespace
		{
			get {
				return codeGeneration.rootnamespace;
			}
			set {
				codeGeneration.rootnamespace = value;
			}
		}
		
		public string DefineSymbols
		{
			get {
				return codeGeneration.definesymbols;
			}
			set {
				codeGeneration.definesymbols = value;
			}
		}
		
		public bool PauseConsoleOutput
		{
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}
		
		//public bool Debugmode
		//{
		//	get {
		//		return codeGeneration.debugmode;
		//	}
		//	set {
		//		codeGeneration.debugmode = value;
		//	}
		//}
		
		public bool Optimize
		{
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		public string MainClass
		{
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		public CompileTarget CompileTarget
		{
			get {
				return codeGeneration.compiletarget;
			}
			set {
				codeGeneration.compiletarget = value;
			}
		}
		
		public bool OptionExplicit
		{
			get {
				return codeGeneration.optionExplicit;
			}
			set {
				codeGeneration.optionExplicit = value;
			}
		}
		
		public bool OptionStrict
		{
			get {
				return codeGeneration.optionStrict;
			}
			set {
				codeGeneration.optionStrict = value;
			}
		}
		
		public string VBDOCOutputFile
		{
			get {
				return vbdoc.outputfile;
			}
			set {
				vbdoc.outputfile = value;
			}
		}
		
		public string[] VBDOCFiles
		{
			get {
				return vbdoc.filestoparse.Split(';');
			}
			set {
				vbdoc.filestoparse = System.String.Join(";", value);
			}
		}
		
		public string VBDOCCommentPrefix
		{
			get {
				return vbdoc.commentprefix;
			}
			set {
				vbdoc.commentprefix = value;
			}
		}
		
		public VBCompilerParameters()
		{
		}
		
		public VBCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
