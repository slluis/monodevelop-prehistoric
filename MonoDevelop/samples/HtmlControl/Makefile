all:
	mcs -g Test.cs -r gtk-sharp -r ../../build/bin/MonoDevelop.Gui.dll -o ../../build/bin/HtmlTest.exe

run:
	mono --debug ../../build/bin/HtmlTest.exe

clean:
	rm -f ../../build/bin/HtmlTest.exe
