MCS=mcs
ASSEMBLY = gtkmozembed-sharp.dll

INCLUDE_APIS = \
	@gtksharp_prefix@/share/gapi/gdk-api.xml \
	@gtksharp_prefix@/share/gapi/gtk-api.xml \
	@gtksharp_prefix@/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

gtkmozembed-api.xml:
	cp $(srcdir)/gtkmozembed-api.raw gtkmozembed-api.xml
	chmod u+w gtkmozembed-api.xml
	gapi-fixup --api=gtkmozembed-api.xml --metadata=$(srcdir)/GtkMozEmbed.metadata

generated-stamp: gtkmozembed-api.xml
	gapi-codegen --generate gtkmozembed-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=$(srcdir) --assembly-name=gtkmozembed-sharp && touch generated-stamp

sources = \
	NewWindow_delegate.cs \
	NewWindow_handler.cs \
	NewWindowOrphan_handler.cs

build_sources = $(addprefix $(srcdir)/, $(sources))

customs = \
	EmbedWidget.custom \
	Single.custom

build_customs = $(addprefix $(srcdir)/, $(customs))

$(ASSEMBLY): generated-stamp $(build_sources)
	$(MCS) --unsafe --target library \
	-r glib-sharp -r gtk-sharp -r gdk-sharp \
	generated/*.cs $(build_sources) -o $(ASSEMBLY) \
	&& cp $(ASSEMBLY) ../build/bin/.

assemblydir = $(prefix)/lib/monodevelop/bin
assembly_DATA = $(ASSEMBLY)
 
CLEANFILES=$(ASSEMBLY) generated-stamp generated/*.cs gtkmozembed-api.xml

EXTRA_DIST = GtkMozEmbed.metadata gtkmozembed-api.raw $(customs) $(sources)

