
DLL = MonoDevelop.Gui.Widgets.dll

REFERENCES = /r:System.Drawing \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Utils.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS) \
	$(GNOME_SHARP_LIBS)

FILES = \
Tree/TreeView.cs \
Tree/TreeNodeCollection.cs \
Tree/TreeNode.cs \
Dock/Docker.cs \
Dock/DockingManager.cs \
IconView/IconView.cs \
AssemblyInfo.cs \
FileBrowser/FileBrowser.cs \
FileSelector/FileSelector.cs \
FolderDialog/FolderDialog.cs \
FolderDialog/BaseFileEntry.cs \
FolderDialog/FolderEntry.cs \
FolderDialog/FileEntry.cs \
Navbar/Navbar.cs \
TabLabel/TabLabel.cs \
GladeWidgetExtract/GladeWidgetExtract.cs \
DragNotebook/DragNotebook.cs \
DataGrid/DataGrid.cs


build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(DLL)

$(DLL): $(build_sources)
	$(CSC) /debug /out:$(DLL) /target:library $(REFERENCES) $(build_sources) \
	&& cp $(DLL) $(top_builddir)/build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)

