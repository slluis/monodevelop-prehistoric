
ASSEMBLY = dbgen.exe

FILES = src/AssemblyInfo.cs \
src/dbgen.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

REFS = /r:$(top_builddir)/build/bin/MonoDevelop.Base.dll /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll
	
$(ASSEMBLY): $(build_sources)
	$(CSC) $(build_sources) $(REFS) /out:$(ASSEMBLY) \
	&& cp $(ASSEMBLY) $(top_builddir)/build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(ASSEMBLY)           

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES)

