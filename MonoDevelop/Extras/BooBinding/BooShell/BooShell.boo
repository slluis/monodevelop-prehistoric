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

namespace BooBinding.BooShell

import System
import System.Collections
import System.IO
import Boo.Lang.Interpreter
import Boo.Lang.Compiler

import Gtk
import GLib

class BooShell(MarshalByRefObject):
	private _interpreter = InteractiveInterpreter(RememberLastValue: true, Print: print)

	private _commandQueue = Queue()
	private _outputQueue = Queue()

	private _thread as System.Threading.Thread

	private _processing as string = "true"

	def Reset() as bool:
		EnqueueCommand (ShellCommand (ShellCommandType.Reset, null))
		return true
	
	def LoadAssembly (assemblyPath as string) as bool:
		EnqueueCommand (ShellCommand (ShellCommandType.Load, assemblyPath))
		return true
	
	def GetOutput() as (string):
		_tmp as string
		lock _processing:
			_tmp = _processing

		while _tmp == "true":
			lock _processing:
				_tmp = _processing
			// Sleep to let other thread process (and grab lock)
			System.Threading.Thread.Sleep (10)

		ret as (string)
		lock _outputQueue:
			if _outputQueue.Count > 0:
				ret = array (string, _outputQueue.Count)
				_outputQueue.CopyTo (ret, 0)
				_outputQueue.Clear()

		return ret
		
	def QueueInput (line as string):
		EnqueueCommand (ShellCommand (ShellCommandType.Eval, line))

	def ThreadRun():
		Application.Init()
		GLib.Idle.Add(ProcessCommands)
		Application.Run()

	def ProcessCommands() as bool:
		com as ShellCommand
		lock _commandQueue:
			if _commandQueue.Count > 0:
				com = _commandQueue.Dequeue()
		if com.Type == ShellCommandType.Eval:
			if com.Data is not null:
				lock _outputQueue:
					_interpreter.LoopEval(com.Data)
		elif com.Type == ShellCommandType.Reset:
			_interpreter.Reset()
		elif com.Type == ShellCommandType.Load:
			if com.Data is not null:
				_interpreter.load(com.Data)

		com.Type = ShellCommandType.NoOp

		lock _commandQueue:
			if _commandQueue.Count == 0:
				lock _processing:
					_processing = "false"

		return true
	
	def Run():
		_thread = System.Threading.Thread(ThreadRun)
		_thread.Start()
	
	def print(obj):
		_outputQueue.Enqueue(obj)
	
	def EnqueueCommand (command as ShellCommand):
		lock _commandQueue:
			_commandQueue.Enqueue (command)
			lock _processing:
				_processing = "true"
	

public enum ShellCommandType:
	NoOp
	Reset
	Load
	Eval

public struct ShellCommand:
	Type as ShellCommandType
	Data as string

	def constructor (type, data):
		self.Type = type
		self.Data = data
