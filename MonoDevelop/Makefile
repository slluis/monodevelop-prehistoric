PROGRAM=build/bin/MonoDevelop.exe
MONOPAD=build/bin/MonoPad.exe
GTKMOZEMBED=build/bin/gtkmozembed-sharp.dll
QMAKE=@ $(MAKE)

$(PROGRAM): $(GTKMOZEMBED) MonoDevelop.Base MonoDevelop.EditorBindings MonoDevelop.DefaultTexteditor SourceEditor CSharpBinding StartPage
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

MonoDevelop.Gui.Widgets:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/MonoDevelop.Gui.Widgets

MonoDevelop.Gui.Utils:
	@ echo Building $@
	$(QMAKE) -C src/Libraries/MonoDevelop.Gui.Utils

CSharpBinding:
	@ echo Building $@
	$(QMAKE) -C src/AddIns/BackendBindings/CSharpBinding

MonoDevelop.DefaultTexteditor: MonoDevelop.TextEditor
	@ echo Building $@
	$(QMAKE) -C src/AddIns/DisplayBindings/TextEditor

MonoDevelop.Base: MonoDevelop.Core ICSharpCode.SharpRefactory ICSharpCode.SharpAssembly MonoDevelop.Gui.Utils MonoDevelop.Gui.Widgets
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

SourceEditor :
	@ pkg-config gtksourceview-1.0  --atleast-version=0.7 || \
	 (echo "You must have GtkSourceView, version 0.7 or greater installed, as well as the c# bindings" && exit 1)
	@ test -f `pkg-config gtk-sharp --variable=prefix`/lib/gtksourceview-sharp.dll || \
	 (echo "You must have the GtkSourceView bindings installed" && exit 1)
	@ echo Building $@
	$(QMAKE) -C src/AddIns/DisplayBindings/SourceEditor

MonoDevelop.EditorBindings :
	@ echo Building $@
	$(QMAKE) -C src/AddIns/DisplayBindings/EditorBindings
	
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

newed : $(PROGRAM)
	NEW_EDITOR=true ./monodevelop

run-profile: $(PROGRAM)
	cd build/bin && mono -O=inline --profile MonoDevelop.exe

profile-pad: $(MONOPAD)
	cd build/bin && mono -O=inline --profile MonoPad.exe
