
CSC = mcs /debug

if ENABLE_DEBUGGER

all: $(ASSEMBLY)

ASSEMBLY = MonoDevelop.Debugger.dll
ADDIN = MonoDevelopDebugger.addin

FILES = \
DebuggerCommands.cs \
DebuggingService.cs \
Gui/DebuggerLocalsPad.cs \
Gui/DebuggerVariablePad.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

../../../build/AddIns/$(ADDIN):
	cp $(ADDIN) ../../../build/AddIns/.

$(ASSEMBLY): $(build_sources) $(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	/r:gtk-sharp /r:glib-sharp \
	/r:$(debugger_prefix)/lib/Mono.Debugger.dll \
	/r:../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../build/bin/MonoDevelop.Core.dll \
	$(build_sources) \
	&& cp $(ASSEMBLY) ../../../build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(FILES) MonoDevelopDebugger.addin

else
all:

endif
