// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Conditions;


using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Core.AddIns.Codons
{
	public interface IMenuCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
	}
}
