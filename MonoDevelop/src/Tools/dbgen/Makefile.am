
CSC = mcs /debug
ASSEMBLY = dbgen.exe

FILES = src/AssemblyInfo.cs \
src/dbgen.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

REFS = /r:../../../build/bin/MonoDevelop.Base.dll
	
$(ASSEMBLY): $(build_sources)
	$(CSC) $(build_sources) $(REFS) /out:$(ASSEMBLY)
	cp $(ASSEMBLY) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(ASSEMBLY)           

CLEANFILES = $(ASSEMBLY)
EXTRA_DIST = $(FILES)

