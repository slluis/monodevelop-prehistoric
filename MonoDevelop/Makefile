PROGRAM=build/bin/SharpDevelop.exe
MONOPAD=build/bin/MonoPad.exe
QMAKE=@ $(MAKE)
$(PROGRAM): SharpDevelop.Base SharpDevelop.DefaultTexteditor CSharpBinding StartPage
	@ echo Building $@
	$(QMAKE) -C src/Main/StartUp

$(MONOPAD): ICSharpCode.TextEditor
	@ echo Building $@
	$(QMAKE) -C samples/MonoPad

StartPage:
	@ echo Building $@
	$(QMAKE) -C src/AddIns/Misc/StartPage

MonoDevelop.Gui:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/MonoDevelop.Gui

CSharpBinding:
	@ echo Building $@
	$(QMAKE) -C src/AddIns/BackendBindings/CSharpBinding

SharpDevelop.DefaultTexteditor: ICSharpCode.TextEditor
	@ echo Building $@
	$(QMAKE) -C src/AddIns/DisplayBindings/TextEditor

SharpDevelop.Base: ICSharpCode.Core ICSharpCode.SharpRefactory ICSharpCode.SharpAssembly MonoDevelop.Gui
	@ echo Building $@
	$(QMAKE) -C src/Main/Base

ICSharpCode.Core:
	@ echo Building $@
	$(QMAKE) -C src/Main/Core

ICSharpCode.TextEditor:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/ICSharpCode.TextEditor 

ICSharpCode.SharpRefactory:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/SharpRefactory

ICSharpCode.SharpAssembly:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/SharpAssembly

clean:
	find ./build/ -name '*.dll' | xargs rm -f
	find ./build/ -name '*.exe' | xargs rm -f

# Should change when IDE starts working
run: runsd

runsd: $(PROGRAM)
	./monodevelop

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

run-profile:
	cd build/bin && mono --profile SharpDevelop.exe
