FILES = \
src/Lexer/Reader/IReader.cs \
src/Lexer/Reader/FileReader.cs \
src/Lexer/Reader/StringReader.cs \
src/Lexer/Lexer.cs \
src/Lexer/Specials/CommentType.cs \
src/Lexer/Specials/PreProcessingDirective.cs \
src/Lexer/Specials/SpecialType.cs \
src/Lexer/Specials/Comment.cs \
src/Lexer/Specials/SpecialTracker.cs \
src/Lexer/Specials/BlankLine.cs \
src/Parser/AST/DebugASTVisitor.cs \
src/Parser/AST/Expressions/BaseReferenceExpression.cs \
src/Parser/AST/Expressions/ParenthesizedExpression.cs \
src/Parser/AST/Expressions/PointerReferenceExpression.cs \
src/Parser/AST/Expressions/InvocationExpression.cs \
src/Parser/AST/Expressions/SizeOfExpression.cs \
src/Parser/AST/Expressions/ArrayInitializerExpression.cs \
src/Parser/AST/Expressions/AssignmentExpression.cs \
src/Parser/AST/Expressions/ThisReferenceExpression.cs \
src/Parser/AST/Expressions/TypeReferenceExpression.cs \
src/Parser/AST/Expressions/UnaryOperatorExpression.cs \
src/Parser/AST/Expressions/UncheckedExpression.cs \
src/Parser/AST/Expressions/PrimitiveExpression.cs \
src/Parser/AST/Expressions/ObjectCreateExpression.cs \
src/Parser/AST/Expressions/StackAllocExpression.cs \
src/Parser/AST/Expressions/CheckedExpression.cs \
src/Parser/AST/Expressions/TypeOfExpression.cs \
src/Parser/AST/Expressions/IdentifierExpression.cs \
src/Parser/AST/Expressions/DirectionExpression.cs \
src/Parser/AST/Expressions/IndexerExpression.cs \
src/Parser/AST/Expressions/Expression.cs \
src/Parser/AST/Expressions/BinaryOperatorExpression.cs \
src/Parser/AST/Expressions/FieldReferenceExpression.cs \
src/Parser/AST/Expressions/ArrayCreateExpression.cs \
src/Parser/AST/Expressions/CastExpression.cs \
src/Parser/AST/Expressions/ConditionalExpression.cs \
src/Parser/AST/Enums.cs \
src/Parser/AST/IASTVisitor.cs \
src/Parser/AST/Modifier.cs \
src/Parser/AST/LookupTableVisitor.cs \
src/Parser/AST/AbstractNode.cs \
src/Parser/AST/Statements/BreakStatement.cs \
src/Parser/AST/Statements/UnsafeStatement.cs \
src/Parser/AST/Statements/CheckedStatement.cs \
src/Parser/AST/Statements/LockStatement.cs \
src/Parser/AST/Statements/FixedStatement.cs \
src/Parser/AST/Statements/BlockStatement.cs \
src/Parser/AST/Statements/SwitchStatement.cs \
src/Parser/AST/Statements/ContinueStatement.cs \
src/Parser/AST/Statements/GotoCaseStatement.cs \
src/Parser/AST/Statements/IfElseStatement.cs \
src/Parser/AST/Statements/StatementExpression.cs \
src/Parser/AST/Statements/LocalVariableDeclaration.cs \
src/Parser/AST/Statements/EmptyStatement.cs \
src/Parser/AST/Statements/LabelStatement.cs \
src/Parser/AST/Statements/ForStatement.cs \
src/Parser/AST/Statements/ReturnStatement.cs \
src/Parser/AST/Statements/GotoStatement.cs \
src/Parser/AST/Statements/ForeachStatement.cs \
src/Parser/AST/Statements/Statement.cs \
src/Parser/AST/Statements/TryCatchStatement.cs \
src/Parser/AST/Statements/ThrowStatement.cs \
src/Parser/AST/Statements/UncheckedStatement.cs \
src/Parser/AST/Statements/IfStatement.cs \
src/Parser/AST/Statements/WhileStatement.cs \
src/Parser/AST/Statements/UsingStatement.cs \
src/Parser/AST/Statements/DoWhileStatement.cs \
src/Parser/AST/GlobalScope/UsingAliasDeclaration.cs \
src/Parser/AST/GlobalScope/UsingDeclaration.cs \
src/Parser/AST/GlobalScope/NamespaceDeclaration.cs \
src/Parser/AST/GlobalScope/TypeDeclaration.cs \
src/Parser/AST/GlobalScope/AttributeSection.cs \
src/Parser/AST/GlobalScope/DelegateDeclaration.cs \
src/Parser/AST/CompilationUnit.cs \
src/Parser/AST/AttributeTargets.cs \
src/Parser/AST/TypeLevel/EventAddRegion.cs \
src/Parser/AST/TypeLevel/TypeReference.cs \
src/Parser/AST/TypeLevel/ConstructorDeclaration.cs \
src/Parser/AST/TypeLevel/PropertySetRegion.cs \
src/Parser/AST/TypeLevel/ParameterDeclarationExpression.cs \
src/Parser/AST/TypeLevel/IndexerDeclaration.cs \
src/Parser/AST/TypeLevel/PropertyGetRegion.cs \
src/Parser/AST/TypeLevel/VariableDeclaration.cs \
src/Parser/AST/TypeLevel/OperatorDeclaration.cs \
src/Parser/AST/TypeLevel/MethodDeclaration.cs \
src/Parser/AST/TypeLevel/FieldDeclaration.cs \
src/Parser/AST/TypeLevel/PropertyDeclaration.cs \
src/Parser/AST/TypeLevel/EventRemoveRegion.cs \
src/Parser/AST/TypeLevel/EventDeclaration.cs \
src/Parser/AST/TypeLevel/DestructorDeclaration.cs \
src/Parser/AST/AbstractASTVisitor.cs \
src/Parser/AST/INode.cs \
src/Parser/generated/ParserUtil.cs \
src/Parser/generated/Modifiers.cs \
src/Parser/generated/Parser.cs \
src/Parser/generated/Error.cs \
src/Parser/generated/keywordlist/Tokens.cs \
src/Parser/generated/keywordlist/Keywords.cs \
src/PrettyPrinter/OutputFormatter.cs \
src/PrettyPrinter/PrettyPrintVisitor.cs \
src/PrettyPrinter/PrettyPrintUtil.cs \
src/PrettyPrinter/PrettyPrintData.cs \
src/PrettyPrinter/SpecialVisitor.cs \
src/CodeDOM/CodeDOMVisitor.cs \
src/VBConverter/VBNetRefactory.cs \
src/VBConverter/VBNetVisitor.cs \
src/AssemblyInfo.cs

build_sources = $(addprefix $(srcdir)/, $(FILES))

DLL = ICSharpCode.SharpRefactory.dll

REFS = /r:System.Drawing.dll

all: $(DLL)

$(DLL): $(FILES)
	@ $(CSC) /out:$(DLL) /target:library $(REFS) $(build_sources) \
	&& cp $(DLL) $(top_builddir)/build/bin/.

test-parser.exe : $(DLL) src/Main.cs
	@ $(CSC) /out:$@ /r:$(DLL) src/Main.cs

run : test-parser.exe
	@ MONO_PATH=`dirname $(DLL)` $(RUNTIME) test-parser.exe $(TEST_SOURCE)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(DLL)

CLEANFILES = $(DLL) $(DLL).mdb

EXTRA_DIST = $(FILES)
