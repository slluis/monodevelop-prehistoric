ASSEMBLY = CSharpBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../build/bin/ICSharpCode.SharpRefactory.dll \
	/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll  \
	$(BASE_DEPENDENCIES_LIBS)

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

$(ASSEMBLY): $(build_sources)
	$(CSC) /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/../../../../data/resources/glade/CSharp.glade,CSharp.glade \
		$(build_sources) \
	&& cp $(ASSEMBLY) ../../../../build/AddIns/AddIns/BackendBindings/.

csharpbindinglibdir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
csharpbindinglib_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

DISTCLEANFILES = CSharpBindingExecutionManager.cs

EXTRA_DIST = $(FILES) CSharpBindingExecutionManager.cs.in

