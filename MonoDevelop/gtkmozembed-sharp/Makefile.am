MCS=mcs
ASSEMBLY = gtkmozembed-sharp.dll

INCLUDE_APIS = \
	@gtksharp_prefix@/share/gapi/gdk-api.xml \
	@gtksharp_prefix@/share/gapi/gtk-api.xml \
	@gtksharp_prefix@/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

generated-stamp: gtkmozembed-api.xml
	gapi-fixup --api=gtkmozembed-api.xml --metadata=GtkMozEmbed.metadata && \
	 	gapi-codegen --generate gtkmozembed-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gtkmozembed-sharp && touch generated-stamp

$(ASSEMBLY): generated-stamp
	$(MCS) --unsafe --target library \
	-r glib-sharp -r gtk-sharp -r gdk-sharp \
	--recurse '*.cs' -o $(ASSEMBLY) \
	&& cp $(ASSEMBLY) ../build/bin/.

assemblydir = $(prefix)/lib/monodevelop/bin
assembly_DATA = $(ASSEMBLY)
 
CLEANFILES=$(ASSEMBLY) generated-stamp generated

EXTRA_DIST = GtkMozEmbed.metadata gtkmozembed-api.xml EmbedWidget.custom Single.custom NewWindow_delegate.cs NewWindow_handler.cs NewWindowOrphan_handler.cs

