
all: $(BINDIR)/$(LOG4NET_DLL)

LOG4NET_DLL=log4net.dll
BINDIR=$(top_builddir)/build/bin

$(BINDIR)/$(LOG4NET_DLL):
	mkdir -p $(BINDIR)
	cp $(srcdir)/$(LOG4NET_DLL) $(BINDIR)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(LOG4NET_DLL)
