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
using ICSharpCode.Core.Services;

using SA = ICSharpCode.SharpAssembly.Assembly;
using SharpDevelop.Internal.Parser;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class SourceView : UserControl
	{
		RichTextBox    rtb;
		CheckBox       chk;
		
		AssemblyTree tree;
		IAmbience ambience;
				
		void CopyEvent(object sender, EventArgs e)
		{
			Clipboard.SetDataObject(new DataObject(DataFormats.Text, rtb.Text));
		}
		
		public SourceView(AssemblyTree tree)
		{
			rtb = new RichTextBox();
			rtb.ReadOnly = true;
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			
			ambience = ambienceService.CurrentAmbience;

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

		string GetAttributes(int indent, IMember member)
		{
			if (member.Attributes.Count == 0) return "";
			return GetAttributes(indent, member.Attributes[0].Attributes);
		}
		
		string GetAttributes(int indent, IClass type)
		{
			if (type.Attributes.Count == 0) return "";
			return GetAttributes(indent, type.Attributes[0].Attributes);
		}
		
		string GetAttributes(int indent, SA.SharpAssembly assembly)
		{
			return GetAttributes(indent, SharpAssemblyAttribute.GetAssemblyAttributes(assembly));
		}
		
		string GetAttributes(int indent, AttributeCollection ca)
		{
			string text = "";
			try {
				foreach(SharpAssemblyAttribute obj in ca) {
					string attrString = obj.ToString();
					text += ambience.WrapAttribute(attrString) + "\n";
				}
			} catch {}
			return text;
		}

		void ShowTypeInfo(IClass type)
		{
			string rt = "";
			{
				string attr2 = GetAttributes(0, (SA.SharpAssembly)type.DeclaredIn);
				rt += ambience.WrapComment("assembly attributes\n") + attr2 + "\n" +
					  ambience.WrapComment("declaration\n");
			}
			string attr = GetAttributes(0, type);
			rt += attr;
			
			rt += ambience.Convert(type);
			
			rt += "\n";
			
			if (type.ClassType != ClassType.Enum) {
				
				rt += "\t" + ambience.WrapComment("events\n");
				
				foreach (IField fieldinfo in type.Fields) {
					rt += GetAttributes(1, fieldinfo);
					rt += "\t" + ambience.Convert(fieldinfo) + "\n";
				}
				
				rt += "\t" + ambience.WrapComment("methods\n");
				
				foreach (IMethod methodinfo in type.Methods) {
					if (methodinfo.IsSpecialName) continue;
					
					rt += GetAttributes(1, methodinfo);
					rt += "\t" + ambience.Convert(methodinfo);
					if (type.ClassType == ClassType.Interface)
						rt += "\n\n";
					else {
						rt += "\n\t\t" + ambience.WrapComment("TODO") + "\n\t" + ambience.ConvertEnd(methodinfo) + "\n\n";
					}
				}
				
				rt += "\t" + ambience.WrapComment("properties\n");
				
				foreach (IProperty propertyinfo in type.Properties) {
					rt += GetAttributes(1, propertyinfo);
					rt += "\t" + ambience.Convert(propertyinfo) + "\n";
				}
				
				rt += "\t" + ambience.WrapComment("events\n");
				
				foreach (IEvent eventinfo in type.Events) {
					rt += GetAttributes(1, eventinfo);
					rt += "\t" + ambience.Convert(eventinfo) + "\n";
				}
			} else { // Enum
				foreach (IField fieldinfo in type.Fields) {					
					if (fieldinfo.IsLiteral) {
						attr = GetAttributes(1, fieldinfo);
						rt += attr;
						rt += "\t" + fieldinfo.Name;
												
						if (fieldinfo is SharpAssemblyField) {
							SharpAssemblyField saField = fieldinfo as SharpAssemblyField;
							if (saField.InitialValue != null) {
								rt += " = " + saField.InitialValue.ToString();
							}
						}

						rt += ",\n";
					}
				}
			}
			
			rt += ambience.ConvertEnd(type);
			
			rtb.Text = rt;
			rtb.Refresh();
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			if(!chk.Checked) return;
			
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			
			rtb.Text = "";
			if (node.Attribute is IClass)  {
				ambience.ConversionFlags = ConversionFlags.All | ConversionFlags.QualifiedNamesOnlyForReturnTypes | ConversionFlags.IncludeBodies;
				/*
				if (node.Attribute is SharpAssemblyClass) {
					if (!(node.Attribute as SharpAssemblyClass).MembersLoaded) (node.Attribute as SharpAssemblyClass).LoadMembers();
				}
				*/
				ShowTypeInfo((IClass)node.Attribute);
			} else {
				switch (node.Type) {
					case NodeType.Namespace:
						rtb.Text = "namespace " + node.Text + "\n{\n\n}";
						break;
					default:
						rtb.Text = tree.ress.GetString("ObjectBrowser.SourceView.NoView");
						break;
				}
			}
				
		}
		
	}
	
}
