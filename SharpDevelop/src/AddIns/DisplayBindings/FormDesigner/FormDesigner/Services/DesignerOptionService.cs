// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel.Design;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DesignerOptionService : IDesignerOptionService
	{
		public const string GridSize   = "GridSize";
		public const string ShowGrid   = "ShowGrid";
		public const string SnapToGrid = "SnapToGrid";
		
		const string GridSizeWidth  = "GridSize.Width";
		const string GridSizeHeight = "GridSize.Height";
		
		public const string FormsDesignerPageName = "SharpDevelop Forms Designer\\General";
		
		Hashtable pageOptionTable = new Hashtable();
		
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public DesignerOptionService()
		{
			Hashtable defaultTable = new Hashtable();
			
			defaultTable[GridSize]   = new Size(8, 8);
			defaultTable[ShowGrid]   = false;
			defaultTable[SnapToGrid] = false;
			
			pageOptionTable[FormsDesignerPageName] = defaultTable;
		}
		
		public object GetOptionValue(string pageName, string valueName)
		{
			switch (valueName) {
				case GridSize:
					return new Size(propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeWidth", 8),
					                propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", 8));
				case ShowGrid:
					return propertyService.GetProperty("FormsDesigner.DesignerOptions.ShowGrid", true);
				case SnapToGrid:
					return propertyService.GetProperty("FormsDesigner.DesignerOptions.SnapToGrid", true);
				case GridSizeWidth:
					return propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeWidth", 8);
				case GridSizeHeight:
					return propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", 8);
				default:
					Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
					
					if (pageTable == null) {
						return null;
					}
					return pageTable[valueName];
			}
		}
		
		public void SetOptionValue(string pageName, string valueName, object val)
		{
			switch (valueName) {
				case GridSize:
					Size size = (Size)val;
					propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeWidth",  size.Width);
					propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", size.Height);
					break;
				case ShowGrid:
					propertyService.GetProperty("FormsDesigner.DesignerOptions.ShowGrid", (bool)val);
					break;
				case SnapToGrid:
					propertyService.GetProperty("FormsDesigner.DesignerOptions.SnapToGrid", (bool)val);
					break;
				case GridSizeWidth:
					propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeWidth", (int)val);
					break;
				case GridSizeHeight:
					propertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", (int)val);
					break;
				default:
					Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
					if (pageTable == null) {
						pageOptionTable[pageName] = pageTable = new Hashtable();
					}
					pageTable[valueName] = val;
					break;
			}
		}
	}
}
