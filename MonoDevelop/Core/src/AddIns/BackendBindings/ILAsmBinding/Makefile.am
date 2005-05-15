
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/ILAsmBinding.dll

DLLS = /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS)

FILES = \
Gui/CompilerParametersPanel.cs \
Project/ILAsmCompilerParameters.cs \
ILAsmCompilerManager.cs \
AssemblyInfo.cs \
ILAsmLanguageBinding.cs

TEMPLATES = \
ILAsmConsoleProject.xpt.xml

ADDIN = ILAsmBinding.addin.xml


TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/,$(TEMPLATES))

assemblydir = $(MD_ADDIN_DIR)/BackendBindings
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
	$(CSC) $(build_sources) $(DLLS) -out:$@ -target:library

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) $(ADDIN) $(TEMPLATES)
include $(top_srcdir)/Makefile.include

