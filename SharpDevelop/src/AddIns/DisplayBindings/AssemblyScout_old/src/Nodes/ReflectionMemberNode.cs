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
using System.Drawing;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionMemberNode : ReflectionNode
	{
		bool special = false;
		
		public ReflectionMemberNode(MethodInfo methodinfo2) : base ("", methodinfo2, ReflectionNodeType.Method)
		{
			SetNodeName();
		}
		
		public ReflectionMemberNode(ConstructorInfo constructorinfo2) : base ("", constructorinfo2, ReflectionNodeType.Constructor)
		{
			SetNodeName();
		}
		
		public ReflectionMemberNode(PropertyInfo prop, bool Special) : base ("", prop, ReflectionNodeType.Property)
		{
			SetNodeName();
			if(special = Special) CreateSpecialNodes(prop);
		}
		
		public ReflectionMemberNode(EventInfo evt, bool Special) : base ("", evt, ReflectionNodeType.Event)
		{
			SetNodeName();
			if(special = Special) CreateSpecialNodes(evt);
		}
		
		public ReflectionMemberNode(FieldInfo fld) : base ("", fld, ReflectionNodeType.Field)
		{
			SetNodeName();
		}
		
		void SetNodeName()
		{
			if (attribute == null) {
				Text = "no name";
				return;
			}
			
			Text = GetMemberNodeText((MemberInfo)attribute);
			if (Text.EndsWith("[static]")) {
				this.NodeFont = new Font("Tahoma", 8, FontStyle.Italic);
			}
		}
		
		void CreateSpecialNodes(PropertyInfo prop)
		{
			MethodInfo get = prop.GetGetMethod();
			
			if (prop.CanRead)
				Nodes.Add(new ReflectionMethodNode(prop.GetGetMethod(true)));
			if (prop.CanWrite)
				Nodes.Add(new ReflectionMethodNode(prop.GetSetMethod(true)));			
		}
		
		void CreateSpecialNodes(EventInfo evt)
		{
			MethodInfo add    = evt.GetAddMethod(true);
			MethodInfo raise  = evt.GetRaiseMethod(true);
			MethodInfo remove = evt.GetRemoveMethod(true);
			
			if (add != null)
				Nodes.Add(new ReflectionMethodNode(add));
			if (raise != null)
				Nodes.Add(new ReflectionMethodNode(raise));
			if (remove != null)
				Nodes.Add(new ReflectionMethodNode(remove));
		}
		
		protected override void SetIcon()
		{
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			if (attribute == null)
				return;
			switch (type) {
				case ReflectionNodeType.Method:
					MethodInfo methodinfo = (MethodInfo)attribute;
					ImageIndex = SelectedImageIndex = classBrowserIconService.GetIcon(methodinfo);
					break;
				
				case ReflectionNodeType.Constructor:
					ConstructorInfo constructorinfo = (ConstructorInfo)attribute;
					ImageIndex = SelectedImageIndex = classBrowserIconService.GetIcon(constructorinfo);
					break;
				
				case ReflectionNodeType.Event:
					EventInfo eventinfo = (EventInfo)attribute;
					ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon(eventinfo);
					break;
								
				case ReflectionNodeType.Property:
					PropertyInfo propertyinfo = (PropertyInfo)attribute;
					ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon(propertyinfo);
					break;
				
				case ReflectionNodeType.Field:
					FieldInfo fieldinfo = (FieldInfo)attribute;
					ImageIndex = SelectedImageIndex = classBrowserIconService.GetIcon(fieldinfo);
					break;
				
			}
		}
		
		public static string GetMemberNodeText(MemberInfo mi)
		{
//			switch (type) {
//				case ReflectionNodeType.Method:
//					Text = ReflectionTree.languageConversion.Convert((MethodInfo)attribute);
//					break;
//				case ReflectionNodeType.Constructor:
//					Text = ReflectionTree.languageConversion.Convert((ConstructorInfo)attribute);
//					break;
//				case ReflectionNodeType.Field:
//					Text = ReflectionTree.languageConversion.Convert((FieldInfo)attribute);
//					break;
//				case ReflectionNodeType.Property:
//					Text = ReflectionTree.languageConversion.Convert((PropertyInfo)attribute);
//					break;
//				case ReflectionNodeType.Event:
//					Text = ReflectionTree.languageConversion.Convert((EventInfo)attribute);
//					break;
//			}	
			
			return GetShortMemberName(mi);
		}
		
		public static string GetShortMemberName(MemberInfo mi) {
			string ret = "";
			
			ret = mi.Name;
			
			try {
				
				if (ret == ".ctor" || ret == ".cctor") ret = mi.DeclaringType.Name;
	
				if (mi is MethodBase) {
					MethodBase mii = mi as MethodBase;
				
					ret += GetParams(mii.GetParameters(), true);
					if (mii.IsStatic) ret += " [static]";
				} else if (mi is PropertyInfo) {
					PropertyInfo ppi = mi as PropertyInfo;
	
					ret += GetParams(ppi.GetIndexParameters(), false);
					
					MethodInfo mmi = ppi.GetGetMethod(true);
					if (mmi == null) mmi = ppi.GetSetMethod(true);
					if (mmi == null) goto noMethod;
					if (mmi.IsStatic) {
						ret += " [static]";
					}
					noMethod:;
										
				} else if (mi is EventInfo) {
					EventInfo evi = mi as EventInfo;
					
					if (evi.GetAddMethod(true).IsStatic) ret += " [static]";
					
				} else if (mi is FieldInfo) {
					FieldInfo fi = mi as FieldInfo;
					if (fi.IsStatic) ret += " [static]";
				}
			
			} catch {}
			
			return ret;
		}

		public static string GetParams(ParameterInfo[] piarr, bool IncludeBrackets) {
			string param = "";
			foreach(ParameterInfo pi in piarr) {
				param += GetNestedName(pi.ParameterType.FullName) + ", ";
			}
			if (param.Length > 0) param = param.Substring(0, param.Length - 2);
			if (param != "" || IncludeBrackets) param = "(" + param + ")";
			return param;
		}

		public static string GetNestedName(string name) {
			int i = name.LastIndexOf(".");
			if (i == -1) return name;
			return name.Substring(i + 1);
		}

		
	}
}
