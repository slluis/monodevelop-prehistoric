ASSEMBLY = CSharpBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll  \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS)

FILES = \
Gui/ChooseRuntimePanel.cs \
Gui/OutputOptionsPanel.cs \
Gui/CodeGenerationPanel.cs \
Parser/SharpDevelopTree/Event.cs \
Parser/SharpDevelopTree/Indexer.cs \
Parser/SharpDevelopTree/Constructor.cs \
Parser/SharpDevelopTree/Method.cs \
Parser/SharpDevelopTree/Class.cs \
Parser/SharpDevelopTree/Parameter.cs \
Parser/SharpDevelopTree/AttributeSection.cs \
Parser/SharpDevelopTree/CompilationUnit.cs \
Parser/SharpDevelopTree/Property.cs \
Parser/SharpDevelopTree/Field.cs \
Parser/SharpDevelopTree/ReturnType.cs \
Parser/TypeVisitor.cs \
Parser/CSharpVisitor.cs \
Parser/Resolver.cs \
Parser/Parser.cs \
Parser/ExpressionFinder.cs \
CSharpAmbience.cs \
Project/CSharpProject.cs \
Project/CSharpCompilerParameters.cs \
CSharpLanguageBinding.cs \
FormattingStrategy/CSharpFormattingStrategy.cs \
CSharpBindingCompilerManager.cs

TEMPLATES = \
templates/AppConfigFile.xft.xml \
templates/BlankCombine.xpt.xml \
templates/ConsoleProject.xpt.xml \
templates/CSharpSessionStateWebService.xft.xml \
templates/CSharpWebService.xft.xml \
templates/EmptyCSharpFile.xft.xml \
templates/EmptyHTMLFile.xft.xml \
templates/EmptyProject.xpt.xml \
templates/EmptyResourceFile.xft.xml \
templates/EmptyTextFile.xft.xml \
templates/EmptyXMLFile.xft.xml \
templates/GladeSharpProject.xpt.xml \
templates/GnomeSharpProgram.xft.xml \
templates/GnomeSharpProject.xpt.xml \
templates/GtkSharpProject.xpt.xml \
templates/GtkSharpWindow.xft.xml \
templates/Library.xpt.xml

build_sources = $(addprefix $(srcdir)/, $(FILES)) CSharpBindingExecutionManager.cs

ADDIN = CSharpBinding.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/,$(notdir $(TEMPLATES)))

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/templates/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(filter %.xft.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xft.xml: $(srcdir)/templates/%.xft.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@
				
$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(ASSEMBLY): $(build_sources) $(srcdir)/CSharp.glade
	mkdir -p $(ADDIN_BUILD)
	$(CSC) /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/CSharp.glade,CSharp.glade \
		$(build_sources) \
	&& cp $(ASSEMBLY) $(ADDIN_BUILD)/.

csharpbindinglibdir = $(libdir)/monodevelop/AddIns/BackendBindings
csharpbindinglib_DATA = $(ASSEMBLY) $(ADDIN)

templatesdir = $(csharpbindinglibdir)/templates
templates_DATA = $(TEMPLATES)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

DISTCLEANFILES = CSharpBindingExecutionManager.cs

EXTRA_DIST = $(FILES) CSharpBindingExecutionManager.cs.in CSharp.glade $(ADDIN) $(TEMPLATES)

