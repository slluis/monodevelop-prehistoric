FILES = \
AssemblyInfo.cs \
MonoCombine.cs \
MonoMakefile.cs \
MonoMakefileFormat.cs \
MonoProject.cs \
Commands.cs \
MonoProjectConfiguration.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

REFS = /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
       $(GTK_SHARP_LIBS) \
       $(GLADE_SHARP_LIBS) \
       $(GCONF_SHARP_LIBS) \
       $(GTKSOURCEVIEW_SHARP_LIBS)

ADDIN = MonoDeveloperExtensions.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDeveloperExtensions

DLL = MonoDeveloperExtensions.dll

DLL_BUILD = $(top_builddir)/build/AddIns/MonoDeveloperExtensions

all: $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL): $(build_sources) $(srcdir)/$(GLADEFILE)
	mcs -target:library -out:$(DLL) $(build_sources) $(REFS)

$(DLL_BUILD)/$(DLL): $(srcdir)/$(DLL)
	mkdir -p $(DLL_BUILD)
	cp $(srcdir)/$(DLL) $(DLL_BUILD)/$(DLL)

CLEANFILES=$(DLL) $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)

EXTRADIST=$(FILES)

MonoDeveloperExtensionsdir = $(libdir)/monodevelop/AddIns/MonoDeveloperExtensions
MonoDeveloperExtensions_DATA = $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)
