// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using ICSharpCode.SharpDevelop.Gui.Pads;

using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;
using ICSharpCode.SharpDevelop.FormDesigner.Gui;

using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public abstract class FormDesignerDisplayBindingBase : AbstractViewContent, IEditable, IClipboardHandler
	{
		protected DesignPanel         designPanel       = null;
		protected DefaultDesignerHost host;
		protected bool                isFormDesignerVisible;
		protected UndoHandler         undoHandler       = new UndoHandler();
		AmbientProperties             ambientProperties = new AmbientProperties();
		
		public virtual TextAreaControl TextAreaControl {
			get {
				return null;
			}
		}
		
		public bool IsFormDesignerVisible {
			get {
				return isFormDesignerVisible;
			}
		}
		
		// IEditable
		public virtual IClipboardHandler ClipboardHandler {
			get {
				return this;
			}
		}
		
		public virtual string Text {
			get {
				return null;
			}
			set {
			}
		}
		
		public IDesignerHost DesignerHost {
			get {
				return host;
			}
		}
		
		public bool EnableCut {
			get {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				return selectionService.SelectionCount >= 0;
			}
		}
		
		public bool EnableCopy {
			get {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				return selectionService.SelectionCount >= 0;
			}
		}
		
		public bool EnablePaste {
			get {
				return true;
			}
		}
		
		public bool EnableDelete {
			get {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				return selectionService.SelectionCount >= 0;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			}
		}
		
		// AbstractViewContent members
		protected override void OnWorkbenchWindowChanged(EventArgs e)
		{
			base.OnWorkbenchWindowChanged(e);
			if (WorkbenchWindow != null) {
				WorkbenchWindow.WindowSelected   += new EventHandler(SelectMe);
				WorkbenchWindow.WindowDeselected += new EventHandler(DeSelectMe);
			}
		}
		
		int lastSideTab    = Int32.MaxValue;
		int defaultSideTab = 0;
		protected void SelectMe(object sender, EventArgs e)
		{
			if (!isFormDesignerVisible) {
				return;
			}
			defaultSideTab  = SharpDevelopSideBar.SideBar.Tabs.IndexOf(SharpDevelopSideBar.SideBar.ActiveTab);
			foreach(AxSideTab tab in ToolboxProvider.SideTabs) {
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					SharpDevelopSideBar.SideBar.Tabs.Add(tab);
				}
			}
			if (ToolboxProvider.SideTabs.Count > 0) {
				lastSideTab = Math.Max(Math.Min(lastSideTab, SharpDevelopSideBar.SideBar.Tabs.Count - 1), 0);
				SharpDevelopSideBar.SideBar.ActiveTab = (AxSideTab)SharpDevelopSideBar.SideBar.Tabs[lastSideTab];
			}
//			SharpDevelopSideBar.SideBar.ActiveTab = tab;
			
			ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetDesignerHost(host);
			
			// set old selection
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			if (oldSelectedComponents != null) {
				ArrayList newSelection = new ArrayList();
				foreach (string name in oldSelectedComponents) {
					newSelection.Add(host.Container.Components[name]);
				}
				selectionService.SetSelectedComponents(newSelection);
				oldSelectedComponents = null;
			}
			
			if (selectionService.SelectionCount == 0) {
				SelectRootComponent();
			}

			
			UpdateSelectableObjects();
			designPanel.Enable();
		}
		ArrayList oldSelectedComponents = null;
		
		protected void DeSelectMe(object sender, EventArgs e)
		{
			if (oldSelectedComponents != null) {
				return;
			}
			try {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				oldSelectedComponents = new ArrayList();
				foreach (IComponent component in selectionService.GetSelectedComponents()) {
					oldSelectedComponents.Add(component.Site.Name);
				}
				
				ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetDesignableObject(null);
				ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetSelectableObjects(null);
				int oldTab = SharpDevelopSideBar.SideBar.Tabs.IndexOf(SharpDevelopSideBar.SideBar.ActiveTab);
				
				foreach(AxSideTab tab in ToolboxProvider.SideTabs) {
					if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
						return;
					}
					SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
				}
				lastSideTab = oldTab;
				defaultSideTab = Math.Max(Math.Min(defaultSideTab, SharpDevelopSideBar.SideBar.Tabs.Count - 1), 0);
				SharpDevelopSideBar.SideBar.ActiveTab = (AxSideTab)SharpDevelopSideBar.SideBar.Tabs[defaultSideTab];
	//			SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
				designPanel.Disable();
				ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.RemoveHost(host);
			} catch (Exception ex) {
				Console.WriteLine("Got exception while deselection:" + ex);
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false; // FIXME
			}
		}
		
		public virtual void Reload()
		{
		}
		
		public override void RedrawContent()
		{
//			((DefaultDesignerHost)DesignerHost).Reload();
		}
		
		public override void Dispose()
		{
			DesignerResourceService designerResourceService = (DesignerResourceService)host.GetService(typeof(System.ComponentModel.Design.IResourceService));
			designerResourceService.SaveResources();
			designPanel.Dispose();
		}
		
		protected string GetDataAs(string what)
		{
			StringWriter writer = new StringWriter();
			switch (what) {
				case "XML":
					XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), host);
					
					XmlDocument doc = new XmlDocument();
					doc.LoadXml("<" + el.Name + " version=\"1.0\"/>");
					
					foreach (XmlNode node in el.ChildNodes) {
						doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
					}
					
					doc.Save(writer);
					break;
				case "C#":
					new CodeDOMGenerator(host, new Microsoft.CSharp.CSharpCodeProvider()).ConvertContentDefinition(writer);
					break;
				case "VB.NET":
					new CodeDOMGenerator(host, new Microsoft.VisualBasic.VBCodeProvider()).ConvertContentDefinition(writer);
					break;
			}
			return writer.ToString();
		}
		
		public override void Save(string fileName)
		{
			ContentName   = fileName;
			XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), host);
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<" + el.Name + " version=\"1.0\"/>");
			
			foreach (XmlNode node in el.ChildNodes) {
				doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
			}
			switch (Path.GetExtension(fileName)) {
				case ".cs":
					new CodeDOMGenerator(host, new Microsoft.CSharp.CSharpCodeProvider()).ConvertContentDefinition(fileName);
					break;
				case ".vb":
					new CodeDOMGenerator(host, new Microsoft.VisualBasic.VBCodeProvider()).ConvertContentDefinition(fileName);
					break;
				default:
					doc.Save(fileName);
					break;
			}
			IsDirty = false;
		}
		/*
		// todo : complete CodeDOM writing
		void SaveCSharp(string fileName, XmlDocument doc)
		{
			
			
			
			foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
				if (node.Attributes["value"] != null) {
				}
			}
			
			co.Members.Add(cm);
			
			
			StreamWriter writer = File.CreateText(fileName);
			cg.GenerateCodeFromNamespace(cnamespace, writer, null);
			writer.Close();
		}*/
		
		public override void Load(string fileName)
		{
			
		}
		
		public virtual void Initialize()
		{
			host   = new DefaultDesignerHost();
			ComponentChangeService changeService = new ComponentChangeService();
			host.AddService(typeof(System.ComponentModel.Design.IComponentChangeService),      changeService);
			host.AddService(typeof(System.Windows.Forms.Design.IUIService),                    new UIService());
			host.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService),       new ICSharpCode.SharpDevelop.FormDesigner.Services.DesignerOptionService());
			host.AddService(typeof(System.ComponentModel.Design.ITypeDescriptorFilterService), new TypeDescriptorFilterService());
			
			host.AddService(typeof(System.Drawing.Design.IToolboxService), ToolboxProvider.ToolboxService);
//			host.AddService(typeof(System.Drawing.Design.IToolboxService), new ToolboxService());
			host.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			
			ExtenderService extenderService = new ExtenderService();
			host.AddService(typeof(System.ComponentModel.Design.IExtenderListService),    extenderService);
			host.AddService(typeof(System.ComponentModel.Design.IExtenderProviderService),extenderService);
			
			host.AddService(typeof(System.ComponentModel.Design.IDesignerHost),        host);
			host.AddService(typeof(System.ComponentModel.IContainer),                  host.Container);
			host.AddService(typeof(System.ComponentModel.Design.IDictionaryService),   new DictionaryService());
			host.AddService(typeof(System.ComponentModel.Design.IEventBindingService), new EventBindingService(host));
			host.AddService(typeof(System.ComponentModel.Design.ISelectionService),    new SelectionService(host));
			
			host.AddService(typeof(AmbientProperties),                                                ambientProperties);
			host.AddService(typeof(System.ComponentModel.Design.Serialization.INameCreationService),  new NameCreationService(host));
			host.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
			host.AddService(typeof(System.ComponentModel.Design.Serialization.IDesignerSerializationService), new DesignerSerializationService(host));
			host.AddService(typeof(System.ComponentModel.Design.IResourceService), new DesignerResourceService(host));
			host.AddService(typeof(ITypeResolutionService), ToolboxProvider.TypeResolutionService);
			host.AddService(typeof(System.ComponentModel.Design.IReferenceService), new ReferenceService(host));
			
			host.AddService(typeof(IHelpService), new HelpService());
			host.AddService(typeof(IDesignerSerializationManager), new CodeDomDesignerSerializetionManager(host));
			host.AddService(typeof(System.Windows.Forms.Design.IMenuEditorService), new MenuEditorService(host));
			
//			host.AddService(typeof(PropertyGrid), PropertyPad.Grid);
			InitializeExtendersForProject(host);
			host.Activate();
			
			host.TransactionClosed += new DesignerTransactionCloseEventHandler(TransactionFinished);
			
			if (designPanel == null) {
				designPanel = new DesignPanel(host);
			} else {
				designPanel.Host = host;
			}
			
			host.AddService(typeof(System.ComponentModel.Design.IMenuCommandService),  new MenuCommandService(host, designPanel));
			
			host.AddService(typeof(DesignPanel), designPanel);
			designPanel.Location = new Point(0, 0);
			designPanel.Dock     = DockStyle.Fill;
			
//			SelectMe(this, EventArgs.Empty);
			ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetDesignerHost(host);
			
			undoHandler.Attach(host);
		}
		
		// IDisplayBinding interface
		public void InitializeFrom(string fileName, string xmlContent)
		{
			Initialize();
			
			XmlFormReader xmlReader = new XmlFormReader(host);
			xmlReader.SetUpDesignerHost(xmlContent);
			
			ContentName = fileName;
			designPanel.SetRootDesigner();
		}
		
		protected virtual void DeselectAllComponents()
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			selectionService.SetSelectedComponents(new object[] {});
		}
		
		protected virtual void SelectRootComponent()
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			selectionService.SetSelectedComponents(new object[] {host.RootComponent});
		}
	
		public virtual void Undo()
		{
			undoHandler.Undo();
		}
		
		public virtual void Redo()
		{
			undoHandler.Redo();
		}
		
		public void Cut(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Cut);
		}
		
		public void Copy(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Copy);
		}
		
		public void Paste(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Paste);
		}
		
		public void Delete(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Delete);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
		}
		
		protected void UpdateSelectableObjects()
		{
			if (host != null) {
				PropertyPad.SetSelectableObjects(host.Container.Components);
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetDesignableObject(selectionService.PrimarySelection);
			}
		}
		
		void TransactionFinished(object sender, DesignerTransactionCloseEventArgs e)
		{
			IsDirty = true;
		}
		
		void InitializeExtendersForProject(IDesignerHost host)
		{
			IExtenderProviderService elsi = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
			
			elsi.AddExtenderProvider(new NameExtender());
		}
		
		/// <summary>
		/// This is used for letting the View Code command work.
		/// </summary>
		public virtual void ShowSourceCode()
		{
		}
		public virtual void ShowSourceCode(int lineNumber)
		{
		}
		public virtual void ShowSourceCode(IComponent component, EventDescriptor e, string methodName)
		{
		}
		public virtual ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return new string[]{};
		}
		
		protected virtual void OnEnableCutChanged(EventArgs e)
		{
			if (EnableCutChanged != null) {
				EnableCutChanged(this, e);
			}
		}
		
		protected virtual void OnEnableCopyChanged(EventArgs e)
		{
			if (EnableCopyChanged != null) {
				EnableCopyChanged(this, e);
			}
		}
		
		protected virtual void OnEnablePasteChanged(EventArgs e)
		{
			if (EnablePasteChanged != null) {
				EnablePasteChanged(this, e);
			}
		}
		
		protected virtual void OnEnableDeleteChanged(EventArgs e)
		{
			if (EnableDeleteChanged != null) {
				EnableDeleteChanged(this, e);
			}
		}
		
		protected virtual void OnEnableSelectAllChanged(EventArgs e)
		{
			if (EnableSelectAllChanged != null) {
				EnableSelectAllChanged(this, e);
			}
		}
		
		public event EventHandler EnableCutChanged;
		public event EventHandler EnableCopyChanged;
		public event EventHandler EnablePasteChanged;
		public event EventHandler EnableDeleteChanged;
		public event EventHandler EnableSelectAllChanged;
	}
}
