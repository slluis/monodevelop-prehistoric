
DLLS = /r:System.Drawing /r:ICSharpCode.SharpZipLib.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/ICSharpCode.SharpAssembly.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Utils.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	/r:System.Data.dll /r:System.Web.Services.dll \
	/r:$(top_builddir)/build/bin/gdl-sharp.dll \
	/r:Mono.Posix.dll \
	$(GTK_SHARP_LIBS) \
	$(GNOME_SHARP_LIBS) \
	$(GNOME_VFS_SHARP_LIBS) \
	$(GECKO_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS) \
	$(GTKHTML_SHARP_LIBS) \
	$(MONODOC_LIBS)

FILES= \
Gui/Pads/ProjectBrowser/ProjectBrowserView.cs \
Gui/Pads/ProjectBrowser/NodeBuilder/DefaultDotNetNodeBuilder.cs \
Gui/Pads/ProjectBrowser/NodeBuilder/IProjectNodeBuilder.cs \
Gui/Pads/ProjectBrowser/BrowserNode/ProjectBrowserNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/FolderNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/AbstractBrowserNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/FileNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/ReferenceNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/CombineBrowserNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/DragDropUtil.cs \
Gui/Pads/ProjectBrowser/BrowserNode/DirectoryNode.cs \
Gui/Pads/ProjectBrowser/BrowserNode/NamedFolderNode.cs \
Gui/Pads/OpenTaskView.cs \
Gui/Pads/TreeViewMemento.cs \
Gui/Pads/FileScout/FileList.cs \
Gui/Pads/FileScout/FileListItem.cs \
Gui/Pads/FileScout/FileScout.cs \
Gui/Pads/PropertyPad/PropertyPadCommands.cs \
Gui/Pads/PropertyPad/PropertyPad.cs \
Gui/Pads/ClassScout/ClassScout.cs \
Gui/Pads/ClassScout/NodeBuilder/DefaultDotNetClassScoutNodeBuilder.cs \
Gui/Pads/ClassScout/NodeBuilder/IClassScoutNodeBuilder.cs \
Gui/Pads/ClassScout/BrowserNode/AbstractClassScoutNode.cs \
Gui/Pads/HelpBrowser/MonodocTreePad.cs \
Gui/Pads/HelpBrowser/HelpViewer.cs \
Gui/Pads/SideBarView.cs \
Gui/Pads/DefaultMonitorPad.cs \
Gui/AbstractPadContent.cs \
Gui/ISecondaryViewContent.cs \
Gui/IViewContent.cs \
Gui/AbstractViewContent.cs \
Gui/GuiService.cs \
Gui/Components/SdMenuSeparator.cs \
Gui/Components/SharpMessageBox.cs \
Gui/Components/StatusBar/AxStatusBar.cs \
Gui/Components/StatusBar/SdStatusBar.cs \
Gui/Components/StatusBar/IStatusIcon.cs \
Gui/Components/StatusBar/AxStatusBarPanel.cs \
Gui/Components/LocalizedPropertyGrid/LocalizedPropertyDescriptor.cs \
Gui/Components/LocalizedPropertyGrid/LocalizedObject.cs \
Gui/Components/LocalizedPropertyGrid/LocalizedPropertyAttribute.cs \
Gui/Components/SideBar/AxSideTabItem.cs \
Gui/Components/SideBar/AxSideTab.cs \
Gui/Components/SideBar/SharpDevelopSideTabItem.cs \
Gui/Components/SideBar/SharpDevelopSideBar.cs \
Gui/Components/SideBar/AxSideBar.cs \
Gui/Components/SdMenu.cs \
Gui/Components/SdMenuCheckBox.cs \
Gui/Components/SdMenuCommand.cs \
Gui/Components/SdToolbarCommand.cs \
Gui/WorkbenchWindowCollection.cs \
Gui/WorkbenchSingleton.cs \
Gui/IMementoCapable.cs \
Gui/Dialogs/CommonAboutDialog.cs \
Gui/Dialogs/OptionPanels/ProjectOptions/CompileFileProjectOptions.cs \
Gui/Dialogs/OptionPanels/ProjectOptions/DeployFileOptions.cs \
Gui/Dialogs/OptionPanels/ProjectOptions/GeneralProjectOptions.cs \
Gui/Dialogs/OptionPanels/ProjectOptions/OutputOptionsPanel.cs \
Gui/Dialogs/OptionPanels/IDEOptions/BuildPanel.cs \
Gui/Dialogs/OptionPanels/IDEOptions/LoadSavePanel.cs \
Gui/Dialogs/OptionPanels/IDEOptions/SelectStylePanel.cs \
Gui/Dialogs/OptionPanels/IDEOptions/CodeGenerationPanel.cs \
Gui/Dialogs/OptionPanels/EditTemplateDialog.cs \
Gui/Dialogs/OptionPanels/ExternalToolPanel.cs \
Gui/Dialogs/OptionPanels/PropertyGridPanel.cs \
Gui/Dialogs/TreeViewOptions.cs \
Gui/Dialogs/WordCountDialog.cs \
Gui/Dialogs/SharpDevelopAboutPanels.cs \
Gui/Dialogs/SplashScreen.cs \
Gui/Dialogs/Wizard/WizardDialog.cs \
Gui/Dialogs/Wizard/CurrentPanelPanel.cs \
Gui/Dialogs/Wizard/StatusPanel.cs \
Gui/Dialogs/Wizard/IWizardPanel.cs \
Gui/Dialogs/Wizard/AbstractWizardPanel.cs \
Gui/Dialogs/CombineConfiguration/CombineConfigurationPanel.cs \
Gui/Dialogs/CombineConfiguration/CombineDependencyPanel.cs \
Gui/Dialogs/CombineConfiguration/CombineStartupPanel.cs \
Gui/Dialogs/CombineConfiguration/CombineBuildOptions.cs \
Gui/Dialogs/CombineConfiguration/ConfigurationManager.cs \
Gui/Dialogs/NewProjectDialog.cs \
Gui/Dialogs/AbstractOptionPanel.cs \
Gui/Dialogs/ReferenceDialog/COMReferencePanel.cs \
Gui/Dialogs/ReferenceDialog/WebReference.cs \
Gui/Dialogs/ReferenceDialog/AssemblyReferencePanel.cs \
Gui/Dialogs/ReferenceDialog/GacReferencePanel.cs \
Gui/Dialogs/ReferenceDialog/AddWebReferenceDialog.cs \
Gui/Dialogs/ReferenceDialog/SelectReferenceDialog.cs \
Gui/Dialogs/ReferenceDialog/ProjectReferencePanel.cs \
Gui/Dialogs/DirtyFilesDialog.cs \
Gui/Dialogs/TipOfTheDay.cs \
Gui/Dialogs/NewFileDialog.cs \
Gui/Dialogs/ProjectOptionsDialog.cs \
Gui/Dialogs/NewLayoutDialog.cs \
Gui/ContentInterfaces/ICodeStyleOperations.cs \
Gui/ContentInterfaces/IBookmarkOperations.cs \
Gui/ContentInterfaces/IClipboardHandler.cs \
Gui/ContentInterfaces/IPrintable.cs \
Gui/ContentInterfaces/IPositionable.cs \
Gui/ContentInterfaces/ICustomizedCommands.cs \
Gui/ContentInterfaces/IEditable.cs \
Gui/ContentInterfaces/IParseInformationListener.cs \
Gui/ContentInterfaces/IParsableContent.cs \
Gui/IBaseViewContent.cs \
Gui/ErrorDialogs/LoadingError.cs \
Gui/IWorkbenchLayout.cs \
Gui/ErrorHandlers/GenericError.cs \
Gui/ErrorHandlers/CombineLoadError.cs \
Gui/IWorkbench.cs \
Gui/BrowserDisplayBinding/AxSHDocVw.cs \
Gui/BrowserDisplayBinding/HtmlViewPane.cs \
Gui/BrowserDisplayBinding/BrowserDisplayBinding.cs \
Gui/BrowserDisplayBinding/HTMLInterfaces.cs \
Gui/BrowserDisplayBinding/SHDocVw.cs \
Gui/IPadContent.cs \
Gui/AbstractBaseViewContent.cs \
Gui/PadContentCollection.cs \
Gui/PixbufList.cs \
Gui/AbstractSecondaryViewContent.cs \
Gui/ViewContentCollection.cs \
Gui/IWorkbenchWindow.cs \
Gui/HtmlControl/IHTMLDocument2.cs \
Gui/HtmlControl/IHTMLElement.cs \
Gui/HtmlControl/IWebBrowser.cs \
Gui/HtmlControl/MozillaControl.cs \
Gui/Workbench/Layouts/SdiWorkspaceWindow.cs \
Gui/Workbench/Layouts/SdiWorkspaceLayout.cs \
Gui/Workbench/WorkbenchMemento.cs \
Gui/Workbench/DefaultWorkbench.cs \
Commands/ClassBrowserCommands/ClassBrowserCommands.cs \
Commands/FileCommands.cs \
Commands/HelpCommands.cs \
Commands/CustomStringTagProvider.cs \
Commands/RunCommands.cs \
Commands/ProjectBrowserCommands/FolderNodeCommands.cs \
Commands/ProjectBrowserCommands/ReferenceFolderNodeCommands.cs \
Commands/ProjectBrowserCommands/GeneralNodeCommands.cs \
Commands/ProjectBrowserCommands/CombineNodeCommands.cs \
Commands/ProjectBrowserCommands/ResourceFolderNodeCommands.cs \
Commands/ProjectBrowserCommands/ProjectNodeCommands.cs \
Commands/ProjectCommands.cs \
Commands/VBConverter/ConvertProject.cs \
Commands/VBConverter/ConvertBuffer.cs \
Commands/SideBarCommands.cs \
Commands/ProjectOptionsCommands.cs \
Commands/EditCommands.cs \
Commands/WindowCommands.cs \
Commands/AutostartCommands.cs \
Commands/ToolsCommands.cs \
Commands/MenuItemBuilders.cs \
Services/File/IFileService.cs \
Services/File/DefaultFileService.cs \
Services/File/FileEventArgs.cs \
Services/File/RecentFiles.cs \
Services/File/RecentOpen.cs \
Services/MenuService/MenuService.cs \
Services/DisplayBinding/DisplayBindingService.cs \
Services/LanguageBinding/LanguageBindingService.cs \
Services/Tasks/TaskService.cs \
Services/Tasks/Task.cs \
Services/Tasks/BackgroundProgressMonitor.cs \
Services/Tasks/OutputProgressMonitor.cs \
Services/Tasks/StatusProgressMonitor.cs \
Services/AmbienceService/AmbienceService.cs \
Services/AmbienceService/AmbienceReflectionDecorator.cs \
Services/AmbienceService/AbstractAmbience.cs \
Services/AmbienceService/CodeDOMGeneratorUtility.cs \
Services/AmbienceService/NetAmbience.cs \
Services/AmbienceService/IAmbience.cs \
Services/ProcessService/ProcessService.cs \
Services/ProcessService/ProcessWrapper.cs \
Services/Project/ProjectEventArgs.cs \
Services/Project/IProjectService.cs \
Services/Project/ProjectRenameEventArgs.cs \
Services/Project/ProjectService.cs \
Services/Project/ParseInformationEventHandler.cs \
Services/Project/CombineEventArgs.cs \
Services/Project/FileFormatManager.cs \
Services/IconService.cs \
Services/StatusBar/DefaultStatusBarService.cs \
Services/StatusBar/IStatusBarService.cs \
Services/Language/LanguageService.cs \
Services/Language/Language.cs \
Services/ResourceNotFoundException.cs \
Services/ParserService/DefaultParserService.cs \
Services/ParserService/CodeCompletionDatabase.cs \
Services/ParserService/ClassProxyCollection.cs \
Services/ParserService/IParserService.cs \
Services/ParserService/ClassProxy.cs \
Services/ParserService/AssemblyInformation.cs \
Services/ParserService/ParseInformation.cs \
Services/ParserService/ClassInformationEventHandler.cs \
Services/ParserService/AssemblyCodeCompletionDatabase.cs \
Services/ParserService/ClassEntry.cs \
Services/ParserService/FileEntry.cs \
Services/ParserService/NamespaceEntry.cs \
Services/ParserService/ProjectCodeCompletionDatabase.cs \
Services/ParserService/SimpleCodeCompletionDatabase.cs \
Services/ParserService/ReferenceEntry.cs \
Services/ResourceService.cs \
Services/StockIcons.cs \
Services/MessageService.cs \
Services/Toolbar/ToolbarService.cs \
Services/MonodocService.cs \
Services/IDebuggerService.cs \
Services/SystemAssemblyService.cs \
Services/DispatchService/AsyncDispatchAttribute.cs \
Services/DispatchService/DispatchService.cs \
Services/DispatchService/FreeDispatchAttribute.cs \
Services/DispatchService/GuiSyncContext.cs \
Services/DispatchService/GuiSyncAbstractService.cs \
Services/DispatchService/GuiSyncObject.cs \
Services/DispatchService/SyncContext.cs \
Services/DispatchService/SyncContextAttribute.cs \
Services/DispatchService/SyncObject.cs \
Services/Runtime.cs \
Internal/Undo/IUndoableOperation.cs \
Internal/Undo/UndoStack.cs \
Internal/Undo/UndoQueue.cs \
Internal/Conditions/LanguageActiveCondition.cs \
Internal/Conditions/OwnerStateCondition.cs \
Internal/Conditions/ProjectActiveCondition.cs \
Internal/Conditions/WindowActiveCondition.cs \
Internal/Conditions/WindowOpenCondition.cs \
Internal/Conditions/CombineOpenCondition.cs \
Internal/Conditions/ProjectOpenCondition.cs \
Internal/Conditions/WorkbenchContextCondition.cs \
Internal/Codons/DisplayBinding/IDisplayBinding.cs \
Internal/Codons/DisplayBinding/ISubDisplayBinding.cs \
Internal/Codons/DisplayBinding/DisplayBindingCodon.cs \
Internal/Codons/LanguageBinding/ICompilerResult.cs \
Internal/Codons/LanguageBinding/FileFilterCodon.cs \
Internal/Codons/LanguageBinding/ILanguageBinding.cs \
Internal/Codons/LanguageBinding/LanguageBindingCodon.cs \
Internal/Codons/LanguageBinding/DefaultCompilerResult.cs \
Internal/Codons/ProjectBinding/IProjectBinding.cs \
Internal/Codons/ProjectBinding/ProjectBindingCodon.cs \
Internal/Codons/DefaultDialogPanelDescriptor.cs \
Internal/Codons/Toolbars/ToolbarItemCodon.cs \
Internal/Codons/IDialogPanel.cs \
Internal/Codons/DialogPanelCodon.cs \
Internal/Codons/IDialogPanelDescriptor.cs \
Internal/Codons/FileFormatCodon.cs \
Internal/Codons/MenuItems/AbstractCheckableMenuCommand.cs \
Internal/Codons/MenuItems/ISubmenuBuilder.cs \
Internal/Codons/MenuItems/IMenuCommand.cs \
Internal/Codons/MenuItems/AbstractMenuCommand.cs \
Internal/Codons/MenuItems/ICheckableMenuCommand.cs \
Internal/Codons/MenuItems/MenuItemCodon.cs \
Internal/Codons/Templates/FileTemplateCodon.cs \
Internal/Codons/Templates/ProjectTemplateCodon.cs \
Internal/DataStructures/PriorityQueue.cs \
Internal/Parser/IRegion.cs \
Internal/Parser/ICompilationUnitBase.cs \
Internal/Parser/IEvent.cs \
Internal/Parser/Collections/PropertyCollection.cs \
Internal/Parser/Collections/TypedCSharpCollection.cs \
Internal/Parser/Collections/IUsingCollection.cs \
Internal/Parser/Collections/TagCollection.cs \
Internal/Parser/Collections/ClassCollection.cs \
Internal/Parser/Collections/EventCollection.cs \
Internal/Parser/Collections/AttributeCollection.cs \
Internal/Parser/Collections/MethodCollection.cs \
Internal/Parser/Collections/CommentCollection.cs \
Internal/Parser/Collections/ParameterCollection.cs \
Internal/Parser/Collections/FieldCollection.cs \
Internal/Parser/Collections/AttributeSectionCollection.cs \
Internal/Parser/ParameterModifier.cs \
Internal/Parser/ClassType.cs \
Internal/Parser/IParser.cs \
Internal/Parser/Implementations/AbstractField.cs \
Internal/Parser/Implementations/AbstractDecoration.cs \
Internal/Parser/Implementations/AbstractCompilationUnit.cs \
Internal/Parser/Implementations/AbstractParameter.cs \
Internal/Parser/Implementations/AbstractProperty.cs \
Internal/Parser/Implementations/AbstractIndexer.cs \
Internal/Parser/Implementations/AbstractAttribute.cs \
Internal/Parser/Implementations/AbstractEvent.cs \
Internal/Parser/Implementations/DefaultRegion.cs \
Internal/Parser/Implementations/AbstractComment.cs \
Internal/Parser/Implementations/AbstractNamedEntity.cs \
Internal/Parser/Implementations/AbstractMember.cs \
Internal/Parser/Implementations/AbstractUsing.cs \
Internal/Parser/Implementations/AbstractClass.cs \
Internal/Parser/Implementations/AbstractMethod.cs \
Internal/Parser/Implementations/AbstractReturnType.cs \
Internal/Parser/Tag.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyMethod.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyAttribute.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyReturnType.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyClass.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyField.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyIndexer.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyEvent.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyParameter.cs \
Internal/Parser/SharpAssemblyLayer/SharpAssemblyProperty.cs \
Internal/Parser/IReturnType.cs \
Internal/Parser/IUsing.cs \
Internal/Parser/IClass.cs \
Internal/Parser/IParameter.cs \
Internal/Parser/IIndexer.cs \
Internal/Parser/IAttribute.cs \
Internal/Parser/ReflectionLayer/ReflectionReturnType.cs \
Internal/Parser/ReflectionLayer/ReflectionProperty.cs \
Internal/Parser/ReflectionLayer/ReflectionClass.cs \
Internal/Parser/ReflectionLayer/ReflectionIndexer.cs \
Internal/Parser/ReflectionLayer/ReflectionMethod.cs \
Internal/Parser/ReflectionLayer/ReflectionField.cs \
Internal/Parser/ReflectionLayer/ReflectionParameter.cs \
Internal/Parser/ReflectionLayer/ReflectionEvent.cs \
Internal/Parser/IDecoration.cs \
Internal/Parser/Comment.cs \
Internal/Parser/ModifierEnum.cs \
Internal/Parser/IMember.cs \
Internal/Parser/IExpressionFinder.cs \
Internal/Parser/IField.cs \
Internal/Parser/IProperty.cs \
Internal/Parser/PersistenceLayer/PersistentIndexer.cs \
Internal/Parser/PersistenceLayer/PersistentField.cs \
Internal/Parser/PersistenceLayer/PersistentMethod.cs \
Internal/Parser/PersistenceLayer/PersistentReturnType.cs \
Internal/Parser/PersistenceLayer/PersistentEvent.cs \
Internal/Parser/PersistenceLayer/PersistentProperty.cs \
Internal/Parser/PersistenceLayer/PersistentParameter.cs \
Internal/Parser/PersistenceLayer/PersistentClass.cs \
Internal/Parser/IMethod.cs \
Internal/Parser/IComment.cs \
Internal/Parser/ICompilationUnit.cs \
Internal/ProgressMonitoring/AggregatedProgressMonitor.cs \
Internal/ProgressMonitoring/BaseProgressMonitor.cs \
Internal/ProgressMonitoring/ConsoleProgressMonitor.cs \
Internal/ProgressMonitoring/IAsyncOperation.cs \
Internal/ProgressMonitoring/IProgressMonitor.cs \
Internal/ProgressMonitoring/LogTextWriter.cs \
Internal/ProgressMonitoring/NullProgressMonitor.cs \
Internal/ProgressMonitoring/NullAsyncOperation.cs \
Internal/ProgressMonitoring/ProgressTracker.cs \
Internal/Project/PrjxFileFormat.cs \
Internal/Project/CmbxFileFormat.cs \
Internal/Project/MdsFileFormat.cs \
Internal/Project/MdpFileFormat.cs \
Internal/Project/IFileFormat.cs \
Internal/Project/ProjectPathItemPropertyAttribute.cs \
Internal/Project/Project/Deployment/DeployInformation.cs \
Internal/Project/Project/Deployment/AssemblyDeploy.cs \
Internal/Project/Project/Deployment/ScriptDeploy.cs \
Internal/Project/Project/Deployment/IDeploymentStrategy.cs \
Internal/Project/Project/Deployment/FileDeploy.cs \
Internal/Project/Project/Collections/ProjectFileCollection.cs \
Internal/Project/Project/Collections/ProjectReferenceCollection.cs \
Internal/Project/Project/UnknownProjectVersionException.cs \
Internal/Project/Project/TypelibImporter.cs \
Internal/Project/Project/AbstractProjectConfiguration.cs \
Internal/Project/Project/Project.cs \
Internal/Project/Project/DotNetProject.cs \
Internal/Project/Project/DotNetProjectBinding.cs \
Internal/Project/Project/DotNetProjectConfiguration.cs \
Internal/Project/Project/ProjectReference.cs \
Internal/Project/Project/ProjectReferenceEventArgs.cs \
Internal/Project/Project/ConvertXml.cs \
Internal/Project/Project/IncludeFilesDialog.cs \
Internal/Project/Project/IConfiguration.cs \
Internal/Project/Project/ProjectCompileEventHandler.cs \
Internal/Project/Project/ProjectFile.cs \
Internal/Project/Project/ProjectFileEventArgs.cs \
Internal/Project/Project/AbstractConfiguration.cs \
Internal/Project/Combine/Combine.cs \
Internal/Project/Combine/CombineExecuteDefinition.cs \
Internal/Project/Combine/CyclicBuildOrderException.cs \
Internal/Project/Combine/CombineConfiguration.cs \
Internal/Project/Combine/NoStartupCombineDefinedException.cs \
Internal/Project/Combine/CombineEntry.cs \
Internal/Project/Combine/CombineEntryEventArgs.cs \
Internal/Project/Combine/CombineEntryRenamedEventArgs.cs \
Internal/Templates/FileDescriptionTemplate.cs \
Internal/Templates/CodeTemplate.cs \
Internal/Templates/CodeTemplateGroup.cs \
Internal/Templates/FileTemplate.cs \
Internal/Templates/TextTemplate.cs \
Internal/Templates/CodeTemplateLoader.cs \
Internal/Templates/INewFileCreator.cs \
Internal/Templates/ProjectTemplates/ProjectTemplate.cs \
Internal/Templates/ProjectTemplates/ProjectDescriptor.cs \
Internal/Templates/ProjectTemplates/CombineDescriptor.cs \
Internal/Templates/ProjectTemplates/ProjectCreateInformation.cs \
Internal/CollectionUtilities/DiffUtility.cs \
Internal/CollectionUtilities/SortUtilityBase.cs \
Internal/CollectionUtilities/SortUtility.cs \
Internal/CollectionUtilities/Comparers.cs \
Internal/ExternalTool/ToolLoader.cs \
Internal/ExternalTool/ExternalTool.cs \
Internal/Serialization/ArrayHandler.cs \
Internal/Serialization/ArrayListHandler.cs \
Internal/Serialization/ClassDataType.cs \
Internal/Serialization/CollectionDataType.cs \
Internal/Serialization/DataContext.cs \
Internal/Serialization/DataNode.cs \
Internal/Serialization/DataCollection.cs \
Internal/Serialization/DataType.cs \
Internal/Serialization/DataIncludeAttribute.cs \
Internal/Serialization/DataItem.cs \
Internal/Serialization/DataItemAttribute.cs \
Internal/Serialization/DataValue.cs \
Internal/Serialization/GenericCollectionHandler.cs \
Internal/Serialization/IExtendedDataItem.cs \
Internal/Serialization/ICollectionHandler.cs \
Internal/Serialization/ExpandedCollectionAttribute.cs \
Internal/Serialization/ItemProperty.cs \
Internal/Serialization/ItemPropertyAttribute.cs \
Internal/Serialization/SerializationContext.cs \
Internal/Serialization/EnumDataType.cs \
Internal/Serialization/PrimitiveDataType.cs \
Internal/Serialization/XmlDataSerializer.cs \
Internal/Serialization/DataSerializer.cs \
AssemblyInfo.cs

OPTIONS = \
options/DefaultEditingLayout.xml \
options/MonoDevelopProperties.xml \
options/MonoDevelop-templates.xml \
options/MonoDevelop-tools.xml \
options/TipsOfTheDay.xml

build_OPTIONS = $(addprefix $(top_builddir)/build/data/options/, $(notdir $(OPTIONS)))

build_sources = $(addprefix $(srcdir)/, $(FILES))

ASSEMBLY = MonoDevelop.Base.dll

ADDIN = MonoDevelopCore.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_OPTIONS)

$(build_OPTIONS): $(top_builddir)/build/data/options/%.xml: $(srcdir)/options/%.xml
	mkdir -p $(top_builddir)/build/data/options
	cp $(srcdir)/options/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(build_sources) $(srcdir)/Base.glade
	$(CSC) $(MONO_POSIX_OEE) -g -codepage:utf8 /out:$(ASSEMBLY) /target:library $(DLLS) \
		/resource:$(srcdir)/Base.glade,Base.glade \
		$(monodoc_libflags) \
		$(build_sources) \
	&& cp $(ASSEMBLY) $(top_builddir)/build/bin/. \
	&& cp $(ASSEMBLY).config $(top_builddir)/build/bin/.

Services/StockIcons.cs : ./Services/process-icons.pl ../../../build/data/resources/icons
	perl  -x$(srcdir)/../../../build/data/resources/icons -w $(srcdir)/./Services/process-icons.pl > $(srcdir)/Services/StockIcons.cs

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(ASSEMBLY) $(ASSEMBLY).config

addindir = $(libdir)/monodevelop/AddIns
addin_DATA = $(ADDIN)

optionsdir = $(libdir)/monodevelop/data/options
options_DATA = $(OPTIONS)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
DISTCLEANFILES = $(ASSEMBLY).config

EXTRA_DIST = $(FILES) ./Services/process-icons.pl $(ASSEMBLY).config.in Base.glade $(ADDIN) $(OPTIONS)

