
CSC = mcs /debug
ASSEMBLY = MonoDevelop.Debugger.dll
ADDIN = $(srcdir)/MonoDevelopDebugger.addin.xml

DLLS = @GTKSHARP@ \
	/r:$(debugger_prefix)/lib/Mono.Debugger.dll \
	/r:../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../build/bin/MonoDevelop.Core.dll

FILES = \
DebuggerCommands.cs \
DebuggingService.cs \
Gui/DebuggerLocalsPad.cs \
Gui/DebuggerVariablePad.cs

EXTRA_DIST = $(FILES) $(ADDIN)

if ENABLE_DEBUGGER

#all: $(ASSEMBLY)
all:


build_sources = $(addprefix $(srcdir)/, $(FILES))

../../../build/AddIns/$(ADDIN):
	cp $(ADDIN) ../../../build/AddIns/.

$(ASSEMBLY): $(build_sources) ../../../build/AddIns/$(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS)\
	&& cp $(ASSEMBLY) ../../../build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY)

else
all:

endif
