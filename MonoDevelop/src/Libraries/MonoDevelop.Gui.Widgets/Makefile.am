
MCS = mcs -debug

DLL = MonoDevelop.Gui.Widgets.dll

REFERENCES = /r:System.Drawing.dll /r:gtk-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:gnome-sharp.dll /r:glib-sharp.dll /r:glade-sharp.dll /r:../../../build/bin/MonoDevelop.Core.dll /r:../../../build/bin/MonoDevelop.Gui.Utils.dll

FILES = ./Tree/TreeView.cs \
./Tree/TreeNodeCollection.cs \
./Tree/TreeNode.cs \
./Dock/Docker.cs \
./Dock/DockingManager.cs \
./IconView/IconView.cs \
./AssemblyInfo.cs \
./FileBrowser/FileBrowser.cs \
./PropertyGrid/PropertyGridWidget.cs \
./PropertyGrid/ELabel.cs \
./PropertyGrid/PropertyGridGroup.cs \
./PropertyGrid/PropertyGridItem.cs \
./PropertyGrid/PropertyGrid.cs \
./FolderDialog/FolderDialog.cs \
./FolderDialog/BaseFileEntry.cs \
./FolderDialog/FolderEntry.cs \
./FolderDialog/FileEntry.cs \
./TabLabel/TabLabel.cs \
./GladeWidgetExtract/GladeWidgetExtract.cs

all: $(DLL)

$(DLL): $(FILES)
	@ $(MCS) /debug /out:$(DLL) /target:library $(REFERENCES) $(FILES) \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

