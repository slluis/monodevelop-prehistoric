DLL=../../../../build/bin/MonoDevelop.EditorBindings.dll

all : $(DLL)

$(DLL) : $(shell find . -name \*.cs)
	mcs $(shell find . -name \*.cs) /out:$@ /t:library /r:gtk-sharp /r:gdk-sharp \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Utils.dll
