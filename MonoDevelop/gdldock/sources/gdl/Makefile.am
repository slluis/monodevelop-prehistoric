INCLUDES = \
	-DG_LOG_DOMAIN=\"GdlDock\" -I..	-I.	\
	-DGDL_GLADEDIR=\".\"			\
	-DGDL_IMAGESDIR=\".\"			\
	$(GDLDOCK_CFLAGS)

gdldocklibdir = $(prefix)/lib/monodevelop/bin

gdldocklib_LTLIBRARIES = libgdldock.la

dist_gdldocklib_DATA = \
	stock-close-12.png \
	stock-menu-left-12.png \
	stock-menu-right-12.png

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

libgdldock_la_SOURCES = \
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
	gdl-tools.h \
	libgdltypebuiltins.c \
	libgdlmarshal.c

libgdldock_la_LIBADD = $(GDLDOCK_LIBS)
libgdldock_la_LDFLAGS = -avoid-version

BUILT_SOURCES = \
	libgdltypebuiltins.h \
	libgdltypebuiltins.c \
	libgdlmarshal.h \
	libgdlmarshal.c

EXTRA_DIST = libgdlmarshal.list layout.glade

CLEANFILES = $(BUILT_SOURCES)

# Install built library to build/bin

# The `pwd` is a hack, but libtool wants an absolute path, and
# top_builddir is apparently always relative to the current dir.
# When we depend on Autoconf 2.53 we can use @abs_top_builddir@
# instead.

all-local:
	$(LIBTOOL) --mode=install $(INSTALL) libgdldock.la `pwd`/$(top_builddir)/build/bin
	$(INSTALL_DATA) $(dist_gdldocklib_DATA) $(top_builddir)/build/bin

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

