
ASSEMBLY = MonoDevelop.Debugger.dll
ADDIN = $(srcdir)/MonoDevelopDebugger.addin.xml

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

all: $(ASSEMBLY)


build_sources = $(addprefix $(srcdir)/, $(FILES))

$(top_builddir)/build/AddIns/$(ADDIN):
	cp $(ADDIN) $(top_builddir)/build/AddIns/.

$(ASSEMBLY): $(build_sources) $(top_builddir)/build/AddIns/$(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS)\
	&& cp $(ASSEMBLY) $(top_builddir)/build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

else
all:

endif
