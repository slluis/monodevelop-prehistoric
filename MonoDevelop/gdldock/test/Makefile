all:
	mcs -g GdlDockTest.cs /r:gtk-sharp.dll /r:../../build/bin/gdl-sharp.dll /r:gnome-sharp.dll -o GdlDockTest.exe

run:
	MONO_PATH="../../build/bin" mono --debug GdlDockTest.exe

clean:
	rm -f GdlDockTest.exe
