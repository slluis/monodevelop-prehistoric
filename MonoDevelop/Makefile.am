SUBDIRS=gdldock gtkmozembed-sharp src build data

bin_SCRIPTS=monodevelop

all:

desktopdir = $(datadir)/applications
desktop_DATA = monodevelop.desktop

run: runmd

runmd: $(PROGRAM)
	cd build/bin && mono --debug MonoDevelop.exe

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe

EXTRA_DIST = $(bin_SCRIPTS) $(desktop_DATA) KNOWN_ISSUES
