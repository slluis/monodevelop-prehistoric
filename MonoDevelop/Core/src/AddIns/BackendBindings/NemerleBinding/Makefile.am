
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/NemerleBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS)

FILES = \
Gui/CodeGenerationPanel.cs \
Project/NemerleParameters.cs \
NemerleBindingCompilerServices.cs \
NemerleLanguageBinding.cs 

TEMPLATES = \
NemerleEmptyProject.xpt.xml \
NemerleGnomeSharpProject.xpt.xml \
NemerleGtkSharpProject.xpt.xml \
NemerleGtkSharpWindow.xft.xml \
EmptyNemerleFile.xft.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))

build_TEMPLATES = $(addprefix $(ADDIN_BUILD)/templates/, $(TEMPLATES))

ADDIN = NemerleBinding.addin.xml

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)

$(filter %.xft.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xft.xml: $(srcdir)/%.xft.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/$(notdir $@) $@

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(FILES) $(srcdir)/Nemerle.glade
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(DLLS) $(build_sources) -out:$@ -target:library \
	-resource:$(srcdir)/Nemerle.glade,Nemerle.glade

assemblydir = $(libdir)/monodevelop/AddIns/BackendBindings/
assembly_DATA = $(ASSEMBLY) $(ADDIN)

templatedir = $(assemblydir)/templates
template_DATA = $(TEMPLATES)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) Nemerle.glade $(ADDIN) $(TEMPLATES)
