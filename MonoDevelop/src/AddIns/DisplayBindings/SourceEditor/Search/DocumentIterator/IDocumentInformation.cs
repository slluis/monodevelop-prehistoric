// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Gui;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	public interface IDocumentInformation
	{
		string FileName {
			get;
		}
		
/*		int CurrentOffset {
			get;
			set;
		}
		
		int EndOffset {
			get;
		}
*/		
		ITextIterator GetTextIterator ();
	}
}
