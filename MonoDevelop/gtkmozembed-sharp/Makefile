MCS=mcs
ASSEMBLY = ../build/bin/gtkmozembed-sharp.dll

INCLUDE_APIS = \
	$(PREFIX)/share/gapi/gdk-api.xml \
	$(PREFIX)/share/gapi/gtk-api.xml \
	$(PREFIX)/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

generated-stamp: gtkmozembed-api.xml
	gapi-fixup --api=gtkmozembed-api.xml --metadata=GtkMozEmbed.metadata && \
	 	gapi-codegen --generate gtkmozembed-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gtkmozembed-sharp && touch generated-stamp

$(ASSEMBLY): generated-stamp
	$(MCS) --unsafe --target library -L @prefix@/lib \
	-r glib-sharp.dll -r gtk-sharp.dll -r gdk-sharp.dll \
	--recurse '*.cs' -o $(ASSEMBLY)
 
clean:
	rm -f generated-stamp
	rm -rf generated
 
