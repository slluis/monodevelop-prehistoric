
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Visualizers.dll

DLLS = $(GTK_SHARP_LIBS) \
	$(MONO_DEBUGGER_LIBS) \
	/r:$(top_builddir)/build/bin/MonoDevelop.Base.dll \
	/r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
        /r:$(top_builddir)/build/bin/ICSharpCode.SharpRefactory.dll \
	/pkg:gtk-sharp


FILES = \
	DialogDebuggerVisualizer.cs \
	IDialogVisualizerService.cs \
	IVisualizerObjectProvider.cs \
	TargetObjectProvider.cs \
	VisualizerDevelopmentHost.cs \
	VisualizerObjectSource.cs


EXTRA_DIST = $(FILES)

if ENABLE_DEBUGGER

all: $(ASSEMBLY)

build_sources = $(addprefix $(srcdir)/, $(FILES))

$(ASSEMBLY): $(build_sources)
	$(CSC) -define:NET_2_0 -define:WITH_GTK -out:$@ -target:library \
	$(build_sources) $(DLLS)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

else
all:

endif
