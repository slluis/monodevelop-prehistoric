FILES= \
MonoDevelopMain.cs \
MonoDevelopOptions.cs \
Dialogs/AddInTreeSettingsHandler.cs \
AssemblyInfo.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = $(top_builddir)/build/bin/MonoDevelop.exe

all: $(DLL)

REFS = /r:System.Drawing \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:Mono.Posix.dll \
	/r:Mono.GetOptions.dll \
	$(GTK_SHARP_LIBS) \
	$(GNOME_SHARP_LIBS)

$(DLL): $(build_sources)
	$(CSC) -out:$@ -define:GTK $(REFS) $(build_sources) \
		-resource:$(srcdir)/SplashScreen.png,SplashScreen.png

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)           

CLEANFILES=$(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES) SplashScreen.png
