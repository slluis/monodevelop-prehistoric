
ASSEMBLY = JavaBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS)

FILES = \
Gui/ProjectConfigurationPropertyPanel.cs \
Project/JavaCompilerParameters.cs \
Project/JavaProject.cs \
JavaBindingCompilerServices.cs \
JavaCompiler.cs \
JavaLanguageBinding.cs \
FormatingStrategy/JavaFormattingStrategy.cs \
JavaBindingExecutionServices.cs \
ProjectTreeBuilder/JavaNodeBuilder.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) $(DLLS) $(build_sources) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) $(top_builddir)/build/AddIns/AddIns/BackendBindings/.

assemblydir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES)

