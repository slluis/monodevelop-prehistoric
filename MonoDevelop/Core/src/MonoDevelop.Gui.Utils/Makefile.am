DLLNAME = MonoDevelop.Gui.Utils.dll
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
DirectoryArchive/GzSupport.cs

all: $(DLL)

$(DLL): $(build_sources)
	$(CSC) -out:$@ /target:library $(REFERENCES) $(build_sources)

assemblydir = $(MD_ASSEMBLY_DIR)
assembly_DATA = $(DLL)

CLEANFILES = $(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)

include $(top_srcdir)/Makefile.include

