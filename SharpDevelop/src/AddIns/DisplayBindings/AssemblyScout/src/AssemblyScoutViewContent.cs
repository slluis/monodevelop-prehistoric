// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using MagicControls = Crownwood.Magic.Controls;

using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	///////////////////////////////////////////
	// AssemblyScoutViewContent Class
	///////////////////////////////////////////
	public class AssemblyScoutViewContent : AbstractViewContent
	{
		string filename = "";
		
		public ResourceService ress = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public MagicControls.TabControl leftTab, rightTab;
		Control control = null;
		AssemblyTree tree = null;
		
		public override Control Control {
			get {
				return control;
			}
		}
		
		public AssemblyTree Tree {
			get {
				return tree;
			}
		}
				
		string untitledName = "";
		public override string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}

		public override string ContentName {
			get {
				return filename;
			}
			set {
				filename = value;
				OnContentNameChanged(null);
			}
		}
		
		public override string TabPageText {
			get {
				return "Assemblies";
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
	
		IWorkbenchWindow workbenchWindow;
		public override IWorkbenchWindow WorkbenchWindow {
			get {
				return workbenchWindow;
			}
			set {
				workbenchWindow = value;
				if (filename == "") {
					workbenchWindow.Title = ress.GetString("ObjectBrowser.AssemblyScout");
				} else {
					workbenchWindow.Title = filename;
				}
			}
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			try {
				foreach(Control ctl in Control.Controls) {
					ctl.Dispose();
				}
			} catch {
				return;
			}
		}
				
		public void SaveFile()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".dll" || Path.GetExtension(fileName) == ".exe";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		
		public IViewContent CreateContentForFile(string fileName)
		{
			Load(fileName);
			return this;
		}

		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
		
		public override void Save()
		{
		}
		
		public override void Save(string filename)
		{
		}
		
		public override void Load(string filename)
		{
			tree.LoadFile(filename);
			this.filename = filename;
			this.ContentName = Path.GetFileName(filename);
			OnContentNameChanged(null);
		}
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public AssemblyScoutViewContent()
		{
			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			
			leftTab            = new MagicControls.TabControl();
			leftTab.Dock       = DockStyle.Left;
			leftTab.Width      = 400;
			leftTab.BoldSelectedPage = true;
			leftTab.IDEPixelBorder   = true;
			
			AssemblyTree assemblyTree      = new AssemblyTree(this);
			this.tree = assemblyTree;
			
			MagicControls.TabPage treeviewpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Tree"));
			treeviewpage.Icon                  = resourceService.GetIcon("Icons.16x16.Class");
			treeviewpage.DockPadding.All       = 8;
			treeviewpage.Controls.Add(assemblyTree);
			leftTab.TabPages.Add(treeviewpage);
			
			MagicControls.TabPage indexviewpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Search"));
			indexviewpage.Icon                  = resourceService.GetIcon("Icons.16x16.FindIcon");
			SearchPanel SearchPanel   = new SearchPanel(assemblyTree);
			SearchPanel.ParentDisplayInfo       = this;
			indexviewpage.DockPadding.All       = 8;
			indexviewpage.Controls.Add(SearchPanel);
			leftTab.TabPages.Add(indexviewpage);
			
			Splitter vsplitter    = new Splitter();
			vsplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

			vsplitter.Location    = new System.Drawing.Point(0, 200);
			vsplitter.TabIndex    = 5;
			vsplitter.TabStop     = false;
			vsplitter.Size        = new System.Drawing.Size(3, 273);
			vsplitter.Dock        = DockStyle.Left;
			
			rightTab = new MagicControls.TabControl();
			rightTab.Dock       = DockStyle.Fill;
			rightTab.BoldSelectedPage = true;
			rightTab.IDEPixelBorder = true;
			
			MagicControls.TabPage memberpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Info"));
			memberpage.Icon                  = resourceService.GetIcon("Icons.16x16.Information");
			memberpage.DockPadding.All       = 8;
			memberpage.Controls.Add(new InfoView(assemblyTree));
			rightTab.TabPages.Add(memberpage);
			
			MagicControls.TabPage ildasmviewpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Disasm"));
			ildasmviewpage.Icon                  = resourceService.GetIcon("Icons.16x16.ILDasm");
			ildasmviewpage.DockPadding.All       = 8;
			ildasmviewpage.Controls.Add(new ILDasmView(assemblyTree));
			rightTab.TabPages.Add(ildasmviewpage);
			
			MagicControls.TabPage sourceviewpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Source"));
			sourceviewpage.Icon                  = resourceService.GetIcon("Icons.16x16.TextFileIcon");
			sourceviewpage.DockPadding.All       = 8;
			sourceviewpage.Controls.Add(new SourceView(assemblyTree));
			rightTab.TabPages.Add(sourceviewpage);

			MagicControls.TabPage xmlviewpage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.XML"));
			xmlviewpage.Icon                  = resourceService.GetIcon("Icons.16x16.XMLFileIcon");
			xmlviewpage.DockPadding.All       = 8;
			xmlviewpage.Controls.Add(new XmlView(assemblyTree));
			rightTab.TabPages.Add(xmlviewpage);
			
			MagicControls.TabPage extproppage = new MagicControls.TabPage(ress.GetString("ObjectBrowser.Extended"));
			extproppage.Icon                  = resourceService.GetIcon("Icons.16x16.Property");
			extproppage.DockPadding.All       = 8;
			extproppage.Controls.Add(new ExtendedPropsPanel(assemblyTree));
			rightTab.TabPages.Add(extproppage);
			
			panel.Controls.Add(rightTab);
			panel.Controls.Add(vsplitter);
			panel.Controls.Add(leftTab);
			
			this.control = panel;
			this.ContentName = ress.GetString("ObjectBrowser.AssemblyScout");
		}
		
		public void LoadStdAssemblies() {
			//try {
			tree.AddAssembly(SA.SharpAssembly.Load("mscorlib"));
			tree.AddAssembly(SA.SharpAssembly.Load("System"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Xml"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Windows.Forms"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Drawing"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Data"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Design"));			
			tree.AddAssembly(SA.SharpAssembly.Load("System.Web"));			
			//} catch {}
		}
		
		public void LoadRefAssemblies() {
			IProjectService projectService = (IProjectService)ServiceManager.Services.GetService(typeof(IProjectService));
			try {
				if (projectService.CurrentSelectedProject == null) return;
				foreach(ProjectReference pr in projectService.CurrentSelectedProject.ProjectReferences) {
					if (pr.ReferenceType == ReferenceType.Project || pr.ReferenceType == ReferenceType.Typelib) continue;
					if (!tree.IsAssemblyLoaded(pr.GetReferencedFileName(null))) {
						try {
							tree.LoadFile(pr.GetReferencedFileName(null));
						} catch (Exception) {
							//MessageBox.Show("Object Browser error:\nError loading assembly " + pr.GetReferencedFileName(null) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
				}
			} catch (Exception) {}
		
		}
		
		
	}
}
