#!/bin/sh
# Run this to generate all the initial makefiles, etc.
# Ripped off from GNOME macros version

DIE=0

PKG_NAME=MonoDevelop
WANT_AUTOCONF="2.5"
srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

(autoconf --version) < /dev/null > /dev/null 2>&1 || {
  echo
  echo "**Error**: You must have \`autoconf' installed to compile MonoDevelop."
  echo "Download the appropriate package for your distribution,"
  echo "or get the source tarball at ftp://ftp.gnu.org/pub/gnu/"
  DIE=1
}

(automake --version) < /dev/null > /dev/null 2>&1 || {
  echo
  echo "**Error**: You must have \`automake' installed to compile MonoDevelop."
  echo "Get ftp://ftp.gnu.org/pub/gnu/automake-1.3.tar.gz"
  echo "(or a newer version if it is available)"
  DIE=1
  NO_AUTOMAKE=yes
}

(intltoolize --version) < /dev/null > /dev/null 2>&1 || {
  echo
  echo "**Error**: You must have \`intltoolize' installed to compile MonoDevelop."
  DIE=1
}

if test "$DIE" -eq 1; then
  exit 1
fi

if test -z "$*"; then
  echo "**Warning**: I am going to run \`configure' with no arguments."
  echo "If you wish to pass any to it, please specify them on the"
  echo \`$0\'" command line."
  echo
fi

case $CC in
xlc )
  am_opt=--include-deps;;
esac

echo "Running glib-gettextize ..."
glib-gettextize --force --copy ||
  { echo "**Error**: glib-gettextize failed."; exit 1; }

echo "Running intltoolize ..."
intltoolize --force --copy --automake ||
  { echo "**Error**: intltoolize failed."; exit 1; }

echo "Running automake --gnu $am_opt ..."
automake --add-missing --gnu $am_opt ||
  { echo "**Error**: automake failed."; exit 1; }

echo "Running autoconf ..."
WANT_AUTOCONF="2.5" autoconf || { echo "**Error**: autoconf failed."; exit 1; }


conf_flags="--enable-maintainer-mode --enable-compile-warnings"

if test x$NOCONFIGURE = x; then
  echo Running $srcdir/configure $conf_flags "$@" ...
  $srcdir/configure $conf_flags "$@" \
  && echo Now type \`make\' to compile $PKG_NAME || exit 1
else
  echo Skipping configure process.
fi
