INCLUDES = \
	-DG_LOG_DOMAIN=\"Gdl\"				\
	-I$(includedir) -I$(top_srcdir)			\
	$(WARN_CFLAGS) $(DEPRECATED_FLAGS)		\
	$(GDL_DEPENDENCIES_CFLAGS) $(XML_CFLAGS)

lib_LTLIBRARIES = libgdl-1.la

libgdl_headers = \
	gdl.h \
	gdl-dock-object.h \
	gdl-dock-master.h \
	gdl-dock.h \
	gdl-dock-item.h \
	gdl-dock-item-grip.h \
	gdl-dock-notebook.h \
	gdl-dock-paned.h \
	gdl-dock-tablabel.h \
	gdl-dock-layout.h \
	gdl-dock-placeholder.h \

libgdl_1_la_SOURCES = \
	$(libgdl_headers) \
	gdl-dock-object.c \
	gdl-dock-master.c \
	gdl-dock.c \
	gdl-dock-item.c \
	gdl-dock-item-grip.c \
	gdl-dock-notebook.c \
	gdl-dock-paned.c \
	gdl-dock-tablabel.c \
	gdl-dock-layout.c \
	gdl-dock-placeholder.c \

libgdl_1_la_LIBADD = \
	$(GDL_DEPENDENCIES_LIBS) \
	$(XML_LIBS)

