MCS = mcs
RUNTIME = mono
ASSEMBLY = ../../build/bin/gdl-sharp.dll

INCLUDE_APIS = \
	@gtksharp_prefix@/share/gapi/gdk-api.xml \
	@gtksharp_prefix@/share/gapi/gtk-api.xml \
	@gtksharp_prefix@/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

generated-stamp: gdl-api.xml
	gapi-fixup --api=gdl-api.xml --metadata=Gdl.metadata && \
	gapi-codegen --generate gdl-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gdl-sharp && touch generated-stamp
 
$(ASSEMBLY): generated-stamp ../../build/bin
	$(MCS) --unsafe --target library \
	-r glib-sharp.dll -r gtk-sharp.dll -r gdk-sharp.dll \
	--recurse '*.cs' -o $(ASSEMBLY)

../../build/bin:
	mkdir ../../build/bin

gdldocklibdir = $(prefix)/lib/monodevelop

install-data-local: $(ASSEMBLY)
	$(mkinstalldirs) $(DESTDIR)$(gdldocklibdir)/bin
	$(INSTALL_DATA) $(ASSEMBLY) $(DESTDIR)$(gdldocklibdir)/bin/gdl-sharp.dll
 
clean:
	rm -f $(ASSEMBLY)
	rm -f generated-stamp
	rm -rf generated

CLEANFILES=$(ASSEMBLY) generated-stamp generated

EXTRA_DIST = gdl-api.xml Gdl.metadata DockItemFlags.cs DockParamFlags.cs
