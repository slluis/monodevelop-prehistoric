
CSC = mcs /debug
ASSEMBLY = MonoDevelop.Nunit.dll
ADDIN = $(srcdir)/MonoDevelopNunit.addin.xml

DLLS = /r:NUnit.Framework.dll \
	@GTKSHARP@ \
	/r:../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../build/bin/MonoDevelop.Base.dll

FILES = AssemblyInfo.cs \
./Commands/NunitCommands.cs \
./Gui/NunitTestTree.cs \
./Gui/NunitResultTree.cs \
./Services/AssemblyStore.cs \
./Services/CancelTestsException.cs \
./Services/CircleRenderer.cs \
./Services/Delegates.cs \
./Services/FixtureAddedEventArgs.cs \
./Services/FixtureLoadErrorEventArgs.cs \
./Services/NunitService.cs \
./Services/QueuedEvent.cs \
./Services/QueuedSuiteFinish.cs \
./Services/QueuedSuiteStart.cs \
./Services/QueuedTestEvent.cs \
./Services/QueuedTestFinish.cs \
./Services/QueuedTestStart.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

../../../build/AddIns/$(ADDIN): $(ADDIN)
	cp $(ADDIN) ../../../build/AddIns/MonoDevelopNunit.addin

$(ASSEMBLY): $(FILES)
	$(CSC) $(DLLS) $(build_sources) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../build/AddIns/.

# not ready to be installed yet
#addindir = $(libdir)/monodevelop/AddIns
#addin_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(FILES) $(ADDIN)

