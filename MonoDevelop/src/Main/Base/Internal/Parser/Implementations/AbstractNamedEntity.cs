// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

namespace MonoDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractNamedEntity : AbstractDecoration
	{
		public static Hashtable fullyQualifiedNames = new Hashtable();
		int nameHashCode = -1;
		
		public virtual string FullyQualifiedName {
			get {
				return (string)fullyQualifiedNames[nameHashCode];
			}
			set {
				nameHashCode = value.GetHashCode();
				if (fullyQualifiedNames[nameHashCode] == null) {
					fullyQualifiedNames[nameHashCode] = value;
				}
			}
		}

		public virtual string Name {
			get {
				if (FullyQualifiedName != null) {
					int lastIndex;
					
					if (CanBeSubclass) {
						lastIndex = FullyQualifiedName.LastIndexOfAny
							(new char[] { '.', '+' });
					} else {
						lastIndex = FullyQualifiedName.LastIndexOf('.');
					}
					
					if (lastIndex < 0) {
						return FullyQualifiedName;
					} else {
						return FullyQualifiedName.Substring(lastIndex + 1);
					}
				}
				return null;
			}
		}

		public virtual string Namespace {
			get {
				if (FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOf('.');
					
					if (lastIndex < 0) {
						return String.Empty;
					} else {
						return FullyQualifiedName.Substring(0, lastIndex);
					}
				}
				return null;
			}
		}
		
		protected virtual bool CanBeSubclass {
			get {
				return false;
			}
		}
	}
}
