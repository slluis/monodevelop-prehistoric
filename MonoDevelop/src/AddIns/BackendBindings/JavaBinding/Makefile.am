
CSC = mcs /debug
ASSEMBLY = JavaBinding.dll
ADDIN = JavaBinding.addin

DLLS = /r:System.Drawing.dll \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../build/bin/ICSharpCode.SharpRefactory.dll \
	/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
	/r:gtk-sharp.dll

FILES = ./Gui/ProjectConfigurationPropertyPanel.cs \
./Project/JavaCompilerParameters.cs \
./Project/JavaProject.cs \
./JavaBindingCompilerServices.cs \
./JavaLanguageBinding.cs \
./FormatingStrategy/JavaFormattingStrategy.cs \
./JavaBindingExecutionServices.cs \
./ProjectTreeBuilder/JavaNodeBuilder.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) $(DLLS) $(FILES) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../../build/AddIns/AddIns/BackendBindings/.

assemblydir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings/
assembly_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)
EXTRA_DIST = $(FILES) $(ADDIN)

