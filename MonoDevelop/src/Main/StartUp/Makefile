BASEDIR=../../..

SOURCES= \
./SharpDevelopMain.cs \
./Dialogs/AddInTreeSettingsHandler.cs \
./Dialogs/ExceptionBox.cs \
./AssemblyInfo.cs

RESOURCES= \
/resource:$(BASEDIR)/data/resources/StringResources.fr.resources \
/resource:$(BASEDIR)/data/resources/StringResources.cn-gb.resources \
/resource:$(BASEDIR)/data/resources/StringResources.jp.resources \
/resource:$(BASEDIR)/data/resources/StringResources.nl.resources \
/resource:$(BASEDIR)/data/resources/StringResources.pl.resources \
/resource:$(BASEDIR)/data/resources/StringResources.ro.resources \
/resource:$(BASEDIR)/data/resources/StringResources.pt.resources \
/resource:$(BASEDIR)/data/resources/StringResources.ru.resources \
/resource:$(BASEDIR)/data/resources/StringResources.goisern.resources \
/resource:$(BASEDIR)/data/resources/BitmapResources.resources \
/resource:$(BASEDIR)/data/resources/StringResources.se.resources \
/resource:$(BASEDIR)/data/resources/StringResources.es.resources \
/resource:$(BASEDIR)/data/resources/StringResources.cz.resources \
/resource:$(BASEDIR)/data/resources/StringResources.kr.resources \
/resource:$(BASEDIR)/data/resources/StringResources.it.resources \
/resource:$(BASEDIR)/data/resources/StringResources.bg.resources \
/resource:$(BASEDIR)/data/resources/StringResources.de.resources \
/resource:$(BASEDIR)/data/resources/StringResources.cn-big.resources \
/resource:$(BASEDIR)/data/resources/StringResources.dk.resources \
/resource:$(BASEDIR)/data/resources/StringResources.resources,StringResources.resources \
/resource:$(BASEDIR)/data/resources/StringResources.br.resources \
/resource:$(BASEDIR)/data/resources/SplashScreen.png,SplashScreen.png

DLL=../../../build/bin/SharpDevelop.exe

all: $(DLL)

$(DLL): $(SOURCES)
	@ mcs /debug /out:$(DLL) /r:System.Drawing.dll \
		/r:../../../build/bin/ICSharpCode.Core.dll \
		/r:../../../build/bin/SharpDevelop.Base.dll \
		/r:glib-sharp.dll \
		/define:GTK /r:gtk-sharp.dll /r:gdk-sharp.dll \
		$(RESOURCES) $(SOURCES)

