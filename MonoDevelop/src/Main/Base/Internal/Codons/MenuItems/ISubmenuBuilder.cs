// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;

using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.Properties;


using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Core.AddIns.Codons
{
	public interface ISubmenuBuilder
	{
		Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner);
	}
}
