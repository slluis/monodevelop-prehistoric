DLL=../../../build/bin/MonoDevelop.Gui.Utils.dll
REFERENCES=/r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /r:glade-sharp.dll

SOURCES = ./FileIcons/FileIconLoader.cs \
./AssemblyInfo.cs \
./VFS/Vfs.cs

all: $(DLL)

$(DLL): $(SOURCES)
	@ mcs /debug /out:$(DLL) /target:library $(REFERENCES) $(SOURCES)

monodeveloplibdir = $(prefix)/lib/monodevelop
  
install-data-local: $(DLL)
	$(mkinstalldirs) $(DESTDIR)$(monodeveloplibdir)/bin
	$(INSTALL_DATA) $(DLL) $(DESTDIR)$(monodeveloplibdir)/bin/MonoDevelop.Gui.Utils.dll

CLEANFILES=$(DLL)

EXTRA_DIST = $(SOURCES)
