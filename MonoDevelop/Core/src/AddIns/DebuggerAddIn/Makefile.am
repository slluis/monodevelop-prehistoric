
ASSEMBLY = MonoDevelop.Debugger.dll
ADDIN = MonoDevelopDebugger.addin.xml
ADDIN_BUILD = $(top_builddir)/build/AddIns/DebuggerAddIn

DLLS = $(GTK_SHARP_LIBS) \
	$(MONO_DEBUGGER_LIBS) \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll

FILES = \
DebuggerCommands.cs \
DebuggingService.cs \
Gui/DebuggerLocalsPad.cs \
Gui/DebuggerVariablePad.cs \
Gui/DebuggerStackTracePad.cs \
Gui/DebuggerThreadPad.cs


EXTRA_DIST = $(FILES) $(ADDIN)

if ENABLE_DEBUGGER

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

build_sources = $(addprefix $(srcdir)/, $(FILES))

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(build_sources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS)\
	&& cp $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

assemblydir = $(libdir)/monodevelop/AddIns/DebuggerAddIn
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

else
all:

endif
