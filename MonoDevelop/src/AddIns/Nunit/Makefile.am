
CSC = mcs /debug
ASSEMBLY = MonoDevelop.Nunit.dll
ADDIN = MonoDevelopNunit.addin

DLLS = /r:NUnit.Framework.dll \
	/r:gtk-sharp.dll /r:glib-sharp.dll /r:gdk-sharp.dll \
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

all: $(ASSEMBLY)

../../../build/AddIns/$(ADDIN): $(ADDIN)
	cp $(ADDIN) ../../../build/AddIns/$(ADDIN)

$(ASSEMBLY): ../../../build/AddIns/$(ADDIN) $(FILES)
	$(CSC) $(DLLS) $(FILES) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../build/AddIns/.

addindir = $(libdir)/monodevelop/AddIns
addin_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(FILES) $(ADDIN)

