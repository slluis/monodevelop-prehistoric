// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using ICSharpCode.SharpDevelop.Gui.HtmlControl;

using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionInfoView : UserControl
	{
		GradientLabel cap = new GradientLabel();
		Label typ = new Label();
		LinkLabel back = new LinkLabel();
		
		// old LinkLabel view
		//LinkLabel ll = new LinkLabel();
		
		// new HtmlControl view
		HtmlControl ht = new HtmlControl();
		Panel pan = new Panel();
		
		ReflectionTree tree;
		IParserService parserService;
		
		ArrayList references = new ArrayList();
		string cssPath;
		string imgPath;
		
		public ReflectionInfoView(ReflectionTree tree)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			this.tree = tree;
			
			imgPath = System.IO.Path.GetTempFileName() + ".png";
			cssPath = System.IO.Path.GetTempFileName() + ".css";
			
			Color col = SystemColors.Control;
			string color = "#" + col.R.ToString("X") + col.G.ToString("X") + col.B.ToString("X");
			
			StreamWriter sw = new StreamWriter(cssPath, false);
			sw.Write(@"body { margin: 0px; border: 0px; overflow: hidden; padding: 0px;
							  background-color: " + color + @"; 
							  background-image: url(" + imgPath + @"); 
							  background-position: bottom right; 
							  background-repeat: no-repeat }
					p { font: 8pt Tahoma }

					a { font: 8pt Tahoma; text-decoration: none; color: blue}
					a:visited { color: blue }
					a:active  { color: blue }
					a:hover   { color: red; text-decoration: underline }");
			sw.Close();
			
			cap.Location  = new Point(0, 0);
			cap.Size      = new Size(Width, 32);
			cap.Text      = tree.ress.GetString("ObjectBrowser.Welcome");
			cap.Font      = new Font("Tahoma", 14);
			cap.BackColor = SystemColors.ControlLight;
			cap.TextAlign = ContentAlignment.MiddleLeft;
			cap.Anchor    = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
						
			string backt  = tree.ress.GetString("ObjectBrowser.Back");
			back.Size     = new Size(40, 16);
			back.Location = new Point(Width - back.Width, 44);
			back.Text     = backt;
			back.TextAlign = ContentAlignment.TopRight;
			back.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			back.Links.Add(0, backt.Length);
			back.LinkClicked += new LinkLabelLinkClickedEventHandler(back_click);
			
			typ.Location  = new Point(0, 44);
			typ.Size      = new Size(Width - back.Width, 16);
			typ.Font      = new Font(Font, FontStyle.Bold);
			typ.Text      = tree.ress.GetString("ObjectBrowser.WelcomeText");
			typ.TextAlign = ContentAlignment.TopLeft;
			typ.Anchor    = cap.Anchor;
			/*			
			ll.Location   = new Point(0, 72);
			ll.Size       = new Size(Width, Height - ll.Top);
			ll.Anchor     = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			ll.LinkClicked += new LinkLabelLinkClickedEventHandler(ll_click);
			ll.ImageAlign = ContentAlignment.BottomRight;
			ll.Text       = tree.ress.GetString("ObjectBrowser.Info.SelectNode");
			ll.Image      = CreateImage(resourceService.GetIcon("Icons.16x16.Class").ToBitmap());
			ll.LinkBehavior = LinkBehavior.HoverUnderline;
			ll.Links.Clear();
			*/
			ht = new HtmlControl();
			//ht.Size = new Size(20, 20);
			//ht.Location = new Point(20, 20);
			ht.BeforeNavigate += new BrowserNavigateEventHandler(HtmlControlBeforeNavigate);
			CreateImage(resourceService.GetIcon("Icons.16x16.Class").ToBitmap());
			ht.CascadingStyleSheet = cssPath;
			string html = RenderHead() + "<p>" + tree.ress.GetString("ObjectBrowser.Info.SelectNode") + RenderFoot();
			ht.Html = html;
			
			pan.Location   = new Point(0, 72);
			pan.DockPadding.Left = 10;
			pan.DockPadding.Bottom = 75;
			pan.Size       = new Size(Width, Height - ht.Top);
			pan.Anchor     = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			pan.Controls.Add(ht);
			
			ht.Dock = DockStyle.Fill;
			
			Controls.AddRange(new Control[] {
				cap, typ, back, pan
			});
			
			Dock = DockStyle.Fill;
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			parserService = (IParserService)ServiceManager.Services.GetService(typeof(IParserService));
		}
		
		~ReflectionInfoView() {
			System.IO.File.Delete(imgPath);
			System.IO.File.Delete(cssPath);
		}
		
		void HtmlControlBeforeNavigate(object sender, BrowserNavigateEventArgs e)
		{
			e.Cancel = true;
			
			try {
			
				string url = e.Url;
				int refnr  = Int32.Parse(url.Substring(5, url.Length - 6));
				object obj = references[refnr];
				
				if (obj is Type) {
					// Go To Type
					tree.GoToType((Type)obj);
					// try {
					// 	tree.SelectedNode.Expand();
					// } catch {}
				} else if (obj is AssemblyName) {
					// Open Assembly Reference
					tree.OpenAssemblyByName((AssemblyName)obj);
				} else if (obj is SaveResLink) {
					SaveResLink link = (SaveResLink)obj;
					tree.SaveResource(link.Asm, link.Name);
				} else if (obj is NamespaceLink) {
					NamespaceLink ns = (NamespaceLink)obj;
					tree.GoToNamespace(ns.Asm, ns.Name);
				}
			} catch { MessageBox.Show("Something failed following this link."); }
		}
		
		string RenderHead()
		{
			return "<div style='margin: 0px; width: 100%'><p>";
		}
		
		string RenderFoot()
		{
			return "</div>";
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			ReflectionNode node = (ReflectionNode)e.Node;
			cap.Text = node.Text;
			typ.Text = node.Type.ToString();
			
			references.Clear();
			CreateImage(tree.ImageList.Images[node.ImageIndex]);
			string html = RenderHead();
			
			try {
			
				switch(node.Type) {
					case ReflectionNodeType.Assembly:
						html += GetAssemblyInfo((Assembly)node.Attribute);
						break;
					case ReflectionNodeType.Library:
						html += GetLibInfo((Assembly)node.Attribute);
						break;
					case ReflectionNodeType.Reference:
						html += GetRefInfo((AssemblyName)node.Attribute);
						break;
					case ReflectionNodeType.Resource:
						html += GetResInfo((Assembly)node.Attribute, node.Text);
						break;
					case ReflectionNodeType.Module:
						html += GetModInfo((Module)node.Attribute);
						break;
					case ReflectionNodeType.Folder:
						html += GetFolderInfo(node.Text);
						break;
					case ReflectionNodeType.Namespace:
						html += GetNSInfo((Assembly)node.Attribute);
						break;
					case ReflectionNodeType.SubTypes:
						html += GetSubInfo();
						break;
					case ReflectionNodeType.SuperTypes:
						html += GetSuperInfo();
						break;
					case ReflectionNodeType.Link:
						html += GetLinkInfo((Type)node.Attribute);
						break;
					case ReflectionNodeType.Type:
						html += GetTypeInfo((Type)node.Attribute);
						break;
					case ReflectionNodeType.Constructor:
						html += GetCtorInfo((ConstructorInfo)node.Attribute);
						break;
					case ReflectionNodeType.Event:
						html += GetEventInfo((EventInfo)node.Attribute);
						break;
					case ReflectionNodeType.Field:
						html += GetFieldInfo((FieldInfo)node.Attribute);
						break;
					case ReflectionNodeType.Method:
						html += GetMethodInfo((MethodInfo)node.Attribute);
						break;
					case ReflectionNodeType.Property:
						html += GetPropInfo((PropertyInfo)node.Attribute);
						break;
					default:
						//ll.Text = "";
						break;
				}
			} catch(Exception ex) {
				html += tree.ress.GetString("ObjectBrowser.Info.CollectError") + "<br>" + ex.ToString().Replace("\n", "<br>");
			}
			
			html += RenderFoot();
			
			ht.Html = html;
		//	ht.CascadingStyleSheet = cssPath;
		}
		
		string GetLinkInfo(Type type)
		{
			string text;
			text = ln(references.Add(type), RT("GotoType"));
			//ll.Links.Add(0, ll.Text.Length, type);
			text += "<br><br>" + RT("LinkedType") + "<br>";
			return text + GetInAsm(type.Assembly);
		}
		
		string GetSubInfo()
		{
			return RT("SubInfo");
		}
		
		string GetSuperInfo()
		{
			return RT("SuperInfo");
		}
		
		string GetNSInfo(Assembly asm)
		{
			return GetInAsm(asm);
		}
		
		string GetFolderInfo(string folder)
		{
			if (folder == tree.ress.GetString("ObjectBrowser.Nodes.Resources")) 
				return RT("ResFInfo");
			else if (folder == tree.ress.GetString("ObjectBrowser.Nodes.References"))
				return RT("RefFInfo");
			else if (folder == tree.ress.GetString("ObjectBrowser.Nodes.Modules"))
				return RT("ModFInfo");
			else
				return RT("NoInfo");
		}
		
		string GetModInfo(Module mod)
		{
			string text = String.Format(RT("ModInfo"), mod.Name, mod.ScopeName, mod.IsResource());
			text += GetCustomAttribs(mod);
			text += "<br>";
			return text + GetInAsm(mod.Assembly);
		}
		
		string GetLibInfo(Assembly asm)
		{
			return RT("LibInfo") + GetInAsm(asm);
		}
		
		string GetRefInfo(AssemblyName asn)
		{
			string text = String.Format(RT("RefInfo"), asn.Name, asn.FullName, asn.Version.ToString(), "");
			return text + ln(references.Add(asn), RT("OpenRef"));
		}
		
		string GetAssemblyInfo(Assembly asm)
		{
			string text = String.Format(RT("AsmInfo"),
			                        asm.GetName().Name, asm.FullName, asm.GetName().Version.ToString(),
			                        asm.Location, asm.GlobalAssemblyCache);
			return text + GetCustomAttribs(asm);
		}
		
		string GetEventInfo(EventInfo info)
		{
			string ret = "public ";
			ret += GetTypeRef(info.EventHandlerType) + " ";
			ret += info.Name;
			ret += "<br><br>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullName);
			if(c == null) goto noDoc;
			foreach(IEvent e in c.Events) {
				if(e.Name == info.Name) {
					ret += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(e.Documentation) + "<br>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<br><br>";
			ret += GetInAsm(info.DeclaringType.Assembly);
			
			return ret;
		}
		
		string GetFieldInfo(FieldInfo info)
		{
			string ret = "";
			if (info.IsPublic) ret += "public ";
 			if (info.IsFamily || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "protected ";
			if (info.IsAssembly || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "internal ";
			if (info.IsPrivate) ret += "private ";
			if (info.IsStatic) ret += "static ";
			if (info.IsLiteral) ret += "const ";
			ret += GetTypeRef(info.FieldType) + " ";
			ret += info.Name;
			ret += "<br><br>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullName);
			if(c == null) goto noDoc;
			foreach(IField f in c.Fields) {
				if(f.Name == info.Name) {
					ret += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(f.Documentation) + "<br>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>" + GetInType(info.DeclaringType);
			ret += "<br><br>" + GetInAsm(info.DeclaringType.Assembly);
			
			return ret;
		}
		
		string GetCtorInfo(ConstructorInfo info)
		{
			string ret = "";
			if (info.IsPublic) ret += "public ";
 			if (info.IsFamily || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "protected ";
			if (info.IsAssembly || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "internal ";
			if (info.IsPrivate) ret += "private ";
			if (info.IsStatic) ret += "static ";
			ret += info.Name + " (";
			ret += GetParameters(info);
			ret += ")<br><br>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullName);
			if(c == null) goto noDoc;
			foreach(IMethod cc in c.Methods) {
				if(cc.IsConstructor && IsSameSig(cc, info)) {
					ret += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(cc.Documentation) + "<br>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>" + GetInType(info.DeclaringType);
			ret += "<br><br>" + GetInAsm(info.DeclaringType.Assembly);
			
			return ret;
		}
		
		string GetMethodInfo(MethodInfo info)
		{
			string ret = "";
			if (info.IsPublic) ret += "public ";
 			if (info.IsFamily || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "protected ";
			if (info.IsAssembly || info.IsFamilyAndAssembly || info.IsFamilyOrAssembly) ret += "internal ";
			if (info.IsPrivate) ret += "private ";
			if (info.IsStatic) ret += "static ";
			if (info.IsVirtual && !info.IsFinal) ret += "virtual ";
			if (info.IsAbstract) ret += "abstract ";
			if (info.ReturnType != typeof(void)) {
				ret += GetTypeRef(info.ReturnType);
			} else {
				ret += "void";
			}
			ret += " " + info.Name + " (";
			ret += GetParameters(info);
			ret += ")<br><br>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullName);
			if(c == null) goto noDoc;
			foreach(IMethod cc in c.Methods) {
				if(cc.Name == info.Name && IsSameSig(cc, info)) {
					ret += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(cc.Documentation) + "<br>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<br><br>";
			ret += GetInAsm(info.DeclaringType.Assembly);
			
			return ret;			
		}
		
		string GetPropInfo(PropertyInfo info)
		{
			string ret = "";
			MethodInfo mi = null;
			
			if (info.CanRead) mi = info.GetGetMethod(true);
			if (mi == null && info.CanWrite) mi = info.GetSetMethod(true);
			if (mi == null) goto nomod;
			if (mi.IsPublic) ret += "public ";
 			if (mi.IsFamily || mi.IsFamilyAndAssembly || mi.IsFamilyOrAssembly) ret += "protected ";
			if (mi.IsAssembly || mi.IsFamilyAndAssembly || mi.IsFamilyOrAssembly) ret += "internal ";
			if (mi.IsPrivate) ret += "private ";
			if (mi.IsStatic) ret += "static ";
			if (mi.IsVirtual && !mi.IsFinal) ret += "virtual ";
			if (mi.IsAbstract) ret += "abstract ";
			nomod:
			if (info.CanRead && !info.CanWrite) ret += "readonly ";
			if (!info.CanRead && info.CanWrite) ret += "writeonly ";
			ret += GetTypeRef(info.PropertyType);
			ret += " " + info.Name + " (";
			ret += GetParameters(info);
			ret += ")<br><br>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullName);
			if(c == null) goto noDoc;
			foreach(IProperty p in c.Properties) {
				if(p.Name == info.Name) {
					ret += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(p.Documentation) + "<br>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<br>";
			ret += GetInAsm(info.DeclaringType.Assembly);
			
			return ret;
		}
		
		bool IsSameSig(IMethod p, MethodBase info)
		{
			ParameterInfo[] pis = info.GetParameters();
			if (p.Parameters.Count != pis.Length) return false;
			
			for(int j = 0; j < pis.Length; j++) {
				IReturnType pi = new ReflectionReturnType(pis[j].ParameterType);
				IParameter ip = p.Parameters[j];
				if (pi.FullyQualifiedName != ip.ReturnType.FullyQualifiedName) return false;
			}
			
			return true;
		}
		
		string GetTypeInfo(Type type)
		{
			string t = "";
			
			if(type.IsPublic)    t += "public ";
			if(type.IsNestedPrivate) t += "private ";
			if(type.IsNotPublic || type.IsNestedAssembly || type.IsNestedFamANDAssem || type.IsNestedFamORAssem)  t += "internal ";
			if(type.IsNestedFamily || type.IsNestedFamORAssem || type.IsNestedFamANDAssem) t += "protected ";
			if(type.IsAbstract)  t += "abstract ";
			if(type.IsSealed)    t += "sealed ";
			
			if(type.IsValueType && type.IsEnum) {
				t += "enum ";
			} else if(type.IsValueType) {
				t += "struct ";
			} else if(type.IsInterface) {
				t += "interface ";
			} else if(type.IsSubclassOf(typeof(System.Delegate))) {
				t += "delegate ";
			} else {
				t += "class ";
			}
						
			if (type.IsSubclassOf(typeof(System.Delegate))) {
				try {
					if (type.GetMethod("Invoke").ReturnType == typeof(void)) {
						t += "void";
					} else {
						t += GetTypeRef(type.GetMethod("Invoke").ReturnType);
					}
				} catch {}
				t += " " + type.FullName + " (";
				try {
					t += GetParameters(type.GetMethod("Invoke"));
				} catch {}
				t += ")";
			} else {
				
				t += type.FullName;
				
			}
			
			t += "<br><br>" + RT("BaseTypes") + "<br>";
			t += GetBaseTypes(type);
			
			t += "<br>" + RT("Attributes") + "<br>";
			t += GetCustomAttribs(type);
			
			IClass c = parserService.GetClass(type.FullName);
			if(c == null) goto noDoc;
			t += "<br>" + RT("Documentation") + "<br>" + GetDocumentation(c.Documentation) + "<br>";
			
			noDoc:
			
			if (type.Namespace == null || type.Namespace == "") goto inAsm;
			t += "<br>";
			t += GetInNS(type.Assembly, type.Namespace);
			
			inAsm:
			t += "<br><br>";
			t += GetInAsm(type.Assembly);
			
			return t;
		}
		
		string GetResInfo(Assembly asm, string name)
		{
			long size = 0;
			try {
				Stream str = asm.GetManifestResourceStream(name);
				size = str.Length;
				str.Close();
			} catch {}
			
			string text = String.Format(RT("ResInfo"), name, size);
			text += GetInAsm(asm);
			
			text += "<br><br>";
			text += ln(references.Add(new SaveResLink(asm, name)), RT("SaveRes"));
			
			return text;
		}
		
		string GetCustomAttribs(ICustomAttributeProvider ca)
		{
			string text = "";
			
			object[] objs;
			
			objs = ca.GetCustomAttributes(true);
			foreach(object obj in objs) {
				Type type = obj.GetType();
				text += ln(references.Add(type), type.FullName);
				text += " (";
				
				PropertyInfo[] pis = type.GetProperties();
				int cp = 1;
				foreach(PropertyInfo pi in pis) {
					try {
						object val = pi.GetValue(obj, null);
						if (val is string) {
							text += (cp == 1 ? "" : ", ") + pi.Name + " = \"" + val.ToString() + "\"";							
						} else if (val is ValueType) {
							text += (cp == 1 ? "" : ", ") + pi.Name + " = " + val.ToString();
						}
					} catch {}
					cp++;
				}
				text += ")<br>";
			}
			
			return text;
		}
		
		string GetBaseTypes(Type type)
		{
			string text = "";
			
			if (type.BaseType == null) goto interf;
			text = ln(references.Add(type.BaseType), type.BaseType.FullName) + "<br>";
			
			interf:
			Type[] interfaces = type.GetInterfaces();
			foreach(Type interfac in interfaces) {
				text += ln(references.Add(interfac), interfac.FullName) + "<br>";
			}
			
			return text;
		}
		
		string GetParameters(MethodBase mb)
		{
			ParameterInfo[] pars = mb.GetParameters();
			return GetParameters(pars);
		}
		
		string GetParameters(PropertyInfo pi)
		{
			ParameterInfo[] pars = pi.GetIndexParameters();
			return GetParameters(pars);
		}
		
		string GetParameters(ParameterInfo[] pars)
		{
			int parc = pars.Length;
			int cpar = 1;
			
			string text = "";
			
			foreach(ParameterInfo par in pars) {
				text += "<br>&nbsp;&nbsp;&nbsp;&nbsp;";
				
				if (par.ParameterType.Name.EndsWith("&")) text += "ref ";
				if (par.IsOut) text += "out ";
				if (par.ParameterType.IsArray && par.ParameterType != typeof(Array) && Attribute.IsDefined(par, typeof(ParamArrayAttribute), true))	
					text += "params ";
				if (par.IsOptional) text += "optional ";
				
				text += GetTypeRef(par.ParameterType);
				text += " " + par.Name + (cpar == parc ? "<br>" : ",");
				
				cpar++;
			}
			
			return text;
		}
		
		string GetInAsm(Assembly asm)
		{
			string text = RT("ContainedIn") + " ";
			text += ln(references.Add(asm.GetName()), asm.GetName().Name);
			return text;
		}
		
		string GetInNS(Assembly asm, string ns)
		{
			string text = RT("Namespace") + " ";
			text += ln(references.Add(new NamespaceLink(asm, ns)), ns);
			return text;
		}
		
		string GetInType(Type type)
		{
			string text = RT("Type") + " ";
			text += ln(references.Add(type), type.FullName);
			return text;
		}
		
		string GetTypeRef(Type type)
		{
			Type reftype = null;
			
			// if array type or reference type
			if (type.FullName.EndsWith("[]")) reftype = Type.GetType(type.FullName.Substring(0, type.FullName.Length - 2));
			if (type.FullName.EndsWith("&") || type.FullName.EndsWith("*")) reftype = Type.GetType(type.FullName.Substring(0, type.FullName.Length - 1));
			if (reftype == null) reftype = type;
			
			string typefullname    = GetTypeString(type.FullName);
			string reftypefullname = GetTypeString(reftype.FullName);
			
			return ln(references.Add(reftype), reftypefullname) + typefullname.Substring(reftypefullname.Length);
		}

		void back_click(object sender, LinkLabelLinkClickedEventArgs ev)
		{
			try {
				tree.GoBack();
			} catch {}
		}
		
		class SaveResLink
		{
			public Assembly Asm;
			public string Name;
			public SaveResLink(Assembly asm, string name)
			{
				Asm  = asm;
				Name = name;
			}
		}
		
		class NamespaceLink
		{
			public Assembly Asm;
			public string Name;
			public NamespaceLink(Assembly asm, string name)
			{
				Asm  = asm;
				Name = name;
			}
		}
		
		public string GetDocumentation(string doc)
		{
			StringReader reader = new StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
						if (elname == "remarks") {
							ret.Append(RTD("Remarks") + "<br>");
						} else if (elname == "example") {
							ret.Append(RTD("Example") + "<br>");
						} else if (elname == "exception") {
							ret.Append(RTD("Exception") + " " + GetCref(xml["cref"]) + ":<br>");
						} else if (elname == "returns") {
							ret.Append(RTD("Returns") + " ");
						} else if (elname == "see") {
							ret.Append(GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "seealso") {
							ret.Append(RTD("SeeAlso") + " " + GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "paramref") {
							ret.Append(xml["name"]);
						} else if (elname == "param") {
							ret.Append(xml["name"].Trim() + ": ");
						} else if (elname == "value") {
							ret.Append(RTD("Value") + " ");
						}
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
						if (elname == "para" || elname == "param") {
							ret.Append("<br>");
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						ret.Append(whitespace.Replace(xml.Value, " "));
					}
				} while(xml.Read());
			} catch {
				return doc;
			}
			return ret.ToString();
		}
		
		string GetCref(string cref)
		{
			if (cref == null) return "";
			if (cref.Length < 2) return cref;
			if (cref.Substring(1, 1) == ":") return cref.Substring(2, cref.Length - 2);
			return cref;
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
		
		string RT(string ResName)
		{
			return tree.ress.GetString("ObjectBrowser.Info." + ResName);
		}
		
		string RTD(string ResName)
		{
			return tree.ress.GetString("ObjectBrowser.Info.Doc." + ResName);
		}
		
		string ln(int rnr, string text)
		{
			return "<a href='as://" + rnr.ToString() + "'>" + text + "</a>";
		}
		
		bool CreateImage(Image pv)
		{
			try {
				Bitmap b = new Bitmap(170, 170, PixelFormat.Format24bppRgb);
				
				Graphics g = Graphics.FromImage(b);
				g.FillRectangle(new SolidBrush(SystemColors.Control), 0, 0, 170, 170);
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				
				g.DrawImage(pv, 5, 5, 160, 160);
				g.FillRectangle(new SolidBrush(Color.FromArgb(220, SystemColors.Control)), 0, 0, 170, 170);
				g.Dispose();
				
				b.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png);
				return true;
			} catch { return false; }
		}
		
	}
	
	public class GradientLabel : Label
	{
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			g.FillRectangle(new SolidBrush(SystemColors.Control), pe.ClipRectangle);
		
			g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
		                                        SystemColors.ControlLightLight,
		                                        SystemColors.Control),
		                                        new Rectangle(0, 0, Width, Height));
		}
	}
		
}
