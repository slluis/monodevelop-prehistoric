
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/JavaBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS)

FILES = \
Gui/ProjectConfigurationPropertyPanel.cs \
Project/JavaCompilerParameters.cs \
JavaBindingCompilerServices.cs \
JavaCompiler.cs \
JavaLanguageBinding.cs

TEMPLATES = \
templates/EmptyJavaFile.xft.xml \
templates/EmptyJavaProject.xpt.xml \
templates/IkvmConsoleApplicationProject.xpt.xml \
templates/IkvmGladeApplicationProject.xpt.xml \
templates/IkvmGnomeApplicationProject.xpt.xml \
templates/IkvmGtkApplicationProject.xpt.xml \
templates/JavaApplet.xft.xml \
templates/JavaApplicationProject.xpt.xml \
templates/JavaApplication.xft.xml \
templates/JavaConsoleApplicationProject.xpt.xml \
templates/JavaDialog.xft.xml \
templates/JavaFrame.xft.xml \
templates/JavaOKDialog.xft.xml \
templates/JavaPanel.xft.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))

ADDIN = JavaBinding.addin.xml


TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/, $(notdir $(TEMPLATES)))

src_TEMPLATES = $(addprefix $(srcdir)/, $(TEMPLATES))

if ENABLE_JAVA
all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)
else
all:
endif

#%.xft.xml: $(srcdir)/templates/%.xft.xml
#	mkdir -p $(TEMPLATES_DIR)
#	cp $(srcdir)/templates/$(notdir $@) $@
#
#%.xpt.xml: $(srcdir)/templates/%.xpt.xml
#	mkdir -p $(TEMPLATES_DIR)
#	cp $(srcdir)/templates/$(notdir $@) $@

$(filter %.xft.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xft.xml: $(srcdir)/templates/%.xft.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/templates/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(FILES)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(DLLS) $(build_sources) -out:$@ -target:library

if ENABLE_JAVA
assemblydir = $(libdir)/monodevelop/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY) $(ADDIN)

templatedir = $(assemblydir)/templates
template_DATA = $(TEMPLATES)
endif

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) $(ADDIN) $(TEMPLATES)

