MCS=mcs
ASSEMBLY = ../build/bin/gtkmozembed-sharp.dll

INCLUDE_APIS = \
	@prefix@/share/gapi/gdk-api.xml \
	@prefix@/share/gapi/gtk-api.xml \
	@prefix@/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

generated-stamp: gtkmozembed-api.xml
	gapi-fixup --api=gtkmozembed-api.xml --metadata=GtkMozEmbed.metadata && \
	 	gapi-codegen --generate gtkmozembed-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gtkmozembed-sharp && touch generated-stamp

$(ASSEMBLY): generated-stamp
	$(MCS) --unsafe --target library \
	-r glib-sharp.dll -r gtk-sharp.dll -r gdk-sharp.dll \
	--recurse '*.cs' -o $(ASSEMBLY)
 
CLEANFILES=$(ASSEMBLY) generated-stamp generated
