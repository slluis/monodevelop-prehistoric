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

DLL = CSharpBinding.dll

all: $(DLL)

$(DLL): $(FILES)
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/ICSharpCode.SharpRefactory.dll \
		/r:gtk-sharp \
		$(FILES) \
	&& cp $(DLL) ../../../../build/AddIns/AddIns/BackendBindings/.

csharpbindinglibdir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
csharpbindinglib_DATA = $(DLL)

CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

