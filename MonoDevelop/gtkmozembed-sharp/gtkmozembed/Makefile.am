MCS=mcs

INCLUDE_APIS = \
	@prefix@/share/gapi/gdk-api.xml \
	@prefix@/share/gapi/gtk-api.xml \
	@prefix@/share/gapi/atk-api.xml
			
all: generated-stamp gtkmozembed-sharp.dll

generated-stamp: gtkmozembed-api.xml
	gapi-fixup --api=gtkmozembed-api.xml --metadata=GtkMozEmbed.metadata && \
	 	gapi-codegen --generate gtkmozembed-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gtkmozembed-sharp && touch generated-stamp
 
gtkmozembed-sharp.dll: generated-stamp
	$(MCS) --unsafe --target library -L @prefix@/lib \
	-r glib-sharp.dll -r gtk-sharp.dll -r gdk-sharp.dll \
	--recurse '@srcdir@/*.cs' -o gtkmozembed-sharp.dll
 
clean:
	rm -f *.dll
	rm -f generated-stamp
	rm -f generated/*.cs
 
maintainerclean: clean
	rm -rf generated
 
install: all
	cp gtkmozembed-sharp.dll $(DESTDIR)@prefix@/lib
 
EXTRA_DIST = @srcdir@/generated/*.cs
