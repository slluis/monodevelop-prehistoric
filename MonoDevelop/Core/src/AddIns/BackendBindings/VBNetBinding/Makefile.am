ASSEMBLY = VBNetBinding.dll

SUBDIRS = SharpRefactoryVB

DLLS = /r:System.Drawing.dll \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../build/bin/ICSharpCode.SharpRefactory.dll \
	/r:../../../../build/bin/ICSharpCode.SharpRefactory.VB.dll \
	/r:../../../../build/bin/ICSharpCode.SharpAssembly.dll \
	/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll  \
	$(BASE_DEPENDENCIES_LIBS)

FILES = \
./AssemblyInfo.cs \
./VBAmbience.cs \
./VBBindingCompilerServices.cs \
./VBBindingExecutionServices.cs \
./VBLanguageBinding.cs \
./Gui/CodeGenerationPanel.cs \
./Gui/OutputOptionsPanel.cs \
./Parser/ExpressionFinder.cs \
./Parser/Parser.cs \
./Parser/Resolver.cs \
./Parser/TypeVisitor.cs \
./Parser/VBNetVisitor.cs \
./Parser/SharpDevelopTree/AttributeSection.cs \
./Parser/SharpDevelopTree/Class.cs \
./Parser/SharpDevelopTree/CompilationUnit.cs \
./Parser/SharpDevelopTree/Constructor.cs \
./Parser/SharpDevelopTree/Event.cs \
./Parser/SharpDevelopTree/Field.cs \
./Parser/SharpDevelopTree/Indexer.cs \
./Parser/SharpDevelopTree/Method.cs \
./Parser/SharpDevelopTree/Parameter.cs \
./Parser/SharpDevelopTree/Property.cs \
./Parser/SharpDevelopTree/ReturnType.cs \
./Project/VBCompilerParameters.cs \
./Project/VBProject.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

$(ASSEMBLY): $(build_sources)
	cd SharpRefactoryVB && make
	$(CSC) /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/../../../../data/resources/glade/VB.glade,VB.glade \
		$(build_sources) \
	&& cp $(ASSEMBLY) ../../../../build/AddIns/AddIns/BackendBindings/.

vbnetbindinglibdir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings
vbnetbindinglib_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

DISTCLEANFILES =

EXTRA_DIST = $(FILES)

