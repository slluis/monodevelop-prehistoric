
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using MonoDevelop.Gui;
using MonoQuery.Gui;
using MonoQuery.Gui.TreeView;
using MonoQuery.Services;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns;

//TODO : dans les MonoQueryList faire correspondre les restrictions vec les objets ajoutés
//TODO : dans les MonoQueryList faire correspondre les dataconnection avec les objets ajoutés
//TODO : ajout statistiques.

namespace MonoQuery.Pads
{
	/// <summary>
	/// This Pad Show a tree where you can add/remove databases connections.
	/// You can administrate databases from this tree.
	/// </summary>
	public class MonoQueryView : AbstractPadContent
	{		
		private static MonoQueryTree monoQueryTree = null;
		private static Gtk.ScrolledWindow scroller = null;
#region AbstractPadContent requirements
		public override Gtk.Widget Control {
			get {
				return scroller;
			}
		}
				
		/// <summary>
		/// Creates a new MonoQueryView object
		/// </summary>
		public MonoQueryView()
			: base( GettextCatalog.GetString( "Database" ), "md-mono-query-database")
		{
			CreateDefaultMonoQuery();
//			monoQueryTree.Dock = DockStyle.Fill;
		}
		
		void CreateDefaultMonoQuery()
		{
			scroller = new Gtk.ScrolledWindow();
			scroller.ShadowType = Gtk.ShadowType.In;
			monoQueryTree = new MonoQueryTree();
			scroller.Add( monoQueryTree );
			
			MonoQueryService service = (MonoQueryService)ServiceManager.GetService(
				typeof(MonoQueryService));
			service.Tree = monoQueryTree;
		}		
		
		public void SaveMonoQueryView()
		{
		}		
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);	
//			monoQueryTree.Refresh();
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			this.SaveMonoQueryView();
			monoQueryTree.Dispose();
		}
#endregion
	}
	
}
