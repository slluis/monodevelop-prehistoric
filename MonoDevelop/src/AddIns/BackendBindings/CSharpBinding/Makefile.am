FILES= \
./Gui/ChooseRuntimePanel.cs \
./Gui/OutputOptionsPanel.cs \
./Gui/CodeGenerationPanel.cs \
./Parser/SharpDevelopTree/Event.cs \
./Parser/SharpDevelopTree/Indexer.cs \
./Parser/SharpDevelopTree/Constructor.cs \
./Parser/SharpDevelopTree/Method.cs \
./Parser/SharpDevelopTree/Class.cs \
./Parser/SharpDevelopTree/Parameter.cs \
./Parser/SharpDevelopTree/AttributeSection.cs \
./Parser/SharpDevelopTree/CompilationUnit.cs \
./Parser/SharpDevelopTree/Property.cs \
./Parser/SharpDevelopTree/Field.cs \
./Parser/SharpDevelopTree/ReturnType.cs \
./Parser/TypeVisitor.cs \
./Parser/CSharpVisitor.cs \
./Parser/Resolver.cs \
./Parser/Parser.cs \
./CSharpBindingExecutionManager.cs \
./CSharpAmbience.cs \
./Project/CSharpProject.cs \
./Project/CSharpCompilerParameters.cs \
./CSharpLanguageBinding.cs \
./FormattingStrategy/CSharpFormattingStrategy.cs \
./CSharpBindingCompilerManager.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = CSharpBinding.dll

all: $(DLL)

$(DLL): $(build_sources)
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/ICSharpCode.SharpRefactory.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll  \
		/r:gtk-sharp \
		/r:glade-sharp \
		/resource:$(srcdir)/../../../../data/resources/glade/CSharp.glade,CSharp.glade \
		$(build_sources) \
	&& cp $(DLL) ../../../../build/AddIns/AddIns/BackendBindings/.

csharpbindinglibdir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
csharpbindinglib_DATA = $(DLL)

CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

