using System;
using System.Collections;

using Gtk;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;

namespace MonoDevelop.Gui.Completion
{
	public class CompletionListWindow : ListWindow, IListDataProvider
	{
		ICompletionWidget completionWidget;
		ICompletionData[] completionData;
		DeclarationViewWindow declarationviewwindow = new DeclarationViewWindow ();
		static DataComparer dataComparer = new DataComparer ();
		
		class DataComparer: IComparer
		{
			public int Compare (object x, object y)
			{
				ICompletionData d1 = x as ICompletionData;
				ICompletionData d2 = y as ICompletionData;
				return String.Compare (d1.Text[0], d2.Text[0], true);
			}
		}
		
		static CompletionListWindow wnd;
		
		static CompletionListWindow ()
		{
			wnd = new CompletionListWindow ();
		}
		
		public CompletionListWindow ()
		{
			SizeAllocated += new SizeAllocatedHandler (ListSizeChanged);
		}
		
		public static void ShowWindow (char firstChar, ICompletionDataProvider provider, ICompletionWidget completionWidget)
		{
			try {
				if (!wnd.ShowListWindow (firstChar, provider,  completionWidget))
					return;
				
				// makes control-space in midle of words to work
				string text = wnd.completionWidget.CompletionText;
				if (text.Length == 0)
					return;
				
				wnd.PartialWord = text; 
				//if there is only one matching result we take it by default
				if (wnd.IsUniqueMatch)
				{	
					wnd.Hide ();
				}
				
				wnd.UpdateWord ();
				
				wnd.PartialWord = wnd.CompleteWord;
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}
		
		bool ShowListWindow (char firstChar, ICompletionDataProvider provider, ICompletionWidget completionWidget)
		{
			this.completionWidget = completionWidget;
			
			completionData = provider.GenerateCompletionData (completionWidget, firstChar);

			if (completionData == null || completionData.Length == 0) return false;
			
			this.Style = completionWidget.GtkStyle;
			
			Array.Sort (completionData, dataComparer);
			
			DataProvider = this;

			int x = completionWidget.TriggerXCoord;
			int y = completionWidget.TriggerYCoord;
			
			int w, h;
			GetSize (out w, out h);
			
			if ((x + w) > Screen.Width)
				x = Screen.Width - w;
			
			if ((y + h) > Screen.Height)
			{
				y = y - completionWidget.TriggerTextHeight - h;
			}

			Move (x, y);
			
			Show ();
			return true;
		}
		
		public static void HideWindow ()
		{
			wnd.Hide ();
		}
		
		public static bool ProcessKeyEvent (Gdk.EventKey e)
		{
			if (!wnd.Visible) return false;
			
			ListWindow.KeyAction ka = wnd.ProcessKey (e);
			
			if ((ka & ListWindow.KeyAction.CloseWindow) != 0)
				wnd.Hide ();
				
			if ((ka & ListWindow.KeyAction.Complete) != 0) {
				wnd.UpdateWord ();
			}
			
			if ((ka & ListWindow.KeyAction.Ignore) != 0)
				return true;

			if ((ka & ListWindow.KeyAction.Process) != 0) {
				if (e.Key == Gdk.Key.Left) {
					wnd.declarationviewwindow.OverloadLeft ();
					return true;
				} else if (e.Key == Gdk.Key.Right) {
					wnd.declarationviewwindow.OverloadRight ();
					return true;
				}
			}

			return false;
		}
		
		void UpdateWord ()
		{
			completionWidget.SetCompletionText(wnd.PartialWord, wnd.CompleteWord);
		}
		
		public new void Hide ()
		{
			base.Hide ();
			declarationviewwindow.HideAll ();
		}
		
		void ListSizeChanged (object obj, SizeAllocatedArgs args)
		{
			// FIXME: crashes on System.Runtime.
			// first line array out of bounds
			//UpdateDeclarationView ();
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			if (evnt.Button == 1 && evnt.Type == Gdk.EventType.TwoButtonPress) {
				wnd.Hide ();
				wnd.UpdateWord ();
			}
			return ret;
		}
		
		protected override void OnSelectionChanged ()
		{
			base.OnSelectionChanged ();
			UpdateDeclarationView ();
		}
		
		void UpdateDeclarationView ()
		{
			ICompletionData data = completionData[List.Selection];
			
			declarationviewwindow.Hide ();
			declarationviewwindow.Clear ();
			
			if (List.GdkWindow == null) return;
			Gdk.Rectangle rect = List.GetRowArea (List.Selection);
			int listpos_x = 0, listpos_y = 0;
			while (listpos_x == 0 || listpos_y == 0)
				GetPosition (out listpos_x, out listpos_y);
			int vert = listpos_y + rect.Y;
			
			int lvWidth = 0, lvHeight = 0;
			while (lvWidth == 0)
				this.GdkWindow.GetSize (out lvWidth, out lvHeight);
			if (vert >= listpos_y + lvHeight - 2) {
				vert = listpos_y + lvHeight - rect.Height;
			} else if (vert < listpos_y) {
				vert = listpos_y;
			}
			int horiz = listpos_x + lvWidth + 2;

			ICompletionDataWithMarkup datawMarkup = data as ICompletionDataWithMarkup;

			string descMarkup = datawMarkup != null ? datawMarkup.DescriptionPango : data.Description;

			declarationviewwindow.Realize ();

			declarationviewwindow.AddOverload (descMarkup);

			CodeCompletionData ccdata = (CodeCompletionData) data;

			foreach (CodeCompletionData odata in ccdata.GetOverloads ()) {
				ICompletionDataWithMarkup odatawMarkup = odata as ICompletionDataWithMarkup;
				declarationviewwindow.AddOverload (odatawMarkup == null ? odata.Description : odatawMarkup.DescriptionPango);
			}

			if (declarationviewwindow.DescriptionMarkup.Length == 0)
				return;

			int dvwWidth, dvwHeight;

			declarationviewwindow.Move (this.Screen.Width+1, vert);
			
			declarationviewwindow.ReshowWithInitialSize ();
			declarationviewwindow.ShowAll ();
			declarationviewwindow.Multiple = (ccdata.Overloads != 0);

			declarationviewwindow.GdkWindow.GetSize (out dvwWidth, out dvwHeight);

			if (this.Screen.Width <= horiz + dvwWidth) {
				horiz = listpos_x - dvwWidth - 10;
			}
			
			declarationviewwindow.Move (horiz, vert);
		}
		
		public int ItemCount 
		{ 
			get { return completionData.Length; } 
		}
		
		public string GetText (int n)
		{
			return completionData[n].Text[0];
		}
		
		public Gdk.Pixbuf GetIcon (int n)
		{
			return RenderIcon (completionData[n].Image, Gtk.IconSize.Menu, "");
		}
	}
}
