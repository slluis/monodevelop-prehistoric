DLLNAME = MonoDevelop.Gui.Utils.dll
CONFIG = $(DLLNAME).config
DLL = $(top_builddir)/build/bin/$(DLLNAME)

REFERENCES = /r:System.Drawing \
	/r:ICSharpCode.SharpZipLib \
	$(GTK_SHARP_LIBS) \
	$(GNOME_SHARP_LIBS)

FILES = \
FileIcons/FileIconLoader.cs \
AssemblyInfo.cs \
ReportingStream/ReportingStream.cs \
DirectoryArchive/Decompressor.cs \
DirectoryArchive/TarDecompressor.cs \
DirectoryArchive/ZipDecompressor.cs \
DirectoryArchive/Bz2Support.cs \
DirectoryArchive/GzSupport.cs \
Glue.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(DLL)

$(DLL): $(build_sources)
	$(CSC) -out:$@ /target:library $(REFERENCES) $(build_sources) \
	&& cp $(CONFIG) $(top_builddir)/build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL) $(CONFIG)

CLEANFILES = $(DLL) $(DLL).mdb
DISTCLEANFILES = $(CONFIG)

EXTRA_DIST = $(FILES) $(CONFIG).in

