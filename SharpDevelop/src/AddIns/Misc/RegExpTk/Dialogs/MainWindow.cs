// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

// thanks to Chris Wille who contributed
// the compile stuff

using System;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection;
using System.IO;

using Reflector.UserInterface;

using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.XmlForms;
using ICSharpCode.Core.Services;

namespace Plugins.RegExpTk {

	public class RegExpTkDialog : XmlForm
	{
		
		class QuickInsert
		{
			string name;
			string text;
			
			public QuickInsert(string name, string text)
			{
				Name = name;
				Text = text;
			}
			
			public string Name
			{
				get {
					return name;
				}
				set {
					name = value;
				}
			}
			
			public string Text
			{
				get {
					return text;
				}
				set {
					text = value;
				}
			}
		}
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		private ErrorProvider compileErrorProvider;
		private CommandBarContextMenu quickInsertMenu          = new CommandBarContextMenu();
		private CommandBarContextMenu matchListViewContextMenu = new CommandBarContextMenu();
		
		public RegExpTkDialog() : base(Application.StartupPath + @"\..\data\resources\dialogs\RegExpTkMainForm.xfrm")
		{
			ArrayList quickies = new ArrayList();
			quickies.Add(new QuickInsert("Ungreedy star", "*?"));
			quickies.Add(new QuickInsert("Word character value", "\\w"));
			quickies.Add(new QuickInsert("Non-word character value", "\\W"));
			quickies.Add(new QuickInsert("Whitespace character", "\\s"));
			quickies.Add(new QuickInsert("Non-whitespace character", "\\S"));
			quickies.Add(new QuickInsert("Digit character", "\\d"));
			quickies.Add(new QuickInsert("Non-digit character", "\\D"));
			
			foreach (QuickInsert insert in quickies) {
				SdMenuCommand cmd = new SdMenuCommand(this, insert.Name, new EventHandler(quickInsert));
				cmd.Tag           = insert.Text;
				quickInsertMenu.Items.Add(cmd);
			}
			
			matchListViewContextMenu.Items.Add(new SdMenuCommand(this, "Show groups", new EventHandler(MatchListViewContextMenu_Clicked)));
			
			
			((Button)ControlDictionary["OkButton"]).Click += new EventHandler(OkButton_Click);
			((CheckBox)ControlDictionary["ReplaceCheckBox"]).CheckedChanged += new EventHandler(ReplaceCheckBox_CheckedChanged);
			((ListView)ControlDictionary["GroupListView"]).SelectedIndexChanged += new EventHandler(GroupListView_SelectedIndexChanged);
			((ListView)ControlDictionary["GroupListView"]).DoubleClick += new EventHandler(GroupListView_DoubleClick);
			((ListView)ControlDictionary["GroupListView"]).MouseUp += new MouseEventHandler(GroupListView_MouseUp);
			((Button)ControlDictionary["ChooseAssemblyFileCompileButton"]).Click += new EventHandler(ChooseAssemblyFileCompileButton_Click);
			((Button)ControlDictionary["CreateAssemblyFileCompileButton"]).Click += new EventHandler(CreateAssemblyFile);
			((Button)ControlDictionary["quickInsertButton"]).MouseDown += new MouseEventHandler(showQuickInsertMenu);
			((Button)ControlDictionary["quickInsertButton"]).Image = resourceService.GetBitmap("Icons.16x16.PasteIcon");
			((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).Font = new Font(FontFamily.GenericMonospace, 9, FontStyle.Regular);
			
			ReplaceCheckBox_CheckedChanged((CheckBox)ControlDictionary["ReplaceCheckBox"], null);
		}
		
		void GroupListView_DoubleClick(object sender, EventArgs e)
		{
			if(((ListView)ControlDictionary["GroupListView"]).SelectedItems.Count > 0) {
				Match match = (Match)((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].Tag;
				showGroupForm(match);
			}
		}
		
		void MatchListViewContextMenu_Clicked(object sender, EventArgs e)
		{
			Match match = (Match)((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].Tag;
			showGroupForm(match);

		}
		
		void showGroupForm(Match match)
		{
			GroupForm groupform = new GroupForm(match);
			groupform.ShowDialog();
		}
		
		void GroupListView_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right && ((ListView)ControlDictionary["GroupListView"]).SelectedItems.Count > 0) {
				Point cords = new Point(((ListView)ControlDictionary["GroupListView"]).Left + e.X, ((ListView)ControlDictionary["GroupListView"]).Top + e.Y + 30);
				matchListViewContextMenu.Show(this, cords);
			}
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		private void quickInsert(object sender, EventArgs e)
		{
			((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).SelectedText += (string)((SdMenuCommand)sender).Tag;
		}
		
		private void showQuickInsertMenu(object sender, MouseEventArgs e)
		{
			((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).Select();
			Point cords = new Point(((Button)ControlDictionary["quickInsertButton"]).Left + e.X, ((Button)ControlDictionary["quickInsertButton"]).Top + e.Y + 30);
			quickInsertMenu.Show(this, cords);
		}
		
		private void CreateAssemblyFile(object sender, EventArgs e)
		{
			RegexOptions options = RegexOptions.Compiled;
			
			if(compileErrorProvider != null) {
				compileErrorProvider.Dispose();
				compileErrorProvider = null;
			}
			compileErrorProvider = new ErrorProvider();
			
			// validate input
			
			bool error = false;
			
			if(((TextBox)ControlDictionary["ClassNameCompileTextBox"]).Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["ClassNameCompileTextBox"], resourceService.GetString("RegExpTk.Messages.ClassNameMissing"));
				error = true;
			}
			
			if(ControlDictionary["RegularExpressionCompileTextBox"].Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["RegularExpressionCompileTextBox"], resourceService.GetString("RegExpTk.Messages.RegexMissing"));
				error = true;
			}
			
			if(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"], resourceService.GetString("RegExpTk.Messages.FilenameMissing"));
				error = true;
			}
			
			foreach(char invalidChar in Path.InvalidPathChars)
			{
				if(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text.IndexOf(invalidChar) != -1) {
					compileErrorProvider.SetError((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"], resourceService.GetString("RegExpTk.Messages.FilenameInvalid"));
					error = true;
				}
			}
			
			if(error) return;
			
			// set options
			if(((CheckBox)ControlDictionary["IgnoreCaseCompileCheckBox"]).Checked)
				options = options | RegexOptions.IgnoreCase;
			
			if(((CheckBox)ControlDictionary["SingleLineCompileCheckBox"]).Checked)
				options = options | RegexOptions.Singleline;
			
			if(((CheckBox)ControlDictionary["IgnoreWhitespaceCompileCheckBox"]).Checked)
				options = options | RegexOptions.IgnorePatternWhitespace;
			
			if(((CheckBox)ControlDictionary["ExplicitCaptureCompileCheckBox"]).Checked)
				options = options | RegexOptions.ExplicitCapture;
			
			if(((CheckBox)ControlDictionary["EcmaScriptCompileCheckBox"]).Checked)
				options = options | RegexOptions.ECMAScript;
			
			if(((CheckBox)ControlDictionary["MultilineCompileCheckBox"]).Checked)
				options = options | RegexOptions.Multiline;
			
			if(((CheckBox)ControlDictionary["RightToLeftCompileCheckBox"]).Checked)
				options = options | RegexOptions.RightToLeft;
			
			try {
				Regex re = new Regex(((TextBox)ControlDictionary["RegularExpressionCompileTextBox"]).Text, options);
			} catch (ArgumentException ae) {
				MessageBox.Show(resourceService.GetString("RegExpTk.Messages.CreationError") + " " + ae.Message);
				return;
			}
			
			RegexCompilationInfo  rci = new RegexCompilationInfo(((TextBox)ControlDictionary["RegularExpressionCompileTextBox"]).Text,
				options, 
				((TextBox)ControlDictionary["ClassNameCompileTextBox"]).Text, 
				((TextBox)ControlDictionary["NamespaceCompileTextBox"]).Text,
				((CheckBox)ControlDictionary["PublibVisibleCompileCheckBox"]).Checked);
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = Path.GetFileNameWithoutExtension(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text);
			
			RegexCompilationInfo[] rciArray = new RegexCompilationInfo[] { rci };
			
			try {
				Regex.CompileToAssembly(rciArray, asmName);
			} catch (ArgumentException ae) {
				MessageBox.Show(resourceService.GetString("RegExpTk.Messages.CompilationError") + " " + ae.Message);
			}
			
			string aboluteFileName =  Path.GetFullPath(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text);
			((StatusBar)ControlDictionary["StatusBar"]).Text = resourceService.GetString("RegExpTk.Messages.FileCreated") + " " + aboluteFileName;
		}
		
		private void ChooseAssemblyFileCompileButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			
			sfd.InitialDirectory = "c:\\";
			sfd.Filter = "Assemblies (*.dll)|*.dll";
			sfd.DefaultExt = "dll";
			sfd.CheckPathExists = true;
			
			if (sfd.ShowDialog() == DialogResult.OK) {
				((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text = sfd.FileName;
			}
		}
		
		private void OkButton_Click(object sender, System.EventArgs e)
		{
			MatchCollection matches = null;
			RegexOptions options = new RegexOptions();
			((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).ForeColor = System.Drawing.Color.Black;
			
			if(((CheckBox)(ControlDictionary["IgnoreCaseCheckBox"])).Checked) {
				options = options | RegexOptions.IgnoreCase;
			}
			
			if(((CheckBox)(ControlDictionary["MultilineCheckBox"])).Checked) {
				options = options | RegexOptions.Multiline;
			}
			
			((ListView)ControlDictionary["GroupListView"]).Items.Clear();
			
			try {
				matches = Regex.Matches(((RichTextBox)ControlDictionary["InputTextBox"]).Text, ((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).Text, options);
				if(((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked) {
					((TextBox)ControlDictionary["ReplaceResultTextBox"]).Text = Regex.Replace(((RichTextBox)ControlDictionary["InputTextBox"]).Text, ((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).Text, ((TextBox)ControlDictionary["ReplacementStringTextBox"]).Text, options);
				}
			}
			catch(Exception exception) {
				((RichTextBox)ControlDictionary["RegularExpressionTextBox"]).ForeColor = System.Drawing.Color.Red;
				((StatusBar)ControlDictionary["StatusBar"]).Text = exception.Message;
				return;
			}
			
			if(matches.Count != 1) {
				((StatusBar)ControlDictionary["StatusBar"]).Text = matches.Count.ToString() + " " + resourceService.GetString("RegExpTk.Messages.Match");
			} else {
				((StatusBar)ControlDictionary["StatusBar"]).Text = matches.Count.ToString() + " " + resourceService.GetString("RegExpTk.Messages.Matches");
			}
			
			RichTextBox inputBox = (RichTextBox)ControlDictionary["InputTextBox"];
			
			TextBox dummy = new TextBox();
			dummy.Text = inputBox.Text;
			inputBox.Text =  dummy.Text;
			
			inputBox.Select(0, inputBox.Text.Length);
			inputBox.SelectionColor = Color.Black;
			inputBox.SelectionFont = dummy.Font;
			
			foreach (Match match in matches) {
				inputBox.Select(match.Index, match.Length);
				inputBox.SelectionColor = Color.Blue;
				
				ListViewItem lvwitem = ((ListView)ControlDictionary["GroupListView"]).Items.Add(match.ToString());
				lvwitem.Tag = match;
				lvwitem.SubItems.Add(match.Index.ToString());
				lvwitem.SubItems.Add((match.Index + match.Length).ToString());
				lvwitem.SubItems.Add(match.Length.ToString());
				lvwitem.SubItems.Add(match.Groups.Count.ToString());
			}
			inputBox.Select(0, 0);
		}
		
		private void GroupListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try 
			{
				((RichTextBox)ControlDictionary["InputTextBox"]).Select(System.Convert.ToInt32(((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].SubItems[1].Text),
				                                                         System.Convert.ToInt32(((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].SubItems[3].Text));
			} catch {
			}
		}
		
		private void ReplaceCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			((TextBox)ControlDictionary["ReplaceResultTextBox"]).Enabled = ((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked;
			((TextBox)ControlDictionary["ReplacementStringTextBox"]).Enabled = ((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked;
		}
		
	}
}
