// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using MonoDevelop.Core.AddIns;

using SharpDevelop.Internal.Parser;
using MonoDevelop.Internal.Project;

using MonoDevelop.Gui;

namespace MonoDevelop.Services {
	public class ClassProxy : IComparable {
		public uint Offset = 0;
		public readonly ClassType ClassType;
		public readonly string FullyQualifiedName;
		public readonly ModifierEnum Modifiers;
		public readonly string Documentation;
		private string className;
		private string namespaceName;
		
		public int CompareTo(object obj) 
		{
			return FullyQualifiedName.CompareTo (((ClassProxy) obj).FullyQualifiedName);
		}
		
		public ClassProxy(BinaryReader reader)
		{
			FullyQualifiedName = reader.ReadString ();
			Documentation      = reader.ReadString ();
			Offset             = reader.ReadUInt32 ();
			Modifiers          = (ModifierEnum) reader.ReadUInt32 ();
			ClassType          = (ClassType) reader.ReadInt16 ();
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write (FullyQualifiedName);
			writer.Write (Documentation);
			writer.Write (Offset);
			writer.Write ((uint) Modifiers);
			writer.Write ((short) ClassType);
		}
		
		public ClassProxy(IClass c)
		{
			this.FullyQualifiedName  = c.FullyQualifiedName;
			this.Documentation       = c.Documentation;
			this.Modifiers           = c.Modifiers;
			this.ClassType           = c.ClassType;
		}
		
		public string Name {
			get {
				if (className == null && FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOfAny (new char[] { '.', '+' });
					
					if (lastIndex < 0)
						className = FullyQualifiedName;
					else
						className = FullyQualifiedName.Substring(lastIndex + 1);
				}
				
				return className;
			}
		}

		public string Namespace {
			get {
				if (namespaceName == null && FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOf ('.');
					
					if (lastIndex < 0)
						namespaceName = string.Empty;
					else
						namespaceName = FullyQualifiedName.Substring (0, lastIndex);
				}
				
				return namespaceName;
			}
		}
	}
}
