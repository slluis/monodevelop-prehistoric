
DLL = $(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll

REFERENCES = -r:System.Drawing.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Gui.Utils.dll \
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
DataGrid/DataGrid.cs \
Commands/ActionCommand.cs \
Commands/ActionType.cs \
Commands/CommandArrayInfo.cs \
Commands/CommandCheckMenuItem.cs \
Commands/Command.cs \
Commands/CommandEntry.cs \
Commands/CommandEntrySet.cs \
Commands/CommandErrorHandler.cs \
Commands/CommandHandlerAttribute.cs \
Commands/CommandHandler.cs \
Commands/CommandInfo.cs \
Commands/CommandManager.cs \
Commands/CommandMenuBar.cs \
Commands/CommandMenu.cs \
Commands/CommandMenuItem.cs \
Commands/CommandToggleToolButton.cs \
Commands/CommandToolbar.cs \
Commands/CommandToolButton.cs \
Commands/CustomCommand.cs \
Commands/CustomMenuItem.cs \
Commands/ICommandMenuItem.cs \
Commands/ICommandRouter.cs \
Commands/ICommandUserItem.cs \
Commands/LinkCommandEntry.cs \
Commands/MenuToolButton.cs

all: $(DLL)

$(DLL): $(build_sources)
	$(CSC) $(CSC_FLAGS) -out:$@ -target:library $(REFERENCES) $(build_sources)

assemblydir = $(MD_ASSEMBLY_DIR)
assembly_DATA = $(DLL)
  
CLEANFILES = $(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)

include $(top_srcdir)/Makefile.include

