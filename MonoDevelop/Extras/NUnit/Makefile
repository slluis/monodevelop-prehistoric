
CSC = mcs
ASSEMBLY_NAME = MonoDevelop.NUnit.dll
ASSEMBLY = ../../build/AddIns/NUnit/$(ASSEMBLY_NAME)
ADDIN = MonoDevelopNUnit.addin.xml

DLLS = -r:nunit.framework.dll \
	-r:nunit.core.dll \
	-r:../../build/bin/MonoDevelop.Core.dll \
	-r:../../build/bin/MonoDevelop.Base.dll \
	-r:../../build/bin/MonoDevelop.Gui.Widgets.dll \
	-pkg:gtk-sharp-2.0

FILES = \
AssemblyInfo.cs \
FixtureAddedEventHandler.cs \
FixtureLoadedErrorEventHandler.cs \
Commands/NUnitCommands.cs \
Gui/CircleImage.cs \
Gui/TestPad.cs \
Gui/ResultsView.cs \
Services/NUnitService.cs

all: $(ASSEMBLY)

../../build/AddIns/NUnit/$(ADDIN): $(ADDIN)
	mkdir -p ../../build/AddIns/NUnit
	cp $(ADDIN) ../../build/AddIns/NUnit/$(ADDIN)

$(ASSEMBLY): $(FILES) ../../build/AddIns/NUnit/$(ADDIN)
	mkdir -p ../../build/AddIns/NUnit
	$(CSC) $(DLLS) $(FILES) -out:$@ -target:library

clean:
	rm -f $(ASSEMBLY) $(ASSEMBLY).mdb
	rm -f ../../build/AddIns/NUnit/$(ADDIN)

