// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using ICSharpCode.TextEditor.Document;


using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	abstract class AbstractNode : TreeNode
	{
		protected NodeOptionPanel panel;
		IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		protected string ResNodeName(string resName)
		{
			try {
				return resourceService.GetString("Dialog.HighlightingEditor.TreeView." + resName);
			} catch {
				return resName;
			}
		}
		
		public NodeOptionPanel OptionPanel {
			get {
				return panel;
			}
		}
		
		public abstract void UpdateNodeText();
		
		// should be made abstract when implementing ToXml()
		public virtual string ToXml() { return ""; }
		
		public static string ReplaceXmlChars(string str)
		{
			return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}
	}
}
