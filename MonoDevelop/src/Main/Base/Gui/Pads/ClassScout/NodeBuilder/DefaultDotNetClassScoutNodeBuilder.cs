// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;

using MonoDevelop.Core.Properties;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Parser;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;

using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Gui.Pads
{
	public class DefaultDotNetClassScoutNodeBuilder : IClassScoutNodeBuilder
	{
		IAmbience languageConversion;

		public DefaultDotNetClassScoutNodeBuilder()
		{
		}

		public bool CanBuildClassTree(IProject project)
		{
			return true;
		}

		void GetCurrentAmbience()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.GetService(typeof(ClassBrowserIconsService));
			AmbienceService          ambienceService = (AmbienceService)ServiceManager.GetService(typeof(AmbienceService));

			languageConversion = ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = ConversionFlags.None;
		}

		private object locker = new object ();
		
		public void UpdateClassTree (TreeNode parentNode, ClassInformationEventArgs e)
		{
			lock (locker) {
				RemoveFromClassTree (parentNode, e.ClassInformation.Removed);
				AddToClassTree (parentNode, e.FileName, e.ClassInformation.Added);
				AddToClassTree (parentNode, e.FileName, e.ClassInformation.Modified);
			}
		}
		
		public void RemoveFromClassTree (TreeNode parentNode, ClassCollection removed) {
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.GetService(typeof(ClassBrowserIconsService));
			
			TreeNode classNode = new TreeNode();
			
			foreach (IClass c in removed) {
				classNode.Text = c.Name;
				classNode.Image = classBrowserIconService.GetIcon (c);
				// TODO: Perf check
				TreeNode node = GetNodeByPath (c.Namespace, parentNode, false);
				if (node != null && !NeedsExpansion (node)) {
					int oldIndex = FindNodeByName (node.Nodes, c.Name);
					if (oldIndex >= 0) {
						node.Nodes[oldIndex].Remove ();
					}
					DropPhantomNamespaces (c.Namespace, parentNode);
				}
			}
		}
		
		public void UpdateClassTree(TreeNode projectNode)
		{
			TreeNode newNode = BuildClassTreeNode((IProject)projectNode.Tag);
			projectNode.Nodes.Clear();
			foreach (TreeNode node in newNode.Nodes)
				projectNode.Nodes.Add(node);
		}
		
		static bool NeedsExpansion (TreeNode nod)
		{
			return (nod.Nodes.Count == 1 && nod.Nodes[0].Text == "");
		}

		public TreeNode BuildClassTreeNode(IProject p)
		{
			Type fus = typeof (FileUtilityService);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(fus);
			Type iconserv = typeof (IconService);
			
			IconService iconService = (IconService)ServiceManager.GetService(iconserv);
			GetCurrentAmbience();

			TreeNode prjNode = new AbstractClassScoutNode(p.Name);
			prjNode.Image = iconService.GetImageForProjectType(p.ProjectType);
			prjNode.Nodes.Add (new TreeNode (""));
			prjNode.Tag = p;
			return prjNode;
		}
		
		public void ExpandNode (TreeNode node)
		{
			if (node.Tag == null)
			{
				// Build the namespace
				string ns = node.Text;
				TreeNode nod = node.Parent;
				while (nod.Tag == null) {
					ns = nod.Text + "." + ns;
					nod = nod.Parent;
				}

				IProject p = (IProject)nod.Tag;
				ExpandNamespaceTree (p, ns, node);
			}
			else if (node.Tag is IProject)
			{
				IProject p = (IProject)node.Tag;
				ExpandNamespaceTree (p, "", node);
			}
		}
		
		void ExpandNamespaceTree (IProject project, string ns, TreeNode node)
		{
			if (!NeedsExpansion (node)) return;
			node.Nodes.Clear ();
			
			IParserService parserService  = (IParserService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IParserService));
			ArrayList contents = parserService.GetNamespaceContents (project, ns, false);
			foreach (object item in contents)
			{
				if (item is string)
				{
					TreeNode newnode = new AbstractClassScoutNode ((string)item);
					newnode.Image = Stock.NameSpace;
					node.Nodes.Add (newnode);
					newnode.Nodes.Add (new TreeNode (""));
				}
				else if (item is IClass)
				{
					node.Nodes.Add (BuildClassNode ((IClass)item));
				}
			}
		}
		
		public void AddToClassTree(TreeNode parentNode, string filename, ClassCollection classes)
		{
			if (NeedsExpansion (parentNode)) return;
			
			foreach (IClass c in classes) {
			
				TreeNode node = GetNodeByPath (c.Namespace, parentNode, true);
				if (node == null || NeedsExpansion (node))
					continue;
				
				TreeNode oldClassNode = GetNodeByName(node.Nodes, c.Name);
				bool wasExpanded = false;
				if (oldClassNode != null) {
					wasExpanded = oldClassNode.IsExpanded;
					oldClassNode.Remove();
				}
				
				TreeNode classNode = BuildClassNode(c);
				if(classNode != null) {
					node.Nodes.Add (classNode);
					
					if (wasExpanded) {
						classNode.Expand();
					}
				}
			}
		}
		
		TreeNode BuildClassNode (IClass c)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.GetService(typeof(ClassBrowserIconsService));

			AbstractClassScoutNode classNode = new AbstractClassScoutNode(c.Name);
			string file = c.Region.FileName;
			
			classNode.Image = classBrowserIconService.GetIcon (c);
			classNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ClassScout/ContextMenu/ClassNode";
			classNode.Tag = new ClassScoutTag(c.Region.BeginLine, file);

			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return null;
			}

			foreach (IClass innerClass in c.InnerClasses) {
				if (innerClass.ClassType == ClassType.Delegate) {
					TreeNode innerClassNode = new AbstractClassScoutNode(languageConversion.Convert(innerClass));
					innerClassNode.Tag = new ClassScoutTag(innerClass.Region.BeginLine, innerClass.Region.FileName == null ? file : innerClass.Region.FileName);
					//innerClassNode.SelectedImageIndex = innerClassNode.ImageIndex = classBrowserIconService.GetIcon(innerClass);
					innerClassNode.Image = classBrowserIconService.GetIcon(innerClass);
					classNode.Nodes.Add(innerClassNode);
				} else {
					TreeNode n = BuildClassNode(innerClass);
					if(classNode != null) {
						classNode.Nodes.Add(n);
					}
				}
			}

			foreach (IMethod method in c.Methods) {
				TreeNode methodNode = new AbstractClassScoutNode(languageConversion.Convert(method));
				methodNode.Tag = new ClassScoutTag(method.Region.BeginLine, method.Region.FileName == null ? file : method.Region.FileName);
				//methodNode.SelectedImageIndex = methodNode.ImageIndex = classBrowserIconService.GetIcon(method);
				methodNode.Image = classBrowserIconService.GetIcon(method);
				classNode.Nodes.Add(methodNode);
			}
			
			foreach (IProperty property in c.Properties) {
				TreeNode propertyNode = new AbstractClassScoutNode(languageConversion.Convert(property));
				propertyNode.Tag = new ClassScoutTag(property.Region.BeginLine, property.Region.FileName == null ? file : property.Region.FileName);
				//propertyNode.SelectedImageIndex = propertyNode.ImageIndex = classBrowserIconService.GetIcon(property);
				propertyNode.Image = classBrowserIconService.GetIcon(property);
				classNode.Nodes.Add(propertyNode);
			}
			
			foreach (IField field in c.Fields) {
				TreeNode fieldNode = new AbstractClassScoutNode(languageConversion.Convert(field));
				fieldNode.Tag = new ClassScoutTag(field.Region.BeginLine, field.Region.FileName == null ? file : field.Region.FileName);
				//fieldNode.SelectedImageIndex = fieldNode.ImageIndex = classBrowserIconService.GetIcon(field);
				fieldNode.Image = classBrowserIconService.GetIcon(field);
				classNode.Nodes.Add(fieldNode);
			}
			
			foreach (IEvent e in c.Events) {
				TreeNode eventNode = new AbstractClassScoutNode(languageConversion.Convert(e));
				eventNode.Tag = new ClassScoutTag(e.Region.BeginLine, e.Region.FileName == null ? file : e.Region.FileName);
				//eventNode.SelectedImageIndex = eventNode.ImageIndex = classBrowserIconService.GetIcon(e);
				eventNode.Image = classBrowserIconService.GetIcon(e);
				classNode.Nodes.Add(eventNode);
			}
			
			return classNode;
		}

		public void DropPhantomNamespaces (string dir, TreeNode parentNode)
		{
			string[] full_path = dir.Split (new char[] { '.' });
			for (int i = full_path.Length - 1; i != -1; i--)
			{
				TreeNode node = GetNodeByPath (String.Join (".", full_path, 0, i + 1), parentNode, false);
				if (node != null && node.Nodes.Count == 0 && node != parentNode)
					node.Remove ();
			}
		}
		
		static public TreeNode GetNodeByPath(string directory, TreeNode parentNode, bool create)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.GetService(typeof(ClassBrowserIconsService));

			string[] treepath   = directory.Split(new char[] { '.' });
			TreeNode curnode = parentNode;
			
			if (NeedsExpansion (parentNode)) return null;
			
			if (treepath.Length == 1 && treepath[0].Length == 0)
				return parentNode;
			
			foreach (string path in treepath) {
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}

				curnode = GetNodeByName (curnode.Nodes, path);
				
				if (curnode == null) {
					if (create) {
						curnode = new AbstractClassScoutNode(path);
						curnode.Image = Stock.NameSpace;
						curnode.Nodes.Add (new TreeNode (""));
						parentNode.Nodes.Add (curnode);
						return null;	// Delay expansion
					} else {
						return null;
					}
				}
				else if (NeedsExpansion (curnode))
					return null;
				
				parentNode = curnode;
			}
			return curnode;
		}

		static TreeNode GetNodeByName(TreeNodeCollection collection, string name)
		{
			foreach (TreeNode node in collection) {
				if (node.Text == name) {
					return node;
				}
			}
			return null;
		}

		static int FindNodeByName(TreeNodeCollection collection, string name)
		{
			for (int n=0; n<collection.Count; n++)
				if (collection[n].Text == name)
					return n;

			return -1;
		}
	}
}
