BASEDIR=$(srcdir)/../../..

FILES= \
./MonoDevelopMain.cs \
./MonoDevelopOptions.cs \
./Dialogs/AddInTreeSettingsHandler.cs \
./Dialogs/ExceptionBox.cs \
./AssemblyInfo.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.exe

all: $(DLL)

REFS = /r:System.Drawing \
	/r:../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../build/bin/MonoDevelop.Base.dll \
	/r:Mono.Posix.dll \
	/r:Mono.GetOptions.dll \
	$(BASE_DEPENDENCIES_LIBS)

$(DLL): $(build_sources)
	mcs /debug /out:$(DLL) \
		/define:GTK $(REFS) \
		$(build_sources) \
		/resource:$(BASEDIR)/data/resources/SplashScreen.png,SplashScreen.png \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)           

CLEANFILES=$(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)
