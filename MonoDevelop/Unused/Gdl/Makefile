CSC = mcs
GDL = MonoDevelop.Dock.dll
TEST = dock-test.exe

GLADE = layout.glade

IMAGES = stock-close-12.png stock-menu-left-12.png stock-menu-right-12.png

RESOURCES = -resource:$(GLADE) -resource:stock-close-12.png -resource:stock-menu-left-12.png -resource:stock-menu-right-12.png

REFS = -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0

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
	DockItemBehavior.cs \
	IPersistLayout.cs

all: $(GDL) $(TEST)

clean:
	rm -f $(GDL) $(GDL).mdb
	rm -f $(TEST) $(TEST).mdb

$(GDL): $(FILES) $(GLADE) $(IMAGES)
	$(CSC) -out:$@ -target:library $(FILES) $(RESOURCES) $(REFS)

$(TEST) : GdlDockTest.cs $(GDL)
	$(CSC) -out:$@ -r:$(GDL) -pkg:gtk-sharp-2.0 GdlDockTest.cs -codepage:utf8

run-test: $(TEST)
	mono $(TEST)

