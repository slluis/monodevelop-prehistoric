
CSC = mcs /debug
ASSEMBLY = JavaBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
	/r:gtk-sharp.dll

FILES = ./Gui/ProjectConfigurationPropertyPanel.cs \
./Project/JavaCompilerParameters.cs \
./Project/JavaProject.cs \
./JavaBindingCompilerServices.cs \
./JavaCompiler.cs \
./JavaLanguageBinding.cs \
./JavaRuntime.cs \
./FormatingStrategy/JavaFormattingStrategy.cs \
./JavaBindingExecutionServices.cs \
./ProjectTreeBuilder/JavaNodeBuilder.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) $(DLLS) $(build_sources) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../../build/AddIns/AddIns/BackendBindings/.

assemblydir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)
EXTRA_DIST = $(FILES)

