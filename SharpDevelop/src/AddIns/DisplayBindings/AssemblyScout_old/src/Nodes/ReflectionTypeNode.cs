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
	public class ReflectionTypeNode : ReflectionNode
	{
		public bool MembersPopulated;
		
		public ReflectionTypeNode(string name, Type type) : base (name, type, ReflectionNodeType.Type)
		{
		}
		
		protected override void SetIcon()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon((Type)attribute);
		}
		
		public override void Populate(bool Private, bool Internal)
		{
			Type type = (Type)attribute;

			Nodes.Clear();
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			ReflectionNode supertype = new ReflectionNode(ress.GetString("ObjectBrowser.Nodes.SuperTypes"), type, ReflectionNodeType.SuperTypes);
			Nodes.Add(supertype);
			
			AddBaseTypes(type, supertype, classBrowserIconService);
			
			foreach (Type baseinterface in  type.GetInterfaces()) {
				ReflectionNode inode = new ReflectionNode(baseinterface.Name,  baseinterface, ReflectionNodeType.Link);
				inode.ImageIndex = inode.SelectedImageIndex = classBrowserIconService.GetIcon(baseinterface);
				supertype.Nodes.Add(inode);
				AddBaseTypes(baseinterface, inode, classBrowserIconService);
			}
			
			// TODO: SubTypes is not implemented
			// Nodes.Add(new ReflectionNode("SubTypes", type, ReflectionNodeType.SubTypes));
			
			populated = true;
		}
		
		public void PopulateMembers(bool Private, bool Internal, bool Special)
		{
			Type type = (Type)attribute;
			ArrayList nodes = new ArrayList();
			TreeNodeComparer comp = new TreeNodeComparer();
			
			BindingFlags bf = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			
			ConstructorInfo[] constructorinfos  = type.GetConstructors(bf);
			MethodInfo[]      methodinfos       = type.GetMethods(bf);
			FieldInfo[]       fieldinfos        = type.GetFields(bf);
			PropertyInfo[]    propertyinfos     = type.GetProperties(bf);
			EventInfo[]       eventinfos        = type.GetEvents(bf);
			
			nodes.Clear();
			foreach (ConstructorInfo constructorinfo in constructorinfos) {
				if (!Private && IsPrivateMember(constructorinfo)) continue;
				if (!Internal && IsInternalMember(constructorinfo)) continue;
				nodes.Add(new ReflectionMemberNode(constructorinfo));
			}
			nodes.Sort(comp);
			foreach (ReflectionNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (MethodInfo methodinfo in methodinfos) {
				if (!Private && IsPrivateMember(methodinfo)) continue;
				if (methodinfo.IsSpecialName) continue;
				if (!Internal && IsInternalMember(methodinfo)) continue;
				nodes.Add(new ReflectionMemberNode(methodinfo));
			}
			nodes.Sort(comp);
			foreach (ReflectionNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (PropertyInfo propertyinfo in propertyinfos) {
				if (!Private && IsPrivateMember(propertyinfo)) continue;
				if (!Internal && IsInternalMember(propertyinfo)) continue;
				nodes.Add(new ReflectionMemberNode(propertyinfo, Special));
			}
			nodes.Sort(comp);
			foreach (ReflectionNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (FieldInfo fieldinfo  in fieldinfos) {
				if (!Private && IsPrivateMember(fieldinfo)) continue;
				if (!Internal && IsInternalMember(fieldinfo)) continue;
				if (fieldinfo.IsSpecialName) continue;
				nodes.Add(new ReflectionMemberNode(fieldinfo));
			}
			nodes.Sort(comp);
			foreach (ReflectionNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (EventInfo eventinfo in eventinfos) {
				if (!Private && IsPrivateMember(eventinfo)) continue;
				if (!Internal && IsInternalMember(eventinfo)) continue;
				nodes.Add(new ReflectionMemberNode(eventinfo, Special));
			}
			nodes.Sort(comp);
			foreach (ReflectionNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			MembersPopulated = true;
		}
		
		private void AddBaseTypes(Type type, ReflectionNode node, ClassBrowserIconsService classBrowserIconService)
		{
			if (type.BaseType != null) {
				ReflectionNode basetype = new ReflectionNode(type.BaseType.Name, type.BaseType, ReflectionNodeType.Link);
				basetype.ImageIndex = basetype.SelectedImageIndex = classBrowserIconService.GetIcon(type.BaseType);
				node.Nodes.Add(basetype);
				AddBaseTypes(type.BaseType, basetype, classBrowserIconService);
			}
		}
		
		public static bool IsPrivateMember(MemberInfo mi) {
			if (mi is MethodBase) return (mi as MethodBase).IsPrivate;
			if (mi is PropertyInfo) {
				if ((mi as PropertyInfo).GetGetMethod() != null)
					return (mi as PropertyInfo).GetGetMethod().IsPrivate;
				if ((mi as PropertyInfo).GetSetMethod() != null)
					return (mi as PropertyInfo).GetSetMethod().IsPrivate;
				return true;
			}
			if (mi is FieldInfo) return (mi as FieldInfo).IsPrivate;
			if (mi is EventInfo) {
				if ((mi as EventInfo).GetAddMethod() != null)
					return (mi as EventInfo).GetAddMethod().IsPrivate;
				return true;
			}
			return false;
		}
		
		public static bool IsInternalMember(MemberInfo mi) {
			if (mi is MethodBase) return ((mi as MethodBase).IsAssembly || (mi as MethodBase).IsFamilyAndAssembly);
			if (mi is PropertyInfo) {
				if ((mi as PropertyInfo).GetGetMethod() != null)
					return IsInternalMember((mi as PropertyInfo).GetGetMethod());
				if ((mi as PropertyInfo).GetSetMethod() != null)
					return IsInternalMember((mi as PropertyInfo).GetSetMethod());
				return false;
			}
			if (mi is FieldInfo) return ((mi as FieldInfo).IsAssembly || (mi as FieldInfo).IsFamilyAndAssembly);
			if (mi is EventInfo) {
				if ((mi as EventInfo).GetAddMethod() != null)
					return IsInternalMember((mi as EventInfo).GetAddMethod());
				return false;
			}
			return false;			
		}
		
	}
}
