ASSEMBLY = gdl-sharp.dll

API = gdl-api.xml
RAWAPI = gdl-api.raw
METADATA = Gdl.metadata

INCLUDE_APIS = \
	@gtksharp_prefix@/share/gapi/gdk-api.xml \
	@gtksharp_prefix@/share/gapi/gtk-api.xml \
	@gtksharp_prefix@/share/gapi/atk-api.xml

FILES = \
	DockItemFlags.cs \
	DockParamFlags.cs

CUSTOM_FILES = \
	DockLayout.custom

build_sources = $(addprefix $(srcdir)/, $(FILES))
build_customs = $(addprefix $(srcdir)/, $(CUSTOM_FILES))

all: $(ASSEMBLY)

$(API): $(srcdir)/$(RAWAPI) $(srcdir)/$(METADATA)
	cp $(srcdir)/$(RAWAPI) $(API)
	chmod u+w $(API)
	gapi-fixup --api=$(API) --metadata=$(srcdir)/$(METADATA)

generated-stamp: $(API) $(build_customs)
	gapi-codegen --generate $(API) --include $(INCLUDE_APIS) --outdir=generated --customdir=$(srcdir) --assembly-name=gdl-sharp && touch generated-stamp

$(ASSEMBLY): $(build_sources) generated-stamp
	mkdir -p ../../build/bin
	$(CSC) --unsafe -target:library \
	@BASE_DEPENDENCIES_LIBS@ \
	$(build_sources) generated/*.cs /out:$(ASSEMBLY) \
	&& cp -p $(ASSEMBLY) ../../build/bin/. \
	&& cp -p $(ASSEMBLY).config ../../build/bin/.

gdldocklibdir = $(libdir)/monodevelop/bin
gdldocklib_DATA = $(ASSEMBLY) $(ASSEMBLY).config

clean-local:
	rm -rf generated

CLEANFILES = $(ASSEMBLY) generated-stamp $(API)
DISTCLEANFILES = $(ASSEMBLY).config

EXTRA_DIST = $(RAWAPI) $(METADATA) $(FILES) $(CUSTOM_FILES) $(ASSEMBLY).config.in

