// created on 16.11.2002 at 21:14

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

using ICSharpCode.FiletypeRegisterer;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	class RegisterFiletypesPanel : AbstractOptionPanel {
		
		ListView list   = new ListView();
		Label    capLbl = new Label();
		CheckBox regChk = new CheckBox();
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		Hashtable wasChecked = new Hashtable();
		
		public RegisterFiletypesPanel()
		{
			// Initialize dialog controls
			InitializeComponent();
			
			// Set previous values
			SelectFiletypes(propertyService.GetProperty(RegisterFiletypesCommand.uiFiletypesProperty, "cmbx|prjx"));
			regChk.Checked = propertyService.GetProperty(RegisterFiletypesCommand.uiRegisterStartupProperty, false);
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				UnRegisterFiletypes();
				RegisterFiletypesCommand.RegisterFiletypes(SelectedFiletypes);
				propertyService.SetProperty(RegisterFiletypesCommand.uiFiletypesProperty, SelectedFiletypes);
				propertyService.SetProperty(RegisterFiletypesCommand.uiRegisterStartupProperty, regChk.Checked);
			}
			return true;
		}
		
		string SelectedFiletypes
		{
			get {
				try {
					string ret = "";
					
					foreach(ListViewItem lv in list.Items) {
						if(lv.Checked) ret += (string)lv.Tag + "|";
					}
					return ret;
				} catch {
					return "";
				}
			}
		}
		
		void UnRegisterFiletypes()
		{
			foreach(ListViewItem lv in list.Items) {
				if((!lv.Checked) && wasChecked.Contains((string)lv.Tag)) {
					RegisterFiletypesCommand.UnRegisterFiletype((string)lv.Tag);
				}
			}
		}
		
		void SelectFiletypes(string types) {
			string[] singleTypes = types.Split('|');
			
			foreach(string str in singleTypes) {
				wasChecked[str] = true;
				foreach(ListViewItem lv in list.Items) {
					if(str == (string)lv.Tag) {
						lv.Checked = true;
					}
				}
			}
		}
		
		void InitializeComponent()
		{
			capLbl.Location  = new Point(8, 8);
			capLbl.Size      = new Size(136, 16);
			capLbl.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			capLbl.Text      = stringParserService.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.RegisterFiletypesPanel.CaptionLabel}");
			
			list.Location    = new Point(8, 30);
			list.Size        = new Size(136, 250);
			list.Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			list.View        = View.List;
			list.CheckBoxes  = true;
			
			FillList(list);
			
			regChk.Location  = new Point(8, 300);
			regChk.Size      = new Size(136, 20);
			regChk.Anchor    = capLbl.Anchor;
			regChk.Text      = stringParserService.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.RegisterFiletypesPanel.RegisterCheckBox}");
			
			this.Controls.AddRange(new Control[] {capLbl, list, regChk});
		}
		
		void FillList(ListView list)
		{
			string[,] Items = RegisterFiletypesCommand.GetFileTypes();
			
			for(int i = 0; i < Items.GetLength(0); ++i) {
				ListViewItem lv;
				lv = new ListViewItem(stringParserService.Parse(Items[i, 0]) + " (." + Items[i, 1] + ")");
				lv.Tag = Items[i, 1];
				list.Items.Add(lv);
			}
		}
	}
}
