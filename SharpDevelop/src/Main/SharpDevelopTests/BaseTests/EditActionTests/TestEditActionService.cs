// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;

using ICSharpCode.SharpUnit;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.TextEditor.Actions;
/*
public class TestEditactionService : ICSharpCode.TextEditor.Actions.IEditActionHandler, 
                                     IClipboardHandler
{
	IDocumentAggregator document = new DocumentAggregatorFactory().CreateDocument();
	bool caretchanged     = false;
	
	
	public TestEditactionService()
	{
		document.Caret.OffsetChanged += new CaretEventHandler(NewCaretPos);	
	}
	void NewCaretPos(object sender, CaretEventArgs e)
	{
		caretchanged = true;
	}
	
	
	public void ExecuteEditAction(IEditAction action)
	{
		selectionChanged = false;
		caretchanged     = false;
		if (action != null) {
			lock (Document) {
				action.Execute(this);
				if (HasSomethingSelected && !selectionChanged && caretchanged) {
					ClearSelection();
				}
			}
		} 
	}
	
	public IClipboardHandler ClipboardHandler {
		get {
			return this;
		}
	}
	public string FileName {
		get {
			return String.Empty;
			
		}
	}
	public IDocumentAggregator Document {
		get {
			return document;
		}
	}
	
	public int MaxVisibleLine {
		get {
			return 24;
		}
	}
	int firstVisibleLine = 1;
	public int FirstVisibleColumn {
		get {
			return firstVisibleLine;
		}
		set {
			firstVisibleLine = value;
		}
	}
	
	int firstVisibleRow = 1;
	public int FirstVisibleRow {
		get {
			return firstVisibleRow;
		}
		set {
			firstVisibleRow = value;
		}
	}
	
	public bool CutCalled = false;
	public void Cut(object sender, EventArgs e)
	{
		CutCalled = true;
	}
	
	public bool CopyCalled = false;
	public void Copy(object sender, EventArgs e)
	{
		CopyCalled = true;
	}
	
	public bool PasteCalled = false;
	public void Paste(object sender, EventArgs e)
	{
		PasteCalled = true;
	}
	
	public bool DeleteCalled = false;
	public void Delete(object sender, EventArgs e)
	{
		DeleteCalled = true;
	}

	public bool SelectAllCalled = false;
	public void SelectAll(object sender, EventArgs e)
	{
		SelectAllCalled = true;
	}

	public void InsertChar(char ch)
	{
		BeginUpdate();
		if (DocumentSelectionMode.Normal == DocumentSelectionMode.Normal &&
		    Document.SelectionCollection.Count > 0) {
			Document.Caret.Offset = Document.SelectionCollection[0].Offset;
			RemoveSelectedText();
		}
		
		Document.Insert(Document.Caret.Offset, ch.ToString());
		EndUpdate();
		
		int lineNr = Document.GetLineNumberForOffset(Document.Caret.Offset);
		UpdateLineToEnd(lineNr, Document.Caret.Offset - Document.GetLineSegment(lineNr).Offset);
		++Document.Caret.Offset;
	}
	
	public void ReplaceChar(char ch)
	{
		BeginUpdate();
		if (DocumentSelectionMode.Normal == DocumentSelectionMode.Normal &&
		    Document.SelectionCollection.Count > 0) {
			Document.Caret.Offset = Document.SelectionCollection[0].Offset;
			RemoveSelectedText();
		}
		
		int lineNr   = Document.GetLineNumberForOffset(Document.Caret.Offset);
		LineSegment  line = Document.GetLineSegment(lineNr);
		if (Document.Caret.Offset < line.Offset + line.Length) {
			Document.Replace(Document.Caret.Offset, 1, ch.ToString());
		} else {
			Document.Insert(Document.Caret.Offset, ch.ToString());
		}
		EndUpdate();
		
		UpdateLineToEnd(lineNr, Document.Caret.Offset - Document.GetLineSegment(lineNr).Offset);
		++Document.Caret.Offset;
	}
	
	// update stuff
	public bool DoUpdate = false;
	
	public void BeginUpdate()
	{
		DoUpdate = true;
	}
	
	public void EndUpdate()
	{
		DoUpdate = false;
	}
	
	public void Refresh()
	{
		// do nothing
	}
	
	public void UpdateToEnd(int lineBegin)
	{
		// do nothing		
	}
	
	public void UpdateLines(int lineBegin, int lineEnd)
	{
		// do nothing		
	}
	
	public void UpdateLineToEnd(int lineNr, int xStart)
	{
		// do nothing		
	}
	
	// caret stuff
	public void ScrollToCaret()
	{
		// do nothing		
	}
	
	// scroll stuff
	public void ScrollTo(int line)
	{
		// do nothing		
	}
	
	
	// selection stuff
	public bool HasSomethingSelected {
		get {
				return Document.SelectionCollection.Count > 0;
			
		}
	}
	
	bool selectionChanged = false;
	public bool AutoClearSelection {
		get {
			return !selectionChanged;
		}
		set {
			selectionChanged = !value;
		}
	}
	
	public void SetSelection(ISelection selection)
	{
		selectionChanged = true;
		ClearSelection();

		if (selection != null) {
			Document.SelectionCollection.Add(selection);
			UpdateLines(selection.StartLine, selection.EndLine);
		}
	}
	
	public void ClearSelection()
	{
		while (Document.SelectionCollection.Count > 0) {
			ISelection s = Document.SelectionCollection[Document.SelectionCollection.Count - 1];
			Document.SelectionCollection.RemoveAt(Document.SelectionCollection.Count - 1);
			UpdateLines(s.StartLine, s.EndLine);
		}
	}
	
	ISelection GetSelectionBetween(int offset, int offset2)
	{
		int min = Math.Min(offset, offset2);
		int max = Math.Max(offset, offset2);
		ISelection ti = new DefaultSelection(Document, min, max - min);
	
		foreach (ISelection s in Document.SelectionCollection) {
			if (SelectionsOverlap(ti, s)) {
				return s;
			}
		}
		return null;
	}
	
	
	public void ExtendSelection(int oldOffset, int newOffset)
	{
		selectionChanged = true;
		ISelection s = GetSelectionBetween(oldOffset, newOffset);
		int min = Math.Min(oldOffset, newOffset);
		int max = Math.Max(oldOffset, newOffset);
		if (s != null) {
			int oldEndOffset = s.Offset + s.Length;
			if (oldOffset <= s.Offset) {
				if (newOffset < oldEndOffset) {
					s.Length = oldEndOffset - newOffset;
					s.Offset = newOffset;
				} else {
					s.Offset = oldEndOffset;
					s.Length = newOffset - oldEndOffset;
				}
			} else  {
				if (newOffset > s.Offset) {
					s.Length = newOffset - s.Offset;
				} else {
					s.Length = s.Offset - newOffset;
					s.Offset = newOffset;
				}
			}
			if (s.Length == 0) {
				Document.SelectionCollection.Remove(s);
			}
			
			UpdateLines(Document.GetLineNumberForOffset(min), Document.GetLineNumberForOffset(max));
		} else {
			if (DocumentSelectionMode.Normal == DocumentSelectionMode.Normal) {
				ClearSelection();
			}
			
			AddToSelection(new DefaultSelection(Document, min, max - min));
		}
	} 
	
	public void AddToSelection(ISelection selection)
	{
		selectionChanged = true;
		if (selection != null) {
			InternalAddToSelection(selection);
		}
	}
	public void RemoveSelectedText()
	{
		ArrayList lines = new ArrayList();
		BeginUpdate();
		int offset = -1;
		bool oneLine = true;
		PriorityQueue queue = new PriorityQueue();
		foreach (ISelection s in Document.SelectionCollection) {
			queue.Insert(-s.Offset, s);
		}
		while (queue.Count > 0) {
			ISelection s = ((ISelection)queue.Remove());
			if (oneLine) {
				int lineBegin = s.StartLine;
				if (lineBegin != s.EndLine) {
					oneLine = false;
				} else {
					lines.Add(lineBegin);
				}
			}
			offset = Document.Caret.Offset = s.Offset;
			Document.Remove(s.Offset, s.Length);
		}
		ClearSelection();
		EndUpdate();
		if (offset != -1) {
			
			if (oneLine) {
//				foreach (int i in lines) {
//					UpdateLine(i);
//				}
			} else {
				Refresh();
			}
		}
	}
	
	void InternalAddToSelection(ISelection selection)
	{
			if (selection.Length == 0) {
				return;
			}
			
			foreach (ISelection s in Document.SelectionCollection) {
			// try and merge existing selections one by
			// one with the new selection
			if (SelectionsOverlap(s, selection)) {
				int newOffset = Math.Min(selection.Offset, s.Offset);
				int newLength = Math.Max(selection.Offset + selection.Length, s.Offset + s.Length) - newOffset;
				
				selection.Offset = newOffset;
				selection.Length = newLength;
				Document.SelectionCollection.Remove(s);
				break;
			}
		}
		Document.SelectionCollection.Add(selection);
		
		UpdateLines(selection.StartLine, selection.EndLine);
	}
	
	bool SelectionsOverlap(ISelection s1, ISelection s2)
	{
		return (s1.Offset <= s2.Offset && s2.Offset <= s1.Offset + s1.Length)                         ||
		       (s1.Offset <= s2.Offset + s2.Length && s2.Offset + s2.Length <= s1.Offset + s1.Length) ||
		       (s1.Offset >= s2.Offset && s1.Offset + s1.Length <= s2.Offset + s2.Length);
	}
}
*/
