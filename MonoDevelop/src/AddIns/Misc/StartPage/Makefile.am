FILES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

DLL = MonoDevelop.StartPage.dll

all: $(DLL)

$(DLL): $(FILES)
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/gtkmozembed-sharp.dll \
		/r:gtk-sharp.dll \
		$(FILES) \
	&& cp $(DLL) ../../../../build/AddIns/AddIns/Misc/StartPage/.

startpagelibdir = $(libdir)/monodevelop/AddIns/AddIns/Misc/StartPage
startpagelib_DATA = $(DLL)

CLEANFILES=$(DLL)

EXTRA_DIST = $(FILES)

