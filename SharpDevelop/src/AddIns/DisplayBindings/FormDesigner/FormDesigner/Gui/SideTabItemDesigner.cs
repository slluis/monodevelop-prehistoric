/*
 * Module : FormDesigner
 * 
 * Project : FormDesigner Loading Library Control.
 * 
 * Source code altering : A1 
 * 
 * Description : création of the Tab item displayed into the AXSideTabDesigner control.
 * 				 the control's creator initialize the toolboxitem of the tab item
 * 
 * Denis ERCHOFF						22/01/2003
 */


//		Denis ERCHOFF		22/01/2003		BEGIN		A1

using System;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel.Design;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class SideTabItemDesigner : SharpDevelopSideTabItem
	{
		
		public void CreatedUserControl()
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
		}
		
		///<summary>create a tabitem from a toolboxitem. It init Icon and name from the tag</summary>
		public SideTabItemDesigner(ToolboxItem tag) : base(tag.DisplayName, tag)
		{
			this.Icon           = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a tabitem from a toolboxitem. It init Icon from the tag</summary>
		public SideTabItemDesigner(string name, ToolboxItem tag) : base(name, tag)
		{
			this.Icon           = tag.Bitmap;
			ReloadToolBox();
		}
		
		///<summary>create a default tabitem : a pointer icon with an empty toolboxitem</summary>
		public SideTabItemDesigner() : base("Pointer")
		{	
			
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			
			Bitmap pointerBitmap = new Bitmap(iconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
//			ToolboxItem toolboxItemPointer = new ToolboxItem();
//			toolboxItemPointer.Bitmap      = pointerBitmap;
//			toolboxItemPointer.DisplayName = "Pointer";
			this.Icon = pointerBitmap;
			this.Tag  = null; //toolboxItemPointer;
			ReloadToolBox();
		}
		
		///<summary>it force to reload toolboxitem into the ToolboxService when the hostchange</summary>
		public void ReloadToolBox()
		{
			if (this.Name != "Pointer") {
				ToolboxProvider.ToolboxService.AddToolboxItem(this.Tag as ToolboxItem);
			}
		}				
	}
}
