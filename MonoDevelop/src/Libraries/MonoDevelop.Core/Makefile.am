FILES= \
./Properties/CharacterEncodings.cs \
./Properties/DefaultProperties.cs \
./Properties/UnknownPropertyNodeException.cs \
./Properties/PropertyEventArgs.cs \
./Properties/IProperties.cs \
./Properties/PropertyFileLoadException.cs \
./Properties/IXmlConvertable.cs \
./Services/FileUtilityService.cs \
./Services/SaveErrorInformDialog.cs \
./Services/AbstractService.cs \
./Services/IMessageService.cs \
./Services/StringParserService.cs \
./Services/PropertyService.cs \
./Services/ServiceManager.cs \
./Services/IService.cs \
./Services/IResourceService.cs \
./Services/IStringTagProvider.cs \
./Services/SaveErrorChooseDialog.cs \
./AssemblyInfo.cs \
./AddIns/Conditions/DuplicateConditionException.cs \
./AddIns/Conditions/ConditionCollection.cs \
./AddIns/Conditions/ICondition.cs \
./AddIns/Conditions/CompareCondition.cs \
./AddIns/Conditions/ConditionWithoutRequiredAttributesException.cs \
./AddIns/Conditions/ConditionBuilder.cs \
./AddIns/Conditions/ConditionNotFoundException.cs \
./AddIns/Conditions/ConditionAttribute.cs \
./AddIns/Conditions/ConditionBuilderCollection.cs \
./AddIns/Conditions/ConditionFactory.cs \
./AddIns/Conditions/AbstractCondition.cs \
./AddIns/Conditions/ComplexConditions.cs \
./AddIns/PathAttribute.cs \
./AddIns/Codons/CodonBuilder.cs \
./AddIns/Codons/CodonNameAttribute.cs \
./AddIns/Codons/ClassCodon.cs \
./AddIns/Codons/CodonNotFoundException.cs \
./AddIns/Codons/AbstractCodon.cs \
./AddIns/Codons/CodonFactory.cs \
./AddIns/Codons/IconCodon.cs \
./AddIns/Codons/AbstractCommand.cs \
./AddIns/Codons/DuplicateCodonException.cs \
./AddIns/Codons/ICommand.cs \
./AddIns/Codons/ICodon.cs \
./AddIns/AddInLoadException.cs \
./AddIns/TreePathNotFoundException.cs \
./AddIns/DefaultAddInTreeNode.cs \
./AddIns/AddIn.cs \
./AddIns/IAddInTree.cs \
./AddIns/DefaultAddInTree.cs \
./AddIns/IAddInTreeNode.cs \
./AddIns/AddInTreeSingleton.cs \
./AddIns/XmlMemberAttributeAttribute.cs \
./AddIns/XmlMemberArraryAttribute.cs \
./AddIns/AddInTreeFormatException.cs \
./AddIns/TypeNotFoundException.cs \
./AddIns/AddInInitializeException.cs \
./AddIns/AddInCollection.cs \
./AddIns/AddInSignatureException.cs \
./Services/GettextCatalog.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = MonoDevelop.Core.dll

all: $(DLL)

$(DLL): $(build_sources)
	mcs /debug /out:$(DLL) /target:library /r:System.Drawing.dll /define:LINUX /resource:$(srcdir)/../../../data/resources/AddIn.xsd,AddIn.xsd \
	$(build_sources) && cp $(DLL) ../../../build/bin/.

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)

CLEANFILES = $(DLL)

EXTRA_DIST = $(FILES)

