// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Denis ERCHOFF" email="d_erchoff@hotmail.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Xml;

namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class ToolComponent
	{
		string fullName;
		string assemblyName;
		bool   isEnabled = true;
		
		public string FullName {
			get {
				return fullName;
			}
			set {
				fullName = value;
			}
		}
		
		public string Name {
			get {
				int idx = fullName.LastIndexOf('.');
				if (idx > 0) {
					return fullName.Substring(idx + 1);
				}
				return fullName;
			}
		}
		
		public string Namespace {
			get {
				int idx = fullName.LastIndexOf('.');
				if (idx > 0) {
					return fullName.Substring(0, idx);
				}
				return String.Empty;
			}
		}
		
		public bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public string AssemblyName {
			get {
				return assemblyName;
			}
			set {
				assemblyName = value;
			}
		}
		protected ToolComponent()
		{
		}
		public ToolComponent(string fullName, string assemblyName)
		{
			this.fullName = fullName;
			this.assemblyName = assemblyName;
		}
		
		public object Clone()
		{
			ToolComponent toolComponent = new ToolComponent();
			toolComponent.FullName     = fullName;
			toolComponent.AssemblyName = assemblyName;
			toolComponent.IsEnabled    = isEnabled;
			return toolComponent;
		}
	}
	
	public class Category 
	{
		string    name;
		bool      isEnabled  = true;
		ArrayList components = new ArrayList();
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public ArrayList ToolComponents {
			get {
				return components;
			}
		}
		
		protected Category()
		{
		}
		
		public Category(string name)
		{
			this.name = name;
		}
		
		public object Clone()
		{
			Category newCategory = new Category();
			newCategory.Name      = name;
			newCategory.IsEnabled = isEnabled;
			foreach (ToolComponent toolComponent in components) {
				newCategory.ToolComponents.Add(toolComponent.Clone());
			}
			return newCategory;
		}
	}
	
	public class ComponentLibraryLoader
	{
		static readonly string VERSION = "1.1.0";
		
		ArrayList assemblies = new ArrayList();
		ArrayList categories = new ArrayList();
		
		public ArrayList Categories {
			get {
				return categories;
			}
			set {
				categories = value;
			}
		}
		
		public ArrayList CopyCategories()
		{
			ArrayList newCategories = new ArrayList();
			foreach (Category category in categories) {
				newCategories.Add(category.Clone());
			}
			return newCategories;
		}
		
		
		public bool LoadToolComponentLibrary(string fileName)
		{
			if (!File.Exists(fileName)) {
				return false;
			}
			
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(fileName);
				
				if (doc.DocumentElement.Name != "SharpDevelopControlLibrary" ||
				    doc.DocumentElement.Attributes["version"] == null ||
				    doc.DocumentElement.Attributes["version"].InnerText != VERSION) {
					return false;
				}
				
				foreach (XmlNode node in doc.DocumentElement["Assemblies"].ChildNodes) {
					if (node.Name == "Assembly") {
						assemblies.Add(node.Attributes["assembly"].InnerText);
					}
				}
				
				foreach (XmlNode node in doc.DocumentElement["Categories"].ChildNodes) {
					if (node.Name == "Category") {
						Category newCategory = new Category(node.Attributes["name"].InnerText);
						
						foreach (XmlNode componentNode in node.ChildNodes) {
							ToolComponent newToolComponent = new ToolComponent(componentNode.Attributes["class"].InnerText,
							                                                   assemblies[Int32.Parse(componentNode.Attributes["assembly"].InnerText)].ToString());
							newCategory.ToolComponents.Add(newToolComponent);
						}
						categories.Add(newCategory);
					}
				}
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		public Assembly GetAssembly(string name)
		{
			return Assembly.Load(name);
		}
		
		public Bitmap GetIcon(ToolComponent component)
		{
			Assembly asm = GetAssembly(component.AssemblyName);
			Type type = asm.GetType(component.FullName);
			Bitmap b = null;
			if (type != null) {
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object attr in attributes) {
					if (attr is ToolboxBitmapAttribute) {
						ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
						b = new Bitmap(toolboxBitmapAttribute.GetImage(type));
						b.MakeTransparent();
						break;
					}
				}
			}
			if (b == null) {
				try {
				 	b = new Bitmap(Image.FromStream(asm.GetManifestResourceStream(component.FullName + ".bmp")));
					b.MakeTransparent();
				} catch (Exception) {}
			
			}
			
			// TODO: Maybe default icon needed ??!?!
			return b;
		}
		
		public void SaveToolComponentLibrary(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<SharpDevelopControlLibrary version=\"" + VERSION + "\"/>");
			Hashtable assemblyHashTable = new Hashtable();
			
			XmlElement assemblyNode = doc.CreateElement("Assemblies");
			doc.DocumentElement.AppendChild(assemblyNode);
			for (int i = 0; i < assemblies.Count; ++i) {
				string assembly = assemblies[i].ToString();
				assemblyHashTable[assembly] = i;
				
				
				XmlElement newAssembly = doc.CreateElement("Assembly");
				newAssembly.SetAttribute("assembly", assembly);
				assemblyNode.AppendChild(newAssembly);
			}
			
			XmlElement categoryNode = doc.CreateElement("Categories");
			doc.DocumentElement.AppendChild(categoryNode);
			foreach (Category category in categories) {
				XmlElement newCategory = doc.CreateElement("Category");
				newCategory.SetAttribute("name", category.Name);
				newCategory.SetAttribute("enabled", category.IsEnabled.ToString());
				categoryNode.AppendChild(newCategory);
				
				foreach (ToolComponent component in category.ToolComponents) {
					XmlElement newToolComponent = doc.CreateElement("ToolComponent");
					newToolComponent.SetAttribute("class", component.FullName);
					
					if (assemblyHashTable[component.AssemblyName] == null) {
						XmlElement newAssembly = doc.CreateElement("Assembly");
						newAssembly.SetAttribute("assembly", component.AssemblyName);
						assemblyNode.AppendChild(newAssembly);
						assemblyHashTable[component.AssemblyName] = assemblyHashTable.Values.Count;
					}
					
					newToolComponent.SetAttribute("assembly", assemblyHashTable[component.AssemblyName].ToString());
					newToolComponent.SetAttribute("enabled", component.IsEnabled.ToString());
					newCategory.AppendChild(newToolComponent);
				}
			}
			doc.Save(fileName);
		}
	}
}
