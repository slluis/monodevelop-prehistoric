if ENABLE_DEBUGGER

all: $(DLL)

DLL = MonoDevelop.Debugger.dll

FILES = \
DebuggerCommands.cs \
DebuggingService.cs \
Gui/DebuggerLocalsPad.cs \
Gui/DebuggerVariablePad.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

../../../build/AddIns/MonoDevelopDebugger.addin:
	cp MonoDevelopDebugger.addin ../../../build/AddIns/

MonoDevelop.Debugger.dll: $(build_sources) ../../../build/AddIns/MonoDevelopDebugger.addin
	mcs /out:$(DLL) /target:library \
	/r:gtk-sharp \
	/r:glib-sharp \
	/r:$(debugger_prefix)/lib/Mono.Debugger.dll \
	/r:../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../build/bin/MonoDevelop.Core.dll \
	$(build_sources) \
	&& cp $(DLL) ../../../build/AddIns/
else
endif
