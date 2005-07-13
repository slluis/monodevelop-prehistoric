
ADDIN_BUILD = $(top_builddir)/build/AddIns/NUnit
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.NUnit.dll

DLLS = /r:nunit.framework.dll \
	/r:nunit.core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
	$(GTK_SHARP_LIBS) \
	$(GLADE_SHARP_LIBS)

FILES = \
AssemblyInfo.cs \
Commands/NUnitCommands.cs \
Gui/CircleImage.cs \
Gui/TestPad.cs \
Gui/TestResultsPad.cs \
Gui/TestNodeBuilder.cs \
Gui/TestChart.cs \
Gui/NUnitOptionsPanel.cs \
Gui/UnitTestOptionsDialog.cs \
Gui/NUnitAssemblyGroupNodeBuilder.cs \
Gui/NUnitAssemblyGroupConfigurationNodeBuilder.cs \
Gui/TestAssemblyNodeBuilder.cs \
Project/NUnitAssemblyGroupFileFormat.cs \
Project/NUnitAssemblyGroupProject.cs \
Project/TestAssembly.cs \
Project/TestAssemblyCollection.cs \
Services/CombineTestGroup.cs \
Services/ITestProvider.cs \
Services/NUnitService.cs \
Services/SystemTestProvider.cs \
Services/ITestProgressMonitor.cs \
Services/TestContext.cs \
Services/UnitTestCollection.cs \
Services/UnitTest.cs \
Services/UnitTestGroup.cs \
Services/UnitTestResult.cs \
Services/UnitTestStatus.cs \
Services/ExternalTestRunner.cs \
Services/NUnitAssemblyTestSuite.cs \
Services/NUnitProjectTestSuite.cs \
Services/NUnitTestCase.cs \
Services/NUnitTestSuite.cs \
Services/GeneralTestOptions.cs \
Services/NUnitOptions.cs \
Services/IResultsStore.cs \
Services/UnitTestResultsStore.cs \
Services/XmlResultsStore.cs

TEMPLATES = \
templates/NUnitAssemblyGroup.xpt.xml

RES = -resource:$(srcdir)/Gui/NUnit.Running.png \
	-resource:$(srcdir)/Gui/NUnit.Failed.png \
	-resource:$(srcdir)/Gui/NUnit.SuccessAndFailed.png \
	-resource:$(srcdir)/Gui/NUnit.None.png \
	-resource:$(srcdir)/Gui/NUnit.NotRun.png \
	-resource:$(srcdir)/Gui/NUnit.Loading.png \
	-resource:$(srcdir)/Gui/NUnit.Success.png \
	-resource:$(srcdir)/nunit.glade

ADDIN = MonoDevelopNUnit.addin.xml

TEMPLATES_DIR = $(ADDIN_BUILD)/templates

build_TEMPLATES = $(addprefix $(TEMPLATES_DIR)/, $(notdir $(TEMPLATES)))

src_TEMPLATES = $(addprefix $(srcdir)/, $(TEMPLATES))

if ENABLE_NUNIT
all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN) $(build_TEMPLATES)
else
all:
endif

$(filter %.xft.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xft.xml: $(srcdir)/templates/%.xft.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(filter %.xpt.xml, $(build_TEMPLATES)): $(TEMPLATES_DIR)/%.xpt.xml: $(srcdir)/templates/%.xpt.xml
	mkdir -p $(TEMPLATES_DIR)
	cp $(srcdir)/templates/$(notdir $@) $@

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ASSEMBLY): $(FILES) $(srcdir)/nunit.glade
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(DLLS) $(RES) $(build_sources) -out:$@ -target:library

if ENABLE_NUNIT
assemblydir = $(MD_ADDIN_DIR)/NUnit
assembly_DATA = $(ASSEMBLY) $(ADDIN)

templatedir = $(assemblydir)/templates
template_DATA = $(TEMPLATES)
endif

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) nunit.glade $(ADDIN) $(TEMPLATES)

include $(top_srcdir)/Makefile.include
