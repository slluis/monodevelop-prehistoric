
CSC = mcs /debug
ASSEMBLY = AddInManager.dll
ADDIN = AddInManager.addin.xml

REFS = \
	-pkg:gtk-sharp \
	/r:../../../../build/bin/MonoDevelop.Core.dll \
	/r:../../../../build/bin/MonoDevelop.Base.dll

FILES = \
./AssemblyInfo.cs \
./Commands/ShowDialog.cs \
./Gui/AddInManagerDialog.cs

addindir = $(libdir)/monodevelop/AddIns
addin_DATA = $(ADDIN) $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) /out:$(ASSEMBLY) /target:library $(FILES) $(REFS) \
	&& cp $(ASSEMBLY) ../../../../build/AddIns/. \
	&& cp $(ADDIN) ../../../../build/AddIns/.

CLEANFILES = $(ASSEMBLY)
EXTRA_DIST = $(FILES) $(ADDIN)
