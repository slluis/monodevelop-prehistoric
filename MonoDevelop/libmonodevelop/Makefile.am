srcdir = .

CFLAGS = -g -Wall -O2
LDFLAGS = -g -shared

MDL_REQUIREMENTS = gtk+-2.0
MDL_DEPENDENCIES_CFLAGS = $(shell pkg-config --cflags $(MDL_REQUIREMENTS))
MDL_DEPENDENCIES_LIBS = $(shell pkg-config --libs $(MDL_REQUIREMENTS))

INCLUDES = \
	-DG_LOG_DOMAIN=\"LibMonoDevelop\" -I..	-I. \
	$(MDL_DEPENDENCIES_CFLAGS)

libmonodevelop_headers =

libmonodevelop_SOURCE = \
	$(libmonodevelop_headers) \
	lmd-key-propagate.c

libmonodevelop_OBJECTS = $(filter-out %.h,$(patsubst %.c,%.o,$(libmonodevelop_SOURCE)))

all: libmonodevelop.so

$(libmonodevelop_OBJECTS): $(libmonodevelop_headers)

libmonodevelop.so: $(libmonodevelop_OBJECTS)
	$(CC) $(LDFLAGS) -shared -fPIC -o libmonodevelop.so $+ && \
	cp libmonodevelop.so ../build/bin/.

%.o: %.c
	$(CC) $(CFLAGS) $(INCLUDES) -c $<
	
monodeveloplibdir = $(prefix)/lib/monodevelop/bin
monodeveloplib_DATA = libmonodevelop.so

clean:
	rm -f libmonodevelop.so $(libmonodevelop_OBJECTS)

CLEANFILES= ../build/bin/libmonodevelop.so libmonodevelop.so

.PHONY: clean

EXTRA_DIST = $(libmonodevelop_SOURCE) $(libmonodevelop_headers)
