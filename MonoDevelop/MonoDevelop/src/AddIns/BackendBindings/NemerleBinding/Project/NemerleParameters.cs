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
			public bool ot                  = false;
			[XmlAttribute("obcm")]
			public bool obcm                = true;
			[XmlAttribute("oocm")]
			public bool oocm                = true;
			[XmlAttribute("oscm")]
			public bool oscm                = true;
			
			[XmlAttribute("parameters")]
			public string parameters        = String.Empty;
		}
		
		CodeGeneration codeGeneration = new CodeGeneration();
		
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
		
		public string Parameters
		{
			get { return codeGeneration.parameters; }
			set { codeGeneration.parameters = value; }
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
