MCS = mcs
RUNTIME = mono
ASSEMBLY = gdl-sharp.dll

INCLUDE_APIS = \
	@gtksharp_prefix@/share/gapi/gdk-api.xml \
	@gtksharp_prefix@/share/gapi/gtk-api.xml \
	@gtksharp_prefix@/share/gapi/atk-api.xml

all: $(ASSEMBLY)

gdl-api.xml: $(srcdir)/gdl-api.raw $(srcdir)/Gdl.metadata
	cp $(srcdir)/gdl-api.raw gdl-api.xml
	chmod u+w gdl-api.xml
	gapi-fixup --api=gdl-api.xml --metadata=$(srcdir)/Gdl.metadata

generated-stamp: gdl-api.xml
	gapi-codegen --generate gdl-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=$(srcdir) --assembly-name=gdl-sharp && touch generated-stamp

$(ASSEMBLY): generated-stamp ../../build/bin
	$(MCS) --unsafe --target library \
	-r glib-sharp -r gtk-sharp -r gdk-sharp \
	--recurse '*.cs' -o $(ASSEMBLY) \
	&& cp $(ASSEMBLY) ../../build/bin/.

../../build/bin:
	mkdir ../../build/bin

gdldocklibdir = $(prefix)/lib/monodevelop/bin
gdldocklib_DATA = $(ASSEMBLY)

clean:
	rm -f $(ASSEMBLY)
	rm -f generated-stamp
	rm -rf generated

CLEANFILES=$(ASSEMBLY) generated-stamp generated/* gdl-api.xml

EXTRA_DIST = gdl-api.raw Gdl.metadata DockItemFlags.cs DockParamFlags.cs DockLayout.custom

