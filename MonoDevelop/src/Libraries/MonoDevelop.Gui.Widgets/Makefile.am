
MCS = mcs -debug

DLL = MonoDevelop.Gui.Widgets.dll

REFERENCES = /r:System.Drawing \
	/r:../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../build/bin/MonoDevelop.Gui.Utils.dll \
	$(BASE_DEPENDENCIES_LIBS)

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
./Navbar/Navbar.cs \
./TabLabel/TabLabel.cs \
./GladeWidgetExtract/GladeWidgetExtract.cs \
./DragNotebook/DragNotebook.cs


build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(DLL)

$(DLL): $(build_sources)
	$(MCS) /debug /out:$(DLL) /target:library $(REFERENCES) $(build_sources) \
	&& cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

