// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace VBBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class VBFormattingStrategy : DefaultFormattingStrategy
	{
		ArrayList statements;
		StringCollection keywords;
		
		bool doCasing;
		bool doInsertion;
		
		public VBFormattingStrategy()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));		
			
			doCasing = propertyService.GetProperty("VBBinding.TextEditor.EnableCasing", true);
			doInsertion = propertyService.GetProperty("VBBinding.TextEditor.EnableEndConstructs", true);
			
			statements = new ArrayList();
			statements.Add(new VBStatement("^if.*?then$", "^end if$", "End If", true));
			statements.Add(new VBStatement("class \\w+$", "^end class$", "End Class", true));
			statements.Add(new VBStatement("module \\w+$", "^end module$", "End Module", true));
			statements.Add(new VBStatement("structure \\w+$", "^end structure$", "End Structure", true));
			statements.Add(new VBStatement("^while ", "^end while$", "End While", true));
			statements.Add(new VBStatement("^select case", "^end select$", "End Select", true));
			statements.Add(new VBStatement("sub \\w+", "^end sub$", "End Sub", true));
			statements.Add(new VBStatement("property \\w+", "^end property$", "End Property", true));
			statements.Add(new VBStatement("function \\w+", "^end function$", "End Function", true));
			statements.Add(new VBStatement("for (.*?)=(.*?) to .*?$", "^next$", "Next", true));
			statements.Add(new VBStatement("^synclock .*?$", "^end synclock$", "End SyncLock", true));
			statements.Add(new VBStatement("^get($|\\(.*?\\)$)", "^end get$", "End Get", true));
			statements.Add(new VBStatement("^set\\(.*?\\)$", "^end set$", "End Set", true));
			statements.Add(new VBStatement("^try$", "^end try$", "End Try", true));
			statements.Add(new VBStatement("^for each .*? in .+?$", "^next$", "Next", true));
			statements.Add(new VBStatement("Enum .*?$", "^end enum$", "End Enum", true));
			
			keywords = new StringCollection();
			keywords.AddRange(new string[] {
				"AddHandler", "AddressOf", "Alias", "And", "AndAlso", "Ansi", "As", "Assembly",
				"Auto", "Boolean", "ByRef", "Byte", "ByVal", "Call", "Case", "Catch",
				"CBool", "CByte", "CChar", "CDate", "CDec", "CDbl", "Char", "CInt", "Class",
				"CLng", "CObj", "Const", "CShort", "CSng", "CStr", "CType",
				"Date", "Decimal", "Declare", "Default", "Delegate", "Dim", "DirectCast", "Do",
				"Double", "Each", "Else", "ElseIf", "End", "Enum", "Erase", "Error",
				"Event", "Exit", "False", "Finally", "For", "Friend", "Function", "Get",
				"GetType", "GoSub", "GoTo", "Handles", "If", "Implements", "Imports", "In",
				"Inherits", "Integer", "Interface", "Is", "Let", "Lib", "Like", "Long",
				"Loop", "Me", "Mod", "Module", "MustInherit", "MustOverride", "MyBase", "MyClass",
				"Namespace", "New", "Next", "Not", "Nothing", "NotInheritable", "NotOverridable", "Object",
				"On", "Option", "Optional", "Or", "OrElse", "Overloads", "Overridable", "Overrides",
				"ParamArray", "Preserve", "Private", "Property", "Protected", "Public", "RaiseEvent", "ReadOnly",
				"ReDim", "REM", "RemoveHandler", "Resume", "Return", "Select", "Set", "Shadows",
				"Shared", "Short", "Single", "Static", "Step", "Stop", "String", "Structure",
				"Sub", "SyncLock", "Then", "Throw", "To", "True", "Try", "TypeOf", 
				"Unicode", "Until", "Variant", "When", "While", "With", "WithEvents", "WriteOnly", "Xor"
				});
		}

		/// <summary>
		/// Define VB.net specific smart indenting for a line :)
		/// </summary>
//		protected override int SmartIndentLine(int lineNr)
//		{
//			if (lineNr > 0) {
//				LineSegment lineAbove = document.GetLineSegment(lineNr - 1);
//				string lineAboveText = document.GetText(lineAbove.Offset, lineAbove.Length).Trim();
//				
//				LineSegment curLine = document.GetLineSegment(lineNr);
//				string  curLineText = document.GetText(curLine.Offset, curLine.Length).Trim();
//			}
//			return AutoIndentLine(lineNr);
//		}
		
		public override int FormatLine(TextArea textArea, int lineNr, int cursorOffset, char ch)
		{
			if (lineNr > 0) {
				LineSegment curLine = textArea.Document.GetLineSegment(lineNr);
				LineSegment lineAbove = lineNr > 0 ? textArea.Document.GetLineSegment(lineNr - 1) : null;
				
				string curLineText = textArea.Document.GetText(curLine.Offset, curLine.Length);
				string lineAboveText = textArea.Document.GetText(lineAbove.Offset, lineAbove.Length);
				
				if (ch == '\n') {
					
					// remove comments
					string texttoreplace = Regex.Replace(lineAboveText, "'.*$", "", RegexOptions.Singleline);
					// remove string content
					MatchCollection strmatches = Regex.Matches(texttoreplace, "\"[^\"]*?\"", RegexOptions.Singleline);
					foreach (Match match in strmatches) {
						texttoreplace = texttoreplace.Remove(match.Index, match.Length).Insert(match.Index, new String('-', match.Length));
					}
				
					if (doCasing) {
						foreach (string keyword in keywords) {
							string regex = "(?:\\W|^)(" + keyword + ")(?:\\W|$)";
							MatchCollection matches = Regex.Matches(texttoreplace, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
							foreach (Match match in matches) {
								textArea.Document.Replace(lineAbove.Offset + match.Groups[1].Index, match.Groups[1].Length, keyword);
							}
						}
					}
					
					if (doInsertion) {
						foreach (VBStatement statement in statements) {
							if (Regex.IsMatch(texttoreplace.Trim(), statement.StartRegex, RegexOptions.IgnoreCase)) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								if (isEndStatementNeeded(textArea, statement, lineNr)) {
									textArea.Document.Insert(textArea.Caret.Offset, "\n" + indentation + statement.EndStatement);
								}
								if (statement.IndentPlus) {
									indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(textArea.Document);
								}
								
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText.Trim());
								
								return indentation.Length;
							}
						}
					}
					
					string indent = GetIndentation(textArea, lineNr - 1);
					if (indent.Length > 0) {
						string newLineText = indent + TextUtilities.GetLineAsString(textArea.Document, lineNr).Trim();
						LineSegment oldLine = textArea.Document.GetLineSegment(lineNr);
						textArea.Document.Replace(oldLine.Offset, oldLine.Length, newLineText);
					}
					return indent.Length;
				}
			}
			return 0;
		}
		
		bool isEndStatementNeeded(TextArea textArea, VBStatement statement, int lineNr)
		{
			int count = 0;
			
			for (int i = 0; i < textArea.Document.TotalNumberOfLines; i++) {
				LineSegment line = textArea.Document.GetLineSegment(i);
				string lineText = textArea.Document.GetText(line.Offset, line.Length).Trim();
				
				if (lineText.StartsWith("'")) {
					continue;
				}
				
				if (Regex.IsMatch(lineText, statement.StartRegex, RegexOptions.IgnoreCase)) {
					count++;
				} else if (Regex.IsMatch(lineText, statement.EndRegex, RegexOptions.IgnoreCase)) {
					count--;
				}
			}
			return count > 0;
		}
		
		class VBStatement
		{
			public string StartRegex   = "";
			public string EndRegex     = "";
			public string EndStatement = "";
			
			public bool IndentPlus = false;
			
			public VBStatement()
			{
			}
			
			public VBStatement(string startRegex, string endRegex, string endStatement, bool indentPlus)
			{
				StartRegex = startRegex;
				EndRegex   = endRegex;
				EndStatement = endStatement;
				IndentPlus   = indentPlus;
			}
		}
	}
}
