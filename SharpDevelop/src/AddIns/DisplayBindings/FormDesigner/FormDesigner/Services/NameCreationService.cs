// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class NameCreationService : INameCreationService
	{
		IDesignerHost host;
		
		public NameCreationService(IDesignerHost host)
		{
			this.host = host;
		}
		
		public string CreateName(IContainer container, Type dataType)
		{
			string name = Char.ToLower(dataType.Name[0]) + dataType.Name.Substring(1);
			
			if (container.Components[name] == null) {
				return name;
			}
			
			int number = 2;
			while (container.Components[name + number.ToString()] != null) {
				++number;
			}
			return name + number.ToString();
		}
		
		public bool IsValidName(string name)
		{
			if (name == null || name.Length == 0 || !(Char.IsLetter(name[0]) || name[0] == '_')) {
				return false;
			}
			
			foreach (char ch in name) {
				if (!Char.IsLetterOrDigit(ch) && ch != '_') {
					return false;
				}
			}
			
			return !((DesignComponentContainer)host.Container).ContainsName(name);
		}
		
		public void ValidateName(string name)
		{
			if (!IsValidName(name)) {
				throw new System.Exception("Invalid name " + name);
			}
		}
	}
}
