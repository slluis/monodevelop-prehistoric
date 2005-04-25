FILES = \
AssemblyInfo.cs \
MonoCombine.cs \
MonoMakefile.cs \
MonoMakefileFormat.cs \
MonoProject.cs \
Commands.cs \
MonoProjectConfiguration.cs

REFS = /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
       $(GTK_SHARP_LIBS) \
       $(GLADE_SHARP_LIBS) \
       $(GCONF_SHARP_LIBS) \
       $(GTKSOURCEVIEW_SHARP_LIBS)

ADDIN = MonoDeveloperExtensions.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDeveloperExtensions

DLL = $(ADDIN_BUILD)/MonoDeveloperExtensions.dll

all: $(ADDIN_BUILD)/$(ADDIN) $(DLL)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL): $(build_sources) $(srcdir)/$(GLADEFILE)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) -target:library -out:$@ $(build_sources) $(REFS)

CLEANFILES = $(DLL) $(DLL).mdb $(ADDIN_BUILD)/$(ADDIN)

EXTRA_DIST = $(FILES) $(ADDIN)

include $(top_srcdir)/Makefile.include

MonoDeveloperExtensionsdir = $(libdir)/monodevelop/AddIns/MonoDeveloperExtensions
MonoDeveloperExtensions_DATA = $(DLL) $(ADDIN_BUILD)/$(ADDIN)

