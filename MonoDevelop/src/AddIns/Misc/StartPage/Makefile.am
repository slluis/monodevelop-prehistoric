SOURCES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

DLL=../../../../build/AddIns/AddIns/Misc/StartPage/MonoDevelop.StartPage.dll

all: $(DLL)

$(DLL): $(SOURCES)
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/gtkmozembed-sharp.dll \
		/r:gtk-sharp.dll \
		$(SOURCES)

CLEANFILES=$(DLL)
