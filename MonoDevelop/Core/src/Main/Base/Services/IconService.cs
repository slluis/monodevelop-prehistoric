// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Threading;
using System.Xml;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Core.Services {
	public class IconService : AbstractService {
		Hashtable extensionHashtable   = new Hashtable ();
		Hashtable projectFileHashtable = new Hashtable ();
		
		public override void InitializeService()
		{
			base.InitializeService();
			InitializeIcons(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Icons"));
		}
		
		public string GetImageForProjectType (string projectType)
		{
			if (projectFileHashtable [projectType] != null)
				return (string) projectFileHashtable [projectType];
			
			return (string) extensionHashtable [".PRJX"];
		}
		
		public string GetImageForFile (string fileName)
		{
			string extension = Path.GetExtension (fileName).ToUpper ();
			
			if (extensionHashtable.Contains (extension))
				return (string) extensionHashtable [extension];
			
			return Stock.MiscFiles;
		}


		void InitializeIcons (IAddInTreeNode treeNode)
		{			
			extensionHashtable[".PRJX"] = Stock.SolutionIcon;
			extensionHashtable[".CMBX"] = Stock.CombineIcon;
		
			IconCodon[] icons = (IconCodon[])treeNode.BuildChildItems(null).ToArray(typeof(IconCodon));
			for (int i = 0; i < icons.Length; ++i) {
				IconCodon iconCodon = icons[i];
				string image;
				if (iconCodon.Location != null)
					throw new Exception ("This should be using stock icons");
				else if (iconCodon.Resource != null)
					image = iconCodon.Resource;
				else
					image = iconCodon.ID;
				
				image = ResourceService.GetStockId (image);
				
				if (iconCodon.Extensions != null) {
					foreach (string ext in iconCodon.Extensions)
						extensionHashtable [ext.ToUpper()] = image;
				}
				if (iconCodon.Language != null)
					projectFileHashtable [iconCodon.Language] = image;
			}
		}
	}
}
