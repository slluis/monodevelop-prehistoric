// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionMethodNode : ReflectionNode
	{
		public ReflectionMethodNode(MethodInfo methodinfo2) : base ("", methodinfo2, ReflectionNodeType.Method)
		{
			SetNodeName();
		}
		
		void SetNodeName()
		{
			if (attribute == null) {
				Text = "no name";
				return;
			}
			Text = ReflectionTree.languageConversion.Convert((MethodInfo)attribute);
		}
		
		protected override void SetIcon()
		{
			if (attribute == null)
				return;
			MethodInfo methodinfo = (MethodInfo)attribute;
			
			if (methodinfo.IsPrivate) { // private
				ImageIndex  = SelectedImageIndex = METHODINDEX + 3;
			} else
			if (methodinfo.IsFamily ) { // protected
				ImageIndex  = SelectedImageIndex = METHODINDEX + 2;
			} 
			ImageIndex  = SelectedImageIndex = METHODINDEX;
		}
	}
}
