// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Text;
using System.Diagnostics;
using System.Security.Policy;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project.Collections;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionTree : TreeView
	{
		public ResourceService ress = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		ArrayList assemblies = new ArrayList();
		ObjectBrowser.DisplayInformationWrapper _parent;
		PropertyService propSvc;
		
		public bool showInternalTypes = true, showInternalMembers = true;
		public bool showPrivateTypes = true, showPrivateMembers = true;
		public bool showSpecial = false;
		
		MenuItem mnuBack;
		MenuItem mnuLoadAsm, mnuLoadStd, mnuLoadRef;
		MenuItem mnuShowPrivTypes, mnuShowIntTypes;
		MenuItem mnuShowPrivMem, mnuShowIntMem, mnuShowSpecial;
		MenuItem mnuRemAsm, mnuCopyTree, mnuSaveRes, mnuJump, mnuOpenRef, mnuDisAsm;
		
		Stack history = new Stack();
		bool histback = false;
		
		ReflectionNode selnode;
		
		public event EventHandler Changed;
		
		internal static AmbienceReflectionDecorator languageConversion;
		
		public ReflectionTree(ObjectBrowser.DisplayInformationWrapper parent) : base()
		{
			if (Changed != null) {} // only to prevent these pesky compiler warning :) M.K.
			
			Dock = DockStyle.Fill;
			
			string resPrefix = "ObjectBrowser.Menu.";
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			propSvc = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			languageConversion = ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = languageConversion.ConversionFlags & ~(ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowParameterNames);
			
			this.ImageList = classBrowserIconService.ImageList;
			
			LabelEdit     = false;
			HotTracking   = false;
			AllowDrop     = true;
			HideSelection = false;
			//Sorted        = true;
			
			mnuBack    = new MenuItem(ress.GetString(resPrefix + "GoBack"), new EventHandler(BackEvt));
			mnuLoadAsm = new MenuItem(ress.GetString(resPrefix + "LoadAssembly"), new EventHandler(LoadAsmEvt));
			mnuLoadStd = new MenuItem(ress.GetString(resPrefix + "LoadStd"), new EventHandler(LoadStdEvt));
			mnuLoadRef = new MenuItem(ress.GetString(resPrefix + "LoadRef"), new EventHandler(LoadRefEvt));
			mnuShowPrivTypes = new MenuItem(ress.GetString(resPrefix + "ShowPrivTypes"), new EventHandler(ShowPrivTypesEvt));
			mnuShowIntTypes  = new MenuItem(ress.GetString(resPrefix + "ShowIntTypes"), new EventHandler(ShowIntTypesEvt));
			mnuShowPrivMem   = new MenuItem(ress.GetString(resPrefix + "ShowPrivMem"), new EventHandler(ShowPrivMemEvt));
			mnuShowIntMem    = new MenuItem(ress.GetString(resPrefix + "ShowIntMem"), new EventHandler(ShowIntMemEvt));
			mnuShowSpecial   = new MenuItem(ress.GetString(resPrefix + "ShowSpecial"), new EventHandler(ShowSpecialEvt));
			mnuRemAsm   = new MenuItem(ress.GetString(resPrefix + "RemoveAsm"), new EventHandler(RemAsmEvt));
			mnuCopyTree = new MenuItem(ress.GetString(resPrefix + "CopyTree"), new EventHandler(CopyTreeEvt));
			mnuSaveRes  = new MenuItem(ress.GetString(resPrefix + "SaveRes"), new EventHandler(SaveResEvt));
			mnuJump     = new MenuItem(ress.GetString(resPrefix + "JumpType"), new EventHandler(JumpEvt));
			mnuOpenRef  = new MenuItem(ress.GetString(resPrefix + "OpenRef"), new EventHandler(OpenRefEvt));
			mnuDisAsm   = new MenuItem(ress.GetString(resPrefix + "DisasmToFile"), new EventHandler(DisAsmEvt));
			
			ContextMenu = new ContextMenu(new MenuItem[] {
				mnuBack,
				new MenuItem("-"),
				mnuLoadAsm,
				mnuLoadStd,
				mnuLoadRef,
				new MenuItem("-"),
				mnuShowPrivTypes,
				mnuShowIntTypes,
				new MenuItem("-"),
				mnuShowPrivMem,
				mnuShowIntMem,
				mnuShowSpecial,
				new MenuItem("-"),
				mnuRemAsm,
				mnuCopyTree,
				mnuSaveRes,
				mnuJump,
				mnuOpenRef,
				mnuDisAsm
			});
			
			mnuShowPrivTypes.Checked = showPrivateTypes    = propSvc.GetProperty("ObjectBrowser.ShowPrivTypes", true);
			mnuShowIntTypes.Checked  = showInternalTypes   = propSvc.GetProperty("ObjectBrowser.ShowIntTypes", true);
			mnuShowPrivMem.Checked   = showPrivateMembers  = propSvc.GetProperty("ObjectBrowser.ShowPrivMembers", true);
			mnuShowIntMem.Checked    = showInternalMembers = propSvc.GetProperty("ObjectBrowser.ShowIntMembers", true);
			mnuShowSpecial.Checked   = showSpecial = propSvc.GetProperty("ObjectBrowser.ShowSpecialMethods", false);
			
			_parent = parent;
		}
		
		public ArrayList Assemblies {
			get {
				return assemblies;
			}
		}
		
		public PrintDocument PrintDocument {
			get {
				return null;
			}
		}
		
		public bool WriteProtected {
			get {
				return false;
			}
			set {
			}
		}
		
		public void LoadFile(string fileName) 
		{
			AddAssembly(Assembly.LoadFrom(fileName));
		}
		
		public bool IsAssemblyLoaded(string filename)
		{
			try {
				foreach(Assembly asm in assemblies) {
					if (asm.Location == filename) return true;
				}
			} finally {
			}
			return false;
		}
		
		public void SaveFile(string filename)
		{
		}
		
		public void AddAssembly(Assembly assembly)
		{
			if (IsAssemblyLoaded(assembly.Location)) return;
			
			assemblies.Add(assembly);
			TreeNode node = new ReflectionFolderNode(Path.GetFileNameWithoutExtension(assembly.CodeBase), assembly, ReflectionNodeType.Assembly, 0, 1);
			Nodes.Add(node);
			PopulateTree((ReflectionNode)node);
		}

		public void RePopulateTreeView()
		{
			foreach (ReflectionNode node in Nodes) {
				node.Populate(showPrivateTypes, showInternalTypes);
				PopulateTree(node);
			}
		}
		
		public void PopulateTree(ReflectionNode parentnode)
		{
			if (!parentnode.Populated)
				parentnode.Populate(showPrivateTypes, showInternalTypes);
			
			foreach (ReflectionNode node in parentnode.Nodes) {
				if (!node.Populated) {
					node.Populate(showPrivateTypes, showInternalTypes);
				}
				PopulateTree(node);
			}
		}
		
		public void GoToMember(MemberInfo member, Assembly MemberAssembly) 
		{
			string paramtext = "";
			paramtext = ReflectionMemberNode.GetMemberNodeText(member);
				
			ReflectionTypeNode typenode = GetTypeNode(member.DeclaringType);
			if (typenode == null) return;
			
			if (!typenode.MembersPopulated) {
				typenode.PopulateMembers(showPrivateMembers, showInternalMembers, showSpecial);
			}
			
			TreeNode foundnode = typenode.GetNodeFromChildren(paramtext);
			if (foundnode == null) return;
			
			foundnode.EnsureVisible();
			SelectedNode = foundnode;
		}
		
		private ReflectionTypeNode GetTypeNode(Type type)
		{
			foreach (ReflectionNode node in Nodes) {
				Assembly assembly = (Assembly)node.Attribute;
				if (type.Assembly.FullName == assembly.FullName) {
					
					// curnode contains Filename node
					ReflectionNode curnode = (ReflectionNode)node.GetNodeFromChildren(Path.GetFileName(assembly.CodeBase));
					
					TreeNode path;
					
					if (type.Namespace == null || type.Namespace == "") {
						path = curnode;
					} else {
						TreeNode tnode = curnode.GetNodeFromChildren(type.Namespace); // get namespace node
						if (tnode == null) {
							return null; // TODO : returns, if the tree isn't up to date.
						} else {
							path = tnode;
						}
					}

					string nodename = type.FullName.Substring(type.Namespace.Length + 1);
										
					TreeNode foundnode = node.GetNodeFromCollection(path.Nodes, nodename);
					return (ReflectionTypeNode)foundnode;
				}
			}
			
			// execute if assembly containing the type is not loaded
			AddAssembly(type.Assembly);
			return GetTypeNode(type);
			
		}
		
		public void GoToNamespace(Assembly asm, string name)
		{
			foreach (ReflectionNode node in Nodes) {
				Assembly assembly = (Assembly)node.Attribute;
				if (asm.FullName == assembly.FullName) {
					
					// curnode contains Filename node
					ReflectionNode curnode = (ReflectionNode)node.GetNodeFromChildren(Path.GetFileName(assembly.CodeBase));
					
					TreeNode tnode = curnode.GetNodeFromChildren(name); // get namespace node
					if (tnode == null) return;
					tnode.EnsureVisible();
					SelectedNode = tnode;
					return;
				}
			}
			
			// execute if assembly containing the type is not loaded
			AddAssembly(asm);
			GoToNamespace(asm, name);			
		}
		
		public void GoToType(Type type)
		{
			ReflectionNode node = GetTypeNode(type);
			if (node == null) return;
			
			node.EnsureVisible();
			SelectedNode = node;
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			ReflectionNode rn = (ReflectionNode)SelectedNode;
			if (rn == null)
				return;
			switch (rn.Type) {
				
				case ReflectionNodeType.Link: // clicked on link, jump to link.
					if (rn.Attribute is Type) {
						GoToType((Type)rn.Attribute);
					}
					break;
				
				case ReflectionNodeType.Reference: // clicked on assembly reference, open assembly
					// check if the assembly is open
					AssemblyName name = (AssemblyName)rn.Attribute;
					OpenAssemblyByName(name);
					break;
			}
		}
		
		public void OpenAssemblyByName(AssemblyName name)
		{
			foreach (ReflectionNode node in Nodes) {
				if (node.Type == ReflectionNodeType.Assembly) {
					if (name.FullName == ((Assembly)node.Attribute).FullName) { // if yes, return
						node.EnsureVisible();
						SelectedNode = node;
						return;
					}
				}
			}
			try {
				AddAssembly(Assembly.Load(name));	
				OpenAssemblyByName(name);
			} catch(Exception ex) {
				MessageBox.Show(String.Format(ress.GetString("ObjectBrowser.LoadError"), name.Name, ex.Message), ress.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			((ReflectionNode)e.Node).OnCollapse();
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			
			ReflectionNode rn = (ReflectionNode)e.Node;
			if (!rn.Populated)
				rn.Populate(showPrivateTypes, showInternalTypes);
			
			if (e.Node is ReflectionTypeNode) {
				ReflectionTypeNode tn = e.Node as ReflectionTypeNode;
				
				if (!tn.MembersPopulated)
					tn.PopulateMembers(showPrivateMembers, showInternalMembers, showSpecial);
			}
			
			((ReflectionNode)e.Node).OnExpand();
		}
		
		protected override void OnMouseDown(MouseEventArgs ev)
		{
			base.OnMouseDown(ev);
			
			ReflectionNode node = GetNodeAt(ev.X, ev.Y) as ReflectionNode;
			if (node != null) {
				if (ev.Button == MouseButtons.Right) histback = true;
				SelectedNode = node;
				histback = false;
				mnuRemAsm.Visible   = (node.Type == ReflectionNodeType.Assembly);
				mnuDisAsm.Visible   = (node.Type == ReflectionNodeType.Assembly);
				mnuCopyTree.Visible = (node.Type == ReflectionNodeType.Library);
				mnuSaveRes.Visible  = (node.Type == ReflectionNodeType.Resource);
				mnuJump.Visible     = (node.Type == ReflectionNodeType.Link);
				mnuOpenRef.Visible  = (node.Type == ReflectionNodeType.Reference);
				selnode = node;
			} else {
				mnuRemAsm.Visible   = false;
				mnuDisAsm.Visible   = false;
				mnuCopyTree.Visible = false;
				mnuSaveRes.Visible  = false;
				mnuJump.Visible     = false;
				mnuOpenRef.Visible  = false;
				selnode = null;
			}
 			
		}
		
		void LoadAsmEvt(object sender, EventArgs e)
		{
			using (SelectReferenceDialog selDialog = new SelectReferenceDialog(new ObjectBrowser.TempProject())) {
				if (selDialog.ShowDialog() == DialogResult.OK) {
					
					foreach (ProjectReference refInfo in selDialog.ReferenceInformations) {
						if (refInfo.ReferenceType == ReferenceType.Typelib) continue;
						if (refInfo.ReferenceType == ReferenceType.Project) continue;
						
						if (!IsAssemblyLoaded(refInfo.GetReferencedFileName(null))) {
							try {
								LoadFile(refInfo.GetReferencedFileName(null));
							} catch (Exception) {}
						}
					}
				}
			}
		}

		void LoadStdEvt(object sender, EventArgs e)
		{
			_parent.LoadStdAssemblies();
		}

		void LoadRefEvt(object sender, EventArgs e)
		{
			_parent.LoadRefAssemblies();
		}

		void ShowPrivTypesEvt(object sender, EventArgs e)
		{
			showPrivateTypes = !showPrivateTypes;
			propSvc.SetProperty("ObjectBrowser.ShowPrivTypes", showPrivateTypes);
			mnuShowPrivTypes.Checked = showPrivateTypes;
			RePopulateTreeView();
		}

		void ShowIntTypesEvt(object sender, EventArgs e)
		{
			showInternalTypes = !showInternalTypes;
			propSvc.SetProperty("ObjectBrowser.ShowIntTypes", showInternalTypes);
			mnuShowIntTypes.Checked = showInternalTypes;			
			RePopulateTreeView();
		}

		void ShowPrivMemEvt(object sender, EventArgs e)
		{
			showPrivateMembers = !showPrivateMembers;
			propSvc.SetProperty("ObjectBrowser.ShowPrivMembers", showPrivateMembers);
			mnuShowPrivMem.Checked = showPrivateMembers;			
			RePopulateTreeView();
		}
		
		void ShowIntMemEvt(object sender, EventArgs e)
		{
			showInternalMembers = !showInternalMembers;
			propSvc.SetProperty("ObjectBrowser.ShowIntMembers", showInternalMembers);
			mnuShowIntMem.Checked = showInternalMembers;			
			RePopulateTreeView();
		}
				
		void ShowSpecialEvt(object sender, EventArgs e)
		{
			showSpecial = !showSpecial;
			propSvc.SetProperty("ObjectBrowser.ShowSpecialMethods", showSpecial);
			mnuShowSpecial.Checked = showSpecial;			
			RePopulateTreeView();
		}
				
		void RemAsmEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			assemblies.Remove((Assembly)selnode.Attribute);
			selnode.Remove();
		}
		
		void CopyTreeEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			StringBuilder stb = new StringBuilder();
			
			stb.Append(selnode.Text + "\n");
			GetSubNodeText(selnode, stb, 1);
			
			Clipboard.SetDataObject(stb.ToString(), true);
			
		}
		
		private static void GetSubNodeText(TreeNode node, StringBuilder build, int indentLevel) {
			foreach (TreeNode tn in node.Nodes) {
				build.Append('\t', indentLevel);
				build.Append(tn.Text + "\n");
				GetSubNodeText(tn, build, indentLevel + 1);
			}
		}
		
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

		void DisAsmEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			Assembly asm = (Assembly)selnode.Attribute;
			
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= asm.GetName().Name;
			sdialog.Filter          = "IL files (*.il)|*.il";
			sdialog.DefaultExt      = ".il";
			sdialog.InitialDirectory = Path.GetDirectoryName(asm.Location);

			DialogResult dr = sdialog.ShowDialog();
			sdialog.Dispose();
			if(dr != DialogResult.OK) return;
			
			try {
				string args = '"' + asm.Location + "\" /NOBAR /OUT=\"" + sdialog.FileName + "\" /ALL ";
                RegistryKey regKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\.NETFramework");
                string cmd = (string)regKey.GetValue("sdkInstallRoot");
                ProcessStartInfo psi = new ProcessStartInfo(fileUtilityService.GetDirectoryNameWithSeparator(cmd) +
                                                            "bin\\ildasm.exe", args);
				
				psi.RedirectStandardError  = true;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput  = true;
				psi.UseShellExecute        = false;
				psi.CreateNoWindow         = true;
				
				Process process = Process.Start(psi);
				string output   = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
								
				MessageBox.Show(String.Format(ress.GetString("ObjectBrowser.ILDasmOutput"), output));
			} catch(Exception ex) {
				MessageBox.Show(String.Format(ress.GetString("ObjectBrowser.ILDasmError"),  ex.ToString()));
			}
		}
		
		void SaveResEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			Assembly asm = (Assembly)selnode.Attribute;
			SaveResource(asm, selnode.Text);
			
		}
		
		public void SaveResource(Assembly asm, string name)
		{
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= name;
			sdialog.Filter          = ress.GetString("ObjectBrowser.Filters.Binary") + "|*.*";
			sdialog.DefaultExt      = ".bin";

			DialogResult dr = sdialog.ShowDialog();
			sdialog.Dispose();
			if(dr != DialogResult.OK) return;
			
			try {
				Stream str = asm.GetManifestResourceStream(name);
				FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
				BinaryWriter wr = new BinaryWriter(fstr);
				byte[] buf = new byte[str.Length];
				str.Read(buf, 0, (int)str.Length);
				wr.Write(buf);
				fstr.Close();
				str.Close();
			} catch {
			}
			
		}
		
		void JumpEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			OnDoubleClick(e);
		}
		
		void OpenRefEvt(object sender, EventArgs e)
		{
			if (selnode == null) return;
			
			OnDoubleClick(e);
		}
		
		void BackEvt(object sender, EventArgs e)
		{
			if (history.Count == 0) return;
			try {
				histback = true;
				TreeNode selnode = (TreeNode)history.Pop();
				selnode.EnsureVisible();
				SelectedNode = selnode;
			} finally {
				histback = false;
			}
		}
		
		protected override void OnBeforeSelect(TreeViewCancelEventArgs ev)
		{
			base.OnBeforeSelect(ev);
			if (!histback) {
				//HACK: stack is cleared if too much elements
				if (history.Count >= 100) history.Clear();
				history.Push(SelectedNode);
			}
		}
		
		public void GoBack()
		{
			BackEvt(mnuBack, new EventArgs());
		}
	}

	public class TreeNodeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return String.Compare(((TreeNode)x).Text, ((TreeNode)y).Text);
		}
	}
}
