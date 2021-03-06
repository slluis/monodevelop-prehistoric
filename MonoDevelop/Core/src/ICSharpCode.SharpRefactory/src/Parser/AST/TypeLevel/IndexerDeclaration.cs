// IndexerDeclaration.cs
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
using System.Drawing;
using System.Collections;

namespace ICSharpCode.SharpRefactory.Parser.AST
{
	public class IndexerDeclaration : AbstractNode
	{
		TypeReference type = null;
		Modifier      modifier;
		ArrayList     parameters = new ArrayList();
		ArrayList     attributes = new ArrayList();
		string        namespaceName;
		Point           bodyStart;
		Point           bodyEnd;
				
		PropertyGetRegion  propertyGetRegion = null;
		PropertySetRegion  propertySetRegion = null;
		
		public TypeReference TypeReference {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public Modifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value;
			}
		}
		
		public bool HasGetRegion {
			get {
				return propertyGetRegion != null;
			}
		}
		
		public bool HasSetRegion {
			get {
				return propertySetRegion != null;
			}
		}
		
		public bool IsReadOnly {
			get {
				return HasGetRegion && !HasSetRegion;
			}
		}
		
		public bool IsWriteOnly {
			get {
				return !HasGetRegion && HasSetRegion;
			}
		}
		
		public PropertyGetRegion GetRegion {
			get {
				return propertyGetRegion;
			}
			set {
				propertyGetRegion = value;
			}
		}
		public PropertySetRegion SetRegion {
			get {
				return propertySetRegion;
			}
			set {
				propertySetRegion = value;
			}
		}
		public ArrayList Attributes {
			get {
				return attributes;
			}
			set {
				attributes = value;
			}
		}
		public string NamespaceName {
			get {
				return namespaceName;
			}
			set {
				namespaceName = value;
			}
		}
		
		public Point BodyStart {
			get {
				return bodyStart;
			}
			set {
				bodyStart = value;
			}
		}
		
		public Point BodyEnd {
			get {
				return bodyEnd;
			}
			set {
				bodyEnd = value;
			}
		}
		
		public IndexerDeclaration(Modifier modifier, ArrayList parameters, ArrayList attributes)
		{
			this.modifier = modifier;
			this.parameters = parameters;
			this.attributes = attributes;
		}
		
		public IndexerDeclaration(TypeReference typeReference, ArrayList parameters, Modifier modifier, ArrayList attributes)
		{
			this.type = typeReference;
			this.modifier = modifier;
			this.parameters = parameters;
			this.attributes = attributes;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
