
CSC = mcs
ASSEMBLY_NAME = MonoDevelop.Nunit.dll
ASSEMBLY = ../../build/AddIns/$(ASSEMBLY_NAME)
ADDIN = MonoDevelopNunit.addin.xml

DLLS = -r:nunit.framework.dll \
	-r:nunit.core.dll \
	-r:../../build/bin/MonoDevelop.Core.dll \
	-r:../../build/bin/MonoDevelop.Base.dll \
	-r:../../build/bin/MonoDevelop.Gui.Widgets.dll \
	-pkg:gtk-sharp-2.0

FILES = \
AssemblyInfo.cs \
Commands/NunitCommands.cs \
Gui/NunitTestTree.cs \
Gui/NunitResultTree.cs \
Services/AssemblyStore.cs \
Services/CancelTestsException.cs \
Services/CircleRenderer.cs \
Services/Delegates.cs \
Services/FixtureAddedEventArgs.cs \
Services/FixtureLoadErrorEventArgs.cs \
Services/NunitService.cs \
Services/QueuedEvent.cs \
Services/QueuedSuiteFinish.cs \
Services/QueuedSuiteStart.cs \
Services/QueuedTestEvent.cs \
Services/QueuedTestFinish.cs \
Services/QueuedTestStart.cs

all: $(ASSEMBLY)

../../build/AddIns/$(ADDIN): $(ADDIN)
	cp $(ADDIN) ../../build/AddIns/$(ADDIN)

$(ASSEMBLY): $(FILES) ../../build/AddIns/$(ADDIN)
	$(CSC) $(DLLS) $(FILES) -out:$@ -target:library

clean:
	rm -f $(ASSEMBLY) $(ASSEMBLY).mdb
	rm -f ../../build/AddIns/$(ADDIN)

