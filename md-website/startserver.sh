#!/bin/bash

# clean wapi, if necessary
rm -rf $HOME/.wapi

# start the mod-mono-server assembly

mono /usr/local/bin/mod-mono-server.exe --root . --applications /:. --nonstop
chmod 666 /tmp/mod_mono_server

/etc/init.d/apache2 restart
