SUBDIRS = AddIns

monodevelopdir = $(prefix)/lib/monodevelop
addindir = $(monodevelopdir)/AddIns

addin_DATA = MonoDevelopNewEditor.addin.xml SharpDevelopCore.addin.xml

EXTRA_DIST = $(addin_DATA)

# for conditionally built addins
CLEANFILES = MonoDevelopDebugger.addin.xml MonoDevelop.Debugger.dll \
MonoDevelopNunit.addin.xml MonoDevelop.Nunit.dll

