
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

TEMPLATES = \
ILAsmConsoleProject.xpt.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))

ADDIN = ILAsmBinding.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/,$(TEMPLATES))

assemblydir = $(libdir)/monodevelop/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY) $(ADDIN)

templatesdir = $(assemblydir)/templates
templates_DATA = $(TEMPLATES)

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(build_sources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(build_sources) $(DLLS) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) $(ADDIN_BUILD)/.

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) $(ADDIN) $(TEMPLATES)

