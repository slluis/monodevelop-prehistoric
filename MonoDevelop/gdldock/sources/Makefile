PREFIX ?= $(shell pkg-config --variable=prefix gapi)
PERLLIB = $(PREFIX)/share/perl5

all:

api: gdl/*.h
	gapi-parser gdl-sharp-sources.xml

