// GccBindingExecutionServices.cs
// Copyright (C) 2001 Mike Krueger
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
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using Core.Util;

using ICSharpCode.SharpDevelop.CombineProjectLayer.Project;
using SharpDevelop.Gui;

namespace GccBinding {
	
	/// <summary>
	/// This class executes files that are compiled with the gcc language binding.
	/// </summary>
	public class GccBindingExecutionServices
	{
		public void Execute(string filename)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = false;
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
		}
		
		public void Execute(IProject project)
		{
			GccCompilerParameters parameters = (GccCompilerParameters)project.ActiveConfiguration;
			
			// !!! useful function : FileUtility.GetDirectoryNameWithSeparator, returns a directory with a separator at the end !!!
			string directory = FileUtility.GetDirectoryNameWithSeparator(parameters.OutputDirectory);

			string exe  = parameters.OutputAssembly + ".exe";
			string args = parameters.CommandLineParameters;
			
			ProcessStartInfo processStartInfo;
			
			if (parameters.PauseConsoleOutput) {
				processStartInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + directory + exe + "\" " + args +  " & pause");
			} else {
				processStartInfo = new ProcessStartInfo("\"" + directory + exe + "\"");
				processStartInfo.Arguments = args;
			}
			
			try {
				processStartInfo.WorkingDirectory = Path.GetDirectoryName(directory);
				processStartInfo.UseShellExecute  =  false;
				
				Process p =	new Process();
				p.StartInfo = processStartInfo;
				p.Start();
			} catch (Exception e) {
				throw new ApplicationException("Can't execute " + "\"" + directory + exe +"\"\n exception got : " + e.ToString());
			}
		}
	}
}