
CSC = mcs /debug
ASSEMBLY = SharpCoco.exe
ATG = src/Coco.ATG

FILES = src/AssemblyInfo.cs \
src/Parser.cs \
src/Trace.cs \
src/Coco.cs \
src/Tab.cs \
src/ParserGen.cs \
src/Scanner.cs \
src/DFA.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

all: $(ASSEMBLY)

$(ASSEMBLY): $(build_sources)
	$(CSC) $(build_sources) /out:$(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)
EXTRA_DIST = $(FILES)

