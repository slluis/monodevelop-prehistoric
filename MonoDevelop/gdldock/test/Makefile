ASSEMBLY = ../../build/bin/GdlDockTest.exe

all:
	mcs -g GdlDockTest.cs /pkg:gnome-sharp /r:../../build/bin/gdl-sharp.dll -o $(ASSEMBLY)

run:
	LD_LIBRARY_PATH="../../build/bin" mono --debug $(ASSEMBLY)

clean:
	rm -f $(ASSEMBLY)
