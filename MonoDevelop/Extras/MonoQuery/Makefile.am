
monoquerydir = $(libdir)/monodevelop/AddIns/MonoQuery
monoquery_DATA = $(ASSEMBLY) MonoQuery.addin.xml

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

ASSEMBLY = MonoQuery.dll

REFERENCES = \
System.Data \
Npgsql \
ByteFX.Data \
../../build/bin/MonoDevelop.Base.dll \
../../build/bin/MonoDevelop.Core.dll \
../../build/bin/MonoDevelop.Gui.Widgets.dll

RESOURCES = \
Gui/Forms/Glade/monoquery.glade

PACKAGES = \
gtk-sharp-2.0 \
gnome-sharp-2.0 \
glade-sharp-2.0 \
gtksourceview-sharp

BUILD_SOURCES = $(addprefix $(srcdir)/, $(FILES))
BUILD_REFERENCES = $(addprefix /r:, $(REFERENCES))
BUILD_PACKAGES = $(addprefix /pkg:, $(PACKAGES))
BUILD_RESOURCES = $(addprefix /resource:, $(RESOURCES))

all: $(ASSEMBLY)

$(ASSEMBLY): mkdirs $(BUILD_SOURCES) ../../build/AddIns/MonoQuery/MonoQuery.addin.xml
	mcs -out:MonoQuery.dll -target:library $(BUILD_SOURCES) \
	$(BUILD_REFERENCES) $(BUILD_PACKAGES) $(BUILD_RESOURCES) \
	&& cp MonoQuery.dll ../../build/AddIns/MonoQuery/

../../build/AddIns/MonoQuery/MonoQuery.addin.xml:
	cp MonoQuery.addin.xml ../../build/AddIns/MonoQuery/

mkdirs:
	mkdir -p ../../build/AddIns/MonoQuery

CLEANFILES=$(ASSEMBLY) $(ASSEMBLY).mdb

EXTRADIST=$(FILES) MonoQuery.addin.xml
