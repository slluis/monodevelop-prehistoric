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

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public enum ReflectionNodeType {
		Folder,
		Resource,
		Assembly,
		Library,
		Namespace,
		Type,
		Constructor,
		Method,
		Field,
		Property,
		SubTypes,
		SuperTypes,
		Reference,
		Event,
		Link,
		Module,
	}
	
	public class ReflectionNode : TreeNode
	{
		protected const int CLASSINDEX     = 14;
		protected const int STRUCTINDEX    = CLASSINDEX + 1 * 4;
		protected const int INTERFACEINDEX = CLASSINDEX + 2 * 4;
		protected const int ENUMINDEX      = CLASSINDEX + 3 * 4;
		protected const int METHODINDEX    = CLASSINDEX + 4 * 4;
		protected const int PROPERTYINDEX  = CLASSINDEX + 5 * 4;
		protected const int FIELDINDEX     = CLASSINDEX + 6 * 4;
		protected const int DELEGATEINDEX  = CLASSINDEX + 7 * 4;
		
		protected ReflectionNodeType type;
		protected string             name;
		protected object             attribute;
		
		protected bool  populated = false;
		
		public static ResourceService ress = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));

		public ReflectionNodeType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public object Attribute {
			get {
				return attribute;
			}
		}
		
		public bool Populated {
			get {
				return populated;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public ReflectionNode(string name, object attribute, ReflectionNodeType type) : base(name)
		{
			this.attribute = attribute;
			this.type = type;
			this.name = name;
			
//			if (type == ReflectionNodeType.Namespace ||
//				type == ReflectionNodeType.Assembly  ||
//				type == ReflectionNodeType.Library   ||
//				type == ReflectionNodeType.Type) {
//					Nodes.Add(new ReflectionNode("", null, ReflectionNodeType.Link));
//			}
			
			SetIcon();
		}
		
	
		protected virtual void SetIcon()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			switch (type) {
				case ReflectionNodeType.Link:
					break;
				
				case ReflectionNodeType.Resource:
					ImageIndex  = SelectedImageIndex = 11;
					break;
				
				case ReflectionNodeType.Reference:
					ImageIndex  = SelectedImageIndex = 8;
					break;
				
				// TODO: MODULE ICON
				case ReflectionNodeType.Module:
					ImageIndex  = SelectedImageIndex = 46;
					break;
				
				case ReflectionNodeType.SubTypes:
					ImageIndex  = SelectedImageIndex = 4;
					break;
					
				case ReflectionNodeType.SuperTypes:
					ImageIndex  = SelectedImageIndex = 5;
					break;
				
				default:
					throw new Exception("ReflectionFolderNode.SetIcon : unknown ReflectionNodeType " + type.ToString());
			}
		}
		
		public virtual void Populate(bool Private, bool Internal)
		{
			switch (type) {
				case ReflectionNodeType.Assembly:
					PopulateAssembly((Assembly)attribute, this);
					break;
				
				case ReflectionNodeType.Library:
					PopulateLibrary((Assembly)attribute, this, Private, Internal);
					break;
			}
			populated = true;
		}
		
		public TreeNode GetNodeFromChildren(string title)
		{
			foreach (TreeNode node in this.Nodes) {
				if (node.Text == title) {
					return node;
				}
			}
			return null;
		}
		
		public TreeNode GetNodeFromCollection(TreeNodeCollection collection, string title)
		{
			foreach (TreeNode node in collection)
				if (node.Text == title) {
					return node;
				}
			return null;
		}
		
		void PopulateLibrary(Assembly assembly, TreeNode parentnode, bool Private, bool Internal)
		{
			parentnode.Nodes.Clear();
			Type[] types = new Type[0];
			
			try {
				types = assembly.GetTypes();
			} catch {
				try {
					types = assembly.GetExportedTypes();
					MessageBox.Show(ress.GetString("ObjectBrowser.OnlyExportedShown"), ress.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				} catch {
					MessageBox.Show(ress.GetString("ObjectBrowser.ErrorLoadingTypes"), ress.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);					
				}
			}
			
			ArrayList nodes = new ArrayList();
			ArrayList namespaces = new ArrayList();
			ArrayList namespacenames = new ArrayList();
			TreeNodeComparer comp = new TreeNodeComparer();
			
			foreach (Type type in types) {
				if(type.ToString().IndexOf("PrivateImplementationDetails") != -1) continue;
				if(type.IsNotPublic && !Internal) continue;
				if(type.IsNestedAssembly && !Internal) continue;
				if(type.IsNestedFamANDAssem && !Internal) continue;
				if(type.IsNestedPrivate && !Private) continue;
				
				string   typename  = type.FullName;
				int      lastindex = typename.LastIndexOf('.');

				nodes.Add(new ReflectionTypeNode(typename.Substring(lastindex + 1), type));
				if (type.Namespace != null && type.Namespace != "") {
					if (!namespacenames.Contains(type.Namespace)) {
						namespaces.Add(new ReflectionFolderNode(type.Namespace, assembly, ReflectionNodeType.Namespace, 3, 3));
						namespacenames.Add(type.Namespace);
					}
				}
			}
			nodes.Sort(comp);
			namespaces.Sort(comp);
			foreach (TreeNode tn in namespaces) {
				parentnode.Nodes.Add(tn);
			}
			foreach (TreeNode tn in nodes) {
				Type type = (Type)((ReflectionNode)tn).Attribute;
				if (type.Namespace != null && type.Namespace != "") {
					GetNodeFromCollection(parentnode.Nodes, type.Namespace).Nodes.Add(tn);
				} else {
					parentnode.Nodes.Add(tn);
				}
			}
				
			
//				TreeNode path      = parentnode;
//				string   nodename  = typename;
//				
//				if (lastindex != -1) {
//					string pathname = typename.Substring(0, lastindex);
//					TreeNode node = GetNodeFromCollection(parentnode.Nodes, pathname);
//					if (node == null) {
//						TreeNode newnode = new ReflectionFolderNode(pathname, assembly, ReflectionNodeType.Namespace, 3, 3);
//						parentnode.Nodes.Add(newnode);
//						path = newnode;
//					} else
//						path = node;
//					nodename = typename.Substring(lastindex + 1);
//				}
//				
//				path.Nodes.Add(new ReflectionTypeNode(nodename, type));

		}
		
		void PopulateAssembly(Assembly assembly, TreeNode parentnode)
		{
			parentnode.Nodes.Clear();
			
			TreeNode node = new ReflectionFolderNode(Path.GetFileName(assembly.CodeBase), assembly, ReflectionNodeType.Library, 2, 2);
			parentnode.Nodes.Add(node);
			
			ReflectionFolderNode resourcefolder = new ReflectionFolderNode(ress.GetString("ObjectBrowser.Nodes.Resources"), assembly, ReflectionNodeType.Folder, 6, 7);
			string[] resources = assembly.GetManifestResourceNames();
			foreach (string resource in resources) {
				resourcefolder.Nodes.Add(new ReflectionNode(resource, assembly, ReflectionNodeType.Resource));
			}
			parentnode.Nodes.Add(resourcefolder);
			
			ReflectionFolderNode referencefolder = new ReflectionFolderNode(ress.GetString("ObjectBrowser.Nodes.References"), assembly, ReflectionNodeType.Folder, 9, 10);
			AssemblyName[] references = assembly.GetReferencedAssemblies();
			foreach (AssemblyName name in references) {
				referencefolder.Nodes.Add(new ReflectionNode(name.Name, name, ReflectionNodeType.Reference));
			}
			parentnode.Nodes.Add(referencefolder);
			
			ReflectionFolderNode modulefolder = new ReflectionFolderNode(ress.GetString("ObjectBrowser.Nodes.Modules"), assembly, ReflectionNodeType.Folder, 9, 10);
			parentnode.Nodes.Add(modulefolder);
			Module[] modules = assembly.GetModules(true);
			foreach(Module module in modules) {
				modulefolder.Nodes.Add(new ReflectionNode(module.Name, module, ReflectionNodeType.Module));
			}
		}
		
		public virtual void OnExpand()
		{
		}
		
		public virtual void OnCollapse()
		{
		}
	}
}
