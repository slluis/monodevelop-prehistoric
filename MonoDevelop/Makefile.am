SUBDIRS=gdldock gtkmozembed-sharp src

desktopdir = $(datadir)/applications
desktop_DATA = monodevelop.desktop

run: runmd

runmd: $(PROGRAM)
	./monodevelop

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

newed : $(PROGRAM)
	NEW_EDITOR=true ./monodevelop

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe
