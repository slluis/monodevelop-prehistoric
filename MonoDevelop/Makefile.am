# leave gdldock first
# (mkdir ../../build/bin/ hack)
SUBDIRS = gdldock libmonodevelop src build data po

bin_SCRIPTS = monodevelop

all:

gnome_data = @gnome_prefix@/share

desktopdir = $(gnome_data)/applications
desktop_DATA = monodevelop.desktop

pixmapdir = $(gnome_data)/pixmaps
pixmap_DATA = monodevelop.png

if ENABLE_XDGMIME

# need to respect $XDG_DATA_DIR probably
xdgmimedir = $(gnome_data)/mime/packages
xdgmime_DATA = monodevelop.xml

install-data-hook:
	update-mime-database $(gnome_data)/mime

uninstall-hook:
	update-mime-database $(gnome_data)/mime

else

install-data-hook:

uninstall-hook:

mimeinfodir = $(gnome_data)/mime-info
mimeinfo_DATA = monodevelop.mime monodevelop.keys

applicationdir = $(gnome_data)/application-registry
application_DATA = monodevelop.applications

endif

run: runmd

runmd: $(PROGRAM)
	cd build/bin && LD_LIBRARY_PATH="$(MOZILLA_HOME)" mono --debug MonoDevelop.exe

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe

# we can't use the variables that are conditionally defined
EXTRA_DIST = $(bin_SCRIPTS) $(desktop_DATA) $(pixmap_DATA) \
	monodevelop.mime monodevelop.keys monodevelop.applications \
	monodevelop.xml mkinstalldirs intltool-extract.in \
	intltool-merge.in intltool-update.in

DISTCLEANFILES = monodevelop intltool-extract intltool-merge intltool-update

