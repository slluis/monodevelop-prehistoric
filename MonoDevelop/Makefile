PROGRAM=build/bin/MonoDevelop.exe
MONOPAD=build/bin/MonoPad.exe
GTKMOZEMBED=build/bin/gtkmozembed-sharp.dll
QMAKE=@ $(MAKE)

$(PROGRAM): $(GTKMOZEMBED) MonoDevelop.Base MonoDevelop.DefaultTexteditor CSharpBinding StartPage
	@ echo Building $@
	$(QMAKE) -C src/Main/StartUp

$(GTKMOZEMBED):
	@ echo Building $@
	$(QMAKE) -C gtkmozembed-sharp

$(MONOPAD): MonoDevelop.TextEditor
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

MonoDevelop.DefaultTexteditor: MonoDevelop.TextEditor
	@ echo Building $@
	$(QMAKE) -C src/AddIns/DisplayBindings/TextEditor

MonoDevelop.Base: MonoDevelop.Core ICSharpCode.SharpRefactory ICSharpCode.SharpAssembly MonoDevelop.Gui
	@ echo Building $@
	$(QMAKE) -C src/Main/Base

MonoDevelop.Core:
	@ echo Building $@
	$(QMAKE) -C src/Main/Core

MonoDevelop.TextEditor:
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
run: runmd

runmd: $(PROGRAM)
	./monodevelop

pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe
