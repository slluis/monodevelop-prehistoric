DLL=../../../build/bin/MonoDevelop.Gui.dll
REFERENCES=/r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /r:glade-sharp.dll

all: $(DLL)

super-docker: */*.cs
	mcs /debug /out:$(DLL) /target:library $(REFERENCES) /define:EXPERIMENTAL_DOCKER /recurse:*.cs

$(DLL): */*.cs
	mcs /debug /out:$(DLL) /target:library $(REFERENCES) /recurse:*.cs

