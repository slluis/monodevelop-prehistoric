FILES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.StartPage.dll

all: $(DLL)

$(DLL): $(build_sources)
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		@GECKO_SHARP@ \
		/r:gtk-sharp \
		$(build_sources) \
	&& cp $(DLL) ../../../../build/AddIns/AddIns/Misc/StartPage/.

startpagelibdir = $(libdir)/monodevelop/AddIns/AddIns/Misc/StartPage
startpagelib_DATA = $(DLL)

CLEANFILES=$(DLL)

EXTRA_DIST = $(FILES)

