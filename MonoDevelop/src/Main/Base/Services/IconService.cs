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

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.Core.Services
{
	public class IconService : AbstractService
	{
		//ImageList imagelist;
		ArrayList imagelist = new ArrayList();
		Hashtable extensionHashtable   = new Hashtable();
		Hashtable projectFileHashtable = new Hashtable();
		Hashtable customIcons          = new Hashtable();
		
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		
		public ArrayList ImageList {
			get {
				lock (imagelist) {
					return imagelist;
				}
			}
		}

		
		int initalsize = 0;
		
		public IconService()
		{
			//imagelist            = new ImageList();
			//imagelist.ColorDepth = ColorDepth.Depth32Bit;
		}
		
		void LoadThread()
		{
			InitializeIcons(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Icons"));
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			Thread myThread = new Thread(new ThreadStart(LoadThread));
			myThread.IsBackground = true;
			myThread.Priority = ThreadPriority.Normal;
			myThread.Start();
		}
		
		public override void UnloadService()
		{
			base.UnloadService();
			//imagelist.Dispose();
		}
		
		public Gdk.Pixbuf GetBitmap(string name)
		{
			if (customIcons[name] != null) {
				return (Gdk.Pixbuf)customIcons[name];
			}
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (resourceService.GetBitmap(name) != null) {
				return resourceService.GetBitmap(name);
			}
			
			return resourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public Gdk.Pixbuf GetIcon(string name)
		{
			Gdk.Pixbuf bitmap = GetBitmap(name);
			return bitmap;
			//return Icon.FromHandle(bitmap.GetHicon());
		}
		
		
		public Gdk.Pixbuf GetImageForProjectType(string projectType)
		{
			lock (imagelist) {
				int index = GetImageIndexForProjectType(projectType);
				if (index >= 0) {
					//return imagelist.Images[index];
					return (Gdk.Pixbuf)imagelist[index];
				}
			}
			return null;
		}
		
		public int GetImageIndexForProjectType(string projectType)
		{
			lock (imagelist) {
				if (projectFileHashtable[projectType] != null) {
					return (int)projectFileHashtable[projectType];
				}
				return (int)extensionHashtable[".PRJX"];
			}
		}

		
		public Gdk.Pixbuf GetImageForFile(string fileName)
		{
			lock (imagelist) {
				int index = GetImageIndexForFile(fileName);
				if (index >= 0) {
					//return imagelist.Images[index];
					return (Gdk.Pixbuf)imagelist[index];
				}
				return null;
			}
		}
	
		public int GetImageIndexForFile(string fileName)
		{
		
			lock (imagelist) {
				string extension = Path.GetExtension(fileName).ToUpper();
				if (extensionHashtable[extension] != null) {
					return (int)extensionHashtable[extension];
				}
				return initalsize;
			}
		}
		

		void InitializeIcons(IAddInTreeNode treeNode)
		{
			//imagelist.ColorDepth = ColorDepth.Depth32Bit;
			initalsize  = imagelist.Count;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			lock (imagelist) {
				imagelist.Add(resourceService.GetBitmap("Icons.16x16.MiscFiles"));
				
				extensionHashtable[".PRJX"] = imagelist.Count;
				imagelist.Add(resourceService.GetBitmap("Icons.16x16.SolutionIcon"));
				
				extensionHashtable[".CMBX"] = imagelist.Count;
				imagelist.Add(resourceService.GetBitmap("Icons.16x16.CombineIcon"));
			
				IconCodon[] icons = (IconCodon[])treeNode.BuildChildItems(null).ToArray(typeof(IconCodon));
				for (int i = 0; i < icons.Length; ++i) {
					IconCodon iconCodon = icons[i];
					Gdk.Pixbuf     image;
					if (iconCodon.Location != null) {
						image = new Gdk.Pixbuf(iconCodon.Location);
						customIcons[iconCodon.ID] = image;
					} else if (iconCodon.Resource != null) {
						image = GetBitmap(iconCodon.Resource);
					} else {
						image = GetBitmap(iconCodon.ID);
					}
					imagelist.Add(image);
					
					if (iconCodon.Extensions != null) {
						foreach (string ext in iconCodon.Extensions) {
							extensionHashtable[ext.ToUpper()] = imagelist.Count - 1;
						}
					}
					if (iconCodon.Language != null) {
						projectFileHashtable[iconCodon.Language] = imagelist.Count - 1;
					}
				}
			}
		}
	}
}
