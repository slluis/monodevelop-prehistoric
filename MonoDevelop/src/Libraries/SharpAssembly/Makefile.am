SDDIR=.
SOURCES= \
$(SDDIR)/src/SharpAssembly/PE/SectionTable.cs \
$(SDDIR)/src/SharpAssembly/PE/IAT.cs \
$(SDDIR)/src/SharpAssembly/PE/NameTable.cs \
$(SDDIR)/src/SharpAssembly/PE/CLIHeader.cs \
$(SDDIR)/src/SharpAssembly/PE/StandardFields.cs \
$(SDDIR)/src/SharpAssembly/PE/DataDirectories.cs \
$(SDDIR)/src/SharpAssembly/PE/ImportTable.cs \
$(SDDIR)/src/SharpAssembly/PE/StreamHeader.cs \
$(SDDIR)/src/SharpAssembly/PE/PEFileHeader.cs \
$(SDDIR)/src/SharpAssembly/PE/NTSpecificFields.cs \
$(SDDIR)/src/SharpAssembly/AssemblyReader.cs \
$(SDDIR)/src/SharpAssembly/SharpAssemblyName.cs \
$(SDDIR)/src/SharpAssembly/AssemblyMetadataTables.cs \
$(SDDIR)/src/SharpAssembly/FusionNative.cs \
./src/SharpAssembly/SharpAssembly.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Param.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/TypeDef.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ModuleRef.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ClassLayout.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/DeclSecurity.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/TypeRef.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AssemblyProcessor.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/FieldLayout.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Module.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Event.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AssemblyRefProcessor.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/MethodSemantics.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AbstractRow.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AssemblyOS.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/FieldMarshal.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ImplMap.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/StandAloneSig.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Method.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ENCMap.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ENCLog.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/FieldRVA.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AssemblyRefOS.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/FieldPtr.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/EventMap.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ManifestResource.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/EventPtr.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/PropertyMap.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Constant.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/PropertyPtr.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/File.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/MemberRef.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Assembly.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Property.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ExportedType.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/TypeSpec.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/InterfaceImpl.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/NestedClass.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/AssemblyRef.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/MethodPtr.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/CodedIndex.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/CustomAttribute.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/ParamPtr.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/Field.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Rows/MethodImpl.cs \
$(SDDIR)/src/SharpAssembly/Metadata/AssemblyMetaData.cs \
$(SDDIR)/src/SharpAssembly/Metadata/MetadataTable.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Signatures/DataTypes.cs \
$(SDDIR)/src/SharpAssembly/Metadata/Signatures/CallingConventions.cs \
$(SDDIR)/src/SharpAssembly/Metadata/MethodBody.cs \
./src/SharpAssembly/AssemblyInfo.cs \
$(SDDIR)/src/SharpAssembly/AssemblyNameNotFoundException.cs 

DLL=../../../build/bin/ICSharpCode.SharpAssembly.dll

all: $(DLL)

$(DLL): $(SOURCES)
	@ mcs /debug /out:$(DLL) /target:library $(SOURCES)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)
  
CLEANFILES=$(DLL)

EXTRA_DIST = $(SOURCES)

