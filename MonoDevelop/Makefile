PROGRAM=build/bin/SharpDevelop.exe
MONOPAD=build/bin/MonoPad.exe

$(PROGRAM): SharpDevelop.Base SharpDevelop.DefaultTexteditor CSharpBinding StartPage
	cd src/Main/StartUp && make

$(MONOPAD): ICSharpCode.TextEditor
	cd samples/MonoPad && make

StartPage:
	cd src/AddIns/Misc/StartPage && make

MonoDevelop.Gui:
	cd src/Libraries/MonoDevelop.Gui && make

CSharpBinding:
	cd src/AddIns/BackendBindings/CSharpBinding && make

SharpDevelop.DefaultTexteditor: ICSharpCode.TextEditor
	cd src/AddIns/DisplayBindings/TextEditor && make

SharpDevelop.Base: ICSharpCode.Core ICSharpCode.SharpRefactory ICSharpCode.SharpAssembly MonoDevelop.Gui
	cd src/Main/Base && make

ICSharpCode.Core:
	cd src/Main/Core && make

ICSharpCode.TextEditor:
	cd src/Libraries/ICSharpCode.TextEditor && make 

ICSharpCode.SharpRefactory:
	cd src/Libraries/SharpRefactory && make

ICSharpCode.SharpAssembly:
	cd src/Libraries/SharpAssembly && make

clean:
	find ./build/ -name '*.dll' | xargs rm -f
	find ./build/ -name '*.exe' | xargs rm -f

# Should change when IDE starts working
run: runsd

runsd: $(PROGRAM)
	cd build/bin && mono --debug SharpDevelop.exe
	
pad: $(MONOPAD)

runpad: $(MONOPAD)
	cd build/bin && mono MonoPad.exe

