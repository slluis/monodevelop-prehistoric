
ASSEMBLY = NemerleBinding.dll

DLLS = /r:System.Drawing.dll \
	/r:../../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../../build/bin/MonoDevelop.SourceEditor.dll \
	/r:../../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../../build/bin/ICSharpCode.SharpRefactory.dll \
	/r:../../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS)

FILES = \
Gui/CodeGenerationPanel.cs \
Gui/OutputPanel.cs \
Project/NemerleParameters.cs \
Project/NemerleProject.cs \
NemerleBindingCompilerServices.cs \
NemerleLanguageBinding.cs 

build_sources = $(addprefix $(srcdir)/, $(FILES)) NemerleBindingExecutionServices.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES) $(srcdir)/Nemerle.glade
	$(CSC) $(DLLS) $(build_sources) /out:$(ASSEMBLY) /target:library \
	/resource:$(srcdir)/Nemerle.glade,Nemerle.glade \
	&& cp $(ASSEMBLY) ../../../../../build/AddIns/AddIns/BackendBindings/.

assemblydir = $(libdir)/monodevelop/AddIns/AddIns/BackendBindings/
assembly_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
DISTCLEANFILES = NemerleBindingExecutionServices.cs
EXTRA_DIST = $(FILES) NemerleBindingExecutionServices.cs.in Nemerle.glade
