FILES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.StartPage.dll

REFS = /r:System.Drawing \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	$(GTK_SHARP_LIBS) \
	$(GECKO_SHARP_LIBS)

ADDIN = StartPage.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/StartPage

all: $(DLL) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL): $(build_sources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) /out:$(DLL) /target:library $(REFS) \
		$(build_sources) \
	&& cp $(DLL) $(ADDIN_BUILD)/.

startpagelibdir = $(libdir)/monodevelop/AddIns/StartPage
startpagelib_DATA = $(DLL) $(ADDIN)

CLEANFILES=$(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES) $(ADDIN)

