using System;
using System.Xml;
using System.Diagnostics;

using MonoDevelop.Internal.Project;

namespace NemerleBinding
{
	public enum CompileTarget
	{
		Executable,
		Library
	};
	
	public class NemerleParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		class CodeGeneration 
		{
			[XmlAttribute("target")]
			public CompileTarget target  = CompileTarget.Executable;
			[XmlAttribute("nostdmacros")]
			public bool nostdmacros         = false;
			[XmlAttribute("nostdlib")]
			public bool nostdlib            = false;
			[XmlAttribute("ot")]
			public bool ot                  = true;
			[XmlAttribute("obcm")]
			public bool obcm                = true;
			[XmlAttribute("oocm")]
			public bool oocm                = true;
			[XmlAttribute("oscm")]
			public bool oscm                = true;
			
			[XmlAttribute("assemblyname")]
			public string assemblyname      = String.Empty;
			[XmlAttribute("outputpath")]
			public string outputpath        = String.Empty;
			[XmlAttribute("parameters")]
			public string parameters        = String.Empty;
		}
		
		[XmlNodeName("Execution")]
		class Execution
		{
			[XmlAttribute("executecommand")]
			public string executecommand    = String.Empty;
		}

		
		CodeGeneration codeGeneration = new CodeGeneration();
		
		Execution      execution      = new Execution();
		
		public CompileTarget Target
		{
			get { return codeGeneration.target; }
			set { codeGeneration.target = value; }
		}
		public bool Nostdmacros
		{
			get { return codeGeneration.nostdmacros; }
			set { codeGeneration.nostdmacros = value; }
		}
		public bool Nostdlib
		{
			get { return codeGeneration.nostdlib; }
			set { codeGeneration.nostdlib = value; }
		}
		public bool Ot
		{
			get { return codeGeneration.ot; }
			set { codeGeneration.ot = value; }
		}
		public bool Obcm
		{
			get { return codeGeneration.obcm; }
			set { codeGeneration.obcm = value; }
		}
		public bool Oocm
		{
			get { return codeGeneration.oocm; }
			set { codeGeneration.oocm = value; }
		}
		public bool Oscm
		{
			get { return codeGeneration.oscm; }
			set { codeGeneration.oscm = value; }
		}
		
		public string AssemblyName
		{
			get { return codeGeneration.assemblyname; }
			set { codeGeneration.assemblyname = value; }
		}
		public string OutputPath
		{
			get { return codeGeneration.outputpath; }
			set { codeGeneration.outputpath = value; }
		}
		public string Parameters
		{
			get { return codeGeneration.parameters; }
			set { codeGeneration.parameters = value; }
		}
		
		public string ExecuteCommand
		{
			get { return execution.executecommand; }
			set { execution.executecommand = value; }
		}

	
		public NemerleParameters()
		{
		}
		public NemerleParameters(string name)
		{
			this.name = name;
		}
	}
}
