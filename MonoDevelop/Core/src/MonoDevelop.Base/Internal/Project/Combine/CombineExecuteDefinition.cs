// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using MonoDevelop.Internal.Serialization;

namespace MonoDevelop.Internal.Project
{
	public enum EntryExecuteType {
		None,
		Execute
	}
	
	public class CombineExecuteDefinition
	{
		CombineEntry combineEntry;
		
		[ItemProperty ("type")]
		EntryExecuteType type = EntryExecuteType.None;

		[ItemProperty]
		string entry;
		
		public CombineExecuteDefinition()
		{
		}
		
		public CombineExecuteDefinition (CombineEntry entry, EntryExecuteType type)
		{
			Entry = entry;
			this.type  = type;
		}
		
		internal void SetCombine (Combine cmb)
		{
			combineEntry = cmb.Entries [entry];
		}
		
		public CombineEntry Entry {
			get { return combineEntry; }
			set {
				combineEntry = value; 
				entry = value != null ? value.Name : null;
			}
		}
		
		public EntryExecuteType Type {
			get { return type; }
			set { type = value; }
		}
	}
}
