// created on 07.08.2003 at 13:46
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using MSjogren.GacTool.FusionNative;
using ICSharpCode.Core.Services;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;


namespace ICSharpCode.SharpDevelop.FormDesigner.Gui
{
	public class AddComponentsDialog : BaseSharpDevelopForm
	{
		ArrayList selectedComponents;
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public ArrayList SelectedComponents {
			get {
				return selectedComponents;
			}
		}
		
		public AddComponentsDialog() : base(propertyService.DataDirectory + @"\resources\dialogs\AddSidebarComponentsDialog.xfrm")
		{
			Icon = null;
			PrintGACCache();
			
			ControlDictionary["browseButton"].Click += new System.EventHandler(this.browseButtonClick);
			((ListView)ControlDictionary["gacListView"]).SelectedIndexChanged += new System.EventHandler(this.gacListViewSelectedIndexChanged);
//			((TextBox)ControlDictionary["fileNameTextBox"]).TextChanged += new System.EventHandler(this.fileNameTextBoxTextChanged);
			ControlDictionary["okButton"].Click += new System.EventHandler(this.buttonClick);
			ControlDictionary["loadButton"].Click += new System.EventHandler(this.loadButtonClick);
		}
		
		void PrintGACCache()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
				
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
									
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string[] info = sb.ToString().Split(',');
				
				string name    = info[0];
				string version = info[1].Substring(info[1].LastIndexOf('=') + 1);
				
				ListViewItem item = new ListViewItem(new string[] {name, version});
				item.Tag = sb.ToString();
				((ListView)ControlDictionary["gacListView"]).Items.Add(item);
			}
		}
		
		void FillComponents(Assembly assembly)
		{
			((ListView)ControlDictionary["componentListView"]).BeginUpdate();
			((ListView)ControlDictionary["componentListView"]).Items.Clear();
			
			if (assembly != null) {
				Hashtable images = new Hashtable();
				ImageList il = new ImageList();
				// try to load res icon
				string[] imgNames = assembly.GetManifestResourceNames();
				
				foreach (string im in imgNames) {
					try {
						Bitmap b = new Bitmap(Image.FromStream(assembly.GetManifestResourceStream(im)));
						b.MakeTransparent();
						images[im] = il.Images.Count;
						il.Images.Add(b);
					} catch {}
				}
				try {
					Module[] ms = assembly.GetModules(false);
					((ListView)ControlDictionary["componentListView"]).SmallImageList = il;
					foreach (Module m in ms) {
						Type[] ts = m.GetTypes();
						foreach (Type t in ts) {
							if (!t.IsSubclassOf(typeof(System.Windows.Forms.Form))) {
								if (t.IsDefined(typeof(ToolboxItemFilterAttribute), true)  || t.IsDefined(typeof(ToolboxItemAttribute), true)) {
									object[] attributes = t.GetCustomAttributes(false);
											
									if (images[t.FullName + ".bmp"] == null) {
										if (t.IsDefined(typeof(ToolboxBitmapAttribute), false)) {
											foreach (object attr in attributes) {
												if (attr is ToolboxBitmapAttribute) {
													ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
													images[t.FullName + ".bmp"] = il.Images.Count;
													Bitmap b = new Bitmap(toolboxBitmapAttribute.GetImage(t));
													b.MakeTransparent();
													il.Images.Add(b);
													break;
												}
											}
										}
										
									}
									if (images[t.FullName + ".bmp"] != null) {
										ListViewItem newItem = new ListViewItem(t.Name);
										newItem.SubItems.Add(t.Namespace);
										newItem.SubItems.Add(assembly.ToString());
										newItem.SubItems.Add(assembly.Location);
										newItem.SubItems.Add(t.Namespace);
										newItem.ImageIndex = (int)images[t.FullName + ".bmp"];
										
										newItem.Checked  = true;
										ToolComponent toolComponent = new ToolComponent(t.FullName, assembly.FullName);
										toolComponent.IsEnabled    = true;
										newItem.Tag = toolComponent;
										((ListView)ControlDictionary["componentListView"]).Items.Add(newItem);
										ToolboxItem item = new ToolboxItem(t);
									}
									
								}
							}
						}
					}
				} catch (Exception e) {
					Console.WriteLine("got exception : " + e.ToString());
				}
			}
			((ListView)ControlDictionary["componentListView"]).EndUpdate();
		}
		
		/* changed this unexpected behaviour -- added a load button, G.B.
		void fileNameTextBoxTextChanged(object sender, System.EventArgs e)
		{
			if (File.Exists(this.fileNameTextBox.Text)) {
				try {
					FillComponents(Assembly.LoadFrom(this.fileNameTextBox.Text));
				} catch (Exception ex) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(ex);
				}
			}
		}
		*/
		
		void gacListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (((ListView)ControlDictionary["gacListView"]).SelectedItems != null && ((ListView)ControlDictionary["gacListView"]).SelectedItems.Count == 1) {
				string assemblyName = ((ListView)ControlDictionary["gacListView"]).SelectedItems[0].Tag.ToString();
				Assembly asm = Assembly.Load(assemblyName);
				FillComponents(asm);
			} else {
				FillComponents(null);
			}
		}
		
		void loadButtonClick(object sender, System.EventArgs e)
		{
			if (!System.IO.File.Exists(ControlDictionary["fileNameTextBox"].Text)) {
				MessageBox.Show("Please enter a valid file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			try {
				Assembly asm = Assembly.LoadFrom(ControlDictionary["fileNameTextBox"].Text);
				FillComponents(asm);
			} catch {
				MessageBox.Show("Please enter the file name of a valid .NET assembly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				FillComponents(null);
			}
		}
		
		void buttonClick(object sender, System.EventArgs e)
		{
			selectedComponents = new ArrayList();
			foreach (ListViewItem item in ((ListView)ControlDictionary["componentListView"]).Items) {
				if (item.Checked) {
					selectedComponents.Add((ToolComponent)item.Tag);
				}
			}
		}
		
		void browseButtonClick(object sender, System.EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["fileNameTextBox"].Text = fdiag.FileName;
				}
			}
		}
	}
}
