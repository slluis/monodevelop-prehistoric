// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃƒÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;

using MonoDevelop.SourceEditor.Gui;
using MonoDevelop.SourceEditor.CodeCompletion;

namespace MonoDevelop.SourceEditor.InsightWindow
{
	public class IndexerInsightDataProvider : IInsightDataProvider
	{
		ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		
		string              fileName = null;
		SourceEditorView    textArea;
		IndexerCollection   methods  = new IndexerCollection();
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		public string GetInsightData(int number)
		{
			IIndexer method = methods[number];
			IAmbience conv = ambienceService.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			return conv.Convert(method) + 
			       "\n" + 
			       CodeCompletionData.GetDocumentation(method.Documentation); // new (by G.B.)
		}
		
		int initialOffset;
		public void SetupDataProvider(string fileName, SourceEditorView textArea)
		{
			this.fileName = fileName;
			this.textArea = textArea;
			Gtk.TextIter initialIter = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark);
			initialOffset = initialIter.Offset;
			
			string word         = TextUtilities.GetExpressionBeforeOffset(textArea, initialOffset);
			string methodObject = word;
			
			// the parser works with 1 based coordinates
			int caretLineNumber      = initialIter.Line + 1;
			int caretColumn          = initialIter.LineOffset + 1;
			IParserService parserService = (IParserService)ServiceManager.Services.GetService(typeof(IParserService));
			ResolveResult results = parserService.Resolve(methodObject,
			                                              caretLineNumber,
			                                              caretColumn,
			                                              fileName,
			                                              textArea.Buffer.Text);
			if (results != null && results.Type != null) {
				foreach (IClass c in results.Type.ClassInheritanceTree) {
					foreach (IIndexer indexer in c.Indexer) {
						methods.Add(indexer);
					}
				}
				foreach (object o in results.ResolveContents) {
					if (o is IClass) {
						foreach (IClass c in ((IClass)o).ClassInheritanceTree) {
							foreach (IIndexer indexer in c.Indexer) {
								methods.Add(indexer);
							}
						}
					}
				}
			}
		}
		
		public bool CaretOffsetChanged()
		{
			Gtk.TextIter caret = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark);
			bool closeDataProvider = caret.Offset <= initialOffset;
			string text = textArea.Buffer.Text;
			
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(caret.Offset, text.Length); ++offset) {
					char ch = text [offset];
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '"':
							insideString = !insideString;
							break;
						case ']':
						case '}':
						case '{':
						case ';':
							if (!(insideChar || insideString)) {
								return true;
							}
							break;
					}
				}
			}
			
			return closeDataProvider;
		}
		
		public bool CharTyped()
		{
			int offset = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark).Offset - 1;
			if (offset >= 0) {
				return textArea.Buffer.Text [offset] == ']';
			}
			return false;
		}
	}
}