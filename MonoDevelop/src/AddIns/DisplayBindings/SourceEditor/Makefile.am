DLL=../../../../build/bin/MonoDevelop.SourceEditor.dll

all : $(DLL)

$(DLL) : $(shell find . -name \*.cs)
	mcs $(shell find . -name \*.cs) /out:$@ /t:library \
	/resource:../../../../data/resources/glade/EditorBindings.glade,EditorBindings.glade \
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
