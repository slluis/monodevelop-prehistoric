
CSC = mcs /debug
ASSEMBLY = CsVbRefactory.dll

FILES = ./CsVbRefactory/AssemblyInfo.cs \
./CsVbRefactory/Parser/AST/Expressions/ArrayInitializerExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/TypeReferenceExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/BinaryOperatorExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/FieldReferenceExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/ArrayCreateExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/ParenthesizedExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/TypeOfExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/AssignmentExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/PrimitiveExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/InvocationExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/UnaryOperatorExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/Expression.cs \
./CsVbRefactory/Parser/AST/Expressions/ThisReferenceExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/IdentifierExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/BaseReferenceExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/CastExpression.cs \
./CsVbRefactory/Parser/AST/Expressions/ObjectCreateExpression.cs \
./CsVbRefactory/Parser/AST/INode.cs \
./CsVbRefactory/Parser/AST/AbstractASTVisitor.cs \
./CsVbRefactory/Parser/AST/Statements/BlockStatement.cs \
./CsVbRefactory/Parser/AST/Statements/ForStatement.cs \
./CsVbRefactory/Parser/AST/Statements/LockStatement.cs \
./CsVbRefactory/Parser/AST/Statements/StatementExpression.cs \
./CsVbRefactory/Parser/AST/Statements/DoWhileStatement.cs \
./CsVbRefactory/Parser/AST/Statements/SwitchStatement.cs \
./CsVbRefactory/Parser/AST/Statements/ThrowStatement.cs \
./CsVbRefactory/Parser/AST/Statements/ForeachStatement.cs \
./CsVbRefactory/Parser/AST/Statements/IfElseStatement.cs \
./CsVbRefactory/Parser/AST/Statements/WhileStatement.cs \
./CsVbRefactory/Parser/AST/Statements/GotoStatement.cs \
./CsVbRefactory/Parser/AST/Statements/Statement.cs \
./CsVbRefactory/Parser/AST/Statements/LabelStatement.cs \
./CsVbRefactory/Parser/AST/Statements/ReturnStatement.cs \
./CsVbRefactory/Parser/AST/Statements/TryCatchStatement.cs \
./CsVbRefactory/Parser/AST/Statements/BreakStatement.cs \
./CsVbRefactory/Parser/AST/Statements/LocalVariableDeclaration.cs \
./CsVbRefactory/Parser/AST/IASTVisitor.cs \
./CsVbRefactory/Parser/AST/GlobalScope/TypeReference.cs \
./CsVbRefactory/Parser/AST/Enums.cs \
./CsVbRefactory/Parser/AST/AbstractNode.cs

DLLS = /r:System.Drawing

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(FILES)

assemblydir = $(libdir)/monodevelop/bin
assembly_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	$(CSC) $(DLLS) $(FILES) /out:$(ASSEMBLY) /target:library \
	&& cp $(ASSEMBLY) ../../../build/bin/.

