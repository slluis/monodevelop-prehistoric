// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	public class ForwardTextFileIterator : ITextIterator
	{
		string fileName;
		FileStream stream;
		StreamReader streamReader;
		ExtendedStreamReader reader;
		bool started;
		
		public ForwardTextFileIterator (string fileName)
		{
			this.fileName = fileName;
			stream = new FileStream (fileName, FileMode.Open, FileAccess.Read);
			streamReader = new StreamReader (stream);
			reader = new ExtendedStreamReader (streamReader);
			Reset();
		}
		
		public char Current {
			get {
				return (char) reader.Peek ();
			}
		}
		
		public int Position {
			get {
				return reader.Position;
			}
			set {
				reader.Position = value;
			}
		}
		
		
		public char GetCharRelative(int offset)
		{
			int pos = reader.Position;
			
			if (offset < 0) {
				offset = -offset;
				for (int n=0; n<offset; n--)
					if (reader.ReadBack () == -1) {
						reader.Position = pos;
						return char.MinValue;
					}
			}
			else {
				for (int n=0; n<offset; n++) {
					if (reader.Read () == -1) {
						reader.Position = pos;
						return char.MinValue;
					}
				}
			}
			
			char c = (char) reader.Peek ();
			reader.Position = pos;
			return c;
		}
		
		public bool MoveAhead(int numChars)
		{
			Debug.Assert(numChars > 0);
			
			if (!started) {
				started = true;
				return (reader.Peek () != -1);
			}
			
			int nc = 0;
			while (nc != -1 && numChars > 0) {
				numChars--;
				nc = reader.Read ();
			}
			
			if (nc == -1) return false;
			return reader.Peek() != -1;
		}
		
		public string ReadToEnd ()
		{
			return reader.ReadToEnd ();
		}
		
		public void Replace (int length, string pattern)
		{
			reader.Remove (length);
			reader.Insert (pattern);
		}

		public void Reset()
		{
			reader.Position = 0;
		}
		
		public void Close ()
		{
			if (reader.Modified)
			{
				string fileBackup = Path.GetTempFileName ();
				File.Move (fileName, fileBackup);
				
				try {
					reader.SaveToFile (fileName);
					reader.Close ();
				}
				catch
				{
					reader.Close ();
					if (File.Exists (fileName)) File.Delete (fileName);
					File.Move (fileBackup, fileName);
					throw;
				}
				
				File.Delete (fileBackup);
			}
			else
				reader.Close ();
		}
	}
}
