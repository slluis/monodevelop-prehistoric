SDDIR=../../../SharpDevelop/src/Main/Core
SOURCES = \
./Gui/Editor/ParserFoldingStrategy.cs \
./Gui/Editor/SharpDevelopTextEditorProperties.cs \
./Gui/Editor/SharpDevelopTextAreaControl.cs \
./Gui/Editor/InsightWindow/IndexerInsightDataProvider.cs \
./Gui/Editor/InsightWindow/MethodInsightDataProvider.cs \
./Gui/Editor/CompletionWindow/TemplateCompletionDataProvider.cs \
./Gui/Editor/CompletionWindow/CodeCompletionDataProvider.cs \
./Gui/Editor/CompletionWindow/CodeCompletionData.cs \
./Gui/Editor/CompletionWindow/CommentCompletionDataProvider.cs \
./Gui/Editor/ErrorDrawer.cs \
./Gui/Editor/ITextAreaControlProvider.cs \
./Gui/Editor/TextEditorDisplayBinding.cs \
./Gui/Dialogs/ExportProjectToHtmlDialog.cs \
./Gui/Dialogs/SortOptionsDialog.cs \
./Gui/Dialogs/ReplaceInFilesDialog.cs \
./Gui/Dialogs/ReplaceDialog.cs \
./Gui/Dialogs/GotoLineNumberDialog.cs \
./Conditions/TextContentCondition.cs \
./Codons/EditActionCodon.cs \
./Commands/ToolCommands.cs \
./Commands/TextAreaContextmenuCommands.cs \
./Commands/FormatCommands.cs \
./Commands/ProjectCommands.cs \
./Commands/SearchCommands.cs \
./Commands/FoldingCommands.cs \
./Commands/CommentRegionCommand.cs \
./Commands/CodeActions.cs \
./Actions.cs \
./Search/DocumentIterator/ProvidedDocumentInformation.cs \
./Search/DocumentIterator/DirectoryDocumentIterator.cs \
./Search/DocumentIterator/AllOpenDocumentIterator.cs \
./Search/DocumentIterator/IDocumentIterator.cs \
./Search/DocumentIterator/ReverseDocumentIterator.cs \
./Search/DocumentIterator/WholeProjectDocumentIterator.cs \
./Search/DocumentIterator/CurrentDocumentIterator.cs \
./Search/SearchReplaceManager.cs \
./Search/DefaultFind.cs \
./Search/SearchResult/DefaultSearchResult.cs \
./Search/SearchResult/ISearchResult.cs \
./Search/SearchReplaceInFilesManager.cs \
./Search/SearchReplaceUtilities.cs \
./Search/IFind.cs \
./Search/TextIterator/ITextIteratorBuilder.cs \
./Search/TextIterator/ForwardTextIteratorBuilder.cs \
./Search/TextIterator/ForwardTextIterator.cs \
./AssemblyInfo.cs \
./XmlFormattingStrategy.cs

DLL=../../../../build/bin/MonoDevelop.DefaultTexteditor.dll

all: $(DLL)

$(DLL): $(SOURCES) ../../../../data/resources/glade/texteditoraddin.glade
	@ mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll \
		/r:../../../../build/bin/MonoDevelop.Core.dll \
		/r:../../../../build/bin/MonoDevelop.Base.dll \
		/r:../../../../build/bin/MonoDevelop.SourceEditor.dll \
		/r:System.DirectoryServices.dll /r:pango-sharp.dll /r:glade-sharp.dll \
		/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll \
		/r:glib-sharp.dll \
		/resource:../../../../data/resources/glade/texteditoraddin.glade,texteditoraddin.glade \
		/define:GTK /r:gtk-sharp.dll /r:gdk-sharp.dll /r:gnome-sharp.dll\
		$(SOURCES)

CLEANFILES=$(DLL)
