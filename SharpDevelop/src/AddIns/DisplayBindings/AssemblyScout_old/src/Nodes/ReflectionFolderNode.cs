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
	public class ReflectionFolderNode : ReflectionNode
	{
		int openindex;
		int closeindex;
		
		public ReflectionFolderNode(string name, object attribute, ReflectionNodeType type,int openindex, int closeindex) : 
			base(name, attribute, type)
		{
			this.openindex = openindex;
			this.closeindex = closeindex;
			OnCollapse();
		}
		
		protected override void SetIcon()
		{
			OnCollapse();
		}
		
		public override void OnExpand()
		{
			ImageIndex  = SelectedImageIndex = closeindex;
		}
		
		public override void OnCollapse()
		{
			ImageIndex  = SelectedImageIndex = openindex;
		}
	}
}
