
ASSEMBLY = ILAsmBinding.dll

DLLS = /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS)

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
	&& cp $(ASSEMBLY) $(top_builddir)/build/AddIns/AddIns/BackendBindings/.

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES)

