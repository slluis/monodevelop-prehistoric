// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
/*	
	interface InfoDeclaration
	{
		string GetNamespace(string name);
		string GetTypeName(Type type);
		string GetTypeModifiers(Type type);
		string GetAbstractionName(Type type);
	}
	
	class GeneralDeclaration : InfoDeclaration
	{
		
		public string GetNamespace(string name)
		{
			return "Namespace " + name;
	    }
	    
	    public string GetTypeName(Type type)
	    {
	    	return type.FullName;
	    }
	    
		public string GetTypeModifiers(Type type)
		{
			string back = "";
			if (type.IsAbstract && !type.IsInterface) 
				back += "MustInherit ";
			
			if (type.IsSealed && !type.IsValueType && !Global.IsDelegate(type)) 
				back += "NotInheritable ";
			
			if (type.IsPublic) 
				back += "Public ";
			else if (type.IsNestedPublic)
				back += "Public "; 
			else if (type.IsNestedPrivate) 
				back += "Private "; 
			else if (type.IsNestedFamily) 
				back += "Protected "; 
			else if (type.IsNestedAssembly) 
				back += "Assembly "; 
			else if (type.IsNestedFamANDAssem) 
				back += "FamilyAndAssembly "; 
			else if (type.IsNestedFamORAssem) 
				back += "FamiliyOrAssembly "; 
			else
				back += "Private ";
			return back;
		}
		
		public string GetAbstractionName(Type type)
		{
			string back = "";
			
			if (type.IsSubclassOf(typeof(Delegate)) && !(Type == typeof(Delegate) || Type == typeof(MulticastDelegate))) {
				back += "Delegate ";
			}
			if (type.IsEnum) 
				back += "Enum "; 
			else if (type.IsInterface) 
		    	back += "Interface "; 
		    else if (type.IsClass) 
    			back += "Class "; 
    		else
    			back += "Struct ";
		}
	}*/
	
	public class ReflectionSourceView : UserControl
	{
		RichTextBox    rtb;
		CheckBox       chk;
		
		ReflectionTree tree;
				
		void CopyEvent(object sender, EventArgs e)
		{
			Clipboard.SetDataObject(new DataObject(DataFormats.Text, rtb.Text));
		}
		
		public ReflectionSourceView(ReflectionTree tree)
		{
			rtb = new RichTextBox();
			rtb.ReadOnly = true;
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			rtb.Font = resourceService.LoadFont("Courier New", 10);
			
			this.tree = tree;
			
			Dock = DockStyle.Fill;
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			rtb.Location = new Point(0, 24);
			rtb.Size = new Size(Width, Height - 24);
			rtb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

			rtb.ContextMenu = new ContextMenu(new MenuItem[] {
					new MenuItem("Copy", new EventHandler(CopyEvent))
				});
			
			chk = new CheckBox();
			chk.Location = new Point(0, 0);
			chk.Size = new Size(250, 16);
			chk.Text = tree.ress.GetString("ObjectBrowser.SourceView.Enable");
			
			chk.CheckedChanged += new EventHandler(Check);
			Check(null, null);
			
			Controls.Add(rtb);
			Controls.Add(chk);
		}
		
		
		void Check(object sender, EventArgs e)
		{
			if(chk.Checked) {
				rtb.BackColor = SystemColors.Window;
			} else {
				rtb.BackColor = SystemColors.Control;
				rtb.Text = "";
			}
		}
		
		static string GetTypeString(string type)
		{
			string[,] types = new string[,] {
				{"System.Void",   "void"},
				{"System.Object", "object"},
				{"System.Boolean", "bool"},
				{"System.Byte", "byte"},
				{"System.SByte", "sbyte"},
				{"System.Char", "char"},
				{"System.Enum", "enum"},
				{"System.Int16", "short"},
				{"System.Int32", "int"},
				{"System.Int64", "long"},
				{"System.UInt16", "ushort"},
				{"System.UInt32", "uint"},
				{"System.UInt64", "ulong"},
				{"System.Single", "float"},
				{"System.Double", "double"},
				{"System.Decimal", "decimal"},
				{"System.String", "string"}
			};
			
			for (int i = 0; i < types.GetLength(0); ++i) {
				type = type.Replace(types[i, 0], types[i, 1]);
			}
			return type;
		}
		
		string GetAttributes(int indent, object[] attributes)
		{
			string back = "";
			if (attributes.Length > 0) {
				foreach (object o in attributes) {
					for (int i = 0; i < indent; ++i)
						back += "\t";
					back += "[";
					Type attrtype = o.GetType();
					back += attrtype.FullName + "(";
					try {
						object result = attrtype.InvokeMember("Value", BindingFlags.Default | BindingFlags.GetField | BindingFlags.GetProperty, null, o, new object [] {});
						if (result is string) {
							back += '"' + result.ToString() + '"';
						} else {
							string resultstring = result.ToString();
							if (result.GetType().IsEnum && !Char.IsDigit(resultstring[0])) {
								back += result.GetType().FullName + ".";
							}
							back += resultstring;
						}
					} catch (Exception) {
						try {
							object result = attrtype.InvokeMember("MemberName", BindingFlags.Default | BindingFlags.GetField | BindingFlags.GetProperty, null, o, new object [] {});
							back += '"' + result.ToString() + '"';
						} catch (Exception) {
						}
					}
					back += ")]\n";
				}
			}
			return back;
		}
		
		string GetAttributes(int indent, MemberInfo type)
		{
			return GetAttributes(indent, type.GetCustomAttributes(true));
		}
		
		void ShowTypeInfo(Type type)
		{
			rtb.Text = "";
			{
				string attr2 = GetAttributes(0, type.Assembly.GetCustomAttributes(true));
				rtb.Text += "// assembly attributes\n" + attr2 + "\n// declaration\n";
			}
			string attr = GetAttributes(0, type);
			rtb.Text += attr;
			
			
			if (type.IsSealed && !type.IsEnum)
				rtb.Text += "sealed ";
			
			if (type.IsAbstract && !type.IsInterface) 
				rtb.Text += "abstract ";
			
			if (type.IsNestedPrivate) {
				rtb.Text += "private ";
			} else
			if (type.IsNotPublic) {
				rtb.Text += "protected ";
			} else
				rtb.Text += "public ";
			
			if (type.IsEnum)
				rtb.Text += "enum ";
			else
			if (type.IsValueType)
				rtb.Text += "struct ";
			else
			if (type.IsInterface)
				rtb.Text += "interface ";
			else
				rtb.Text += "class ";
			
			rtb.Text += type.Name;
			
			if (!type.IsEnum) {
				Type[] interfaces = type.GetInterfaces();
				if (interfaces.Length > 0) {
					rtb.Text += " : ";
					for (int i = 0; i < interfaces.Length; ++i) {
						rtb.Text += interfaces[i].FullName;
						if (i + 1 <interfaces.Length)
							rtb.Text += ", ";
					}
				}
			}
			
			rtb.Text += "\n{\n";
			
			if (!type.IsEnum) {
				ConstructorInfo[] constructorinfos  = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo[]      methodinfos       = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				FieldInfo[]       fieldinfos        = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				PropertyInfo[]    propertyinfos     = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				EventInfo[]       eventinfos        = type.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				
				foreach (FieldInfo fieldinfo  in fieldinfos) {
					if (fieldinfo.DeclaringType.Equals(type)) {
						rtb.Text += "\t";
						
						if (fieldinfo.IsPrivate) { // private
							rtb.Text += "private ";
						} else
						if (fieldinfo.IsFamily) { // protected
							rtb.Text += "protected ";
						} else
							rtb.Text += "public ";
						
						if (fieldinfo.IsStatic) {
							rtb.Text += "static ";
						}
						
						rtb.Text += GetTypeString(fieldinfo.FieldType.ToString()) + " " + fieldinfo.Name + ";\n\n";
					}
				}
				
				foreach (ConstructorInfo constructorinfo in constructorinfos) {
					if (constructorinfo.DeclaringType.Equals(type))
					if ((constructorinfo.Attributes & MethodAttributes.SpecialName) == 0) {
						attr = GetAttributes(1, constructorinfo);
						rtb.Text += attr;
						rtb.Text += "\t";
						
						if (!type.IsInterface) {
							if (constructorinfo.IsPrivate) { // private
								rtb.Text += "private ";
							} else
							if (constructorinfo.IsFamily ) { // protected
								rtb.Text += "protected ";
							} else
								rtb.Text += "public ";
							
							if (constructorinfo.IsStatic) {
								rtb.Text += "static ";
							}
							if (constructorinfo.IsAbstract) {
								rtb.Text += "abstract ";
							}
						}
						
						rtb.Text += " " + type.Name +"(";
						ParameterInfo[] pinfo = constructorinfo.GetParameters();
						for (int i = 0; i < pinfo.Length; ++i) {
							string typetxt = pinfo[i].ParameterType.ToString();
//							if (pinfo[i].IsRetval)
//								rtb.Text += "ref ";
//							if (pinfo[i].IsOut)
//								rtb.Text += "out ";
							if (typetxt[typetxt.Length-1] == '&') {
								typetxt = "ref " + typetxt.Substring(0, typetxt.Length-1);
							}
							rtb.Text += GetTypeString(typetxt) + " " + pinfo[i].Name  +  ((i < pinfo.Length - 1) ? ", " : "");
						}
						if (type.IsInterface)
							rtb.Text += ");\n\n";
						else {
							rtb.Text += ")\n\t{\n\t\t// TODO\n\t}\n\n";
						}
					}
				}
				
				foreach (MethodInfo methodinfo in methodinfos) {
					if (methodinfo.DeclaringType.Equals(type))
					if ((methodinfo.Attributes & MethodAttributes.SpecialName) == 0) {
						attr = GetAttributes(1, methodinfo);
						rtb.Text += attr;
						rtb.Text += "\t";
						
						if (!type.IsInterface) {
							if (methodinfo.IsPrivate) { // private
								rtb.Text += "private ";
							} else
							if (methodinfo.IsFamily ) { // protected
								rtb.Text += "protected ";
							} else
								rtb.Text += "public ";
							
							if (methodinfo.IsStatic) {
								rtb.Text += "static";
							}
							if (methodinfo.IsAbstract) {
								rtb.Text += "abstract ";
							}
						}
						
						rtb.Text += GetTypeString(methodinfo.ReturnType.ToString()) + " " + methodinfo.Name +"(";
						ParameterInfo[] pinfo = methodinfo.GetParameters();
						for (int i = 0; i < pinfo.Length; ++i) {
							string typetxt = pinfo[i].ParameterType.ToString();
//							if (pinfo[i].IsRetval)
//								rtb.Text += "ref ";
//							if (pinfo[i].IsOut)
//								rtb.Text += "out ";
							if (typetxt[typetxt.Length-1] == '&') {
								typetxt = "ref " + typetxt.Substring(0, typetxt.Length-1);
							}
							rtb.Text += GetTypeString(typetxt) + " " + pinfo[i].Name  +  ((i < pinfo.Length - 1) ? ", " : "");
						}
						if (type.IsInterface)
							rtb.Text += ");\n\n";
						else {
							rtb.Text += ")\n\t{\n\t\t//TODO\n\t}\n\n";
						}
					}
				}
				
				foreach (PropertyInfo propertyinfo in propertyinfos) {
					if (propertyinfo.DeclaringType.Equals(type)) {
						attr = GetAttributes(1, propertyinfo);
						rtb.Text += attr;
						rtb.Text += "\t";
						
						rtb.Text += GetTypeString(propertyinfo.PropertyType.ToString()) + " " + propertyinfo.Name + " {\n";
						
						if (propertyinfo.CanRead) {
							rtb.Text += "\t\tget";
							if (type.IsInterface)
								rtb.Text += ";\n";
							else {
								rtb.Text += " {\n\t\t\t// TODO\n\t\t}\n";
							}
						}
						if (propertyinfo.CanWrite) {
							rtb.Text += "\t\tset";
							if (type.IsInterface)
								rtb.Text += ";\n";
							else {
								rtb.Text += " {\n\t\t\t// TODO\n\t\t}\n";
							}
						}
						rtb.Text += "\t}\n\n";
					}
				}
				
				foreach (EventInfo eventinfo in eventinfos) {
					if (eventinfo.DeclaringType.Equals(type)) {
						rtb.Text += "\tevent " + eventinfo.EventHandlerType + " " + eventinfo.Name + ";\n";
					}
				}
			}
			
			if (type.IsEnum) {
				FieldInfo[] fieldinfos = type.GetFields();
				object enumobj = type.Assembly.CreateInstance(type.FullName);
				foreach (FieldInfo fieldinfo in fieldinfos) {					
					if (fieldinfo.IsLiteral) {
						attr = GetAttributes(1, fieldinfo);
						rtb.Text += attr;
						rtb.Text += "\t" + fieldinfo.Name + " = " + (int)fieldinfo.GetValue(enumobj) + ",\n";
					}
				}
			}
			
			rtb.Text += "}";
			rtb.Refresh();
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			if(!chk.Checked) return;
			
			ReflectionNode node = (ReflectionNode)e.Node;
			
			rtb.Text = "";
			if (node is ReflectionTypeNode) 
				ShowTypeInfo((Type)node.Attribute);
			else
				switch (node.Type) {
					case ReflectionNodeType.Namespace:
						rtb.Text = "namespace " + node.Text + "\n{\n\n}";
						break;
					default:
						rtb.Text = tree.ress.GetString("ObjectBrowser.SourceView.NoView");
						break;
				}
				
		}
	}
}
