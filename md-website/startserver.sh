#!/bin/bash

# clean wapi, if necessary
rm -rf $HOME/.wapi
rm /tmp/mod_mono_server

# set PATH
export PATH=$PATH:/usr/bin

# start the mod-mono-server assembly
mono /usr/bin/mod-mono-server.exe --root . --applications /:. --nonstop
chmod 666 /tmp/mod_mono_server

/etc/init.d/httpd restart
