
ASSEMBLY = prj2make-sharp-lib.dll
ADDIN = $(srcdir)/prj2make-sharp-lib.addin.xml


DLLS = $(GTK_SHARP_LIBS) \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Gui.Widgets.dll


FILES = \
MsPrjHelper.cs \
PrjHelper.cs \
MdPrjHelper.cs \
CsprojInfo.cs \
PrjxInfo.cs \
pkgconfiginvoker.cs \
sharp_d_cmbx.cs \
sharp_d_prjx.cs \
csproj_ver2003.cs \
AssemblyInfo.cs \
prj2make-sharp-lib-Commands.cs
	
EXTRA_DIST = $(FILES) $(ADDIN)

all: $(ASSEMBLY)

build_sources = $(addprefix $(srcdir)/, $(FILES))

$(top_builddir)/build/AddIns/$(ADDIN):
	cp $(ADDIN) $(top_builddir)/build/AddIns/.

$(ASSEMBLY): $(build_sources) $(top_builddir)/build/AddIns/$(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS)\
	&& cp $(ASSEMBLY) $(top_builddir)/build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

