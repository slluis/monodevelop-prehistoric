
// TODO: Port to GTK#

using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoQuery.Gui
{
	/*
	public class MonoQueryPanel : System.Windows.Forms.Panel
	{		
		private MonoQuery.Gui.TreeView.MonoQueryTree monoQueryTreeView;
		private System.Windows.Forms.ImageList monoQueryImageList;
		private System.Windows.Forms.ToolBar monoQueryToolBar;	
		private System.Windows.Forms.ToolBarButton btnRefresh;
		private System.Windows.Forms.ToolBarButton btnAddConnection;
		private System.Windows.Forms.ToolBarButton btnSep;
		
		
		public MonoQueryPanel() : base()
		{		
			IconService iconService = (IconService)ServiceManager.GetService(typeof(IconService));
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			
			this.monoQueryToolBar = new System.Windows.Forms.ToolBar();
			this.monoQueryImageList = new System.Windows.Forms.ImageList();
			this.monoQueryTreeView = new MonoQuery.Gui.TreeView.MonoQueryTree();
			this.btnRefresh = new System.Windows.Forms.ToolBarButton();
			this.btnAddConnection = new System.Windows.Forms.ToolBarButton();
			this.btnSep = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
					
			// 
			// monoQueryImageList
			// 
			this.monoQueryImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.monoQueryImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.monoQueryImageList.TransparentColor = System.Drawing.Color.DarkCyan;
			
			this.monoQueryImageList.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.AddConnection"));
			this.monoQueryImageList.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.Refresh"));								
						
			// 
			// toolBar
			// 
			this.monoQueryToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.monoQueryToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
								this.btnRefresh, this.btnSep, this.btnAddConnection});
			this.monoQueryToolBar.DropDownArrows = true;
			this.monoQueryToolBar.Location = new System.Drawing.Point(0, 0);
			this.monoQueryToolBar.Name = "toolBar";
			this.monoQueryToolBar.ShowToolTips = true;
			this.monoQueryToolBar.Size = new System.Drawing.Size(292, 42);
			this.monoQueryToolBar.TabIndex = 0;						
			this.monoQueryToolBar.ImageList = this.monoQueryImageList;
			
			this.btnRefresh.ImageIndex = 1;
			this.btnRefresh.ToolTipText = stringParserService.Parse("${res:MonoQuery.ToolTip.Refresh}");

			this.btnSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;

			this.btnAddConnection.ImageIndex = 0;
			this.btnAddConnection.ToolTipText = stringParserService.Parse("${res:MonoQuery.ToolTip.AddConnection}");			
			
			// 
			// treeView
			// 
			this.monoQueryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monoQueryTreeView.ImageIndex = -1;
			this.monoQueryTreeView.Location = new System.Drawing.Point(0, 42);
			this.monoQueryTreeView.Name = "treeView";
			this.monoQueryTreeView.SelectedImageIndex = -1;
			this.monoQueryTreeView.Size = new System.Drawing.Size(292, 224);
			this.monoQueryTreeView.TabIndex = 1;
			// 
			// CreatedForm
			// 
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.monoQueryTreeView);
			this.Controls.Add(this.monoQueryToolBar);
			this.Name = "MonoQueryPanel";
			this.ResumeLayout(false);									
		}		
	}*/
}
