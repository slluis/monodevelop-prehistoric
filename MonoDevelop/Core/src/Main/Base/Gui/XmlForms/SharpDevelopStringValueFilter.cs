﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;

namespace MonoDevelop.Gui.XmlForms
{/*
	/// <summary>
	/// This interface is used to filter the values defined in the xml files.
	/// It could be used for the localization of control texts.
	/// </summary>
	public class SharpDevelopStringValueFilter : IStringValueFilter
	{
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		/// <summary>
		/// Is called for each value string in the definition xml file.
		/// </summary>
		/// <returns>
		/// The filtered text value
		/// </returns>
		public string GetFilteredValue(string originalValue)
		{
			bool useFlatStyle = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("MonoDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			stringParserService.Properties["BORDERSTYLE"] = useFlatStyle ? BorderStyle.FixedSingle.ToString() : BorderStyle.Fixed3D.ToString();
			stringParserService.Properties["FLATSTYLE"]   = useFlatStyle ? FlatStyle.Flat.ToString() : FlatStyle.Standard.ToString();			
			string back = stringParserService.Parse(originalValue);
			return back;
		}
	}*/
}