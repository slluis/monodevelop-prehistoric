DLLNAME = MonoDevelop.SourceEditor.dll
CONFIG = $(DLLNAME).config
DLL = $(top_builddir)/build/bin/$(DLLNAME)

FILES = \
AssemblyInfo.cs \
CodeCompletion/CodeCompletionData.cs \
CodeCompletion/ICompletionData.cs \
CodeCompletion/CompletionWindow.cs \
CodeCompletion/CompletionListWindow.cs \
CodeCompletion/ListWindow.cs \
CodeCompletion/TextUtilities.cs \
CodeCompletion/TemplateCompletionDataProvider.cs \
CodeCompletion/CommentCompletionDataProvider.cs \
CodeCompletion/ICompletionDataProvider.cs \
CodeCompletion/DeclarationViewWindow.cs \
CodeCompletion/CodeCompletionDataProvider.cs \
Gui/SourceEditorWidget.cs \
Gui/SourceEditorBuffer.cs \
Gui/SourceEditorView.cs \
Gui/SourceEditorDisplayBinding.cs \
Gui/OptionPanels/BehaviorTextEditorPanel.cs \
Gui/OptionPanels/MarkersTextEditorPanel.cs \
Gui/OptionPanels/CodeTemplatePanel.cs \
Gui/OptionPanels/GeneralTextEditorPanel.cs \
Gui/Dialogs/EditTemplateGroupDialog.cs \
Gui/Dialogs/GotoLineNumberDialog.cs \
Gui/Dialogs/ReplaceDialog.cs \
Gui/Dialogs/ReplaceInFilesDialog.cs \
InsightWindow/IndexerInsightDataProvider.cs \
InsightWindow/MethodInsightDataProvider.cs \
InsightWindow/InsightWindow.cs \
InsightWindow/IInsightDataProvider.cs \
Commands/CommentRegionCommand.cs \
Commands/BookmarkCommands.cs \
Commands/CodeActions.cs \
Commands/IEditAction.cs \
Commands/ToolCommands.cs \
Commands/FoldingCommands.cs \
Commands/SearchCommands.cs \
Commands/ProjectCommands.cs \
Commands/FormatCommands.cs \
Commands/TextAreaContextmenuCommands.cs \
Properties/TextEditorProperties.cs \
FormattingStrategy/DefaultFormattingStrategy.cs \
FormattingStrategy/IndentStyle.cs \
FormattingStrategy/IFormattingStrategy.cs \
FormattingStrategy/IFormattableDocument.cs \
Codons/EditActionCodon.cs \
Search/DocumentIterator/IDocumentInformation.cs \
Search/DocumentIterator/EditorDocumentInformation.cs \
Search/DocumentIterator/FileDocumentInformation.cs \
Search/DocumentIterator/DirectoryDocumentIterator.cs \
Search/DocumentIterator/AllOpenDocumentIterator.cs \
Search/DocumentIterator/IDocumentIterator.cs \
Search/DocumentIterator/ReverseDocumentIterator.cs \
Search/DocumentIterator/WholeProjectDocumentIterator.cs \
Search/DocumentIterator/CurrentDocumentIterator.cs \
Search/SearchStrategy/KMPSearchStrategy.cs \
Search/SearchStrategy/ISearchStrategy.cs \
Search/SearchStrategy/BruteForceSearchStrategy.cs \
Search/SearchStrategy/RegExSearchStrategy.cs \
Search/SearchStrategy/WildcardSearchStrategy.cs \
Search/SearchReplaceManager.cs \
Search/DefaultFind.cs \
Search/SearchResult/DefaultSearchResult.cs \
Search/SearchResult/ISearchResult.cs \
Search/SearchOptions.cs \
Search/SearchReplaceInFilesManager.cs \
Search/SearchReplaceUtilities.cs \
Search/IFind.cs \
Search/TextIterator/ITextIterator.cs \
Search/TextIterator/ForwardTextFileIterator.cs \
Search/TextIterator/ForwardTextIterator.cs \
Search/TextIterator/ExtendedStreamReader.cs \
Search/ITextBufferStrategy.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

REFS = /r:System.Drawing.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Utils.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	$(GTK_SHARP_LIBS) \
	$(GNOME_VFS_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS) \
	$(GCONF_SHARP_LIBS) \
	$(GTKSOURCEVIEW_SHARP_LIBS)

ADDIN = MonoDevelopEditor.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns

all : $(DLL) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL) : $(build_sources) $(srcdir)/EditorBindings.glade $(srcdir)/texteditoraddin.glade
	mkdir -p $(top_builddir)/build/bin
	$(CSC) $(build_sources) -out:$@ -t:library $(REFS) \
	-resource:$(srcdir)/EditorBindings.glade,EditorBindings.glade \
	-resource:$(srcdir)/texteditoraddin.glade,texteditoraddin.glade \
	&& cp $(CONFIG) $(top_builddir)/build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL) $(CONFIG)

addindir = $(libdir)/monodevelop/AddIns
addin_DATA = $(ADDIN)

CLEANFILES = $(DLL) $(DLL).mdb
DISTCLEANFILES = $(CONFIG)

EXTRA_DIST = $(FILES) $(CONFIG).in EditorBindings.glade texteditoraddin.glade $(ADDIN)
