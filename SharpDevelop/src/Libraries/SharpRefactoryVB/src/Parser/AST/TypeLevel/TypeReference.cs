// TypeReference.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections;
using System.Text;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class TypeReference
	{
		string type;
		string systemType;
		int[]  rankSpecifier;
		
		static Hashtable types = new Hashtable();
		static TypeReference()
		{
			types.Add("Boolean",    "System.Boolean");
			types.Add("Byte",    "System.Byte");
			types.Add("Date",	 "System.DateTime");
			types.Add("Char",    "System.Char");
			types.Add("Decimal", "System.Decimal");
			types.Add("Double",  "System.Double");
			types.Add("Single",   "System.Single");
			types.Add("Integer",     "System.Int32");
			types.Add("Long",    "System.Int64");
			types.Add("Object",  "System.Object");
			types.Add("Short",   "System.Int16");
			types.Add("String",  "System.String");
		}
		
		public string Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public string SystemType {
			get {
				return systemType;
			}
		}
		
		public int[] RankSpecifier {
			get {
				return rankSpecifier;
			}
		}
		
		public bool IsArrayType {
			get {
				return rankSpecifier != null && rankSpecifier.Length > 0;
			}
		}
		
		string GetSystemType(string type)
		{
			return (string)types[type];
		}
		
		public TypeReference(string type)
		{
			this.systemType = GetSystemType(type);
			this.type = type;
		}
		
		public TypeReference(string type, int[] rankSpecifier)
		{
			this.type = type;
			this.systemType = GetSystemType(type);
			this.rankSpecifier = rankSpecifier;
		}
		
		public override string ToString()
		{
			return String.Format("[TypeReference: Type={0}, RankSpeifier={2}]", type, rankSpecifier);
		}
	}
}
