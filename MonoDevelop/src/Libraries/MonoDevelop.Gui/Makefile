DLL=../../../build/bin/MonoDevelop.Gui.dll

all: $(DLL)

super-docker: */*.cs
	mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /define:EXPERIMENTAL_DOCKER /recurse:*.cs

$(DLL): */*.cs
	mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /recurse:*.cs

