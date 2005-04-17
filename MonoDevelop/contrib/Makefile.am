
BINDIR=$(top_builddir)/build/bin
LOG4NET_DLL=log4net.dll
LOG4NET = $(BINDIR)/$(LOG4NET_DLL)

all: $(LOG4NET)

$(LOG4NET): $(srcdir)/$(LOG4NET_DLL)
	mkdir -p $(BINDIR)
	cp $(srcdir)/$(LOG4NET_DLL) $(LOG4NET)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(LOG4NET)

CLEANFILES = $(LOG4NET)
EXTRA_DIST = $(LOG4NET_DLL)

