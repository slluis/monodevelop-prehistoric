DLL = MonoDevelop.Gui.Utils.dll
REFERENCES=/r:System.Drawing /r:gtk-sharp /r:gdk-sharp /r:pango-sharp /r:gnome-sharp /r:glib-sharp /r:glade-sharp /r:ICSharpCode.SharpZipLib

FILES = ./FileIcons/FileIconLoader.cs \
./AssemblyInfo.cs \
./VFS/Vfs.cs \
./ReportingStream/ReportingStream.cs \
./DirectoryArchive/Decompressor.cs \
./DirectoryArchive/TarDecompressor.cs \
./DirectoryArchive/ZipDecompressor.cs \
./DirectoryArchive/Bz2Support.cs \
./DirectoryArchive/GzSupport.cs \
./Glue.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(DLL)

$(DLL): $(build_sources)
	mcs /debug /out:$(DLL) /target:library $(REFERENCES) $(build_sources) \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)

CLEANFILES=$(DLL)

EXTRA_DIST = $(FILES)

