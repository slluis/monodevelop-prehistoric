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

namespace BooBinding.Gui

import System
import System.Collections
import System.IO
import Boo.Lang.Interpreter
import Boo.Lang.Compiler
import BooBinding.Properties

class BooShellModel(IShellModel):
	
	private _interpreter = InteractiveInterpreter(RememberLastValue: true, Print: print)

	private _outSink as StreamWriter
	private _outSource as StreamReader

	private props = BooShellProperties()

	MimeType as string:
		get:
			return "text/x-boo"

	def constructor():
		_stream = MemoryStream()
		_outSink = StreamWriter(_stream)
		_outSource = StreamReader (_stream)
	
	Properties as ShellProperties:
		get:
			return props
	
	def Reset() as bool:
		_interpreter.Reset()
		return true
		
	def ProcessInput (line as String) as (string):
		// Make sure our fake stdout is at the beginning
		_outSink.BaseStream.SetLength(0)
		_outSink.BaseStream.Seek(0, SeekOrigin.Begin)

		// Save tradition stdout, and redirect Console
		// to local StreamWriter. Catches any print, etc calls
		// to be output to the local shell
		_stdout = Console.Out
		Console.SetOut(_outSink)

		_interpreter.LoopEval(line)

		// Restore stdout, and prep our fake stdout for reading
		Console.SetOut(_stdout)
		_outSink.Flush()
		_outSink.BaseStream.Seek(0, SeekOrigin.Begin)

		retList = ArrayList()
		_outputLine as string = _outSource.ReadLine()

		while _outputLine is not null:
			retList.Add(_outputLine)
			_outputLine = _outSource.ReadLine()

		ret = cast ((string), retList.ToArray(typeof(string)))

		_ = _interpreter.LastValue
		if _ is not null:
			_interpreter.SetValue("_", _)

		return ret
	
	def print(obj):
		print "${obj}"
