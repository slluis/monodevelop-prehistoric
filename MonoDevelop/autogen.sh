#! /bin/sh

aclocal
automake --add-missing --gnu
autoconf
./configure $@

