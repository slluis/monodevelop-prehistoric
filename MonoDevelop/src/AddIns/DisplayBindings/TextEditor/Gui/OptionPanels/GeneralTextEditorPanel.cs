// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Text;
using Gtk;
using Pango;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// General texteditor options panel.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{
		FontDescription selectedFont = FontDescription.FromString ("Courier New");
		//selectedFont.Size = 10;
		
		int encoding = Encoding.UTF8.CodePage;
		int selectedIndex = 0;
		
		CheckButton enableDoublebufferingCheckBox;
		CheckButton enableCodeCompletionCheckBox;
		CheckButton enableFoldingCheckBox;
		Entry fontNameDisplayTextBox;
		CheckButton enableAAFontRenderingCheckBox;
		
		public override void LoadPanelContents()
		{
			VBox mainVBox = new VBox (false, 0);
			Frame genOptions = new Frame ("General Options");
			Frame fontOptions = new Frame ("Font");
			Frame encOptions = new Frame ("File encoding");
			
			VBox genVBox = new VBox (true, 0);
			enableCodeCompletionCheckBox = new CheckButton ();
			enableCodeCompletionCheckBox.Label = "Enable Code Completion";
			enableCodeCompletionCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("EnableCodeCompletion", true);
			genVBox.PackStart (enableCodeCompletionCheckBox);
			
			enableFoldingCheckBox = new CheckButton ();
			enableFoldingCheckBox.Label = "Enable Code Folding";
			enableFoldingCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("EnableFolding", true);
			genVBox.PackStart (enableFoldingCheckBox);
			genOptions.Add (genVBox);
			
			enableDoublebufferingCheckBox = new CheckButton ();
			enableDoublebufferingCheckBox.Label = "Enable Double Buffering";
			enableDoublebufferingCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("DoubleBuffer", true);
			genVBox.PackStart (enableDoublebufferingCheckBox);
			
			VBox fontVBox = new VBox (true, 0);
			fontVBox.PackStart (new Label ("Text Font:"));
			
			HBox hb = new HBox (false, 0);
			fontNameDisplayTextBox = new Entry ();
			fontNameDisplayTextBox.Text = ((IProperties)CustomizationObject).GetProperty("DefaultFont", selectedFont).ToString();
			hb.PackStart (fontNameDisplayTextBox);
			
			Button browseButton = new Button ("_Select");
			browseButton.Clicked += new EventHandler(SelectFontEvent);
			hb.PackStart (browseButton);
			fontVBox.PackStart (hb);
			
			enableAAFontRenderingCheckBox = new CheckButton ();
			enableAAFontRenderingCheckBox.Label = "_Render font aliased";
			enableAAFontRenderingCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("UseAntiAliasFont", false);
			fontVBox.PackStart (enableAAFontRenderingCheckBox);
			fontOptions.Add (fontVBox);
			
			VBox encVBox = new VBox (true, 0);
			OptionMenu textEncodingComboBox = new OptionMenu ();
			textEncodingComboBox.Changed += new EventHandler (OnOptionChanged);
			
			Menu m = new Menu ();
			foreach (String name in CharacterEncodings.Names) {
				m.Append (new MenuItem (name));
			}
			textEncodingComboBox.Menu = m;
			encVBox.PackStart (new Label ("Choose _encoding"));
			encVBox.PackStart (textEncodingComboBox);
			encOptions.Add (encVBox);
			
			int i = 0;
			try {
				i = CharacterEncodings.GetEncodingIndex((Int32)((IProperties)CustomizationObject).GetProperty("Encoding", encoding));
			} catch {
				i = CharacterEncodings.GetEncodingIndex(encoding);
			}
			
			selectedIndex = i;
			encoding = CharacterEncodings.GetEncodingByIndex(i).CodePage;
			
			selectedFont = FontDescription.FromString (fontNameDisplayTextBox.Text);
			
			mainVBox.PackStart (genOptions, false, true, 0);
			mainVBox.PackStart (fontOptions, false, true, 0);
			mainVBox.PackStart (encOptions, false, true, 0);
			this.Add (mainVBox);
		}
		
		public override bool StorePanelContents()
		{
			((IProperties) CustomizationObject).SetProperty ("DoubleBuffer", enableDoublebufferingCheckBox.Active);
			((IProperties) CustomizationObject).SetProperty ("UseAntiAliasFont",     enableAAFontRenderingCheckBox.Active);
			((IProperties) CustomizationObject).SetProperty ("EnableCodeCompletion", enableCodeCompletionCheckBox.Active);
			((IProperties) CustomizationObject).SetProperty ("EnableFolding",        enableFoldingCheckBox.Active);
			((IProperties) CustomizationObject).SetProperty ("DefaultFont",          selectedFont);
			((IProperties) CustomizationObject).SetProperty ("Encoding",             CharacterEncodings.GetCodePageByIndex (selectedIndex));
			return true;
		}
		
		//static Font ParseFont(string font)
		//{
		//	string[] descr = font.Split(new char[]{',', '='});
		//	return new Font(descr[1], Single.Parse(descr[3]));
		//}
		
		void SelectFontEvent(object sender, EventArgs e)
		{
			FontSelectionDialog fdialog = new FontSelectionDialog ("Select a font");
				fdialog.SetFontName (selectedFont.ToString ());
				
				int response = fdialog.Run ();
				fdialog.Hide ();
				if (response == (int) ResponseType.Ok) {
					FontDescription newFont  = FontDescription.FromString (fdialog.FontName);
					fontNameDisplayTextBox.Text = newFont.ToString();
					selectedFont  = newFont;
					((IProperties)CustomizationObject).SetProperty("DefaultFont",          selectedFont);
					
				}
		}
		
		private void OnOptionChanged (object o, EventArgs args)
		{
			selectedIndex = ((OptionMenu) o).History;
		}
	}
}
