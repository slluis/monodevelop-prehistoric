DLL = MonoDevelop.SourceEditor.dll

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
Gui/Pads/CompilerMessageView.cs \
Gui/OptionPanels/BehaviorTextEditorPanel.cs \
Gui/OptionPanels/MarkersTextEditorPanel.cs \
Gui/OptionPanels/CodeTemplatePanel.cs \
Gui/OptionPanels/GeneralTextEditorPanel.cs \
Gui/Dialogs/EditTemplateGroupDialog.cs \
Gui/Dialogs/ExportProjectToHtmlDialog.cs \
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
	/r:../../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../../build/bin/MonoDevelop.Gui.Utils.dll \
	/r:../../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS) \
	$(GCONF_SHARP_LIBS) \
	$(GTKSOURCEVIEW_SHARP_LIBS)

all : $(DLL)

$(DLL) : $(build_sources) $(srcdir)/EditorBindings.glade $(srcdir)/texteditoraddin.glade
	$(CSC) $(build_sources) /debug /out:$(DLL) /t:library $(REFS) \
	/resource:$(srcdir)/EditorBindings.glade,EditorBindings.glade \
	/resource:$(srcdir)/texteditoraddin.glade,texteditoraddin.glade \
	&& cp $(DLL) ../../../../../build/bin/. && cp $(DLL).config ../../../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL) $(DLL).config

CLEANFILES = $(DLL) $(DLL).mdb
DISTCLEANFILES = $(DLL).config

EXTRA_DIST = $(FILES) $(DLL).config.in EditorBindings.glade texteditoraddin.glade
