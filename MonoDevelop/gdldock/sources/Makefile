PREFIX ?= $(shell pkg-config --variable=prefix gapi)
PERLLIB = $(PREFIX)/share/perl5

all:
	cd gdl && make

api: gdl/*.h
	gapi-parser gdl-sharp-sources.xml

