CSC = mcs
GDL = MonoDevelop.Dock.dll
TEST = dock-test.exe

FILES = AssemblyInfo.cs \
	DockedHandler.cs \
	DockMaster.cs \
	Stock.cs \
	DockObjectFlags.cs \
	DockNotebook.cs \
	DockRequest.cs \
	DockBar.cs \
	DockBarButton.cs \
	DockLayout.cs \
	DockPlacement.cs \
	DetachedHandler.cs \
	DockItemGrip.cs \
	DockItem.cs \
	DockPlaceholder.cs \
	DockObject.cs \
	DockParamFlags.cs \
	Dock.cs \
	DockPaned.cs \
	DockItemBehavior.cs

all: $(GDL) $(TEST)

clean:
	rm -f $(GDL) $(GDL).mdb
	rm -f $(TEST) $(TEST).mdb

$(GDL): $(FILES)
	$(CSC) -out:$@ -target:library $(FILES) -pkg:gtk-sharp-2.0

$(TEST) : GdlDockTest.cs $(GDL)
	$(CSC) -out:$@ -r:$(GDL) -pkg:gtk-sharp-2.0 GdlDockTest.cs

run-test: $(TEST)
	mono $(TEST)

