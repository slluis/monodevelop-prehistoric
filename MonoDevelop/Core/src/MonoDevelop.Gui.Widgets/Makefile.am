
DLL = $(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll

REFERENCES = /r:System.Drawing \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Utils.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS) \
	$(GNOME_SHARP_LIBS) \
	$(GNOME_VFS_SHARP_LIBS)

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
	$(CSC) -out:$@ -target:library $(REFERENCES) $(build_sources)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)

