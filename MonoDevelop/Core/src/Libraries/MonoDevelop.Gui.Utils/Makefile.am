DLL = MonoDevelop.Gui.Utils.dll

REFERENCES = /r:System.Drawing \
	/r:ICSharpCode.SharpZipLib \
	$(BASE_DEPENDENCIES_LIBS)

FILES = \
FileIcons/FileIconLoader.cs \
AssemblyInfo.cs \
VFS/Vfs.cs \
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
	$(CSC) /out:$(DLL) /target:library $(REFERENCES) $(build_sources) \
	&& cp $(DLL) ../../../build/bin/. && cp $(DLL).config ../../../build/bin

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL) $(DLL).config

CLEANFILES = $(DLL) $(DLL).mdb
DISTCLEANFILES = $(DLL).config

EXTRA_DIST = $(FILES) $(DLL).config.in

