
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/BooBinding.dll

DLLS = -r:System.Drawing \
	-r:System.Xml \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	-r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(BOO_LIBS) \
	$(GTK_SHARP_LIBS) \
	$(GCONF_SHARP_LIBS) \
	$(GTKSOURCEVIEW_SHARP_LIBS)

FILES = \
Gui/ShellTextView.boo \
Gui/IShellModel.boo \
Gui/BooShellModel.boo \
Gui/OptionPanels/CodeCompilationPanel.boo \
Gui/OptionPanels/GeneralShellPanel.boo \
Gui/OptionPanels/GeneralBooShellPanel.boo \
Project/BooCompilerParameters.boo \
Properties/ShellProperties.boo \
Properties/BooShellProperties.boo \
BooBindingCompilerServices.boo \
BooAmbience.boo \
BooShellPadContent.boo \
BooCompiler.boo \
BooLanguageBinding.boo \
Parser/BooParser.boo \
Parser/Resolver.boo \
Parser/TypeMembers.boo \
Parser/ExpressionFinder.boo \
Parser/ReturnType.boo \
Parser/VariableLookupVisitor.boo \
Parser/ExpressionTypeVisitor.boo \
Parser/Tree.boo \
Parser/Visitor.boo

TEMPLATES = \
templates/BooGtkSharpProject.xpt.xml \
templates/BooGtkSharpWindow.xft.xml \
templates/EmptyBooFile.xft.xml \
templates/EmptyBooProject.xpt.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))

ADDIN = BooBinding.addin.xml


TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/, $(notdir $(TEMPLATES)))

src_TEMPLATES = $(addprefix $(srcdir)/, $(TEMPLATES))

if ENABLE_BOO
all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)
else
all:
endif

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
	$(BOOC) $(DLLS) $(build_sources) -o:$@ -t:library

if ENABLE_BOO
assemblydir = $(libdir)/monodevelop/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY) $(ADDIN)

templatedir = $(assemblydir)/templates
template_DATA = $(TEMPLATES)
endif

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) $(ADDIN) $(TEMPLATES)

