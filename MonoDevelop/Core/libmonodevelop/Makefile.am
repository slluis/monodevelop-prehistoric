INCLUDES =							\
	-I$(top_srcdir)						\
	-DGNOMELOCALEDIR=\""$(datadir)/locale"\"

monodeveloplibdir = $(prefix)/lib/monodevelop/bin
monodeveloplib_LTLIBRARIES = libmonodevelop.la

libmonodevelop_la_SOURCES =		\
	gettext-utils.c			\
	gettext-utils.h	

all-local:
	$(LIBTOOL) --mode=install $(INSTALL) libmonodevelop.la `pwd`/$(top_builddir)/build/bin
