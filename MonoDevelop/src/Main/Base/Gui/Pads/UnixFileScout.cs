// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using MonoDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class FileList : Gtk.TreeView
	{
		private FileSystemWatcher watcher;
		private ItemCollection Items;
		private Gtk.ListStore store;
		
//		private MagicMenus.PopupMenu menu = null;
		
		public FileList()
		{
			Items = new ItemCollection(this);
			ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			//Columns.Add("File", 100, HorizontalAlignment.Left);
			//Columns.Add("Size", -2, HorizontalAlignment.Right);
			//Columns.Add("Last modified", -2, HorizontalAlignment.Left);
			
			store = new Gtk.ListStore (typeof (string), typeof (string), typeof(string), typeof(FileListItem));
			Model = store;

			HeadersVisible = true;
			HeadersClickable = true;
			Reorderable = true;
			RulesHint = true;

			Gtk.TreeViewColumn name_column = new Gtk.TreeViewColumn ();
			name_column.Title = "File";
			
			Gtk.TreeViewColumn size_column = new Gtk.TreeViewColumn ();
			size_column.Title = "Size";

			Gtk.TreeViewColumn modi_column = new Gtk.TreeViewColumn ();
			modi_column.Title = "Last modified";

			Gtk.CellRendererText render1 = new Gtk.CellRendererText ();
			name_column.PackStart (render1, false);
			name_column.AddAttribute (render1, "text", 0);
			
			Gtk.CellRendererText render2 = new Gtk.CellRendererText ();
			size_column.PackStart (render2, false);
			size_column.AddAttribute (render2, "text", 1);
			
			Gtk.CellRendererText render3 = new Gtk.CellRendererText ();
			modi_column.PackStart (render3, false);
			modi_column.AddAttribute (render3, "text", 2);
				
			//listView.AppendColumn (complete_column);
			AppendColumn(name_column);
			AppendColumn(size_column);
			AppendColumn(modi_column);

			
//			menu = new MagicMenus.PopupMenu();
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Delete file", new EventHandler(deleteFiles)));
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Rename", new EventHandler(renameFile)));
			
			try {
				watcher = new FileSystemWatcher();
			} catch {}
			
			if(watcher != null) {
				watcher.NotifyFilter = NotifyFilters.FileName;
				watcher.EnableRaisingEvents = false;
				
				watcher.Renamed += new RenamedEventHandler(fileRenamed);
				watcher.Deleted += new FileSystemEventHandler(fileDeleted);
				watcher.Created += new FileSystemEventHandler(fileCreated);
				watcher.Changed += new FileSystemEventHandler(fileChanged);
			}
			
			//HideSelection 	= false;
			//GridLines		= true;
			//LabelEdit		= true;
			//SmallImageList = IconManager.List;
			//HeaderStyle 	= ColumnHeaderStyle.Nonclickable;
			//View 				= View.Details;
			//Alignment		= ListViewAlignment.Left;
		}
		
		void ItemAdded(FileListItem item) {
			store.AppendValues(System.IO.Path.GetFileName(item.FullName), item.Size, item.LastModified, item);
		}
		
		void ItemRemoved(FileListItem item) {
			// TODO
		}
		
		void Clear() {
			store.Clear();
		}
		
		void fileDeleted(object sender, FileSystemEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.FullPath.ToLower()) {
					Items.Remove(fileItem);
					break;
				}
			}
		}
		
		void fileChanged(object sender, FileSystemEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.FullPath.ToLower()) {
					
					FileInfo info = new FileInfo(e.FullPath);
					
					fileItem.Size = Math.Round((double)info.Length / 1024).ToString() + " KB";
					fileItem.LastModified = info.LastWriteTime.ToString();
					break;
				}
			}
		}
		
		void fileCreated(object sender, FileSystemEventArgs e)
		{
			FileInfo info = new FileInfo(e.FullPath);
			
			FileListItem fileItem = Items.Add(new FileListItem(e.FullPath,
				Math.Round((double)info.Length / 1024).ToString() + " KB",
				info.LastWriteTime.ToString())
			);
			
			//Items.Add(fileItem);
		}
		
		void fileRenamed(object sender, RenamedEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.OldFullPath.ToLower()) {
					fileItem.FullName = e.FullPath;
					//fileItem.Text = e.Name;
					break;
				}
			}
		}
		
		void renameFile(object sender, EventArgs e)
		{
		/*
			if(SelectedItems.Count == 1) {
				//SelectedItems[0].BeginEdit();
			}
		*/
		}
		
		void deleteFiles(object sender, EventArgs e)
		{
/*			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			
			if (messageService.AskQuestion("Are you sure ?", "Delete files")) {
				foreach(FileListItem fileItem in SelectedItems)
				{
					try {
						File.Delete(fileItem.FullName);
					} catch(Exception ex) {
						messageService.ShowError(ex, "Couldn't delete file '" + Path.GetFileName(fileItem.FullName) + "'");
						break;
					}
				}
			}
*/
		}
		
/*		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			
			ListViewItem itemUnderMouse = GetItemAt(PointToScreen(new Point(e.X, e.Y)).X, PointToScreen(new Point(e.X, e.Y)).Y);
			
			if(e.Button == MouseButtons.Right && this.SelectedItems.Count > 0) {
//				menu.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
			}
		}
		
		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			
			if(e.Label == null) {
				e.CancelEdit = true;
				return;
			}
			
			string filename = ((FileListItem)Items[e.Item]).FullName;
			string newname = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + e.Label;
			
			try {
				File.Move(filename, newname);
				((FileListItem)Items[e.Item]).FullName = newname;
			} catch(Exception ex) {
				e.CancelEdit = true;
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(ex, "Rename failed");
			}
		}
*/		
		public void ShowFilesInPath(string path)
		{
			string[] files;
			Items.Clear();
		
			try {
				files = Directory.GetFiles(path);
			} catch (Exception) {
				return;
			}
			
			watcher.Path = path;
			watcher.EnableRaisingEvents = true;
			
			foreach (string file in files) {
				string filename = System.IO.Path.GetFileName(file);
				if (filename != null && filename.Length > 0 && filename[0] == '.') {
					continue;
				}
				FileInfo info = new FileInfo(file);
				FileListItem fileItem = Items.Add(new FileListItem(file,
					Math.Round((double)info.Length / 1024).ToString() + " KB",
					info.LastWriteTime.ToString()
				));
			}
			
			//EndUpdate();
		}
		
		public class FileListItem
		{
			string fullname;
			string size;
			string lastModified;
			
			public string FullName {
				get {
					return fullname;
				} 
				set {
					fullname = value;
				}
			}
			
			public string Size {
				get {
					return size;
				}
				set {
					size = value;
				}
			}
			
			public string LastModified {
				get {
					return lastModified;
				}
				set {
					lastModified = value;
				}
			}
			
			public FileListItem(string fullname, string size, string lastModified) 
			{
				this.fullname = fullname;
				this.size = size;
				this.lastModified = lastModified;
				//ImageIndex = IconManager.GetIndexForFile(fullname);
			}
		}
		
		class ItemCollection {
			FileList parent;
			ArrayList list = new ArrayList();
			
			public ItemCollection(FileList parent) {
				this.parent = parent;
			}
			
			public FileListItem Add(FileListItem item) {
				list.Add(item);
				parent.ItemAdded(item);
				return item;
			}
			
			public void Remove(FileListItem item) {
				parent.ItemRemoved(item);
				list.Remove(item);
			}
			
			public void Clear() {
				list.Clear();
				parent.Clear();
			}
			
			public IEnumerator GetEnumerator() {
				ArrayList copy = (ArrayList)list.Clone();
				return copy.GetEnumerator();
			}
		}
	}
	
	public class FileScout : Gtk.VPaned, IPadContent
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public Gtk.Widget Control {
			get {
				return this;
			}
		}
		
		public void BringToFront() {
			// TODO
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.FileScoutLabel");
			}
		}
		
		public string Icon {
			get {
				return "Icons.16x16.OpenFolderBitmap";
			}
		}
		
		public void RedrawContent()
		{
			//OnTitleChanged(null);
			//OnIconChanged(null);
		}
		
		//Splitter      splitter1     = new Splitter();
		
		FileList   filelister = new FileList();
		ShellTree  filetree   = new ShellTree();
		
		public FileScout()
		{
			//Dock      = DockStyle.Fill;
			
			//filetree.Dock = DockStyle.Top;
			//filetree.BorderStyle = BorderStyle.Fixed3D;
			//filetree.Location = new System.Drawing.Point(0, 22);
			//filetree.Size = new System.Drawing.Size(184, 157);
			//filetree.TabIndex = 1;
			//filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
			filetree.Selection.Changed += new EventHandler(OnDirChanged);
			//ImageList imglist = new ImageList();
			//imglist.ColorDepth = ColorDepth.Depth32Bit;
			/*imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.FLOPPY"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.DRIVE"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.CDROM"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.NETWORK"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Desktop"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PersonalFiles"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.MyComputer"));*/
			
			//filetree.ImageList = imglist;
			
			//filelister.Dock = DockStyle.Fill;
			//filelister.BorderStyle = BorderStyle.Fixed3D;
			//filelister.Location = new System.Drawing.Point(0, 184);
			
			//filelister.Sorting = SortOrder.Ascending;
			//filelister.Size = new System.Drawing.Size(184, 450);
			//filelister.TabIndex = 3;
			//filelister.ItemActivate += new EventHandler(FileSelected);
			filelister.RowActivated += new GtkSharp.RowActivatedHandler(FileSelected);

			
			//splitter1.Dock = DockStyle.Top;
			//splitter1.Location = new System.Drawing.Point(0, 179);
			//splitter1.Size = new System.Drawing.Size(184, 5);
			//splitter1.TabIndex = 2;
			//splitter1.TabStop = false;
			//splitter1.MinSize = 50;
			//splitter1.MinExtra = 50;
			
			//this.Controls.Add(filelister);
			//this.Controls.Add(splitter1);
			//this.Controls.Add(filetree);
			
			Gtk.ScrolledWindow treesw = new Gtk.ScrolledWindow ();
			treesw.Add(filetree);
			Gtk.Frame treef  = new Gtk.Frame();
			treef.Add(treesw);
			
			Gtk.ScrolledWindow listsw = new Gtk.ScrolledWindow ();
			listsw.Add(filelister);
			Gtk.Frame listf  = new Gtk.Frame();
			listf.Add(listsw);
			
			
			Pack1(treef, true, true);
			Pack2(listf, true, true);
		}
		
		void OnDirChanged(object sender, EventArgs args) 
/*		void DirectorySelected(object sender, TreeViewEventArgs e)*/
		{
			filelister.ShowFilesInPath(filetree.NodePath + System.IO.Path.DirectorySeparatorChar);
		}

//		void FileSelected(object sender, EventArgs e)
		void FileSelected(object sender, GtkSharp.RowActivatedArgs e)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService    fileService    = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			
			//foreach (FileList.FileListItem item in filelister.SelectedItems) {
			Gtk.TreeIter iter;
			for (filelister.Model.GetIterFirst(out iter); filelister.Model.IterNext(out iter) == true;) {
				if (filelister.Selection.IterIsSelected(iter) == false) {
					continue;
				} 
				FileList.FileListItem item = (FileList.FileListItem)filelister.Model.GetValue(iter, 3);
				switch (System.IO.Path.GetExtension(item.FullName)) {
					case ".cmbx":
					case ".prjx":
						projectService.OpenCombine(item.FullName);
						break;
					default:
						fileService.OpenFile(item.FullName);
						break;
				}
			}
		}
/*
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		*/
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}

	public class ShellTree : TreeView
	{
		public string NodePath {
			get {
				if (SelectedNode == null) {
					return "";
				}
				return (string)SelectedNode.Tag;
			}
			set {
				PopulateShellTree(value);
			}
		}
		
		public ShellTree()
		{
			//Sorted = true;
			TreeNode rootNode = Nodes.Add("/");
			rootNode.Tag = "/";
			//rootNode.ImageIndex = 6;
			//rootNode.SelectedImageIndex = 6;
			//rootNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			rootNode.Nodes.Add("");
			PopulateSubDirectory(rootNode, 1);
			
			
			
			//TreeNode myFilesNode = rootNode.Nodes.Add("My Documents");
			//myFilesNode.ImageIndex = 7;
			//myFilesNode.SelectedImageIndex = 7;
			//myFilesNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			//myFilesNode.Nodes.Add("");
			
			//TreeNode computerNode = rootNode.Nodes.Add("My Computer");
			//computerNode.ImageIndex = 8;
			//computerNode.SelectedImageIndex = 8;
			//computerNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			/*
			foreach (string driveName in Environment.GetLogicalDrives()) {
				DriveObject drive = new DriveObject(driveName);
				
				TreeNode node = new TreeNode(drive.ToString());
				node.Nodes.Add(new TreeNode(""));
				node.Tag = driveName.Substring(0, driveName.Length - 1);
				computerNode.Nodes.Add(node);
				
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				
				switch(DriveObject.GetDriveType(driveName)) {
					case DriveType.Removeable:
						node.ImageIndex = node.SelectedImageIndex = 2;
						break;
					case DriveType.Fixed:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
					case DriveType.Cdrom:
						node.ImageIndex = node.SelectedImageIndex = 4;
						break;
					case DriveType.Remote:
						node.ImageIndex = node.SelectedImageIndex = 5;
						break;
					default:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
				}
			}
			
			foreach (string directory in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))) {
				TreeNode node = rootNode.Nodes.Add(Path.GetFileName(directory));
				node.Tag = directory;
				node.ImageIndex = node.SelectedImageIndex = 0;
				node.Nodes.Add(new TreeNode(""));
			}
			
			rootNode.Expand();
			computerNode.Expand();
			*/
			InitializeComponent();
		}
		
		int getNodeLevel(TreeNode node)
		{
			TreeNode parent = node;
			int depth = 0;
			
			while(true)
			{
				parent = parent.Parent;
				if(parent == null) {
					return depth;
				}
				depth++;
			}
		}
		
		void InitializeComponent ()
		{
			//BeforeSelect   += new TreeViewCancelEventHandler(SetClosedIcon);
			//AfterSelect    += new TreeViewEventHandler(SetOpenedIcon);
		}

/*		
		void SetClosedIcon(object sender, TreeViewCancelEventArgs e) // Set icon as closed
		{
			if (SelectedNode != null) {
				if(getNodeLevel(SelectedNode) > 2) {
					SelectedNode.ImageIndex = SelectedNode.SelectedImageIndex = 0;
				}
			}
		}
		
		void SetOpenedIcon(object sender, TreeViewEventArgs e) // Set icon as opened
		{
			if(getNodeLevel(e.Node) > 2) {
				if (e.Node.Parent != null && e.Node.Parent.Parent != null) {
					e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
				}
			}
		}
*/	
		void PopulateShellTree(string path)
		{
			string[]  pathlist = path.Split(new char[] { System.IO.Path.DirectorySeparatorChar });
			
			TreeNodeCollection  curnode = Nodes;
			
			foreach(string dir in pathlist) {
				foreach(TreeNode childnode in curnode) {
					if (((string)childnode.Tag).ToUpper().Equals(dir.ToUpper())) {
						SelectedNode = childnode;
						
						PopulateSubDirectory(childnode, 2);
						childnode.Expand();
						
						curnode = childnode.Nodes;
						break;
					}
				}
			}
		}
		
		void PopulateSubDirectory(TreeNode curNode, int depth)
		{
			if (--depth < 0) {
				return;
			}
			
			if (curNode.Nodes.Count == 1 && curNode.Nodes[0].Text.Equals("")) {
				
				string[] directories = null;
				try {
					directories  = Directory.GetDirectories(curNode.Tag.ToString() + System.IO.Path.DirectorySeparatorChar);
				} catch (Exception) {
					return;
				}
				
				curNode.Nodes.Clear();
				
				foreach (string fulldir in directories) {
					try {
						string dir = System.IO.Path.GetFileName(fulldir);
						
						FileAttributes attr = File.GetAttributes(fulldir);
						if ((attr & FileAttributes.Hidden) == 0) {
							TreeNode node   = curNode.Nodes.Add(dir);
							node.Tag = curNode.Tag.ToString() + System.IO.Path.DirectorySeparatorChar + dir;
							//node.ImageIndex = node.SelectedImageIndex = 0;
							
							node.Nodes.Add(""); // Add dummy child node to make node expandable
							
							PopulateSubDirectory(node, depth);
						}
					} catch (Exception) {
					}
				}
			} else {
				foreach (TreeNode node in curNode.Nodes) {
					PopulateSubDirectory(node, depth); // Populate sub directory
				}
			}
		}

		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			//Cursor.Current = Cursors.WaitCursor;
			try {
				// do not populate if the "My Cpmputer" node is expaned
//				if(e.Node.Parent != null && e.Node.Parent.Parent != null) {
//					PopulateSubDirectory(e.Node, 2);
					//Cursor.Current = Cursors.Default;
//				} else {
					PopulateSubDirectory(e.Node, 1);
					//Cursor.Current = Cursors.Default;
//				}
			} catch (Exception excpt) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(excpt, "Device error");
				e.Cancel = true;
			}
			//Cursor.Current = Cursors.Default;
		}
	}
}
