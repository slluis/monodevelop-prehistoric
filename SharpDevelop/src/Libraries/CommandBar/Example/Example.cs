// ---------------------------------------------------------
// Windows Forms CommandBar controls for .NET.
// Copyright (C) 2001-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// roeder@aisto.com
// ---------------------------------------------------------
namespace CommandBar.Example
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Windows.Forms;
	using Reflector.UserInterface;

	public class Example : Form
	{
		[STAThread()]
		public static void Main(String[] args)
		{
			Application.Run(new Example());
		}
	
		private CommandBarManager commandBarManager = new CommandBarManager();
		private CommandBar menuBar = new CommandBar(CommandBarStyle.Menu);	
		private CommandBar toolBar = new CommandBar(CommandBarStyle.ToolBar);
		
		private CommandBarItem enabledButton;
		private CommandBarItem visibleButton;
	
		public Example()
		{
			this.Icon = SystemIcons.Application;
			this.Text = "Example";
			this.Size = new Size(500, 500);
			this.Controls.Add(new StatusBar());
	
			// Menu and toolbar
			CommandBarButton newButton = new CommandBarButton(Images.New, "&New", null, Keys.Control | Keys.N);
			CommandBarButton openButton = new CommandBarButton(Images.Open, "&Open...", null, Keys.Control | Keys.O);
			CommandBarButton saveButton = new CommandBarButton(Images.Save, "&Save", null, Keys.Control | Keys.S);
	
			toolBar.Items.Add(newButton);
			toolBar.Items.Add(openButton);
			toolBar.Items.Add(saveButton);
			toolBar.Items.AddSeparator();
	
			CommandBarButton cutButton = new CommandBarButton(Images.Cut, "Cu&t", null, Keys.Control | Keys.X);
			CommandBarItem copyButton = new CommandBarButton(Images.Copy, "&Copy", null, Keys.Control | Keys.C);
			CommandBarItem pasteButton = new CommandBarButton(Images.Paste, "&Paste", null, Keys.Control | Keys.V);
			CommandBarItem deleteButton = new CommandBarButton(Images.Delete, "&Delete", null, Keys.Delete);
	
			this.toolBar.Items.Add(cutButton);
			this.toolBar.Items.Add(copyButton);
			this.toolBar.Items.Add(pasteButton);
			this.toolBar.Items.Add(deleteButton);
			this.toolBar.Items.AddSeparator();
	
			CommandBarButton undoButton = new CommandBarButton(Images.Undo, "&Undo", null, Keys.Control | Keys.Z);
			CommandBarButton redoButton = new CommandBarButton(Images.Redo, "&Redo", null, Keys.Control | Keys.Y);
			
			this.toolBar.Items.Add(undoButton);
			this.toolBar.Items.Add(redoButton);
			this.toolBar.Items.AddSeparator();
	
			CommandBarMenu fileMenu = menuBar.Items.AddMenu("&File");
			fileMenu.Items.Add(newButton);
			fileMenu.Items.Add(openButton);
			fileMenu.Items.Add(saveButton);
			fileMenu.Items.AddButton("&Save As...", null);
			fileMenu.Items.AddSeparator();
			fileMenu.Items.AddButton(Images.Preview, "Print Pre&view", null);
			fileMenu.Items.AddButton(Images.Print, "&Print", null, Keys.Control | Keys.P);
			fileMenu.Items.AddSeparator();
			fileMenu.Items.AddButton("E&xit", new EventHandler(this.Exit_Click));
		
			CommandBarMenu editMenu = this.menuBar.Items.AddMenu("&Edit");
			editMenu.Items.Add(undoButton);
			editMenu.Items.Add(redoButton);
			editMenu.Items.AddSeparator();
			editMenu.Items.Add(cutButton);
			editMenu.Items.Add(copyButton);
			editMenu.Items.Add(pasteButton);
			editMenu.Items.Add(deleteButton);
			editMenu.Items.AddSeparator();
			editMenu.Items.AddButton("Select &All", null, Keys.Control | Keys.A);
			
			CommandBarMenu viewMenu = this.menuBar.Items.AddMenu("&View");
			CommandBarMenu goToMenu = viewMenu.Items.AddMenu("&Go To");
			goToMenu.Items.AddButton(Images.Back, "&Back", null, Keys.Alt | Keys.Left);
			goToMenu.Items.AddButton(Images.Forward, "&Forward", null, Keys.Alt | Keys.Right);
			goToMenu.Items.AddSeparator();
			goToMenu.Items.AddButton(Images.Home, "&Home", null);
	
			viewMenu.Items.AddButton(Images.Stop, "&Stop", null, Keys.Escape);
			viewMenu.Items.AddButton(Images.Refresh, "&Refresh", null, Keys.F5);
	
			this.enabledButton = new CommandBarButton(Images.Tiles, "&Enabled", null);
			this.enabledButton.IsEnabled = false;
			this.visibleButton = new CommandBarButton(Images.Icons, "&Visible", null);
			this.visibleButton.IsVisible = false;
			CommandBarCheckBox checkedPlainCheckBox = new CommandBarCheckBox("&Checked Plain");
			checkedPlainCheckBox.IsChecked = true;
			CommandBarCheckBox checkedBitmapCheckBox = new CommandBarCheckBox(Images.List, "&Checked Bitmap");
			checkedBitmapCheckBox.IsChecked = true;

			toolBar.Items.Add(enabledButton);
			toolBar.Items.Add(visibleButton);
			toolBar.Items.Add(checkedPlainCheckBox);
			toolBar.Items.Add(checkedBitmapCheckBox);

			toolBar.Items.AddSeparator();

			ComboBox comboBox = new ComboBox();
			comboBox.Width = 100;
			toolBar.Items.AddComboBox("Combo Box", comboBox);

			toolBar.Items.AddSeparator();
	
			CommandBarMenu testMenu = menuBar.Items.AddMenu("&Test");
			testMenu.Items.AddButton("&Enabled On/Off", new EventHandler(ToggleEnabled_Click));
			testMenu.Items.Add(this.enabledButton);
			testMenu.Items.AddSeparator();
			testMenu.Items.AddButton("&Visible On/Off", new EventHandler(ToggleVisible_Click));
			testMenu.Items.Add(this.visibleButton);
			testMenu.Items.AddSeparator();
			testMenu.Items.Add(checkedPlainCheckBox);
			testMenu.Items.Add(checkedBitmapCheckBox);
			testMenu.Items.AddSeparator();
			testMenu.Items.AddButton("Change Font", new EventHandler(this.ChangeFont_Click));
	
			CommandBarMenu helpMenu = menuBar.Items.AddMenu("&Help");
			helpMenu.Items.AddButton(Images.Mail, "&Your Feedback", null);
			helpMenu.Items.AddSeparator();
			helpMenu.Items.AddButton("&About", null);
	
			this.commandBarManager.CommandBars.Add(this.menuBar);
			this.commandBarManager.CommandBars.Add(this.toolBar);
			this.Controls.Add(this.commandBarManager);
			
			// Context menu
			CommandBarContextMenu contextMenu = new CommandBarContextMenu();
			contextMenu.Items.Add(undoButton);
			contextMenu.Items.Add(redoButton);
			contextMenu.Items.AddSeparator();
			contextMenu.Items.Add(cutButton);
			contextMenu.Items.Add(copyButton);
			contextMenu.Items.Add(pasteButton);
			contextMenu.Items.Add(deleteButton);
			this.ContextMenu = contextMenu;
		}

		// Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this.commandBarManager.PreProcessMessage(ref msg))
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
	
		private void Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	
		private void ToggleEnabled_Click(object sender, EventArgs e)
		{
			this.enabledButton.IsEnabled = !this.enabledButton.IsEnabled;
		}
	
		private void ToggleVisible_Click(object sender, EventArgs e)
		{
			this.visibleButton.IsVisible = !this.visibleButton.IsVisible;
		}
	
		private void ChangeFont_Click(object sender, EventArgs e)
		{
			FontDialog dialog = new FontDialog();
			dialog.Font = menuBar.Font;
			
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				menuBar.Font = dialog.Font;
				toolBar.Font = dialog.Font;
			}
		}
	
		private class Images
		{
			private static Image[] images = null;
	
			// ImageList.Images[int index] does not preserve alpha channel.
			static Images()
			{
				// TODO alpha channel PNG loader is not working on .NET Service RC1
				Bitmap bitmap = new Bitmap("Example16.tif");
				int count = (int) (bitmap.Width / bitmap.Height);
				images = new Image[count];
				Rectangle rectangle = new Rectangle(0, 0, bitmap.Height, bitmap.Height);
				for (int i = 0; i < count; i++)
				{
					images[i] = bitmap.Clone(rectangle, bitmap.PixelFormat);
					rectangle.X += bitmap.Height;
				}
			}	
	
			public static Image New               { get { return images[0];  } }
			public static Image Open              { get { return images[1];  } }
			public static Image Save              { get { return images[2];  } }
			public static Image Cut               { get { return images[3];  } }
			public static Image Copy              { get { return images[4];  } }
			public static Image Paste             { get { return images[5];  } }
			public static Image Delete            { get { return images[6];  } }
			public static Image Properties        { get { return images[7];  } }
			public static Image Undo              { get { return images[8];  } }
			public static Image Redo              { get { return images[9];  } }
			public static Image Preview           { get { return images[10]; } }
			public static Image Print             { get { return images[11]; } }
			public static Image Search            { get { return images[12]; } }
			public static Image ReSearch          { get { return images[13]; } }
			public static Image Help              { get { return images[14]; } }
			public static Image ZoomIn            { get { return images[15]; } }
			public static Image ZoomOut           { get { return images[16]; } }
			public static Image Back              { get { return images[17]; } }
			public static Image Forward           { get { return images[18]; } }
			public static Image Favorites         { get { return images[19]; } }
			public static Image AddToFavorites    { get { return images[20]; } }
			public static Image Stop              { get { return images[21]; } }
			public static Image Refresh           { get { return images[22]; } }
			public static Image Home              { get { return images[23]; } }
			public static Image Edit              { get { return images[24]; } }
			public static Image Tools             { get { return images[25]; } }
			public static Image Tiles             { get { return images[26]; } }
			public static Image Icons             { get { return images[27]; } }
			public static Image List              { get { return images[28]; } }
			public static Image Details           { get { return images[29]; } }
			public static Image Pane              { get { return images[30]; } }
			public static Image Culture           { get { return images[31]; } }
			public static Image Languages         { get { return images[32]; } }
			public static Image History           { get { return images[33]; } }
			public static Image Mail              { get { return images[34]; } }
			public static Image Parent            { get { return images[35]; } }
			public static Image FolderProperties  { get { return images[36]; } }
		}
	}
}
