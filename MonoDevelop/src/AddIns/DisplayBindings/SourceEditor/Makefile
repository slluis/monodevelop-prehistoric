DLL=../../../../build/bin/MonoDevelop.SourceEditor.dll

all : $(DLL)

$(DLL) : $(shell find . -name \*.cs)
	mcs $(shell find . -name \*.cs) /out:$@ /t:library /r:gtk-sharp /r:gdk-sharp /r:gtksourceview-sharp /r:glib-sharp \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Utils.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
		/r:pango-sharp.dll /r:gnome-sharp.dll /r:glade-sharp.dll \
		/r:gconf-sharp.dll \
		/r:System.Drawing.dll
