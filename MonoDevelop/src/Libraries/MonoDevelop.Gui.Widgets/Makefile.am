
MCS = mcs -debug

DLL = MonoDevelop.Gui.Widgets.dll

REFERENCES = /r:System.Drawing /r:gtk-sharp /r:gdk-sharp /r:pango-sharp /r:gnome-sharp /r:glib-sharp /r:glade-sharp /r:../../../build/bin/MonoDevelop.Core.dll /r:../../../build/bin/MonoDevelop.Gui.Utils.dll

FILES = ./Tree/TreeView.cs \
./Tree/TreeNodeCollection.cs \
./Tree/TreeNode.cs \
./Dock/Docker.cs \
./Dock/DockingManager.cs \
./IconView/IconView.cs \
./AssemblyInfo.cs \
./FileBrowser/FileBrowser.cs \
./FolderDialog/FolderDialog.cs \
./FolderDialog/BaseFileEntry.cs \
./FolderDialog/FolderEntry.cs \
./FolderDialog/FileEntry.cs \
./TabLabel/TabLabel.cs \
./GladeWidgetExtract/GladeWidgetExtract.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(DLL)

$(DLL): $(build_sources)
	@ $(MCS) /debug /out:$(DLL) /target:library $(REFERENCES) $(build_sources) \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

