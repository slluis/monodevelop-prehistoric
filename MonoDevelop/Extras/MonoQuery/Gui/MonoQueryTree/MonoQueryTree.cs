// created on 04/11/2003 at 16:05

namespace MonoQuery.Gui.TreeView
{

using System;
using System.Xml;
using System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Gui.Widgets;

using MonoQuery.Gui;
using MonoQuery.Collections;
using MonoQuery.Codons;

using RS = MonoDevelop.Core.Services.ResourceService;

	///<summary>
	/// This class shows all databases connections in a treeview. 
	///</summary>	
	public class MonoQueryTree : MonoDevelop.Gui.Widgets.TreeView
	{									
//		private System.Windows.Forms.ImageList pNodeImages;
		private MenuService menuService = null;

		public static MonoQueryStringDictionary SchemaClassDict;		

		///<summary>
		/// Create a MonoQueryTree objec 
		///</summary>			
		public MonoQueryTree() : base()
		{		
//			IconService iconService = (IconService)ServiceManager.GetService(typeof(IconService));
			
			this.menuService = (MenuService)ServiceManager.GetService(typeof(MenuService));
			
//			this.pNodeImages = new ImageList();
//			this.pNodeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
//			this.pNodeImages.ImageSize = new System.Drawing.Size(16, 16);
//			this.pNodeImages.TransparentColor = System.Drawing.Color.DarkCyan;
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.DataBaseRoot"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.DatabaseConnectionClose"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.DatabaseConnection"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.TablesRoot"));	
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.ViewsRoot"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.ProceduresRoot"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.Table"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.View"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.Procedure"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.Column"));
//			this.pNodeImages.Images.Add(iconService.GetBitmap("Icons.16x16.MonoQuery.NodeError"));
//			this.ImageList = this.pNodeImages;
			
			SchemaClassDict = new MonoQueryStringDictionary();
						
			LoadMonoQueryConnectionCodon();
			
			//Add the Root Node.
			this.Nodes.Add( new MonoQueryNodeDatabaseRoot() );
			
			// Wire click event
			this.ButtonReleaseEvent += new Gtk.ButtonReleaseEventHandler( OnButtonRelease );		
		}
		
		protected void LoadMonoQueryConnectionCodon()
		{
			IAddInTreeNode AddinNode;		
			
			AddinNode = (IAddInTreeNode)AddInTreeSingleton.AddInTree.GetTreeNode("/MonoQuery/Connection");
			foreach ( DictionaryEntry entryChild in AddinNode.ChildNodes)
			{
				IAddInTreeNode ChildNode = entryChild.Value as IAddInTreeNode;
				if ( ChildNode != null )
				{
					MonoQueryConnectionCodon codon = ChildNode.Codon as MonoQueryConnectionCodon;
					if ( codon != null )
					{
					  if ( SchemaClassDict.Contains( codon.Schema ) )
					  {
					  	SchemaClassDict[codon.Schema] = codon.Node;
					  }
					  else
					  {
					  	SchemaClassDict.Add(codon.Schema,codon.Node);
					  }					  	
					}					
				}
			}			
		}
		
//		///<summary>
//		/// Select the node under the mouse cursor
//		///</summary>		
//		protected override void OnMouseDown(MouseEventArgs e)
//		{						
//			if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
//			{
//				this.SelectedNode = this.GetNodeAt( e.X, e.Y );
//			}
//			
//			
//			base.OnMouseDown(e);
//		}
//		
//		///<summary>
//		/// Display the context menu associated with a node type 
//		///</summary>		
//		protected override void OnMouseUp(MouseEventArgs e)
//		{						
//			if (e.Button == MouseButtons.Right && this.SelectedNode != null && SelectedNode is IMonoQueryNode) {
//				IMonoQueryNode selectedBrowserNode = SelectedNode as IMonoQueryNode;		
//				if ( selectedBrowserNode.AddinContextMenu != "" )
//				{
//					menuService.ShowContextMenu(this, selectedBrowserNode.AddinContextMenu, this, e.X, e.Y);
//				}
//			}
//
//			base.OnMouseUp(e);
//		}

		/// <summary>
		/// Display the context menu associated with a node type
		/// </summary>
		public void OnButtonRelease( object o, Gtk.ButtonReleaseEventArgs args )
		{
			if ( args.Event.Button == 3			// Right Click
				&& this.SelectedNode != null
				&& this.SelectedNode is IMonoQueryNode )
			{
				IMonoQueryNode selectedBrowserNode = SelectedNode as IMonoQueryNode;
				if ( selectedBrowserNode.AddinContextMenu != "" )
				{
					menuService.ShowContextMenu( this, selectedBrowserNode.AddinContextMenu, this );
				}
			}
		}

//		
//		protected override void OnItemDrag(ItemDragEventArgs e)
//		{
//			base.OnItemDrag(e);
//			AbstractMonoQueryNode node = e.Item as AbstractMonoQueryNode;
//			if (node != null) 
//			{
//				DataObject dataObject = null;
//				
//				if ( node.SchemaClass != null )
//				{
//					dataObject = node.SchemaClass.DragObject;
//					
//					if (dataObject != null )
//					{
//						dataObject.SetData(node.GetType() , node);
//						DoDragDrop(dataObject, DragDropEffects.All);
//					}
//				}												
//			}
//		}
		
		protected override void OnBeforeExpand( TreeViewCancelEventArgs e )
		{
			MonoQueryNodeConnection node = e.Node as MonoQueryNodeConnection;
			
			if ( node != null )
			{
				if ( node.IsConnected == false )
				{
					node.Connect();
				}
			}

			IMonoQueryNode myNode = e.Node as IMonoQueryNode;
			
			if ( myNode != null )
			{
				myNode.Refresh();
			}

			base.OnBeforeExpand( e );
		}

		
//		protected override void OnAfterExpand(TreeViewEventArgs e)
//		{	
//			IMonoQueryNode node = e.Node as IMonoQueryNode;
//
//			if ( node != null )
//			{
//				node.Refresh();
//			}
//			
//			base.OnAfterExpand( e );
//		}	
//				
//		protected override void OnAfterCollapse( TreeViewEventArgs e )
//		{
//			IMonoQueryNode node = e.Node as IMonoQueryNode;
//			
//			if ( node != null )
//			{
//				node.Clear();
//			}
//			
//			base.OnAfterCollapse( e );
//		}
	}
}
