// created on 07.08.2003 at 13:36
using System;
using System.Collections;
using System.Windows.Forms;
using ICSharpCode.Core.Services;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class ConfigureSideBarDialog : BaseSharpDevelopForm
	{
		ArrayList oldComponents;
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public ConfigureSideBarDialog() : base(propertyService.DataDirectory + @"\resources\dialogs\ConfigureSidebarDialog.xfrm")
		{
			oldComponents = ToolboxProvider.ComponentLibraryLoader.CopyCategories();
			
			FillCategories();
			categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
			componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			ControlDictionary["okButton"].Click               += new System.EventHandler(this.okButtonClick);
			ControlDictionary["newCategoryButton"].Click      += new System.EventHandler(this.newCategoryButtonClick);
			ControlDictionary["renameCategoryButton"].Click   += new System.EventHandler(this.renameCategoryButtonClick);
			ControlDictionary["removeCategoryButton"].Click   += new System.EventHandler(this.removeCategoryButtonClick);
			ControlDictionary["addComponentsButton"].Click    += new System.EventHandler(this.button3Click);
			ControlDictionary["removeComponentsButton"].Click += new System.EventHandler(this.removeComponentsButtonClick);
			((ListView)ControlDictionary["categoryListView"]).SelectedIndexChanged += new System.EventHandler(this.categoryListViewSelectedIndexChanged);
		
		}
		
		void FillCategories()
		{
			((ListView)ControlDictionary["categoryListView"]).BeginUpdate();
			((ListView)ControlDictionary["categoryListView"]).Items.Clear();
			foreach (Category category in ToolboxProvider.ComponentLibraryLoader.Categories) {
				ListViewItem newItem = new ListViewItem(category.Name);
				newItem.Checked = category.IsEnabled;
				newItem.Tag     = category;
				((ListView)ControlDictionary["categoryListView"]).Items.Add(newItem);
			}
			((ListView)ControlDictionary["categoryListView"]).EndUpdate();
		}
		
		void FillComponents()
		{
			((ListView)ControlDictionary["componentListView"]).ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
			((ListView)ControlDictionary["componentListView"]).BeginUpdate();
			((ListView)ControlDictionary["componentListView"]).Items.Clear();
			((ListView)ControlDictionary["componentListView"]).SmallImageList = new ImageList();
			
			if (((ListView)ControlDictionary["categoryListView"]).SelectedItems != null && ((ListView)ControlDictionary["categoryListView"]).SelectedItems.Count == 1) {
				Category category = (Category)((ListView)ControlDictionary["categoryListView"]).SelectedItems[0].Tag;
				foreach (ToolComponent component in category.ToolComponents) {
					((ListView)ControlDictionary["componentListView"]).SmallImageList.Images.Add(ToolboxProvider.ComponentLibraryLoader.GetIcon(component));
					
					ListViewItem newItem = new ListViewItem(component.Name);
					newItem.SubItems.Add(component.Namespace);
					newItem.SubItems.Add(component.AssemblyName);
					
					newItem.Checked = component.IsEnabled;
					newItem.Tag     = component;
					newItem.ImageIndex = ((ListView)ControlDictionary["componentListView"]).SmallImageList.Images.Count - 1;
					((ListView)ControlDictionary["componentListView"]).Items.Add(newItem);
				}
			}
			((ListView)ControlDictionary["componentListView"]).EndUpdate();
			((ListView)ControlDictionary["componentListView"]).ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.componentListViewItemCheck);
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void categoryListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			ControlDictionary["renameCategoryButton"].Enabled = ControlDictionary["removeComponentsButton"].Enabled = ControlDictionary["addComponentsButton"].Enabled = CurrentCategory != null;
			FillComponents();
		}
		
		Category CurrentCategory {
			get {
				if (((ListView)ControlDictionary["categoryListView"]).SelectedItems != null && ((ListView)ControlDictionary["categoryListView"]).SelectedItems.Count == 1) {
					return (Category)((ListView)ControlDictionary["categoryListView"]).SelectedItems[0].Tag;
				}
				return null;
			}
		}
		
		void button3Click(object sender, System.EventArgs e)
		{
			AddComponentsDialog addComponentsDialog = new AddComponentsDialog();
			if (addComponentsDialog.ShowDialog() == DialogResult.OK) {
				foreach (ToolComponent component in addComponentsDialog.SelectedComponents) {
					CurrentCategory.ToolComponents.Add(component);
				}
				FillComponents();
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void newCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(null, this);
			if (renameCategoryDialog.ShowDialog() == DialogResult.OK) {
				ToolboxProvider.ComponentLibraryLoader.Categories.Add(new Category(renameCategoryDialog.CategoryName));
				FillCategories();
			}
		}
		
		void removeCategoryButtonClick(object sender, System.EventArgs e)
		{
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			if (messageService.AskQuestion("Are you sure to remove this category ?")) {
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				ToolboxProvider.ComponentLibraryLoader.Categories.Remove(CurrentCategory);
				FillCategories();
				FillComponents();
				categoryListViewSelectedIndexChanged(this, EventArgs.Empty);
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		void componentListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			ControlDictionary["removeComponentsButton"].Enabled = ((ListView)ControlDictionary["componentListView"]).SelectedItems != null && ((ListView)ControlDictionary["componentListView"]).SelectedItems.Count > 0;
		}
		
		void removeComponentsButtonClick(object sender, System.EventArgs e)
		{
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			if (messageService.AskQuestion("Are you sure to remove the selected components from the category ?")) {
				foreach (ListViewItem item in ((ListView)ControlDictionary["componentListView"]).SelectedItems) {
					CurrentCategory.ToolComponents.Remove((ToolComponent)item.Tag);
				}
				FillComponents();
				componentListViewSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		
		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			if (oldComponents != null) {
				ToolboxProvider.ComponentLibraryLoader.Categories = oldComponents;
			}
		}
		
		void renameCategoryButtonClick(object sender, System.EventArgs e)
		{
			RenameCategoryDialog renameCategoryDialog = new RenameCategoryDialog(this.CurrentCategory.Name, this);
			if (renameCategoryDialog.ShowDialog() == DialogResult.OK) {
				this.CurrentCategory.Name = renameCategoryDialog.CategoryName;
				FillCategories();
			}
		}
		
		void okButtonClick(object sender, System.EventArgs e)
		{
			oldComponents = null;
			
			foreach (ListViewItem item in ((ListView)ControlDictionary["categoryListView"]).Items) {
				Category category = (Category)item.Tag;
				category.IsEnabled = item.Checked;
			}
					
			ToolboxProvider.SaveToolbox();
		}
		
		void componentListViewItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			ToolComponent tc = (ToolComponent)((ListView)ControlDictionary["componentListView"]).Items[e.Index].Tag;
			tc.IsEnabled = !tc.IsEnabled;
		}
	}
}
