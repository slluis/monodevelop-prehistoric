using System;
using System.Collections;

namespace MonoDevelop.Gui {
	public class ListView {
		private Gtk.TreeView treeview;
		private ListViewItemCollection items = new ListViewItemCollection();
		private ColumnHeaderCollection columns = new ColumnHeaderCollection();
		
		public ListView() {
			treeview = new Gtk.TreeView();
		}
		
		public Gtk.Widget Control {
			get {
				return treeview;
			}
		}
		
		public ListViewItemCollection Items {
			get {
				return items;
			}
		}
		
		public ColumnHeaderCollection Columns {
			get {
				return columns;
			}
		}
		
		public ListViewItemCollection SelectedItems {
			get {
				return items; // TODO
			}
		}
		
		public void BeginUpdate() {
			// TODO
		}
		
		public void EndUpdate() {
			// TODO
		}
	}
	
	public class ListViewItem {
		private string text;
		private ListViewItemCollection subItems = new ListViewItemCollection();
		
		public ListViewItem(string text) {
			this.text = text;
		}
		
		public ListViewItemCollection SubItems {
			get {
				return subItems;
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
	}
	
	public class ColumnHeaderCollection {
		public void Add(string title, int width, HorizontalAlignment align) {
			// TODO
		}
	}
	
	public class ListViewItemCollection: ArrayList {
		public ListViewItem Add(ListViewItem it) {
			return null; //TODO
		}
		
		public new ListViewItem this[int index] {
			get {
				return null; // TODO
			}
		}

	}
	
	public enum HorizontalAlignment {
		Left,
		Right
	}
}
