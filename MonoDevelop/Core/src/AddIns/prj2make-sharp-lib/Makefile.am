
ASSEMBLY = prj2make-sharp-lib.dll
ADDIN = $(srcdir)/prj2make-sharp-lib.addin.xml


DLLS = $(BASE_DEPENDENCIES_LIBS) \
	/r:../../../../build/bin/MonoDevelop.Base.dll \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.Gui.Widgets.dll


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

../../../../build/AddIns/$(ADDIN):
	cp $(ADDIN) ../../../../build/AddIns/.

$(ASSEMBLY): $(build_sources) ../../../../build/AddIns/$(ADDIN)
	$(CSC) /out:$(ASSEMBLY) /target:library \
	$(build_sources) $(DLLS)\
	&& cp $(ASSEMBLY) ../../../../build/AddIns/

assemblydir = $(libdir)/monodevelop/AddIns
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

