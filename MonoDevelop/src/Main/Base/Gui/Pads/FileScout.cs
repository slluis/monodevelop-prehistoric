// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
//using System.Text;
//using System.Drawing;
using System.Collections;
using System.Resources;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui.Utils;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class FileList : Gtk.TreeView
	{
		private FileSystemWatcher watcher;
		private ItemCollection Items;
		private Gtk.ListStore store;
		private Gtk.Menu popmenu = null;
		
		public FileList()
		{
			Items = new ItemCollection(this);
			ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			store = new Gtk.ListStore (typeof (string), typeof (string), typeof(string), typeof(FileListItem), typeof (Gdk.Pixbuf));
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

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			name_column.PackStart (pix_render, false);
			name_column.AddAttribute (pix_render, "pixbuf", 4);
			
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
			
			watcher = new FileSystemWatcher ();
			
			if(watcher != null) {
				watcher.NotifyFilter = NotifyFilters.FileName;
				watcher.EnableRaisingEvents = false;
				
				watcher.Renamed += new RenamedEventHandler(fileRenamed);
				watcher.Deleted += new FileSystemEventHandler(fileDeleted);
				watcher.Created += new FileSystemEventHandler(fileCreated);
				watcher.Changed += new FileSystemEventHandler(fileChanged);
			}
		}
		
		internal void ItemAdded(FileListItem item) {
			store.AppendValues(item.Text, item.Size, item.LastModified, item, item.Icon);
		}
		
		void ItemRemoved(FileListItem item) {
			// TODO
		}
		
		internal void Clear() {
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
			
			Items.Add(fileItem);
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
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			
			if (messageService.AskQuestion("Are you sure ?", "Delete files")) {
				/*foreach(FileListItem fileItem in SelectedItems)
				{
					try {
						File.Delete(fileItem.FullName);
					} catch(Exception ex) {
						messageService.ShowError(ex, "Couldn't delete file '" + Path.GetFileName(fileItem.FullName) + "'");
						break;
					}
				}*/
			}
		}
		
		public class FileListItem
		{
			string fullname;
			string text;
			string size;
			string lastModified;
			Gdk.Pixbuf icon;
			
			public string FullName {
				get {
					return fullname;
				} 
				set {
					fullname = System.IO.Path.GetFullPath(value);
					text = System.IO.Path.GetFileName(fullname);
				}
			}
			
			public String Text {
				get {
					return text;
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

			public Gdk.Pixbuf Icon {
				get {
					return icon;
				}
				set {
					icon = value;
				}
			}
			
			public FileListItem(string fullname, string size, string lastModified) 
			{
				this.size = size;
				this.lastModified = lastModified;
				//FIXME: This is because //home/blah is not the same as /home/blah according to Icon.LookupSync, if we get weird behaviours, lets look at this again, see if we still need it.
				FullName = fullname.Substring (1);
				icon = FileIconLoader.GetPixbufForFile (FullName, 24, 24);
			}

			public FileListItem (string name)
			{
				FileInfo fi = new FileInfo (name);
				this.size = Math.Round ((double) fi.Length / 1024).ToString () + " KB";
				this.lastModified = fi.LastWriteTime.ToString ();
				FullName = System.IO.Path.GetFullPath (name); 
				icon = FileIconLoader.GetPixbufForFile (FullName, 24, 24);
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
				return resourceService.GetString ("MainWindow.Windows.FileScoutLabel");
			}
		}
		
		public string Icon {
			get {
				return MonoDevelop.Gui.Stock.OpenFolderBitmap;
			}
		}
		
		public void RedrawContent()
		{
			//OnTitleChanged(null);
			//OnIconChanged(null);
		}
		
		FileList filelister = new FileList ();
		FileBrowser fb = new FileBrowser ();
		PropertyService PropertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));

		public FileScout()
		{
			fb.DirectoryChangedEvent += new DirectoryChangedEventHandler (OnDirChanged);
			filelister.RowActivated += new Gtk.RowActivatedHandler(FileSelected);

			Gtk.Frame treef  = new Gtk.Frame ();
			treef.Add (fb);

			Gtk.ScrolledWindow listsw = new Gtk.ScrolledWindow ();
			listsw.Add (filelister);
			
			this.Pack1 (treef, true, true);
			this.Pack2 (listsw, true, true);

			fb.TreeView.Selection.SelectPath (new Gtk.TreePath ("0"));
			
			OnDirChanged (fb.CurrentDir);
			this.ShowAll ();
		}

		void OnDirChanged (string path) 
		{
			filelister.Clear ();

			PropertyService p = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool ignoreHidden = !p.GetProperty ("ICSharpCode.SharpDevelop.Gui.FileScout.ShowHidden", false);
			fb.IgnoreHidden = ignoreHidden;

			foreach (string f in fb.Files)
			{
				if (!(System.IO.Path.GetFileName (f)).StartsWith ("."))
				{
					FileList.FileListItem it = new FileList.FileListItem (f);
					filelister.ItemAdded (it);
				}
				else
				{
					if (!ignoreHidden)
					{
						FileList.FileListItem it = new FileList.FileListItem (f);
						filelister.ItemAdded (it);
					
					}
				}
			}
		}

		void FileSelected(object sender, Gtk.RowActivatedArgs e)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService    fileService    = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			Gtk.TreeIter iter;
			if (filelister.Model.GetIterFirst(out iter) == false) {
				return;
			}
			do {
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
						Console.WriteLine (item.FullName);
						fileService.OpenFile(item.FullName);
						break;
				}
			} while (filelister.Model.IterNext(out iter) == true);
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
}
