
ASSEMBLY = MonoDevelop.Debugger.dll
ADDIN = $(srcdir)/MonoDevelopDebugger.addin.xml

DLLS = $(BASE_DEPENDENCIES_LIBS) \
	$(MONO_DEBUGGER_LIBS) \
	/r:../../Core/build/bin/MonoDevelop.Base.dll \
	/r:../../Core/build/bin/MonoDevelop.Core.dll

FILES = \
DebuggerCommands.cs \
DebuggingService.cs \
Gui/DebuggerLocalsPad.cs \
Gui/DebuggerVariablePad.cs \
Gui/DebuggerStackTracePad.cs \
Gui/DebuggerThreadPad.cs

EXTRA_DIST = $(FILES) $(ADDIN)

if ENABLE_DEBUGGER

all: $(ASSEMBLY)


build_sources = $(addprefix $(srcdir)/, $(FILES))

../../Core/build/AddIns/$(ADDIN):
	cp $(ADDIN) ../../Core/build/AddIns/.

$(ASSEMBLY): $(build_sources) ../../../build/AddIns/$(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS) \
	&& cp $(ASSEMBLY) ../../Core/build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

else
all:

endif
