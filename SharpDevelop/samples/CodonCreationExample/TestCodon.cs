using System;
using System.Collections;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns.Conditions;

namespace MyPlugin {
	
	[CodonNameAttribute("TestCodon")]
	public class TestCodon : AbstractCodon
	{
        [XmlMemberAttributeAttribute("text")]
        string text  = null;
		
		// BuildItem:
		// This method actually builds an object which is used by the add-in
		// (codon class could not be used, when you want a menuitem or display binding object)
		//
		// The BuildItem gets following arguments :
		// an owner object               : this is the object which creates the item
		// A arraylist with the subitems : if this codon has subitems in it's path here are the
		//                                 build items stored for these the codon may use these
		//                                 or not, if not they get lost, if any where there.
		//                                 'Normal' codons don't need them (then the tree-path is a list)
		//                                 But for example menuitems use them.
		// The action                    : May be ConditionFailedAction.Disable, in this case the codon
		//                                 should be disabled, or if this can't be made it should not
		//                                 take any action
		public override object BuildItem(object owner, ArrayList subItems, ConditionFailedAction action)
		{
			return text;
		}
	}
}
