RUNTIME = mono
DOCK = ../../../build/bin/MonoDevelop.Dock.dll
TEST = dock-test.exe

GLADE = layout.glade

IMAGES = stock-close-12.png stock-menu-left-12.png stock-menu-right-12.png

RESOURCES = -resource:$(GLADE) -resource:stock-close-12.png -resource:stock-menu-left-12.png -resource:stock-menu-right-12.png

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DOCK)

REFS = $(GTK_SHARP_LIBS) $(GLADE_SHARP_LIBS)

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
	Dock.cs \
	DockPaned.cs \
	DockItemBehavior.cs \
	ExportAttribute.cs \
	AfterAttribute.cs

TEST_FILES = GdlDockTest.cs

all: $(DOCK) $(TEST)

CLEANFILES = $(DOCK) $(DOCK).mdb $(TEST) $(TEST).mdb
EXTRA_DIST = $(FILES) $(GLADE) $(IMAGES) $(TEST_FILES)

$(DOCK): $(FILES) $(GLADE) $(IMAGES)
	mkdir -p ../../../build/bin/
	$(CSC) -out:$@ -target:library $(FILES) $(RESOURCES) $(REFS)

$(TEST) : $(TEST_FILES) $(DOCK)
	$(CSC) -out:$@ -r:$(DOCK) -pkg:gtk-sharp-2.0 $(TEST_FILES) -codepage:utf8

run-test: $(TEST)
	MONO_PATH=../../../build/bin $(RUNTIME) $(TEST)

