FILES= 

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.Sonata.dll

REFS = 	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	$(BASE_DEPENDENCIES_LIBS)

all: $(DLL)

$(DLL): $(build_sources)
	mcs /debug /out:$(DLL) /target:library $(REFS) \
		$(build_sources) \
	&& cp $(DLL) ../../../../build/AddIns/AddIns/Misc/Sonata/.

startpagelibdir = $(libdir)/monodevelop/AddIns/AddIns/Misc/Sonata
startpagelib_DATA = $(DLL)

CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

