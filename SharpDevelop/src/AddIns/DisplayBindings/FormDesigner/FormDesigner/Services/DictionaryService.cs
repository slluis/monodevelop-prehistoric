// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DictionaryService : IDictionaryService
	{
		Hashtable table = new Hashtable();
		
#region System.ComponentModel.Design.IDictionaryService interface implementation
		public void SetValue(object key, object val)
		{
			if (key != null) {
				table[key] = val;
			}
		}
		
		public object GetValue(object key)
		{
			return key == null ? null : table[key];
		}
		
		public object GetKey(object val)
		{
			foreach (DictionaryEntry entry in table) {
				if (entry.Value == val) {
					return entry.Key;
				}
			}
			return null;
		}
#endregion
	}
}
