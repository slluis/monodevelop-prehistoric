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

build_sources = $(addprefix $(srcdir)/, $(FILES)) CSharpBindingExecutionManager.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(srcdir)/CSharp.glade
	$(CSC) /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/CSharp.glade,CSharp.glade \
		$(build_sources) \
	&& cp $(ASSEMBLY) $(top_builddir)/build/AddIns/AddIns/BackendBindings/.

csharpbindinglibdir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
csharpbindinglib_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

DISTCLEANFILES = CSharpBindingExecutionManager.cs

EXTRA_DIST = $(FILES) CSharpBindingExecutionManager.cs.in CSharp.glade

