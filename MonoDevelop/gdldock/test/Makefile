ASSEMBLY = ../../build/bin/GdkDockTest.exe

all:
	mcs -g GdlDockTest.cs /r:gtk-sharp.dll /r:../../build/bin/gdl-sharp.dll /r:gnome-sharp.dll -o $(ASSEMBLY)

run:
	LD_LIBRARY_PATH="../../build/bin" mono --debug $(ASSEMBLY)

clean:
	rm -f $(ASSEMBLY)
