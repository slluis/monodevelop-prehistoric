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

namespace BooBinding.BooShellServer

import System
import System.IO
import System.Collections

import System.Net.Sockets
import System.Runtime.Remoting
import System.Runtime.Remoting.Channels

import BooBinding.Remoting
import BooBinding.BooShell
import Mono.Posix

if argv.Length != 1:
	print "ERROR: BooShellServer called with an invalid number of arguments"
	System.Environment.Exit(1)

print "Starting server listening on ${argv[0]}"
File.Delete (argv[0])
props = Hashtable()
props["path"] = argv[0]
chan = UnixChannel (props, BinaryClientFormatterSinkProvider (), BinaryServerFormatterSinkProvider ())
ChannelServices.RegisterChannel(chan);
RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(BooShell), "BooShell", WellKnownObjectMode.Singleton);
	
while true:
	System.Threading.Thread.Sleep (10)
