SOURCES= \
AssemblyInfo.cs \
ICSharpCodePage.cs \
Main.cs \
StartPage.cs

DLL=../../../../build/AddIns/AddIns/Misc/StartPage/ICSharpCode.StartPage.dll

all: $(DLL)

$(DLL): $(SOURCES)
	mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll \
		/r:../../../../build/bin/ICSharpCode.Core.dll \
		/r:../../../../build/bin/SharpDevelop.Base.dll \
		/r:gtk-sharp.dll /r:gtkmozembed-sharp.dll \
		$(SOURCES)
