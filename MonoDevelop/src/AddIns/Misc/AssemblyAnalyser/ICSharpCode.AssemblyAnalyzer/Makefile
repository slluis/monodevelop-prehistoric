
ASSEMBLY = ICSharpCode.AssemblyAnalyzer.dll
CSC = mcs /debug

FILES = \
./Engine/AssemblyAnalyser.cs \
./Engine/Resolution.cs \
./Rules/AbstractRule.cs \
./Rules/IParameterRule.cs \
./Rules/IMemberRule.cs \
./Rules/IConstructorRule.cs \
./Rules/IPropertyRule.cs \
./Rules/NamingUtilities.cs \
./Rules/IAssemblyRule.cs \
./Rules/AbstractReflectionRule.cs \
./Rules/DesignRules/AssemblyVersionNumberRule.cs \
./Rules/DesignRules/TypesShouldBeInNamespacesRule.cs \
./Rules/DesignRules/EventsHaveTwoParametersRule.cs \
./Rules/DesignRules/AssemblyClsCompliantRule.cs \
./Rules/DesignRules/PropertiesShouldNotHaveSetOnly.cs \
./Rules/DesignRules/InterfaceNotEmptyRule.cs \
./Rules/DesignRules/EventSecondParameterIsEventArgsRule.cs \
./Rules/DesignRules/EventHandlersReturnVoidRule.cs \
./Rules/DesignRules/TypesHaveNoPublicInstanceFields.cs \
./Rules/DesignRules/AbstractTypesPublicConstructorsRule.cs \
./Rules/DesignRules/AvoidNamespacesWithFewMembers.cs \
./Rules/DesignRules/EventFirstParameterIsObjectRule.cs \
./Rules/DesignRules/AssemblyStrongNameRule.cs \
./Rules/ProtectionLevels.cs \
./Rules/IModuleRule.cs \
./Rules/ITypeRule.cs \
./Rules/IMethodRule.cs \
./Rules/IRule.cs \
./Rules/IReflectionRule.cs \
./Rules/IFieldRule.cs \
./Rules/IMethodBaseRule.cs \
./Rules/PriorityLevel.cs \
./Rules/NamingRules/CheckCollectionSuffix.cs \
./Rules/NamingRules/PermissionSuffixIsPermissionRule.cs \
./Rules/NamingRules/NamespacesArePascalCased.cs \
./Rules/NamingRules/ParametersAreCamelCased.cs \
./Rules/NamingRules/EventFirstParameterNameIsSender.cs \
./Rules/NamingRules/FlagEnumerationsArePlural.cs \
./Rules/NamingRules/MembershipConditionNamesSuffixIsMembershipCondition.cs \
./Rules/NamingRules/InterfacesPrefixIsI.cs \
./Rules/NamingRules/MembersDoNotContainUnderscores.cs \
./Rules/NamingRules/ExceptionSuffixIsExceptionRule.cs \
./Rules/NamingRules/ParameterNamesDoNotHaveUnderscores.cs \
./Rules/NamingRules/MembersArePascalCased.cs \
./Rules/NamingRules/OnlyEventHandlerSuffixIsEventHandlerRule.cs \
./Rules/NamingRules/OnlyPermissionsSuffixPermissionRule.cs \
./Rules/NamingRules/OnlyExceptionsSuffixExceptionRule.cs \
./Rules/NamingRules/TypeNamesDoNotContainUnderscores.cs \
./Rules/NamingRules/OnlyStreamsSuffixStreamRule.cs \
./Rules/NamingRules/EnumsHaveNoEnumSuffix.cs \
./Rules/NamingRules/NamespacesDoNotContainUnderscores.cs \
./Rules/NamingRules/TypesArePascalCased.cs \
./Rules/NamingRules/OnlyAttributesSuffixAttributeRule.cs \
./Rules/NamingRules/OnlyCollectionsSuffixCollectionRule.cs \
./Rules/NamingRules/DelegatesHaveNoDelegateSuffix.cs \
./Rules/NamingRules/EventsDoNotHaveBeforeOrAfterPrefix.cs \
./Rules/NamingRules/TypesImplementingInterfacesHaveNoSuffixImplRule.cs \
./Rules/NamingRules/EventHandlerSuffixIsEventHandler.cs \
./Rules/NamingRules/EventSecondParameterNameIsE.cs \
./Rules/NamingRules/EventArgsSuffixIsEventArgsRule.cs \
./Rules/NamingRules/StreamSuffixIsStreamRule.cs \
./Rules/NamingRules/DictionaryTypeSuffixIsDictionary.cs \
./Rules/NamingRules/OnlyEventArgsSuffixEventArgsRule.cs \
./Rules/NamingRules/AttributeSuffixIsAttributeRule.cs \
./Rules/IEventRule.cs \
./Rules/INamespaceRule.cs \
./AssemblyInfo.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) /out:$(ASSEMBLY) /target:library $(FILES) 
