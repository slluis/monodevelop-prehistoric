SUBDIRS = AddIns

monodevelopdir = $(prefix)/lib/monodevelop
addindir = $(monodevelopdir)/AddIns

addin_DATA = MonoDevelopNewEditor.addin SharpDevelopCore.addin

EXTRA_DIST = $(addin_DATA)

# for conditionally built addins
CLEANFILES = MonoDevelopDebugger.addin MonoDevelop.Debugger.dll \
MonoDevelopNunit.addin MonoDevelop.Nunit.dll

