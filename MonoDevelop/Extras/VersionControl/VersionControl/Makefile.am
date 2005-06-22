
ADDIN_BUILD = $(top_builddir)/build/AddIns/VersionControl
ASSEMBLY = $(ADDIN_BUILD)/VersionControl.dll

DLLS = 
FILES = *.cs

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)

$(ASSEMBLY): $(FILES)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(FILES) -out:$@ -target:library

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES)

all: $(ASSEMBLY)

include $(top_srcdir)/Makefile.include
