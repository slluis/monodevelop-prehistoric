
ASSEMBLY = ILAsmBinding.dll

DLLS = /r:../../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
	$(BASE_DEPENDENCIES_LIBS)

FILES = \
Gui/CompilerParametersPanel.cs \
Project/CompilationTarget.cs \
Project/ILAsmProject.cs \
Project/ILAsmCompilerParameters.cs \
ILAsmCompilerManager.cs \
AssemblyInfo.cs \
ILAsmLanguageBinding.cs \
ILAsmExecutionManager.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

assemblydir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(build_sources)
	$(CSC) $(build_sources) $(DLLS) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../../../build/AddIns/AddIns/BackendBindings/.

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES)

