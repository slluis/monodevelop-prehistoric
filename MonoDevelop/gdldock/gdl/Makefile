MCS = mcs
RUNTIME = mono
ASSEMBLY = ../../build/bin/gdl-sharp.dll
PREFIX ?= $(shell pkg-config --variable=prefix gapi)

INCLUDE_APIS = \
	$(PREFIX)/share/gapi/gdk-api.xml \
	$(PREFIX)/share/gapi/gtk-api.xml \
	$(PREFIX)/share/gapi/atk-api.xml
			
all: generated-stamp $(ASSEMBLY)

generated-stamp: gdl-api.xml
	gapi-fixup --api=gdl-api.xml --metadata=Gdl.metadata && \
	gapi-codegen --generate gdl-api.xml --include $(INCLUDE_APIS) --outdir=generated --customdir=. --assembly-name=gdl-sharp && touch generated-stamp
 
$(ASSEMBLY): generated-stamp
	$(MCS) --unsafe --target library \
	-r glib-sharp.dll -r gtk-sharp.dll -r gdk-sharp.dll \
	--recurse '*.cs' -o $(ASSEMBLY)
 
clean:
	rm -f $(ASSEMBLY)
	rm -f generated-stamp
	rm -rf generated

