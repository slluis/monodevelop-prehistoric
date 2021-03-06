//
// ItemSetCodon.cs
//
// Author:
//   Lluis Sanchez Gual
//

//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Commands;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Core.AddIns.Codons
{
	[CodonNameAttribute ("ItemSet")]
	public class ItemSetCodon : AbstractCodon
	{
		[XmlMemberAttribute ("_label")]
		string label;
		
		[XmlMemberAttribute("icon")]
		string icon;
		
		public override object BuildItem (object owner, ArrayList subItems, ConditionCollection conditions)
		{
			if (label == null) label = ID;

			label = Runtime.StringParserService.Parse (GettextCatalog.GetString (label));
			if (icon != null) icon = ResourceService.GetStockId (AddIn, icon);
			CommandEntrySet cset = new CommandEntrySet (label, icon);
			foreach (object e in subItems) {
				CommandEntry ce = e as CommandEntry;
				if (ce != null)
					cset.Add (ce);
				else
					throw new InvalidOperationException ("Invalid ItemSet child: " + e);
			}
			return cset;
		}
	}
}
