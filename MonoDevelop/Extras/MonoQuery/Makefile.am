FILES = \
AssemblyInfo.cs \
Codons/MonoQueryConnectionCodon.cs \
Collection/MonoQuerySchemaClassCollection.cs \
Collection/MonoQueryStringDictionary.cs \
Collection/MonoQueryListDictionary.cs \
Collection/MonoQueryParameterCollection.cs \
Collection/ConnectionProviderDescriptor.cs \
Commands/Abstract/AbstractMonoQueryCommand.cs \
Commands/MonoQueryCommands.cs \
Connection/Abstract/AbstractMonoQueryConnectionWrapper.cs \
Connection/Interface/IConnection.cs \
Connection/Npgsql/NpgsqlConnectionWrapper.cs \
Connection/Mysql/MysqlConnectionWrapper.cs \
Exceptions/Abstract/MonoQueryAbstractException.cs \
Exceptions/ConnectionStringException.cs \
Exceptions/ExecuteProcedureException.cs \
Exceptions/ExecuteSQLException.cs \
Exceptions/OpenConnectionException.cs \
SchemaClass/Abstract/AbstractMonoQuerySchemaClass.cs \
SchemaClass/Interface/ISchemaClass.cs \
SchemaClass/MonoQuerySchemaClass.cs \
MonoQueryView.cs \
Gui/Abstract/AbstractMonoQueryNode.cs \
Gui/DataView/SQLParameterInput.cs \
Gui/DataView/MonoQueryDataView.cs \
Gui/Interface/IMonoQueryNode.cs \
Gui/MonoQueryTree/MonoQueryNodesRoot.cs \
Gui/MonoQueryTree/MonoQueryTree.cs \
Gui/MonoQueryTree/MonoQueryDataNodes.cs \
Gui/MonoQueryPanel.cs \
Gui/Forms/CreateConnectionDruid.cs \
Gui/SqlQueryView/SqlQueryView.cs \
Services/MonoQueryService.cs

if ENABLE_MONOQUERY_SQLITE
SQLITE_FILES=Connection/Sqlite/SqliteConnectionWrapper.cs
else
SQLITE_FILES=
endif


build_sources = $(addprefix $(srcdir)/, $(FILES)) \
	$(addprefix $(srcdir)/, $(SQLITE_FILES))

REFS = /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
       /r:Npgsql.dll \
       /r:System.Data.dll \
       /r:ByteFX.Data.dll \
       /r:Mono.Data.SqliteClient \
       $(GTK_SHARP_LIBS) \
       $(GLADE_SHARP_LIBS) \
       $(GCONF_SHARP_LIBS) \
       $(GTKSOURCEVIEW_SHARP_LIBS)

ADDIN = MonoQuery.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoQuery

DLL = MonoQuery.dll

DLL_BUILD = $(top_builddir)/build/AddIns/MonoQuery

all: $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL): $(build_sources) $(srcdir)/$(GLADEFILE)
	mcs -target:library -out:$(DLL) $(build_sources) $(REFS)

$(DLL_BUILD)/$(DLL): $(srcdir)/$(DLL)
	mkdir -p $(DLL_BUILD)
	cp $(srcdir)/$(DLL) $(DLL_BUILD)/$(DLL)

CLEANFILES=$(DLL) $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)

EXTRADIST=$(FILES)

monoquerydir = $(libdir)/monodevelop/AddIns/MonoQuery
monoquery_DATA = $(DLL_BUILD)/$(DLL) $(ADDIN_BUILD)/$(ADDIN)
