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
import System.Diagnostics
import System.Collections
import System.IO
import System.Threading
import BooBinding.Properties

import BooBinding.BooShell
import BooBinding.Remoting
import System.Runtime.Remoting
import System.Runtime.Remoting.Channels

class BooShellModel(IShellModel):
	private _props = BooShellProperties()

	private _commandQueue = Queue()
	private _outputQueue = Queue()

	private _outputHandler as callable
	
	private _thread as System.Threading.Thread

	private _process as Process

	private _booShell as BooShell

	MimeType as string:
		get:
			return "text/x-boo"

	Properties as ShellProperties:
		get:
			return _props
	
	def constructor ():
		pass

	def constructor (program_path as string, socket_path as string):
		StartShellServer (program_path, socket_path)
		GetRemoteShellObject (socket_path)
		_booShell.Run()

	def StartShellServer(program_path as string, socket_path as string):
		psi = ProcessStartInfo()
		psi.FileName = "mono"
		psi.Arguments = "${program_path} ${socket_path}"
		_process = Process.Start(psi)
	
	def GetRemoteShellObject (socket_path as string):
		chan = UnixChannel (Hashtable(), BinaryClientFormatterSinkProvider (), BinaryServerFormatterSinkProvider ())
		ChannelServices.RegisterChannel(chan)
		_booShell = Activator.GetObject (typeof(BooShell), "unix://${socket_path}?BooShell")

			
	def Reset() as bool:
		_booShell.Reset()
		return true
	
	def LoadAssembly (assemblyPath as string) as bool:
		_booShell.LoadAssembly (assemblyPath)
		return true
	
	def GetOutput() as (string):
		ret as (string)
		lock _outputQueue:
			if _outputQueue.Count > 0:
				ret = array (string, _outputQueue.Count)
				_outputQueue.CopyTo (ret, 0)
				_outputQueue.Clear()

		return ret

		
	def QueueInput (line as string):
		try:
			Monitor.Enter (_commandQueue)
			_commandQueue.Enqueue (line)
			Monitor.Pulse (_commandQueue)
		ensure:
			Monitor.Exit (_commandQueue)

	def ThreadRun():
		while true:
			com as string
			try:
				Monitor.Enter (_commandQueue)
				if _commandQueue.Count == 0:
					Monitor.Wait (_commandQueue)

				com = _commandQueue.Dequeue()


				if com is not null:
					print "Sending out command '${com}'"
					_booShell.QueueInput (com)
					print "trying to get the output!"
					lines = _booShell.GetOutput()
					print "got the output!"
					if lines is not null:
						EnqueueOutput(lines)
					com = null
					lock _outputQueue:
						if _outputHandler is not null:
							_outputHandler()

			ensure:
				Monitor.Exit (_commandQueue)

	
	def Run():
		_thread = System.Threading.Thread(ThreadRun)
		_thread.Start()
	
	def RegisterOutputHandler (handler as callable):
		_outputHandler = handler
	
	def EnqueueOutput (lines as (string)):
		lock _outputQueue:
			for line in lines:
				_outputQueue.Enqueue(line)
	
	def Dispose():
		_thread.Abort()
		_process.Kill()
		_booShell = null
		
	def print(obj):
		lock _outputQueue:
			_outputQueue.Enqueue(obj)
