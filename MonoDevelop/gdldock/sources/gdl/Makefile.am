srcdir = .

CFLAGS = -g -Wall -O2
LDFLAGS = -g -shared

GLIB_GENMARSHAL = $(shell which glib-genmarshal)
GLIB_MKENUMS = $(shell which glib-mkenums)

GDL_REQUIREMENTS = gtk+-2.0 libglade-2.0 libxml-2.0
GDL_DEPENDENCIES_CFLAGS = $(shell pkg-config --cflags $(GDL_REQUIREMENTS))
GDL_DEPENDENCIES_LIBS = $(shell pkg-config --libs $(GDL_REQUIREMENTS))

INCLUDES = \
	-DG_LOG_DOMAIN=\"GdlDock\" -I..	-I. \
	$(GDL_DEPENDENCIES_CFLAGS)

libgdldock_headers = \
	gdl-dock-bar.h \
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
	gdl-stock.h

libgdldock_SOURCE = \
	$(libgdldock_headers) \
	gdl-i18n.h \
	gdl-dock-bar.c \
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
	gdl-stock.c \
	libgdltypebuiltins.c \
	libgdlmarshal.c

libgdldock_OBJECTS = $(filter-out %.h,$(patsubst %.c,%.o,$(libgdldock_SOURCE)))

GENERATED_SOURCE = \
	libgdltypebuiltins.h \
	libgdltypebuiltins.c \
	libgdlmarshal.h \
	libgdlmarshal.c

all: libgdldock.so

$(libgdldock_OBJECTS): $(libgdldock_headers) libgdltypebuiltins.h libgdlmarshal.h

libgdldock.so: $(libgdldock_OBJECTS)
	$(CC) $(LDFLAGS) -shared -fPIC -o libgdldock.so $+ && \
	cp libgdldock.so ../../../build/bin && \
	cp *.png ../../../build/bin

%.o: %.c
	$(CC) $(CFLAGS) $(INCLUDES) -c $<
	
# enumerations code generation

libgdltypebuiltins.c: $(libgdldock_headers)
	$(GLIB_MKENUMS) \
			--fhead "#include <glib-object.h>\n" \
			--fhead "#include \"libgdltypebuiltins.h\"\n\n" \
			--fprod "\n/* enumerations from \"@filename@\" */" \
			--vhead "static const G@Type@Value _@enum_name@_values[] = {" \
			--vprod "  { @VALUENAME@, \"@VALUENAME@\", \"@valuenick@\" }," \
			--vtail "  { 0, NULL, NULL }\n};\n\n" \
			--vtail "GType\n@enum_name@_get_type (void)\n{\n" \
			--vtail "  static GType type = 0;\n\n" \
			--vtail "  if (!type)\n" \
			--vtail "    type = g_@type@_register_static (\"@EnumName@\", _@enum_name@_values);\n\n" \
			--vtail "  return type;\n}\n\n" \
		$(libgdldock_headers) > xgen-gtbec \
	&& cp xgen-gtbec $(@F) \
	&& rm -f xgen-gtbec

libgdltypebuiltins.h: $(libgdldock_headers)
	$(GLIB_MKENUMS) \
			--fhead "#ifndef __LIBGDLTYPEBUILTINS_H__\n" \
			--fhead "#define __LIBGDLTYPEBUILTINS_H__ 1\n\n" \
			--fhead "#include <gdl/gdl-dock.h>\n\n" \
			--fhead "G_BEGIN_DECLS\n\n" \
			--ftail "G_END_DECLS\n\n" \
			--ftail "#endif /* __LIBGDLTYPEBUILTINS_H__ */\n" \
			--fprod "\n/* --- @filename@ --- */" \
			--eprod "#define GDL_TYPE_@ENUMSHORT@ @enum_name@_get_type()\n" \
			--eprod "GType @enum_name@_get_type (void);\n" \
		$(libgdldock_headers) > xgen-gth \
	&& cp xgen-gth $(@F) \
	&& rm -f xgen-gth

libgdlmarshal.h: libgdlmarshal.list
	$(GLIB_GENMARSHAL) \
		--prefix=gdl_marshal $(srcdir)/libgdlmarshal.list --header > xgen-gmc \
	&& cp xgen-gmc $(@F) \
	&& rm -f xgen-gmc

libgdlmarshal.c: libgdlmarshal.h
	echo "#include \"libgdlmarshal.h\"" > xgen-gmc && \
	$(GLIB_GENMARSHAL) \
		--prefix=gdl_marshal $(srcdir)/libgdlmarshal.list --body >> xgen-gmc \
	&& cp xgen-gmc $(@F) \
	&& rm -f xgen-gmc

gdldocklibdir = $(prefix)/lib/monodevelop/bin
gdldocklib_DATA = libgdldock.so stock-close-12.png \
	stock-menu-left-12.png stock-menu-right-12.png

clean:
	rm -f libgdldock.so $(libgdldock_OBJECTS) $(GENERATED_SOURCE)

CLEANFILES = libgdldock.so

.PHONY: clean

EXTRA_DIST = $(libgdldock_SOURCE) $(libgdldock_headers) stock-close-12.png stock-menu-left-12.png stock-menu-right-12.png libgdlmarshal.list gdl-tools.h
