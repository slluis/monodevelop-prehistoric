FILES = *.cs

REFS = /r:$(ADDIN_BUILD)/Diff.dll \
	   /r:$(ADDIN_BUILD)/DiffWidget.dll \
	   /r:$(ADDIN_BUILD)/VersionControl.dll \
	   /r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll \
       /r:$(top_builddir)/build/bin/MonoDevelop.SourceEditor.dll \
       /r:System.Data.dll \
       $(GTK_SHARP_LIBS) \
       $(GLADE_SHARP_LIBS) \
       $(GCONF_SHARP_LIBS) \
       $(GTKSOURCEVIEW_SHARP_LIBS)

RES = -resource:overlay_added.png \
	  -resource:overlay_modified.png \
	  -resource:overlay_conflicted.png \
	  -resource:overlay_normal.png \
	  -resource:overlay_locked.png

ADDIN = VersionControl.addin.xml

ADDIN_BUILD = $(top_builddir)/build/AddIns/VersionControl

DLL = $(ADDIN_BUILD)/VersionControlAddIn.dll

all: $(ADDIN_BUILD)/$(ADDIN) $(DLL)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(DLL): $(FILES)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) -target:library -out:$@ $(FILES) $(REFS) $(RES)

CLEANFILES = $(DLL) $(DLL).mdb $(ADDIN_BUILD)/$(ADDIN)

EXTRA_DIST = $(FILES) $(ADDIN)


