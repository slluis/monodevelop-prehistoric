// GccCompilerParameters.cs
// Copyright (C) 2002 Mike Krueger
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Xml;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.CombineProjectLayer.Project;
using ICSharpCode.SharpDevelop.CombineProjectLayer.Attributes;

namespace GccBinding {
	
	public enum Compiler {
		Gcc, 
		Gpp
	};	
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class GccCompilerParameters : AbstractProjectConfiguration
	{
		[XmlAttribute("whichcompiler")]
		Compiler whichCompiler;
		
		[XmlAttribute("commandlineparameters")]
		string commandLineParameters;
		
		[XmlAttribute("pauseconsoleoutput")]
		bool pauseConsoleOutput;
		
		public bool PauseConsoleOutput {
			get {
				return pauseConsoleOutput;
			}
			set {
				pauseConsoleOutput = value;
			}
		}
		
		public string CommandLineParameters {
			get {
				return commandLineParameters;
			}
			set {
				commandLineParameters = value;
			}
		}
		
		public Compiler WhichCompiler {
			get {
				return whichCompiler;
			}
			set {
				whichCompiler = value;
			}
		}
	}
}
