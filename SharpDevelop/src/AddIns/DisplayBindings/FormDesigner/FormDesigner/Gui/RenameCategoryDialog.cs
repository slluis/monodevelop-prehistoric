// created on 08.08.2003 at 13:02
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;


namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class RenameCategoryDialog : BaseSharpDevelopForm
	{
		string categoryName = String.Empty;
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public string CategoryName {
			get {
				return categoryName;
			}
		}
		
		public RenameCategoryDialog(string categoryName, Form owner) : base(propertyService.DataDirectory + @"\resources\dialogs\RenameSidebarCategoryDialog.xfrm")
		{
			this.Owner = owner;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				
			if (categoryName == null) {
				ControlDictionary["categoryNameTextBox"].Text = "New Category";
				Text = stringParserService.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.NewCategoryDialogName}");
			} else {
				this.categoryName = categoryName;
				ControlDictionary["categoryNameTextBox"].Text = categoryName;
				Text = stringParserService.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.RenameCategoryDialog.RenameCategoryDialogName}");
			}
			ControlDictionary["okButton"].Click += new EventHandler(okButtonClick);
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void ShowDuplicateErrorMessage()
		{
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			messageService.ShowError("Duplicate name, please choose another one.");
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void okButtonClick(object sender, System.EventArgs e)
		{
			if (categoryName != ControlDictionary["categoryNameTextBox"].Text) {
				foreach (Category cat in ToolboxProvider.ComponentLibraryLoader.Categories) {
					if (cat.Name == ControlDictionary["categoryNameTextBox"].Text) {
						ShowDuplicateErrorMessage();
						return;
					}
				}
					
				foreach (AxSideTab tab in SharpDevelopSideBar.SideBar.Tabs) {
					if (!(tab is SideTabDesigner) && !(tab is CustomComponentsSideTab)) {
						if (tab.Name == ControlDictionary["categoryNameTextBox"].Text) {
							ShowDuplicateErrorMessage();
							return;
						}
					}
				}
				
				categoryName = ControlDictionary["categoryNameTextBox"].Text;
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
	}
}
