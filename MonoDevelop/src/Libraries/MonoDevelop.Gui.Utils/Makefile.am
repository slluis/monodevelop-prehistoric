DLL = MonoDevelop.Gui.Utils.dll
REFERENCES=/r:System.Drawing /r:gtk-sharp /r:gdk-sharp /r:pango-sharp /r:gnome-sharp /r:glib-sharp /r:glade-sharp

FILES = ./FileIcons/FileIconLoader.cs \
./AssemblyInfo.cs \
./VFS/Vfs.cs \
./Glue.cs

all: $(DLL)

$(DLL): $(FILES)
	@ mcs /debug /out:$(DLL) /target:library $(REFERENCES) $(FILES) \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES=$(DLL)

EXTRA_DIST = $(FILES)

