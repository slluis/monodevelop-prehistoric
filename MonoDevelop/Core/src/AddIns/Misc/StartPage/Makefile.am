FILES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.StartPage.dll

REFS = /r:System.Drawing \
	/r:../../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../../build/bin/MonoDevelop.Base.dll \
	$(GTK_SHARP_LIBS) \
	$(GECKO_SHARP_LIBS)

all: $(DLL)

$(DLL): $(build_sources)
	$(CSC) /out:$(DLL) /target:library $(REFS) \
		$(build_sources) \
	&& cp $(DLL) ../../../../../build/AddIns/AddIns/Misc/StartPage/.

startpagelibdir = $(libdir)/monodevelop/AddIns/AddIns/Misc/StartPage
startpagelib_DATA = $(DLL)

CLEANFILES=$(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)

