
ASSEMBLY = ICSharpCode.AssemblyAnalyzer.Addin.dll
CSC = mcs /debug

REFS = \
	-pkg:gtk-sharp \
	-pkg:gecko-sharp \
	/r:System.Drawing.dll \
	/r:../../../../../build/bin/ICSharpCode.AssemblyAnalyzer.dll \
	/r:../../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../../build/bin/MonoDevelop.Base.dll

FILES = \
./Gui/AssemblyAnalyserControl.cs \
./Gui/AssemblyAnalyserView.cs \
./Gui/Panels/ResultListControl.cs \
./Gui/Panels/AssemblyRuleViewControl.cs \
./Gui/Panels/AssemblyTreeControl.cs \
./Gui/Panels/ResultDetailsView.cs \
./Commands.cs \
./AssemblyInfo.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) /out:$(ASSEMBLY) /target:library $(FILES) $(REFS) \
	&& cp $(ASSEMBLY) ../../../../../build/bin/.
