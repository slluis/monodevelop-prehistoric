DLL = MonoDevelop.Gui.Utils.dll
REFERENCES=/r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /r:glade-sharp.dll

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

