BASEDIR=$(srcdir)/../../..

FILES= \
./MonoDevelopMain.cs \
./Dialogs/AddInTreeSettingsHandler.cs \
./Dialogs/ExceptionBox.cs \
./AssemblyInfo.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.exe

all: $(DLL)

$(DLL): $(build_sources)
	mcs /debug /out:$(DLL) /r:System.Drawing \
		/r:../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../build/bin/MonoDevelop.Base.dll \
		/define:GTK \
		@GTKSHARP@ \
		@GNOMESHARP@ \
		$(build_sources) \
		/resource:$(BASEDIR)/data/resources/StringResources.resources,StringResources.resources \
		/resource:$(BASEDIR)/data/resources/SplashScreen.png,SplashScreen.png \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)           

CLEANFILES=$(DLL)

EXTRA_DIST = $(FILES)
