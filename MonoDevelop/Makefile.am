# leave gdldock first
# (mkdir ../../build/bin/ hack)
SUBDIRS=gdldock libmonodevelop gtkmozembed-sharp src build data

bin_SCRIPTS=monodevelop

all:

gnome_data = @gnome_prefix@/share

desktopdir = $(gnome_data)/applications
desktop_DATA = monodevelop.desktop

pixmapdir = $(gnome_data)/pixmaps
pixmap_DATA = monodevelop.png

mimeinfodir = $(gnome_data)/mime-info
mimeinfo_DATA = monodevelop.mime monodevelop.keys

applicationdir = $(gnome_data)/application-registry
application_DATA = monodevelop.applications

run: runmd

runmd: $(PROGRAM)
	cd build/bin && MONO_PATH="`pkg-config --variable=libdir monodoc`/monodoc" mono --debug MonoDevelop.exe

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe

EXTRA_DIST = $(bin_SCRIPTS) $(desktop_DATA) \
	$(mimeinfo_DATA) $(pixmap_DATA) $(application_DATA) KNOWN_ISSUES

