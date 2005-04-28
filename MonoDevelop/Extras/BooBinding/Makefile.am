
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/BooBinding.dll
REMOTING_LIB = $(ADDIN_BUILD)/BooShellUnixRemoting.dll
BOOSHELL_LIB = $(ADDIN_BUILD)/BooShell.dll
BOOSHELL_SERVER_EXE = $(ADDIN_BUILD)/BooShellServer.exe

DLLS = -r:System.Drawing \
	-r:System.Xml \
	-r:System.Runtime.Remoting \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	-r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	-r:$(BOOSHELL_LIB) \
	-r:$(REMOTING_LIB) \
	$(BOO_LIBS) \
	$(GTK_SHARP_LIBS) \
	$(GCONF_SHARP_LIBS) \
	$(GTKSOURCEVIEW_SHARP_LIBS)

REMOTING_DLLS = -r:System.Runtime.Remoting \
		-r:Mono.Posix

BOOSHELL_DLLS = $(GTK_SHARP_LIBS) \
		$(BOO_LIBS)

BOOSHELL_SERVER_DLLS = -r:$(BOOSHELL_LIB) \
			-r:$(REMOTING_LIB) \
			-r:System.Runtime.Remoting \
			-r:Mono.Posix

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

REMOTING_FILES = Remoting/UnixChannel.cs \
Remoting/UnixClient.cs \
Remoting/UnixClientChannel.cs \
Remoting/UnixClientTransportSink.cs \
Remoting/UnixClientTransportSinkProvider.cs \
Remoting/UnixListener.cs \
Remoting/UnixMessageIO.cs \
Remoting/UnixServerChannel.cs \
Remoting/UnixServerTransportSink.cs

BOOSHELL_FILES = BooShell/BooShell.boo

BOOSHELL_SERVER_FILES= BooShellServer/BooShellServer.boo

TEMPLATES = \
templates/BooGtkSharpProject.xpt.xml \
templates/BooGtkSharpWindow.xft.xml \
templates/EmptyBooFile.xft.xml \
templates/EmptyBooProject.xpt.xml

build_sources = $(addprefix $(srcdir)/, $(FILES))
remoting_build_sources = $(addprefix $(srcdir)/, $(REMOTING_FILES))
booshell_build_sources = $(addprefix $(srcdir)/, $(BOOSHELL_FILES))
booshell_server_build_sources = $(addprefix $(srcdir)/, $(BOOSHELL_SERVER_FILES))

ADDIN = BooBinding.addin.xml

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/, $(notdir $(TEMPLATES)))

src_TEMPLATES = $(addprefix $(srcdir)/, $(TEMPLATES))

if ENABLE_BOO
all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES) \
	$(BOOSHELL_LIB) $(BOOSHELL_SERVER_EXE)
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

$(ASSEMBLY): $(FILES) $(REMOTING_LIB) $(BOOSHELL_LIB)
	mkdir -p $(ADDIN_BUILD)
	$(BOOC) $(DLLS) $(build_sources) -o:$@ -t:library

$(REMOTING_LIB): $(REMOTING_FILES)
	mkdir -p $(ADDIN_BUILD)
	$(MCS) $(REMOTING_DLLS) $(remoting_build_sources) -out:$@ -t:library

$(BOOSHELL_LIB): $(BOOSHELL_FILES)
	mkdir -p $(ADDIN_BUILD)
	$(BOOC) $(BOOSHELL_DLLS) $(booshell_build_sources) -o:$@ -t:library

$(BOOSHELL_SERVER_EXE): $(BOOSHELL_SERVER_FILES)
	mkdir -p $(ADDIN_BUILD)
	$(BOOC) $(BOOSHELL_SERVER_DLLS) $(booshell_server_build_sources) -o:$@

if ENABLE_BOO
assemblydir = $(libdir)/monodevelop/AddIns/BackendBindings
assembly_DATA = $(ASSEMBLY) $(ADDIN) $(REMOTING_LIB) $(BOOSHELL_LIB) \
		$(BOOSHELL_SERVER_EXE)

templatedir = $(assemblydir)/templates
template_DATA = $(TEMPLATES)
endif

CLEANFILES = $(ASSEMBLY) $(REMOTING_LIB) $(BOOSHELL_LIB) $(BOOSHELL_SERVER_EXE)
EXTRA_DIST = $(FILES) $(ADDIN) $(TEMPLATES) $(REMOTING_FILES) \
	     $(BOOSHELL_FILES) $(BOOSHELL_SERVER_FILES)

