/*
 * Module : FormDesigner
 * 
 * Project : FormDesigner Loading Library Control.
 * 
 * Source code altering : A1 
 * 
 * Description : Creation of the SideTabDesigner which load controls from an assembly
 * 				 Use for FromDesigner.
 * 
 * Denis ERCHOFF						22/01/2003
 */


//		Denis ERCHOFF		22/01/2003		BEGIN		A1

using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class SideTabDesigner : AxSideTab
	{
		protected bool loadImages = true;
		IToolboxService toolboxService;
		
		public void CreatedUserControl()
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
		}
		
		protected SideTabDesigner(AxSideBar sideBar, string name, IToolboxService toolboxService) : base(sideBar, name)
		{
			this.toolboxService = toolboxService;
			this.CanSaved = false;
			
			AddDefaultItem();
		}
		
		protected void AddDefaultItem()
		{
			this.Items.Add(new SideTabItemDesigner());
			//Event the user click on an another "control" itemin the current tab
			this.ChoosedItemChanged += new EventHandler(SelectedTabItemChanged);
		}
		
		///<summary>Load an assembly's controls</summary>
		public SideTabDesigner(AxSideBar sideBar, Category category, IToolboxService toolboxService) : this(sideBar, category.Name, toolboxService)
		{
			foreach (ToolComponent component in category.ToolComponents) {
				if (component.IsEnabled) {
					ToolboxItem toolboxItem = new ToolboxItem();
					toolboxItem.TypeName    = component.FullName;
					toolboxItem.Bitmap      = ToolboxProvider.ComponentLibraryLoader.GetIcon(component);
					toolboxItem.DisplayName = component.Name;
					Assembly asm = Assembly.Load(component.AssemblyName);
					toolboxItem.AssemblyName = asm.GetName();
					
					this.Items.Add(new SideTabItemDesigner(toolboxItem));
				}
			}
		}
		
		protected void LoadAssembly(string assemblyName)
		{
			Assembly assembly = FindAssembly(assemblyName);
			if (assembly == null) {
				assembly = Assembly.LoadWithPartialName(assemblyName);
			}
			
			if (assembly != null) {
				BuildToolboxFromAssembly(assembly);
			}
		}
		
		protected void BuildToolboxFromAssembly(Assembly assembly)
		{
			ArrayList toolboxItems = GetToolboxItemsFromAssembly(assembly);
			foreach (ToolboxItem toolboxItem in toolboxItems) {
				//adding the toolboxitem into the tab with his bitmap and caption
				//loaded from the assembly
				this.Items.Add(new SideTabItemDesigner(toolboxItem));
			}
		}
		
		
		protected Assembly FindAssembly(string assemblyName)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
			{
				if (assembly.GetName().Name == assemblyName) 
				{
					return assembly;
				}
			}
			return null;
		}
		
		protected ArrayList GetToolboxItemsFromAssembly(Assembly assembly)
		{
			ArrayList toolBoxItems = new ArrayList();
			
			Hashtable images = new Hashtable();
			ImageList il = new ImageList();
			// try to load res icon
			string[] imgNames = assembly.GetManifestResourceNames();
			
			foreach (string im in imgNames) {
				try {
					Bitmap b = new Bitmap(Image.FromStream(assembly.GetManifestResourceStream(im)));
					b.MakeTransparent();
					images[im] = il.Images.Count;
					il.Images.Add(b);
				} catch {}
			}
			
			Module[] ms = assembly.GetModules(false);
			
			foreach (Module m in ms) {
				Type[] ts = m.GetTypes();
				foreach (Type t in ts) {
					if (!t.IsSubclassOf(typeof(System.Windows.Forms.Form))) {
						if (t.IsDefined(typeof(ToolboxItemFilterAttribute), true) || t.IsDefined(typeof(ToolboxItemAttribute), true)) {
							if (images[t.FullName + ".bmp"] == null) {
								object[] attributes = t.GetCustomAttributes(false);
								if (t.IsDefined(typeof(ToolboxBitmapAttribute), false)) {
									foreach (object attr in attributes) {
										if (attr is ToolboxBitmapAttribute) {
											ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
											images[t.FullName + ".bmp"] = il.Images.Count;
											Bitmap b = new Bitmap(toolboxBitmapAttribute.GetImage(t));
											b.MakeTransparent();
											il.Images.Add(b);
											break;
										}
									}
								}
							}
							if (images[t.FullName + ".bmp"] != null) {
								ToolboxItem item = new ToolboxItem(t);
								toolBoxItems.Add(item);
							}
						}
					}
				}
			}
			return toolBoxItems;
		}
			
		void SelectedTabItemChanged(object sender, EventArgs e)
		{
			AxSideTabItem item = (sender as AxSideTab).ChoosedItem;
			if (item == null) {
				toolboxService.SetSelectedToolboxItem(null);
			} else {
				toolboxService.SetSelectedToolboxItem(item.Tag as ToolboxItem);
			}
		}
	}
}
