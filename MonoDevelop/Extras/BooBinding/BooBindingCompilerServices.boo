#region license
// Copyright (c) 2005, Peter Johanson (latexer@gentoo.org)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.Diagnostics
import System.IO
import System.CodeDom.Compiler
import System.Text

import MonoDevelop.Gui.Components
import MonoDevelop.Services
import MonoDevelop.Core.Services
import MonoDevelop.Internal.Project

public class BooBindingCompilerServices:
	public def CanCompile (fileName as string):
		return Path.GetExtension(fileName) == ".boo"
	
	public def GetCompilerName (cp as BooCompilerParameters):
		if (cp.Compiler == BooCompiler.Boo):
			return "boo"

		return "booc"
	
	def Compile (projectFiles as ProjectFileCollection, references as ProjectReferenceCollection, configuration as DotNetProjectConfiguration, monitor as IProgressMonitor) as ICompilerResult:
		compilerparameters = cast(BooCompilerParameters, configuration.CompilationParameters)
		if compilerparameters is null:
			compilerparameters = BooCompilerParameters()
		
		// FIXME: Use outdir
		//outdir = configuration.OutputDirectory
		options = ""

		compiler = GetCompilerName (compilerparameters)
		
		if configuration.DebugMode:
			options += " -debug "

		options += " -o:" + configuration.CompiledOutputName

		if references is not null:
			for lib as ProjectReference in references:
				fileName = lib.GetReferencedFileName()
				// FIXME: DO we need all these tests?
				if lib.ReferenceType == ReferenceType.Gac:
					options += " -r:" + fileName + " "
				elif lib.ReferenceType == ReferenceType.Assembly:
					options += " -r:" + fileName + " "
				elif lib.ReferenceType == ReferenceType.Project:
					options += " -r:" + fileName + " "

		files  = ""
		
		for finfo as ProjectFile in projectFiles:
			if finfo.Subtype != Subtype.Directory:
				if finfo.BuildAction == BuildAction.Compile:
					files = files + " \"" + finfo.Name + "\""

		
		// FIXME: Add selection of output assembly types (library, exe, etc)
		if configuration.CompileTarget == CompileTarget.Exe:
			options += " -t:exe "
		elif configuration.CompileTarget == CompileTarget.Library:
			options += " -t:library "
		elif configuration.CompileTarget == CompileTarget.WinExe:
			options += " -t:winexe "

		if compilerparameters.Culture != String.Empty:
			options += " -c:${compilerparameters.Culture} "

		if compilerparameters.Ducky:
			options += " -ducky "

		options += files

		tf = TempFileCollection ()
		output, error = DoCompilation (monitor, compiler, options, tf, configuration, compilerparameters)
		cr = ParseOutput (tf, output, error)
		File.Delete (output)
		File.Delete (error)
		return cr

	private def DoCompilation (monitor as IProgressMonitor , compiler as string , args as string , tf as TempFileCollection , configuration as DotNetProjectConfiguration , compilerparameters as BooCompilerParameters):
		output = Path.GetTempFileName ()
		error = Path.GetTempFileName ()

		try:
			monitor.BeginTask (null, 2)
			monitor.Log.WriteLine ("Compiling Boo source code ...")
			arguments = String.Format ("-c \"{0} {1} > {2} 2> {3}\"", (compiler, args, output, error))
			si = ProcessStartInfo ("/bin/sh", arguments)
			// print "${si.FileName}, ${si.Arguments}"
			si.RedirectStandardOutput = true
			si.RedirectStandardError = true
			si.UseShellExecute = false
			p = Process ()
			p.StartInfo = si
			p.Start ()
			p.WaitForExit ()

			monitor.Step (1)

			return output, error;
		ensure:
			monitor.EndTask ()

	
	def ParseOutput (tf as TempFileCollection , stdout as string, stderr as string) as ICompilerResult:
		compilerOutput = StringBuilder ()
		cr = CompilerResults (tf)
		
		for s as string in ( stdout, stderr ):
			sr = File.OpenText (s);
			while true:
				next = sr.ReadLine ()
				if next is null:
					break

				error = CreateErrorFromString (next)

				if error is not null:
					cr.Errors.Add (error)

			sr.Close ()

		return DefaultCompilerResult (cr, compilerOutput.ToString ())

	private static def CreateErrorFromString (error as string) as CompilerError:
		//print "${error}"
		// FIXME: Better checking to make sure we have an error we can parse

		err_pieces = /:/.Split(error)

		// FIXME: i18n of "Fatal Error" check
		if err_pieces.Length == 2 and err_pieces[0] == "Fatal error":
			cerror = CompilerError()
			cerror.ErrorText = error
			return cerror

		if (err_pieces.Length < 3):
			return null

		// Uses extensive LastIndexOf to avoid problems with filenames
		// and directories with "(" or ")" in their names
		last_open_bracket = err_pieces[0].LastIndexOf("(")
		last_close_bracket = err_pieces[0].LastIndexOf(")")

		file = err_pieces[0].Substring(0,last_open_bracket)
		line, column = /,/.Split (err_pieces[0].Substring (last_open_bracket + 1, last_close_bracket - last_open_bracket - 1))

		cerror = CompilerError ()

		// Rejoin the split error back the way it originally was
		cerror.ErrorText = join(err_pieces[1:], ":")
		cerror.FileName = file
		cerror.Column = Int32.Parse(column)
		cerror.Line = Int32.Parse(line)
		if (err_pieces[2].Trim() == "WARNING"):
			cerror.IsWarning = true

		return cerror
