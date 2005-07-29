
BINDIR=$(top_builddir)/build/bin
LOG4NET_DLL=log4net.dll
LOG4NET = $(BINDIR)/$(LOG4NET_DLL)
CECIL_DLL=Mono.Cecil.dll
CECIL = $(BINDIR)/$(CECIL_DLL)

all: $(LOG4NET) $(CECIL)

$(LOG4NET): $(srcdir)/$(LOG4NET_DLL)
	mkdir -p $(BINDIR)
	cp $(srcdir)/$(LOG4NET_DLL) $(LOG4NET)

$(CECIL): $(srcdir)/$(CECIL_DLL)
	mkdir -p $(BINDIR)
	cp $(srcdir)/$(CECIL_DLL) $(CECIL)

assemblydir = $(MD_ASSEMBLY_DIR)
assembly_DATA = $(LOG4NET) $(CECIL)

CLEANFILES = $(LOG4NET) $(CECIL)
EXTRA_DIST = $(LOG4NET_DLL) $(CECIL_DLL)

