// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using onlyconnect;

using Reflector.UserInterface;

namespace HtmlBackendBinding
{
	public class HtmlViewContent : AbstractViewContent, ITextAreaControlProvider, IEditable
	{
		TabControl   tabControl;
		IViewContent htmlTextEditor;
		AxSideTab    tab = null;
		
//		CommandBar    menuToolBar = null;
		
		onlyconnect.HtmlEditor htmlEditor;
		
		// toolbar elements
//		ComboBox fontComboBox = new TextComboBox(true);
		
		
		public TextAreaControl TextAreaControl {
			get {
				return (TextAreaControl)htmlTextEditor.Control;
			}
		}
		
		public IClipboardHandler ClipboardHandler {
			get {
				if (tabControl.SelectedTab.Controls[0] is IEditable) {
					return ((IEditable)tabControl.SelectedTab.Controls[0]).ClipboardHandler;
				}
				return new HtmlEditorClipboardHandler(htmlEditor);
			}
		}
		
		public string Text {
			get {
				return ((IEditable)htmlTextEditor).Text;
			}
			set {
				((IEditable)htmlTextEditor).Text = value;
			}
		}
		
		public override string ContentName {
			get {
				return htmlTextEditor.ContentName;
			}
			set {
				htmlTextEditor.ContentName = value;
			}
		}
			
		public override string UntitledName {
			get {
				return htmlTextEditor.UntitledName;
			}
			set {
				htmlTextEditor.UntitledName = value;
			}
		}
		
		public override Control Control {
			get {
				return tabControl;
			}
		}
		
		// AbstractViewContent members
		public override IWorkbenchWindow WorkbenchWindow {
			set {
				base.WorkbenchWindow = value;
				WorkbenchWindow.WindowSelected   += new EventHandler(SelectMe);
				WorkbenchWindow.WindowDeselected += new EventHandler(DeSelectMe);
			}
		}
		
		public HtmlViewContent(string fileName)
		{
			TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
			htmlTextEditor               = tdb.CreateContentForFile(fileName);
			InitializeComponent();
			ContentName = fileName;
		}
		
		public HtmlViewContent(string languageName, string content)
		{
			TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
			htmlTextEditor               = tdb.CreateContentForLanguage(languageName, content);
			InitializeComponent();
			htmlEditor.HtmlEvent += new HtmlEventHandler(ClickMe);
		}
		
		
		
		void ClickMe(object sender, HtmlEventArgs e)
		{
			string type = e.Event.type;
			
			switch (type) {
				case "click":
					if (SharpDevelopSideBar.SideBar.ActiveTab.ChoosedItem.Tag is Element) {
						Element elem = (Element)SharpDevelopSideBar.SideBar.ActiveTab.ChoosedItem.Tag;
						elem.HtmlElement = htmlEditor.Document.createElement(elem.TagName);
						
						string html = htmlEditor.Document.body.innerHTML + elem.ToString();
						
						htmlEditor.Document.body.innerHTML = html;
//						
//						mshtml.IHTMLDOMNode body = (mshtml.IHTMLDOMNode);
//						body.appendChild((mshtml.IHTMLDOMNode)elem.HtmlElement);
//						Console.WriteLine("done");
						
						tab.ChoosedItem = tab.Items[0];
						SharpDevelopSideBar.SideBar.Refresh();
					}
					break;
			}
		}
		
		public void Undo()
		{
			this.htmlEditor.Document.execCommand("Undo", false, null);
		}
		
		public void Redo()
		{
			this.htmlEditor.Document.execCommand("Redo", false, null);
		}
		
		void SelectMe(object sender, EventArgs e)
		{
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(tab);
			}
			SharpDevelopSideBar.SideBar.ActiveTab = tab;
		}
		
		void DeSelectMe(object sender, EventArgs e)
		{
			SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
		}
		
		void InitializeComponent()
		{
			tabControl = new TabControl();
			tabControl.SelectedIndexChanged += new EventHandler(TabIndexChanged);
			tabControl.Alignment = TabAlignment.Bottom;
			
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			tabControl.ImageList = new ImageList();
			tabControl.ImageList.Images.Add(iconService.GetBitmap("XmlFileIcon"));
			tabControl.ImageList.Images.Add(iconService.GetBitmap("Icons.16x16.DesignPanel"));
			
			
			TabPage sourcePage = new TabPage("Source");
			sourcePage.ImageIndex = 0;
			htmlTextEditor.Control.Dock = DockStyle.Fill;
			htmlTextEditor.DirtyChanged += new EventHandler(TextAreaIsDirty);
			sourcePage.Controls.Add(htmlTextEditor.Control);
			tabControl.TabPages.Add(sourcePage);
			
			TabPage designerPage = new TabPage("Design");
			designerPage.ImageIndex = 1;
			htmlEditor = new onlyconnect.HtmlEditor();
			htmlEditor.IsDesignMode = true;
			htmlEditor.Dock = DockStyle.Fill;
			htmlEditor.TabIndex = 7;
			htmlEditor.LoadDocument(Text);
			designerPage.Controls.Add(htmlEditor);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ICSharpCode.Core.Services.IResourceService));
			
//			menuToolBar = new ToolBarEx();
//			
//			ToolBarItem separator = new ToolBarItem();
//			separator.Style = ToolBarItemStyle.Separator;
//			
//			fontComboBox.Width = 150;
//			foreach (FontFamily fontFamily in new InstalledFontCollection().Families) {
//				fontComboBox.Items.Add(fontFamily.Name);
//			}
//			fontComboBox.SelectedIndexChanged += new EventHandler(FontComboBoxIndexChanged);
//			
//			TextComboBox sizeComboBox = new TextComboBox(true);
//			sizeComboBox.Width = 50;
//			for (int i = 1; i <= 7; ++i) {
//				sizeComboBox.Items.Add(i.ToString());
//			}
//			sizeComboBox.SelectedIndexChanged += new EventHandler(SizeComboBoxIndexChanged);
//			
//			menuToolBar.Items.AddRange(new ToolBarItem[] {
//				new ToolBarItem(ToolBarItemStyle.ComboBox, fontComboBox),
//				new ToolBarItem(ToolBarItemStyle.ComboBox, sizeComboBox),
//				separator,
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.AdjustColor"), new EventHandler(OnSetForeColorToolBarButton), Keys.None, "Set Foreground Color"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.AdjustBackgroundColor"), new EventHandler(OnSetBackColorToolBarButton), Keys.None, "Set Background Color"),
//				separator,
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.BoldText"), new EventHandler(OnBoldToolBarButton), Keys.None, "Bold"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.ItalicText"), new EventHandler(OnItalicToolBarButton), Keys.None, "Italic"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.UnderlineText"), new EventHandler(OnUnderlineToolBarButton), Keys.None, "Underline"),
//				separator,
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.Left"), new EventHandler(OnLeftToolBarButton), Keys.None, "Left"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.Center"), new EventHandler(OnCenterToolBarButton), Keys.None, "Center"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.Right"), new EventHandler(OnRightToolBarButton), Keys.None, "Right"),
//				separator,
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.OutDent"), new EventHandler(OnUnindentToolBarButton), Keys.None, "Unindent"),
//				new ToolBarItem(resourceService.GetBitmap("Icons.16x16.Indent"),  new EventHandler(OnIndentToolBarButton), Keys.None, "Indent"),
//				separator
//			});
//			
//			ReBar reBar = new ReBar();
//			reBar.Bands.Add(menuToolBar);
//			designerPage.Controls.Add(reBar);
//			
			tabControl.TabPages.Add(designerPage);
			
			
			// ADD #D TAB PAGE
			tab = new AxSideTab(SharpDevelopSideBar.SideBar, "Forms");
			tab.CanSaved = false;
			
			{
				ToolboxItem toolboxItem = new ToolboxItem();
				AxSideTabItem tabItem = SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Pointer", null);
				tabItem.Icon = new Bitmap(iconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
				tab.Items.Add(tabItem);
			}
			
			
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Label", new LabelElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.LabelElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Text Box", new InputTextElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputTextElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Text Area", new TextAreaElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.TextAreaElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Password", new InputPasswordElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputPasswordElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Button", new InputButtonElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputButtonElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Submit Button", new InputSubmitElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputSubmitElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Reset Button", new InputResetElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputResetElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Image Button", new InputImageElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputImageElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Check Box", new InputCheckBoxElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputCheckBoxElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Radio Button", new InputRadioElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputRadioElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Combo Box", new ComboBoxElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputCheckBoxElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("List Box", new ListBoxElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.ListBoxElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Hidden Field", new InputHiddenElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputHiddenElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("File Upload", new InputFileElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.InputFileElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Group Box", new GroupBoxElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.FieldSetElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Anchor", new AnchorElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.AnchorElement")));			
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Image", new ImageElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.ImageElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Table", new TableElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.TableElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Span", new SpanElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.SpanElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Div", new DivElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.DivElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Panel", new PanelElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.PanelElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("IFrame", new IFrameElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.IFrameElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Horizontal Rule", new HRulerElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.HorizontalRuleElement")));
			tab.Items.Add(SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Form", new FormElement(), resourceService.GetBitmap("Icons.16x16.HtmlElements.FormElement")));
			
			SharpDevelopSideBar.SideBar.Tabs.Add(tab);
		}
		
		void FontComboBoxIndexChanged(object sender, EventArgs e)
		{
//			TextComboBox fontComboBox = sender as TextComboBox;
//			if (fontComboBox != null) {
//				this.htmlEditor.Document.execCommand("FontName", false, fontComboBox.Text);
//			}
		}
		
		void SizeComboBoxIndexChanged(object sender, EventArgs e)
		{
//			TextComboBox sizeComboBox = sender as TextComboBox;
//			if (sizeComboBox != null) {
//				this.htmlEditor.Document.execCommand("FontSize", false, sizeComboBox.Text);
//			}
		}
		
		void OnSetForeColorToolBarButton(object sender, EventArgs e)
		{
			string fg = GetColorString();
			if (fg != null) {
				this.htmlEditor.Document.execCommand("forecolor", false, fg);
			}
		}
		
		void OnSetBackColorToolBarButton(object sender, EventArgs e)
		{
			string bg = GetColorString();
			if (bg != null) {
				this.htmlEditor.Document.execCommand("backcolor", false, bg);
			}
		}
		
		void OnBoldToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("bold", false, null);
		}
		
		void OnItalicToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("italic", false, null);
		}
		
		void OnUnderlineToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("underline", false, null);
		}
		
		void OnLeftToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("JustifyLeft", false, null);
		}
		
		void OnCenterToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("JustifyCenter", false, null);
		}
		
		void OnRightToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("JustifyRight", false, null);
		}

		void OnIndentToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Indent", false, null);
		}
		void OnUnindentToolBarButton(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Outdent", false, null);
		}
		
		string GetColorString()
		{
			using (ColorDialog cd = new ColorDialog()) {
				if (cd.ShowDialog() == DialogResult.OK) {
					string colorstr = "#" + cd.Color.ToArgb().ToString("X").Substring(2);
					return colorstr;
				}
				return null;
			}
		}
		
		void TextAreaIsDirty(object sender, EventArgs e)
		{
			base.IsDirty = htmlTextEditor.IsDirty;
		}
		
		void TabIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex) {
				case 0:
					Text = htmlEditor.GetDocumentSource();
					break;
				case 1:
					htmlEditor.LoadDocument(Text);
					break;
			}
		}
		
		public override void Save(string fileName)
		{
			ContentName = fileName;
			htmlTextEditor.Save(fileName);
		}
		
		public override void Load(string fileName)
		{
			ContentName = fileName;
			htmlTextEditor.Load(fileName);
		}
		
		public override void Save()
		{
			Save(ContentName);
		}
				
	}
}
