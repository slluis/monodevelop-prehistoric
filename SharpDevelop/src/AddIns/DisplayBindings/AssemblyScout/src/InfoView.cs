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

using ICSharpCode.SharpDevelop.Gui.HtmlControl;

using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.Services;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class InfoView : UserControl
	{
		GradientLabel cap = new GradientLabel();
		Label typ = new Label();
		LinkLabel back = new LinkLabel();
		
		HtmlControl ht = new HtmlControl();
		Panel pan = new Panel();
		
		AssemblyTree tree;
		IParserService parserService;
		IAmbience ambience;
		PropertyService propertyService;
		
		ArrayList references = new ArrayList();
		string cssPath;
		string imgPath;
		string resPath;
		
		public InfoView(AssemblyTree tree)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));			
			
			ambience = ambienceService.CurrentAmbience;
			
			this.tree = tree;
			
			imgPath = Path.Combine(propertyService.ConfigDirectory, "tempPicture.png");
			cssPath = Path.Combine(propertyService.ConfigDirectory, "tempStylesheet.css");
			resPath = Path.Combine(propertyService.ConfigDirectory, "tempResource");
			
			Color col = SystemColors.Control;
			string color = "#" + col.R.ToString("X") + col.G.ToString("X") + col.B.ToString("X");
			
			StreamWriter sw = new StreamWriter(cssPath, false);
			sw.Write(@"body { margin: 0px; border: 0px; overflow: hidden; padding: 0px;
							  background-color: " + color + @"; 
							  background-image: url(" + imgPath + @"); 
							  background-position: bottom right; 
							  background-repeat: no-repeat }
							  
					p   { font: 8pt Tahoma }
					div { margin: 0px; width: 100% }
					p.bottomline { font: 8pt Tahoma; border-bottom: 1px solid black; margin-bottom: 3px }
					p.docmargin  { font: 8pt Tahoma; margin-top: 0px; margin-bottom: 0px; padding-left: 8px; padding-right: 8px; padding-bottom: 3px; border-bottom: 1px solid black }

					a   { font: 8pt Tahoma; text-decoration: none; color: blue}
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
			
			ht = new HtmlControl();
			//ht.Size = new Size(20, 20);
			//ht.Location = new Point(20, 20);
			ht.BeforeNavigate += new BrowserNavigateEventHandler(HtmlControlBeforeNavigate);
			CreateImage(resourceService.GetIcon("Icons.16x16.Class").ToBitmap());
			ht.CascadingStyleSheet = cssPath;
			string html = RenderHead() + tree.ress.GetString("ObjectBrowser.Info.SelectNode") + RenderFoot();
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
		
		~InfoView() {
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
				
				if (obj is IClass) {
					// Go To Type
					tree.GoToType((IClass)obj);
					// try {
					// 	tree.SelectedNode.Expand();
					// } catch {}
				} else if (obj is AssemblyTree.RefNodeAttribute) {
					// Open Assembly Reference
					tree.OpenAssemblyByName((AssemblyTree.RefNodeAttribute)obj);
				} else if (obj is SaveResLink) {
					SaveResLink link = (SaveResLink)obj;
					tree.SaveResource(link.Asm, link.Name);
				} else if (obj is NamespaceLink) {
					NamespaceLink ns = (NamespaceLink)obj;
					tree.GoToNamespace(ns.Asm, ns.Name);
				}
			} catch { 
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError("Something failed following this link.");
			}
		}
		
		string RenderHead()
		{
			return "<div><p>";
		}
		
		string RenderFoot()
		{
			return "</div>";
		}
				
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			cap.Text = node.Text;
			typ.Text = node.Type.ToString();
			
			references.Clear();
			
			ambience.LinkArrayList = references;
			ambience.ConversionFlags = ConversionFlags.AssemblyScoutDefaults;
						
			CreateImage(tree.ImageList.Images[node.ImageIndex]);
			ht.Cursor = Cursors.Default;
			string html = RenderHead();
			
			try {
			
				switch(node.Type) {
					case NodeType.Assembly:
						html += GetAssemblyInfo((SA.SharpAssembly)node.Attribute);
						break;
					case NodeType.Library:
						html += GetLibInfo((SA.SharpAssembly)node.Attribute);
						break;
					case NodeType.Reference:
						html += GetRefInfo((AssemblyTree.RefNodeAttribute)node.Attribute);
						break;
					case NodeType.Resource:
						html += GetResInfo((SA.SharpAssembly)node.Attribute, node.Text);
						break;
					case NodeType.SingleResource:
						html += GetSingleResInfo(node.Attribute, node.Text);
						break;
					case NodeType.Folder:
						html += GetFolderInfo(node.Text);
						break;
					case NodeType.Namespace:
						html += GetNSInfo((SA.SharpAssembly)node.Attribute);
						break;
					case NodeType.SubTypes:
						html += GetSubInfo();
						break;
					case NodeType.SuperTypes:
						html += GetSuperInfo();
						break;
					case NodeType.Link:
						html += GetLinkInfo((IClass)node.Attribute);
						break;
					case NodeType.Type:
						html += GetTypeInfo((IClass)node.Attribute);
						break;
					case NodeType.Event:
						html += GetEventInfo((IEvent)node.Attribute);
						break;
					case NodeType.Field:
						html += GetFieldInfo((IField)node.Attribute);
						break;
					case NodeType.Constructor:
					case NodeType.Method:
						html += GetMethodInfo((IMethod)node.Attribute);
						break;
					case NodeType.Property:
						html += GetPropInfo((IProperty)node.Attribute);
						break;
					default:
						break;
				}
			} catch(Exception ex) {
				html += "<p class='bottomline'>" + tree.ress.GetString("ObjectBrowser.Info.CollectError") + "<p>" + ex.ToString().Replace("\n", "<br>");
			}
			
			html += RenderFoot();
			
			ht.Html = html;
		}
		
		string GetLinkInfo(IClass type)
		{
			string text;
			text = ln(references.Add(type), RT("GotoType"));
			text += "<p>" + RT("LinkedType") + " ";
			return text + GetInAsm((SA.SharpAssembly)type.DeclaredIn);
		}
		
		string GetSubInfo()
		{
			return RT("SubInfo");
		}
		
		string GetSuperInfo()
		{
			return RT("SuperInfo");
		}
		
		string GetNSInfo(SA.SharpAssembly asm)
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
		
		string GetLibInfo(SA.SharpAssembly asm)
		{
			return RT("LibInfo") + "<p>" + GetInAsm(asm);
		}
		
		string GetRefInfo(AssemblyTree.RefNodeAttribute asn)
		{
			string text = String.Format(RT("RefInfo"), asn.RefName.Name, asn.RefName.FullName, asn.RefName.Version.ToString(), "");
			return text + ln(references.Add(asn), RT("OpenRef"));
		}
		
		string GetResInfo(SA.SharpAssembly asm, string name)
		{
			long size = 0;
			try {
				size = asm.GetManifestResourceSize(name);
			} catch {}
			
			string text = String.Format(RT("ResInfo"), name, size);
			text += GetInAsm(asm);
			
			text += "<p>";
			text += ln(references.Add(new SaveResLink(asm, name)), RT("SaveRes"));
			
			if (propertyService.GetProperty("AddIns.AssemblyScout.ShowResPreview", true) == false) return text;
			
			try {
				if (name.ToLower().EndsWith(".bmp") || name.ToLower().EndsWith(".gif") 
					|| name.ToLower().EndsWith(".png") || name.ToLower().EndsWith(".jpg")) {
					byte[] res = asm.GetManifestResource(name);
					FileStream fstr = new FileStream(resPath, FileMode.Create);
					BinaryWriter wr = new BinaryWriter(fstr);
					wr.Write(res);
					fstr.Close();
					
					text += "<p>Preview:<p>";
					text += "<img src=\"" + resPath + "\">";
				}
				if (name.ToLower().EndsWith(".tif")) {
					byte[] res = asm.GetManifestResource(name);
					Image tifImg = Image.FromStream(new MemoryStream(res));
					tifImg.Save(resPath, ImageFormat.Bmp);
					
					text += "<p>Preview:<p>";
					text += "<img src=\"" + resPath + "\">";					
				}
				if (name.ToLower().EndsWith(".ico")) {
					byte[] res = asm.GetManifestResource(name);
					Icon icon = new Icon(new MemoryStream(res));
					Bitmap b = new Bitmap(icon.Width, icon.Height);
					
					Graphics g = Graphics.FromImage(b);
					g.FillRectangle(new SolidBrush(SystemColors.Control), 0, 0, b.Width, b.Height);
					g.DrawIcon(icon, 0, 0);
					            
					b.Save(resPath, System.Drawing.Imaging.ImageFormat.Png);
					
					text += "<p>Preview:<p>";
					text += "<img src=\"" + resPath + "\">";					
				}
				if (name.ToLower().EndsWith(".cur")) {
					byte[] res = asm.GetManifestResource(name);
					Cursor cursor = new Cursor(new MemoryStream(res));
					
					Bitmap b = new Bitmap(cursor.Size.Width, cursor.Size.Height);
					
					Graphics g = Graphics.FromImage(b);
					g.FillRectangle(new SolidBrush(SystemColors.Control), 0, 0, b.Width, b.Height);
					cursor.Draw(g, new Rectangle(0, 0, 32, 32));
					            
					b.Save(resPath, System.Drawing.Imaging.ImageFormat.Png);
					
					text += "<p>Preview:<p>";
					text += "<img src=\"" + resPath + "\">";					
				}
				if (name.ToLower().EndsWith(".txt") || name.ToLower().EndsWith(".xml") ||
				    name.ToLower().EndsWith(".xsd") || name.ToLower().EndsWith(".htm") ||
				    name.ToLower().EndsWith(".html") || name.ToLower().EndsWith(".xshd") ||
				    name.ToLower().EndsWith(".xsl") || name.ToLower().EndsWith(".txt")) {
					byte[] res = asm.GetManifestResource(name);
					string utf = System.Text.UTF8Encoding.UTF8.GetString(res);
					
					text += "<p>Preview:<br>";
					text += "<textarea style='border: 1px solid black; width: 300px; height: 400px; font: 8pt Tahoma'>";
					text += utf;
					text += "</textarea>";
				}
			} catch {}
			
			return text;
		}
		
		string GetSingleResInfo(object singleRes, string name)
		{
			int len = name.Length;
			if (name.LastIndexOf(":") != -1) len = name.LastIndexOf(":");
			string ret = "Name: " + name.Substring(0, len) + "<p>";
			
			if (singleRes != null) {
				ret += "Type: " + singleRes.GetType().Name + "<p>";
				ret += "Value:<br>" + singleRes.ToString();
			}
			
			return ret;
		}
		
		string GetAssemblyInfo(SA.SharpAssembly asm)
		{
			string text = String.Format(RT("AsmInfo"),
			                        asm.Name, asm.FullName, asm.GetAssemblyName().Version.ToString(),
			                        asm.Location, asm.FromGAC);
			text += GetCustomAttribs(asm);
			return text;
		}
		
		string GetEventInfo(IEvent info)
		{
			string ret = ambience.Convert(info);
			
			ret += "<p>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IEvent e in c.Events) {
				if(e.Name == info.Name) {
					if (e.Documentation == null || e.Documentation == "") continue;
					ret += "<p class='bottomline'>" + RT("Documentation") + "<p class='docmargin'>" + GetDocumentation(e.Documentation) + "<p>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<p>";
			ret += GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn);
			
			return ret;
		}
		
		string GetFieldInfo(IField info)
		{
			string ret = ambience.Convert(info);
			
			if (info is SharpAssemblyField) {
				SharpAssemblyField saField = info as SharpAssemblyField;
				if (saField.InitialValue != null) {
					ret += " = " + saField.InitialValue.ToString();
				}
			}
			
			ret += "<p>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IField f in c.Fields) {
				if(f.Name == info.Name) {
					if (f.Documentation == null || f.Documentation == "") continue;
					ret += "<p class='bottomline'>" + RT("Documentation") + "<p class='docmargin'>" + GetDocumentation(f.Documentation) + "<p>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>" + GetInType(info.DeclaringType);
			ret += "<p>" + GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn);
			
			return ret;
		}
		
		string GetMethodInfo(IMethod info)
		{
			string ret = ambience.Convert(info);
			
			ret += "<p>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IMethod cc in c.Methods) {
				if (cc.Name == info.Name) {
					if (cc.Documentation == null || cc.Documentation == "") continue;
					ret += "<p class='bottomline'>" + RT("Documentation") + "<p class='docmargin'>" + GetDocumentation(cc.Documentation) + "<p>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<p>";
			ret += GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn);
			
			return ret;			
		}
		
		string GetPropInfo(IProperty info)
		{
			string ret = ambience.Convert(info);
			
			ret += "<p>" + RT("Attributes") + "<br>";
			ret += GetCustomAttribs(info);
			
			IClass c = parserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IProperty p in c.Properties) {
				if(p.Name == info.Name) {
					if (p.Documentation == null || p.Documentation == "") continue;
					ret += "<p class='bottomline'>" + RT("Documentation") + "<p class='docmargin'>" + GetDocumentation(p.Documentation) + "<p>";
					break;
				}
			}
						
			noDoc:
			
			ret += "<br>";
			ret += GetInType(info.DeclaringType);
			ret += "<p>";
			ret += GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn);
			
			return ret;
		}
		
		string GetTypeInfo(IClass type)
		{
			string t = ambience.Convert(type);
			
			t += "<p>" + RT("BaseTypes") + "<br>";
			t += GetBaseTypes(type as SharpAssemblyClass);
			
			t += "<br>" + RT("Attributes") + "<br>";
			t += GetCustomAttribs(type);
			
			IClass c = parserService.GetClass(type.FullyQualifiedName.Replace("+", "."));
			if (c == null) goto noDoc;
			if (c.Documentation == null || c.Documentation == "") goto noDoc;
			t += "<p class='bottomline'>" + RT("Documentation") + "<p class='docmargin'>" + GetDocumentation(c.Documentation) + "<p>";
			
			noDoc:
			
			if (type.Namespace == null || type.Namespace == "") goto inAsm;
			t += "<br>";
			t += GetInNS((SA.SharpAssembly)type.DeclaredIn, type.Namespace);
			
			inAsm:
			t += "<p>";
			t += GetInAsm((SA.SharpAssembly)type.DeclaredIn);
			
			return t;
		}
		
		string GetCustomAttribs(SA.SharpAssembly assembly)
		{
			return GetCustomAttribs(SharpAssemblyAttribute.GetAssemblyAttributes(assembly));
		}
		
		string GetCustomAttribs(IClass type)
		{
			if (type.Attributes.Count == 0) return "";
			return GetCustomAttribs(type.Attributes[0].Attributes);
		}
		
		string GetCustomAttribs(IMember member)
		{
			if (member.Attributes.Count == 0) return "";
			return GetCustomAttribs(member.Attributes[0].Attributes);
		}
		
		string GetCustomAttribs(AttributeCollection ca)
		{
			string text = "";
			
			try {
			
				foreach(SharpAssemblyAttribute obj in ca) {
					text += ln(references.Add(obj.AttributeType), obj.Name);
					
					string attrString = obj.ToString();
					text += attrString.Substring(obj.Name.Length)+ "<br>";
	
				}
			} catch {
				text = "An error occured while looking for attributes.<br>";
			}
			return text;
		}
		
		string GetBaseTypes(SharpAssemblyClass type)
		{
			string text = "";
			
			if (type == null) return "";
			if (type.BaseTypeCollection.Count == 0) return "";
			
			foreach (SharpAssemblyClass basetype in type.BaseTypeCollection) {
				text += ln(references.Add(basetype), basetype.FullyQualifiedName) + "<br>";
			}

			return text;
		}

		string GetInAsm(SA.SharpAssembly asm)
		{
			string text = RT("ContainedIn") + " ";
			text += ln(references.Add(new AssemblyTree.RefNodeAttribute(asm, asm.GetAssemblyName())), asm.Name);
			return text;
		}
		
		string GetInNS(SA.SharpAssembly asm, string ns)
		{
			string text = RT("Namespace") + " ";
			text += ln(references.Add(new NamespaceLink(asm, ns)), ns);
			return text;
		}
		
		string GetInType(IClass type)
		{
			string text = RT("Type") + " ";
			text += ln(references.Add(type), type.FullyQualifiedName);
			return text;
		}

		void back_click(object sender, LinkLabelLinkClickedEventArgs ev)
		{
			try {
				tree.GoBack();
			} catch {}
		}
		
		class SaveResLink
		{
			public SA.SharpAssembly Asm;
			public string Name;
			public SaveResLink(SA.SharpAssembly asm, string name)
			{
				Asm  = asm;
				Name = name;
			}
		}
		
		class NamespaceLink
		{
			public SA.SharpAssembly Asm;
			public string Name;
			public NamespaceLink(SA.SharpAssembly asm, string name)
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
							ret.Append("<b>" + RTD("Remarks") + "</b><br>");
						} else if (elname == "example") {
							ret.Append("<b>" + RTD("Example") + "</b><br>");
						} else if (elname == "exception") {
							ret.Append("<b>" + RTD("Exception") + "</b> " + GetCref(xml["cref"]) + ":<br>");
						} else if (elname == "returns") {
							ret.Append("<b>" + RTD("Returns") + "</b> ");
						} else if (elname == "see") {
							ret.Append(GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "seealso") {
							ret.Append("<b>" + RTD("SeeAlso") + "</b> " + GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "paramref") {
							ret.Append("<i>" + xml["name"] + "</i>");
						} else if (elname == "param") {
							ret.Append("<i>" + xml["name"].Trim() + "</i>: ");
						} else if (elname == "value") {
							ret.Append("<b>" + RTD("Value") + "</b> ");
						} else if (elname == "summary") {
							ret.Append("<b>" + RTD("Summary") + "</b> ");
						}
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
						if (elname == "para" || elname == "param") {
							ret.Append("<br>");
						}
						if (elname == "exception") {
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
			if (cref.Substring(1, 1) == ":") return "<u>" + cref.Substring(2, cref.Length - 2) + "</u>";
			return "<u>" + cref + "</u>";
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
