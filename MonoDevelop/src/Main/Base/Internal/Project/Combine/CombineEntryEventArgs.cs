// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Internal.Project
{
	public delegate void CombineEntryEventHandler(object sender, CombineEntryEventArgs e);
	
	public class CombineEntryEventArgs : EventArgs
	{
		Combine combine;
		CombineEntry entry;
		
		public Combine Combine {
			get {
				return combine;
			}
		}
		
		public CombineEntry CombineEntry {
			get {
				return entry;
			}
		}
		
		public CombineEntryEventArgs (Combine combine, CombineEntry entry)
		{
			this.combine = combine;
			this.entry = entry;
		}
	}
}
