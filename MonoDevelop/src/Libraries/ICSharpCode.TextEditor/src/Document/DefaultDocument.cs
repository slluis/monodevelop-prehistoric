// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing;

using ICSharpCode.TextEditor.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// Describes the caret marker
	/// </summary>
	public enum LineViewerStyle {
		/// <summary>
		/// No line viewer will be displayed
		/// </summary>
		None,
		
		/// <summary>
		/// The row in which the caret is will be marked
		/// </summary>
		FullRow
	}
	
	/// <summary>
	/// Describes the indent style
	/// </summary>
	public enum IndentStyle {
		/// <summary>
		/// No indentation occurs
		/// </summary>
		None,
		
		/// <summary>
		/// The indentation from the line above will be
		/// taken to indent the curent line
		/// </summary>
		Auto, 
		
		/// <summary>
		/// Inteligent, context sensitive indentation will occur
		/// </summary>
		Smart
	}
	
	/// <summary>
	/// Describes the bracket highlighting style
	/// </summary>
	public enum BracketHighlightingStyle {
		
		/// <summary>
		/// Brackets won't be highlighted
		/// </summary>
		None,
		
		/// <summary>
		/// Brackets will be highlighted if the caret is on the bracket
		/// </summary>
		OnBracket,
		
		/// <summary>
		/// Brackets will be highlighted if the caret is after the bracket
		/// </summary>
		AfterBracket
	}
	
	/// <summary>
	/// Describes the selection mode of the text area
	/// </summary>
	public enum DocumentSelectionMode {
		/// <summary>
		/// The 'normal' selection mode.
		/// </summary>
		Normal,
		
		/// <summary>
		/// Selections will be added to the current selection or new
		/// ones will be created (multi-select mode)
		/// </summary>
		Additive
	}
	
	/// <summary>
	/// The default <see cref="IDocument"/> implementation.
	/// </summary>
	internal class DefaultDocument : IDocument
	{	
		bool readOnly = false;
		
		ILineManager          lineTrackingStrategy = null;
		IBookMarkManager      bookmarkManager      = null;
		ITextBufferStrategy   textBufferStrategy   = null;
		IFormattingStrategy   formattingStrategy   = null;
		FoldingManager        foldingManager       = null;
		UndoStack             undoStack            = new UndoStack();
		ITextEditorProperties  textEditorProperties = new DefaultTextEditorProperties();
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return textEditorProperties;
			}
			set {
				textEditorProperties = value;
			}
		}
		
		
		public UndoStack UndoStack {
			get {
				return undoStack;
			}
		}
		
		public ArrayList LineSegmentCollection {
			get {
				return lineTrackingStrategy.LineSegmentCollection;
			}
		}
		
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				readOnly = value;
			}
		}
		
		public ILineManager LineManager {
			get {
				return lineTrackingStrategy;
			}
			set {
				lineTrackingStrategy = value;
			}
		}
		
		public ITextBufferStrategy TextBufferStrategy {
			get {
				return textBufferStrategy;
			}
			set {
				textBufferStrategy = value;
			}
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				return formattingStrategy;
			}
			set {
				formattingStrategy = value;
			}
		}
		
		public FoldingManager FoldingManager {
			get {
				return foldingManager;
			}
			set {
				foldingManager = value;
			}
		}
		
		public IHighlightingStrategy HighlightingStrategy {
			get {
				return lineTrackingStrategy.HighlightingStrategy;
			}
			set {
				lineTrackingStrategy.HighlightingStrategy = value;
			}
		}
		
		public int TextLength {
			get {
				return textBufferStrategy.Length;
			}
		}
		
		public IBookMarkManager BookmarkManager {
			get {
				return bookmarkManager;
			}
			set {
				bookmarkManager = value;
			}
		}
		
		public string TextContent {
			get {
				return GetText(0, textBufferStrategy.Length);
			}
			set {
				Debug.Assert(textBufferStrategy != null);
				Debug.Assert(lineTrackingStrategy != null);
				OnDocumentAboutToBeChanged(new DocumentEventArgs(this, 0, 0, value));
				textBufferStrategy.SetContent(value);
				lineTrackingStrategy.SetContent(value);
				OnDocumentChanged(new DocumentEventArgs(this, 0, 0, value));				
				OnTextContentChanged(EventArgs.Empty);
			}
		}
		
		public void Insert(int offset, string text)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, -1, text));
			DateTime time = DateTime.Now;
			textBufferStrategy.Insert(offset, text);
			
			time = DateTime.Now;
			lineTrackingStrategy.Insert(offset, text);
			
			time = DateTime.Now;
			
			undoStack.Push(new UndoableInsert(this, offset, text));
			
			time = DateTime.Now;
			OnDocumentChanged(new DocumentEventArgs(this, offset, -1, text));
		}
		
		public void Remove(int offset, int length)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length));
			undoStack.Push(new UndoableDelete(this, offset, GetText(offset, length)));
			
			textBufferStrategy.Remove(offset, length);
			lineTrackingStrategy.Remove(offset, length);
			
			
			OnDocumentChanged(new DocumentEventArgs(this, offset, length));
		}
		
		public void Replace(int offset, int length, string text)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length, text));
			undoStack.Push(new UndoableReplace(this, offset, GetText(offset, length), text));
			
			textBufferStrategy.Replace(offset, length, text);
			lineTrackingStrategy.Replace(offset, length, text);
			
			OnDocumentChanged(new DocumentEventArgs(this, offset, length, text));
		}
		
		public char GetCharAt(int offset)
		{
			return textBufferStrategy.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return textBufferStrategy.GetText(offset, length);
		}
		
		public int TotalNumberOfLines {
			get {
				return lineTrackingStrategy.TotalNumberOfLines;
			}
		}
		
		public int GetLineNumberForOffset(int offset)
		{
			return lineTrackingStrategy.GetLineNumberForOffset(offset);
		}
		
		public LineSegment GetLineSegmentForOffset(int offset)
		{
			return lineTrackingStrategy.GetLineSegmentForOffset(offset);
		}
		
		public LineSegment GetLineSegment(int line)
		{
			return lineTrackingStrategy.GetLineSegment(line);
		}
		
		public int GetLogicalLine(int lineNumber)
		{
			return lineTrackingStrategy.GetLogicalLine(lineNumber);
		}

		public int GetVisibleLine(int lineNumber)
		{
			return lineTrackingStrategy.GetVisibleLine(lineNumber);
		}
		
		public int GetNextVisibleLineAbove(int lineNumber, int lineCount)
		{
			return lineTrackingStrategy.GetNextVisibleLineAbove(lineNumber, lineCount);
		}
		
		public int GetNextVisibleLineBelow(int lineNumber, int lineCount)
		{
			return lineTrackingStrategy.GetNextVisibleLineBelow(lineNumber, lineCount);
		}
		
		public Point OffsetToPosition(int offset)
		{
			int lineNr = GetLineNumberForOffset(offset);
			LineSegment line = GetLineSegment(lineNr);
			return new Point(offset - line.Offset, lineNr);
		}
		
		public int PositionToOffset(Point p)
		{
			if (p.Y >= this.TotalNumberOfLines) {
				return 0;
			}
			LineSegment line = GetLineSegment(p.Y);
			return Math.Min(this.TextLength, line.Offset + Math.Min(line.Length, p.X));
		}
		
		protected void OnDocumentAboutToBeChanged(DocumentEventArgs e)
		{
			if (DocumentAboutToBeChanged != null) {
				DocumentAboutToBeChanged(this, e);
			}
		}
		
		protected void OnDocumentChanged(DocumentEventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
		
		public event DocumentEventHandler DocumentAboutToBeChanged;
		public event DocumentEventHandler DocumentChanged;
		
		// UPDATE STUFF
		ArrayList updateQueue = new ArrayList();
		
		public ArrayList UpdateQueue {
			get { 
				return updateQueue;
			}
		}
		
		public void RequestUpdate(TextAreaUpdate update)
		{
			updateQueue.Add(update);
		}
		
		public void CommitUpdate()
		{
			if (UpdateCommited != null) {
				UpdateCommited(this, EventArgs.Empty);
			}
		}
		
		protected virtual void OnTextContentChanged(EventArgs e)
		{
			if (TextContentChanged != null) {
				TextContentChanged(this, e);
			}
		}
		
		public event EventHandler UpdateCommited;
		public event EventHandler TextContentChanged;
	}
}
