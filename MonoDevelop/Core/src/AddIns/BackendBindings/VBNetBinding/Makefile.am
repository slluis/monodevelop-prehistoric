ASSEMBLY = VBNetBinding.dll

SUBDIRS = SharpRefactoryVB

DLLS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.VB.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpAssembly.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS)

FILES = \
AssemblyInfo.cs \
VBAmbience.cs \
VBBindingCompilerServices.cs \
VBLanguageBinding.cs \
Gui/CodeGenerationPanel.cs \
Parser/ExpressionFinder.cs \
Parser/Parser.cs \
Parser/Resolver.cs \
Parser/TypeVisitor.cs \
Parser/VBNetVisitor.cs \
Parser/SharpDevelopTree/AttributeSection.cs \
Parser/SharpDevelopTree/Class.cs \
Parser/SharpDevelopTree/CompilationUnit.cs \
Parser/SharpDevelopTree/Constructor.cs \
Parser/SharpDevelopTree/Event.cs \
Parser/SharpDevelopTree/Field.cs \
Parser/SharpDevelopTree/Indexer.cs \
Parser/SharpDevelopTree/Method.cs \
Parser/SharpDevelopTree/Parameter.cs \
Parser/SharpDevelopTree/Property.cs \
Parser/SharpDevelopTree/ReturnType.cs \
Project/VBCompilerParameters.cs

TEMPLATES = \
templates/EmptyVBFile.xft.xml \
templates/EmptyVBProject.xpt.xml \
templates/VBConsoleApplicationProject.xpt.xml \
templates/VBGtkSharpProject.xpt.xml \
templates/VBGtkSharp2Project.xpt.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))

ADDIN = VBNetBinding.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/, $(notdir $(TEMPLATES)))

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/templates/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(filter %.xft.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xft.xml: $(srcdir)/templates/%.xft.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(build_sources) $(srcdir)/VB.glade
	mkdir -p $(ADDIN_BUILD)
	cd SharpRefactoryVB && make
	$(CSC) /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/VB.glade,VB.glade \
		$(build_sources) \
	&& cp $(ASSEMBLY) $(ADDIN_BUILD)/.

vbnetbindinglibdir = $(libdir)/monodevelop/AddIns/BackendBindings
vbnetbindinglib_DATA = $(ASSEMBLY) $(ADDIN)

templatesdir = $(vbnetbindinglibdir)/templates/
templates_DATA = $(TEMPLATES)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

DISTCLEANFILES =

EXTRA_DIST = $(FILES) VB.glade $(ADDIN) $(TEMPLATES)

