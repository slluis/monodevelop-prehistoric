// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Reflection.Emit;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;


namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionSearchPanel : UserControl
	{
		System.Windows.Forms.Label searchfor  = new System.Windows.Forms.Label();
		System.Windows.Forms.Label foundcount = new System.Windows.Forms.Label();
		TextBox   searchstringbox = new TextBox();
		ListView  itemsfound      = new ListView();
		Button    button          = new Button();
		ComboBox  searchtypes     = new ComboBox();
		
		ReflectionTree tree;
		ObjectBrowser.DisplayInformationWrapper _parent;
		
		public ReflectionSearchPanel(ReflectionTree tree)
		{
			Dock = DockStyle.Fill;
			
			this.tree = tree;
			
			searchfor.Text     = tree.ress.GetString("ObjectBrowser.Search.SearchFor");
			searchfor.Location = new Point(0, 0);
			searchfor.Size     = new Size(70, 12);
			searchfor.Anchor   = AnchorStyles.Top | AnchorStyles.Left;
			
			foundcount.Text      = "0 " + tree.ress.GetString("ObjectBrowser.Search.ItemsFound");
			foundcount.Location  = new Point(searchfor.Width + 5, 0);
			foundcount.Size      = new Size(Width - searchfor.Width - 5, searchfor.Height);
			foundcount.TextAlign = ContentAlignment.TopRight;
			foundcount.Anchor    = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
			
			searchstringbox.Location = new Point(0, 17);
			searchstringbox.Width    = Width;
			searchstringbox.Height   = 30;
			searchstringbox.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			searchstringbox.KeyUp    += new KeyEventHandler(searchbox_keyup);
			
			button.Location = new Point(Width - 52, 44);
			button.Size     = new Size(52, 21);
			button.Text     = tree.ress.GetString("ObjectBrowser.Search.Search");
			button.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			button.Click    += new EventHandler(Showtypes);
			
			searchtypes.Location      = new Point(0, 44);
			searchtypes.Width         = Width - 60;
			searchtypes.Height        = 30;
			searchtypes.Anchor        = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			searchtypes.DropDownStyle = ComboBoxStyle.DropDownList;
			searchtypes.Items.Add(tree.ress.GetString("ObjectBrowser.Search.TypesAndMembers"));
			searchtypes.Items.Add(tree.ress.GetString("ObjectBrowser.Search.TypesOnly"));
			searchtypes.SelectedIndex = 0;
			
			itemsfound.Location       = new Point(0, 71);
			itemsfound.Width          = Width;
			itemsfound.FullRowSelect  = true;
			itemsfound.MultiSelect    = false;
			itemsfound.Height         = Height - 71;
			itemsfound.Anchor         = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			itemsfound.View           = View.Details;
			itemsfound.SmallImageList = tree.ImageList;
			
			itemsfound.Columns.Add(tree.ress.GetString("ObjectBrowser.Search.Name"), 160, HorizontalAlignment.Left);
			itemsfound.Columns.Add(tree.ress.GetString("ObjectBrowser.Search.Type"),  70, HorizontalAlignment.Left);
//			itemsfound.Columns.Add("Namespace", 100, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Assembly",  125, HorizontalAlignment.Left);
			itemsfound.DoubleClick += new EventHandler(SelectItem);
			
			Controls.Add(button);
			Controls.Add(searchfor);
			Controls.Add(foundcount);
			Controls.Add(searchstringbox);
			Controls.Add(itemsfound);
			Controls.Add(searchtypes);
		}
		
		public ObjectBrowser.DisplayInformationWrapper ParentDisplayInfo {
			get {
				return _parent;
			}
			set {
				_parent = value;
			}
		}
		
		void SelectItem(object sender, EventArgs e)
		{
			if (itemsfound.SelectedItems.Count != 1)
				return;
			
			if(itemsfound.SelectedItems[0] is TypeItem) {
				TypeItem item = (TypeItem)itemsfound.SelectedItems[0];
				tree.GoToType(item.type);
				
			} else if (itemsfound.SelectedItems[0] is MemberItem) {
				MemberItem member = (MemberItem)itemsfound.SelectedItems[0];
				tree.GoToMember(member.member, member.assembly);
			}
						
			ParentDisplayInfo.leftTab.SelectedTab = ParentDisplayInfo.leftTab.TabPages[0];
		}
		
		class TypeItem : ListViewItem {
			public Type type;
			public TypeItem(Type type) : 
				base (new string[] {type.Name, GetType(type), Path.GetFileName(type.Assembly.CodeBase)})
			{
				this.type = type;
				this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(type);
			}

			private static string GetType(Type type) {
				if(type.IsEnum) {
					return "Enum";
				} else if(type.IsInterface) {
					return "Interface";
				} else if(type.IsValueType) {
					return "Structure";
				} else {
					return "Class";
				}
			}
		}

		
		class MemberItem : ListViewItem {
			public MemberInfo member;
			public Assembly assembly;

			public MemberItem(MemberInfo member, Assembly assembly) : 
				base (new string[] {member.DeclaringType.Name + "." + member.Name, GetType(member), Path.GetFileName(assembly.CodeBase)})
			{
				this.member = member;
				this.assembly = assembly;
				if(member is MethodInfo) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as MethodInfo);
				} else if(member is ConstructorInfo) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as ConstructorInfo);
				} else if(member is FieldInfo) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as FieldInfo);
				} else if(member is PropertyInfo) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as PropertyInfo);
				} else if(member is EventInfo) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as EventInfo);
				}
			}
			
			private static string GetType(MemberInfo member) {
				if(member is MethodInfo) {
					return "Method";
				} else if(member is ConstructorInfo) {
					return "Constructor";
				} else if(member is FieldInfo) {
					return "Field";
				} else if(member is PropertyInfo) {
					return "Property";
				} else if(member is EventInfo) {
					return "Event";
				} else {
					return "unknown";
				}
			}
		}
		
		void searchbox_keyup(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				Showtypes(sender, new EventArgs());
		}
		
		void Showtypes(object sender, EventArgs e)
		{
			bool searchMembers = (searchtypes.SelectedIndex == 0);
			
			if(searchstringbox.Text == "") return;
			string searchfor = searchstringbox.Text.ToLower();
			
			itemsfound.Items.Clear();
			itemsfound.BeginUpdate();
			
			foreach (Assembly asm in tree.Assemblies) {
				Type[] types;
				try {
					types = asm.GetTypes();
				} catch {
					try {
						types = asm.GetExportedTypes();
					} catch {
						types = new Type[0];
					}
				}
				foreach (Type type in types) {
					if(type.IsNotPublic && !(tree.showInternalTypes)) continue;
					if(type.IsNestedAssembly && !(tree.showInternalTypes)) continue;
					if(type.IsNestedFamANDAssem && !(tree.showInternalTypes)) continue;
					if(type.IsNestedPrivate && !(tree.showPrivateTypes)) continue;
					
					if (type.Name.ToLower().IndexOf(searchfor) >= 0) {
						itemsfound.Items.Add(new TypeItem(type));
					}
					
					if (!searchMembers) continue;
					
					MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
					foreach(MemberInfo member in members) {
						if(member.MemberType == MemberTypes.NestedType) continue;
						if(member is MethodInfo) {
							if (((MethodInfo)member).IsSpecialName) continue;
						}
						
						if(ReflectionTypeNode.IsInternalMember(member) && !(tree.showInternalMembers)) continue;
						if(ReflectionTypeNode.IsPrivateMember(member) && !(tree.showPrivateMembers)) continue;
						
						if(member.Name.ToLower().IndexOf(searchfor) >= 0) {
							itemsfound.Items.Add(new MemberItem(member, type.Assembly));
						}
					}
				}
			}
			
			itemsfound.EndUpdate();
			foundcount.Text = itemsfound.Items.Count.ToString() + " " + tree.ress.GetString("ObjectBrowser.Search.ItemsFound");
		}
		
	}
}
