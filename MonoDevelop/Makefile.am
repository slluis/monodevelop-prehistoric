# leave gdldock first
# (mkdir ../../build/bin/ hack)
SUBDIRS=gdldock libmonodevelop gtkmozembed-sharp src build data

bin_SCRIPTS=monodevelop

all:

desktopdir = $(datadir)/applications
desktop_DATA = monodevelop.desktop

pixmapdir = $(datadir)/pixmaps
pixmap_DATA = monodevelop.png

# FIXME install to gnome prefix
mimeinfodir = $(datadir)/mime-info
mimeinfo_DATA = monodevelop.mime monodevelop.keys

applicationdir = $(datadir)/application-registry
application_DATA = monodevelop.applications

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

EXTRA_DIST = $(bin_SCRIPTS) $(desktop_DATA) \
	$(mimeinfo_DATA) $(pixmap_DATA) $(application_DATA) KNOWN_ISSUES

