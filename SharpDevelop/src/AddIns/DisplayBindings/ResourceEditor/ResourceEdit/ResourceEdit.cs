// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Resources;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Undo;
using System.Drawing.Printing;

namespace ResEdit
{
	/// <summary>
	/// This class allows viewing and editing of windows resource files
	/// both in XML as in normal format.
	/// </summary>
	public class ResourceEdit : ListView
	{
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader type     = new ColumnHeader();
		ColumnHeader content  = new ColumnHeader();
		
		Hashtable resources = new Hashtable();
		ImageList images    = new ImageList();
		
		ResourceClipboardHandler clipboardHandler = null;
		UndoStack                undoStack        = null;
		bool                     writeProtected   = false;
		
		public event EventHandler         Changed;
		
		public bool WriteProtected {
			get {
				return writeProtected;
			}
			set {
				writeProtected = value;
			}
		}
		
		public Hashtable Resources {
			get {
				return resources;
			}
		}
		
		public UndoStack UndoStack {
			get {
				return undoStack;
			}
		}
		public IClipboardHandler ClipboardHandler {
			get {
				return clipboardHandler;
			}
		}
		
		public PrintDocument PrintDocument {
			get { // TODO
				return null; 
			}
		}
		
		public ResourceEdit()
		{
			clipboardHandler = new ResourceClipboardHandler(this);
			undoStack        = new UndoStack();
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			name.Text     = resourceService.GetString("ResourceEditor.ResourceEdit.NameColumn");
			name.Width    = 250;
			
			type.Text     = resourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			type.Width    = 170;
			
			content.Text  = resourceService.GetString("ResourceEditor.ResourceEdit.ContentColumn");
			content.Width = 300;
			
			Columns.AddRange(new ColumnHeader[] {name, type, content});
			
			FullRowSelect = true;
			AutoArrange   = true;
			Alignment     = ListViewAlignment.Left;
			View          = View.Details;
			HeaderStyle   = ColumnHeaderStyle.Nonclickable;
			GridLines     = true;
			Activation    = ItemActivation.TwoClick;
			Sorting       = SortOrder.Ascending;
			Dock          = DockStyle.Fill;
			
			BorderStyle   = System.Windows.Forms.BorderStyle.None;
			
			
			
			
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.string"));
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.bmp"));
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.icon"));
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.cursor"));
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.bin"));
			images.Images.Add(resourceService.GetIcon("Icons.16x16.ResourceEditor.obj"));
			SmallImageList = images;
			
			ItemActivate += new EventHandler(ClickItem);
			
			
			
			ContextMenu = new ContextMenu(new MenuItem[] {
											new MenuItem(resourceService.GetString("ResourceEditor.ResourceEdit.ContextMenu.AddStringEntry"), 
											             new EventHandler(NewEvt)),
											             
											new MenuItem(resourceService.GetString("ResourceEditor.ResourceEdit.ContextMenu.AddFiles"),     new EventHandler(AddEvt)),
											new MenuItem("-"),
											new MenuItem(resourceService.GetString("XML.MainMenu.FileMenu.SaveAs"), new EventHandler(SaveAsEvt)),
											new MenuItem(resourceService.GetString("ResourceEditor.ResourceEdit.ContextMenu.Rename"),     new EventHandler(RenameEvt), Shortcut.F2),
											new MenuItem(resourceService.GetString("ResourceEditor.ResourceEdit.ContextMenu.Delete"), new EventHandler(clipboardHandler.Delete)),
											new MenuItem("-"),
											new MenuItem(resourceService.GetString("XML.MainMenu.EditMenu.Cut"),    new EventHandler(clipboardHandler.Cut)),
											new MenuItem(resourceService.GetString("XML.MainMenu.EditMenu.Copy"),   new EventHandler(clipboardHandler.Copy)),
											new MenuItem(resourceService.GetString("XML.MainMenu.EditMenu.Paste"),  new EventHandler(clipboardHandler.Paste)),
											new MenuItem("-"),
											new MenuItem(resourceService.GetString("XML.MainMenu.EditMenu.SelectAll"), new EventHandler(clipboardHandler.SelectAll))
			});
		}
		
		public void LoadFile(string filename)
		{
			Stream s      = File.OpenRead(filename);
			switch (Path.GetExtension(filename).ToUpper()) {
				case ".RESX":
					ResXResourceReader rx = new ResXResourceReader(s);
//                  This don't work, because of a bug in the current CSharp implementation, I think
//                  try it again next release, must use ugly version.
//					foreach (DictionaryEntry entry in rx) {
//						if (!resources.ContainsKey(entry.Key))
//							resources.Add(entry.Key, entry.Value);
//					}
					
					// ugly version (from Framework Reference)
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext()) 
						if (!resources.ContainsKey(n.Key))
							resources.Add(n.Key, n.Value);
					
					rx.Close();
					break;
				case ".RESOURCES":
					ResourceReader rr = new ResourceReader(s);
					foreach (DictionaryEntry entry in rr) {
						if (!resources.ContainsKey(entry.Key))
							resources.Add(entry.Key, entry.Value);
					}
					rr.Close();
					break;
			}
			s.Close();
			InitializeListView();
		}
		
		public void SaveFile(string filename)
		{
			Debug.Assert(!writeProtected, "ICSharpCode.SharpDevelop.Gui.Edit.Resource.ResourceEdit.SaveFile(string filename) : trying to save a write protected file");
			switch (Path.GetExtension(filename).ToUpper()) {
				case ".RESX":		// write XML resource
					ResXResourceWriter rxw    = new ResXResourceWriter(filename);
					foreach (DictionaryEntry entry in resources) {
						if (entry.Value != null) {
							rxw.AddResource(entry.Key.ToString(), entry.Value);
						}
					}
					rxw.Generate();
					rxw.Close();
				break;
				
				default:			// write default resource
					ResourceWriter rw = new ResourceWriter(filename);
					foreach (DictionaryEntry entry in resources) {
						rw.AddResource(entry.Key.ToString(), entry.Value);
					}
					rw.Generate();
					rw.Close();
				break;
			}
		}
		
		
		public void NewEvt(object sender, EventArgs e)
		{
			if (writeProtected) {
				return;
			}
			
			using (EditEntry ed = new EditEntry()) {
				if (ed.ShowDialog() == DialogResult.OK) {
					if(resources.ContainsKey(ed.Entry.Key)) {
						ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
						MessageBox.Show(resourceService.GetString("ResourceEditor.ResourceEdit.KeyAlreadyDefinedError"),
						                resourceService.GetString("Global.ErrorText"), 
						                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					resources.Add(ed.Entry.Key, ed.Entry.Value);
					Items.Add(new ListViewItem(new String[] {(string)ed.Entry.Key, "System.String", (string)ed.Entry.Value},
				                                   ImageIndex("System.String")));
					// not necessary
					//InitializeListView();
				}
			}
			OnChanged();
		}
		
		public void AddEvt(object sender, EventArgs e)
		{
			if (writeProtected) {
				return;
			}
			
			using (OpenFileDialog fdiag = new OpenFileDialog()) {
				fdiag.AddExtension   = true;
				fdiag.Filter         = "All files (*.*)|*.*";
				fdiag.Multiselect    = true;
				fdiag.CheckFileExists = true;
								
				if (fdiag.ShowDialog() == DialogResult.OK) {
					foreach (string filename in fdiag.FileNames) {
						string oresname = Path.ChangeExtension(Path.GetFileName(filename), null);
						if (oresname == "") oresname = "new";
						
						string resname = oresname;
						
						int i = 0;
						TestName:
						if (resources.ContainsKey(resname)) {
							if (i == 10) {
								continue;
							}
							i++;
							resname = oresname + "_" + i.ToString();
							goto TestName;
						}
						
						object tmp = LoadResource(filename);
						if (tmp == null) {
							continue;
						}
						resources.Add(resname, tmp);
						
					}
					InitializeListView();
				}
			}
			OnChanged();
		}
		
		/// <summary>
		/// Jeff Huntsman: added small utility SaveAs
		/// SaveAsEvt saves the current selected resource 
		/// depending on its type.
		/// </summary>
		/// <returns>void</returns>
		public void SaveAsEvt(object sender, EventArgs e)
		{
			if (SelectedItems.Count != 1) {
				return;
			}
			
			string key = SelectedItems[0].Text;
			if (!resources.ContainsKey(key)) {
				return;
			}
			
			object val = resources[key];
			
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= key;
			if (val is Bitmap) {
				sdialog.Filter 		= "Bitmap files (*.bmp)|*.bmp";
				sdialog.DefaultExt 	= ".bmp";
			} else if (val is Icon) {
				sdialog.Filter 		= "Icon files (*.ico)|*.ico";
				sdialog.DefaultExt 	= ".ico";
			} else if (val is Cursor) {
				sdialog.Filter 		= "Cursor files (*.cur)|*.cur";
				sdialog.DefaultExt 	= ".cur";
			} else if (val is byte[]){
				sdialog.Filter      = "Binary files (*.*)|*.*";
				sdialog.DefaultExt  = ".bin";
			} else {
				return;
			}
			
			DialogResult dr = sdialog.ShowDialog();
			sdialog.Dispose();
			if (dr != DialogResult.OK) {
				return;
			}
			
			try {
				if (val is Icon) {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					((Icon)val).Save(fstr);
					fstr.Close();
				} else if (val is Image) {
					Image img = (Image)val;
					img.Save(sdialog.FileName);
				} else {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					BinaryWriter wr = new BinaryWriter(fstr);
					wr.Write((byte[])val);
					fstr.Close();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "Can't save resource to " + sdialog.FileName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
			}							
		}
		
		
		void OnChanged()
		{
			if (Changed != null) {
				Changed(this, null);
			}
		}
						
		object LoadResource(string name)
		{
			switch (Path.GetExtension(name).ToUpper()) {
				case ".CUR":
					try {
						return new Cursor(name);
					} catch {
						return null;
					}
				case ".ICO":
					try {
						return new Icon(name);
					} catch {
						return null;
					}
				default:
					// try to read a bitmap
					try { 
						return new Bitmap(name); 
					} catch {}
					
					// try to read a serialized object
					try {
						Stream r = File.Open(name, FileMode.Open);
						try {
							BinaryFormatter c = new BinaryFormatter();
							object o = c.Deserialize(r);
							r.Close();
							return o;
						} catch { r.Close(); }
					} catch { }
					
					// try to read a byte array :)
					try {
						FileStream s = new FileStream(name, FileMode.Open);
						BinaryReader r = new BinaryReader(s);
						Byte[] d = new Byte[(int) s.Length];
						d = r.ReadBytes((int) s.Length);
						s.Close();
						return d;
					} catch (Exception e) { 
						MessageBox.Show(e.Message, "Can't load resource " + name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
					}
				break;
			}
			return null;
		}
		
		
		public void RenameEvt(object sender, EventArgs e)
		{
			if (writeProtected || SelectedItems.Count != 1) {
				return;
			}
			
			ListViewItem lv = SelectedItems[0];
			string key = lv.Text;
			
			if (!resources.ContainsKey(key)) {
				return;
			}
			
			object val = resources[key];
			
			ShowBox:
			using(RenameEntry re = new RenameEntry(key)) {
				if (re.ShowDialog() == DialogResult.OK) {
					// if no change is made
					if (re.Value == key) {
						return;
					}
					// If new key exists already
					if(resources.ContainsKey(re.Value)) {
						ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
						DialogResult mbr = MessageBox.Show("This key is already defined! Please choose another one.", resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
						if (mbr == DialogResult.OK) {
							goto ShowBox;
						}
						return;
					}
					resources.Remove(key);
					resources.Add(re.Value, val);
					lv.Text = re.Value;
					lv.EnsureVisible();
					// not necessary
					//InitializeListView();
				}
			}
			OnChanged();
		}
		
		public void InitializeListView()
		{
			BeginUpdate();
			Items.Clear();
			
			foreach (DictionaryEntry entry in resources) {
				string type = entry.Value.GetType().FullName;
				string tmp  = String.Empty;
				
				switch (type) {
					case "System.String":
						tmp = entry.Value.ToString();
						break;
					case "System.Byte[]":
						tmp = "[Size = " + ((byte[])entry.Value).Length + "]";
						break;
					case "System.Drawing.Bitmap":
						tmp = "[Width = " + ((Bitmap)entry.Value).Size.Width + ", Height = " + ((Bitmap)entry.Value).Size.Height + "]";
						break;
					case "System.Drawing.Icon":
						tmp = "[Width = " + ((Icon)entry.Value).Size.Width + ", Height = " + ((Icon)entry.Value).Size.Height + "]";
						break;
				}
				ListViewItem lv = new ListViewItem(new String[] {entry.Key.ToString(), type, tmp}, ImageIndex(type));
				Items.Add(lv);
			}
			EndUpdate();
		}
		
		// new function to determine the image index of the specified type name
		private int ImageIndex(string type)
		{
			switch(type) {
				case "System.String":
					return 0;
				case "System.Drawing.Bitmap":
					return 1;
				case "System.Drawing.Icon":
					return 2;
				case "System.Windows.Forms.Cursor":
					return 3;
				case "System.Byte[]":
					return 4;
				default:
					return 5;
			}
		}
		
		protected void ClickItem(object sender, EventArgs e)
		{
			if (SelectedItems.Count != 1) {
				return;
			}
			
			string key = SelectedItems[0].Text;
			if (!resources.ContainsKey(key)) {
				return;
			}
			
			object val = resources[key];
			
			if (val is Icon) {
				BitmapView bv = new BitmapView("[" + key + "]", ((Icon)val).ToBitmap(), null);
				bv.Show();
			} else if (val is Bitmap) {
				BitmapView bv = new BitmapView("[" + key + "]", (Bitmap)val, null);
				bv.Show();
			} else if (val is Cursor) {
				Cursor c = (Cursor)val;
				Bitmap a = new Bitmap(c.Size.Width, c.Size.Height);
				Graphics g = Graphics.FromImage(a);
				g.FillRectangle(new SolidBrush(Color.DarkCyan), 0, 0, a.Width, a.Height);
				c.Draw(g, new Rectangle(0, 0, a.Width, a.Height));
				BitmapView bv = new BitmapView(Text + " [" + key + "]", a, null);
				bv.Show();
			} else if (val is string) {
				ListViewItem lv = SelectedItems[0];
				EditEntry ed = new EditEntry(key, val);
				if (writeProtected) {
					ed.Protect();
				}
				if (ed.ShowDialog() == DialogResult.OK) {
					if((string)ed.Entry.Key == key) {
						goto NoKeyChange;
					}
					if(resources.ContainsKey(ed.Entry.Key)) {
						ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
						MessageBox.Show("This key is already defined.",  resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				NoKeyChange:
					resources.Remove(key);
					resources.Add(ed.Entry.Key, ed.Entry.Value);
					OnChanged();
					lv.SubItems[0].Text = (string)ed.Entry.Key;
					lv.SubItems[2].Text = (string)ed.Entry.Value;
					// not necessary
					//InitializeListView();
				}
				ed.Dispose();
			} else if (val is byte[]) {
				BinaryView bv = new BinaryView((byte[])val, "Viewing " + key);;
				try {
					bv.Show();
				} catch {
					bv.Dispose();
				}
			} else {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				MessageBox.Show("This entry is of unknown type and cannot be viewed.", resourceService.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}
