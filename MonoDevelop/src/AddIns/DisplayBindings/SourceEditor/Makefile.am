DLL=../../../../build/bin/MonoDevelop.SourceEditor.dll

SOURCES = ./CodeCompletion/CodeCompletionData.cs \
./CodeCompletion/ICompletionData.cs \
./CodeCompletion/CompletionWindow.cs \
./CodeCompletion/TextUtilities.cs \
./CodeCompletion/TemplateCompletionDataProvider.cs \
./CodeCompletion/CommentCompletionDataProvider.cs \
./CodeCompletion/ICompletionDataProvider.cs \
./CodeCompletion/DeclarationViewWindow.cs \
./CodeCompletion/CodeCompletionDataProvider.cs \
./Gui/SourceEditorWidget.cs \
./Gui/SourceEditorBuffer.cs \
./Gui/SourceEditorView.cs \
./Gui/SourceEditorDisplayBinding.cs \
./Gui/Pads/CompilerMessageView.cs \
./Gui/OptionPanels/BehaviorTextEditorPanel.cs \
./Gui/OptionPanels/MarkersTextEditorPanel.cs \
./Gui/OptionPanels/CodeTemplatePanel.cs \
./Gui/OptionPanels/GeneralTextEditorPanel.cs \
./Gui/EditTemplateGroupDialog.cs \
./Gui/Dialogs/GotoLineNumberDialog.cs \
./Gui/Dialogs/ReplaceDialog.cs \
./Gui/Dialogs/ReplaceInFilesDialog.cs \
./InsightWindow/IndexerInsightDataProvider.cs \
./InsightWindow/MethodInsightDataProvider.cs \
./InsightWindow/InsightWindow.cs \
./InsightWindow/IInsightDataProvider.cs \
./Commands/CommentRegionCommand.cs \
./Commands/BookmarkCommands.cs \
./Commands/CodeActions.cs \
./Commands/IEditAction.cs \
./Commands/ToolCommands.cs \
./Commands/FoldingCommands.cs \
./Commands/SearchCommands.cs \
./Commands/ProjectCommands.cs \
./Commands/FormatCommands.cs \
./Commands/TextAreaContextmenuCommands.cs \
./Properties/TextEditorProperties.cs \
./FormattingStrategy/DefaultFormattingStrategy.cs \
./FormattingStrategy/IndentStyle.cs \
./FormattingStrategy/IFormattingStrategy.cs \
./FormattingStrategy/IFormattableDocument.cs \
./Codons/EditActionCodon.cs \
./Search/DocumentIterator/ProvidedDocumentInformation.cs \
./Search/DocumentIterator/DirectoryDocumentIterator.cs \
./Search/DocumentIterator/AllOpenDocumentIterator.cs \
./Search/DocumentIterator/IDocumentIterator.cs \
./Search/DocumentIterator/ReverseDocumentIterator.cs \
./Search/DocumentIterator/WholeProjectDocumentIterator.cs \
./Search/DocumentIterator/CurrentDocumentIterator.cs \
./Search/SearchStrategy/KMPSearchStrategy.cs \
./Search/SearchStrategy/ISearchStrategy.cs \
./Search/SearchStrategy/BruteForceSearchStrategy.cs \
./Search/SearchStrategy/RegExSearchStrategy.cs \
./Search/SearchStrategy/WildcardSearchStrategy.cs \
./Search/SearchReplaceManager.cs \
./Search/DefaultFind.cs \
./Search/SearchResult/DefaultSearchResult.cs \
./Search/SearchResult/ISearchResult.cs \
./Search/SearchOptions.cs \
./Search/SearchReplaceInFilesManager.cs \
./Search/SearchReplaceUtilities.cs \
./Search/IFind.cs \
./Search/TextIterator/ITextIterator.cs \
./Search/TextIterator/ITextIteratorBuilder.cs \
./Search/TextIterator/ForwardTextIteratorBuilder.cs \
./Search/TextIterator/ForwardTextIterator.cs \
./Search/ITextBufferStrategy.cs

all : $(DLL)

$(DLL) : $(SOURCES)
	mcs $(SOURCES) /debug /out:$(DLL) /t:library \
	/resource:../../../../data/resources/glade/EditorBindings.glade,EditorBindings.glade \
	/resource:../../../../data/resources/glade/texteditoraddin.glade,texteditoraddin.glade \
	 /r:gtk-sharp /r:gdk-sharp $(GTKSOURCEVIEW_SHARP) /r:glib-sharp \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Utils.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
		/r:pango-sharp.dll /r:gnome-sharp.dll /r:glade-sharp.dll \
		/r:gconf-sharp.dll \
		/r:System.Drawing.dll

monodeveloplibdir = $(prefix)/lib/monodevelop

install-data-local: $(DLL)
	$(mkinstalldirs) $(DESTDIR)$(monodeveloplibdir)/bin
	$(INSTALL_DATA) $(DLL) $(DESTDIR)$(monodeveloplibdir)/bin/MonoDevelop.SourceEditor.dll

CLEANFILES=$(DLL)

EXTRA_DIST = $(SOURCES)
