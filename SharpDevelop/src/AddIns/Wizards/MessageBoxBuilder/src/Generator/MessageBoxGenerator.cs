// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace Plugins.Wizards.MessageBoxBuilder.Generator {
	
	public class MessageBoxGenerator
	{
		MessageBoxButtons       messageBoxButtons       = MessageBoxButtons.OK;
		MessageBoxIcon          messageBoxIcon          = MessageBoxIcon.None;
		MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button1;
		
		string text    = String.Empty;
		string caption = String.Empty;
		
		bool   generateReturnValue = false;
		string variableName        = String.Empty;
		bool   generateSwitchCase  = false;
		
		public MessageBoxButtons MessageBoxButtons {
			get {
				return messageBoxButtons;
			}
			set {
				messageBoxButtons = value;
				OnChanged(null);
			}
		}

		public MessageBoxIcon MessageBoxIcon {
			get {
				return messageBoxIcon;
			}
			set {
				messageBoxIcon = value;
				OnChanged(null);
			}
		}

		public MessageBoxDefaultButton MessageBoxDefaultButton {
			get {
				return messageBoxDefaultButton;
			}
			set {
				messageBoxDefaultButton = value;
				OnChanged(null);
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				OnChanged(null);
			}
		}
		
		public string Caption {
			get {
				return caption;
			}
			set {
				caption = value;
				OnChanged(null);
			}
		}

		public bool GenerateReturnValue {
			get {
				return generateReturnValue;
			}
			set {
				generateReturnValue = value;
				OnChanged(null);
			}
		}

		public string VariableName {
			get {
				return variableName;
			}
			set {
				variableName = value;
				OnChanged(null);
			}
		}

		public bool GenerateSwitchCase {
			get {
				return generateSwitchCase;
			}
			set {
				generateSwitchCase = value;
				OnChanged(null);
			}
		}
		
		public void PreviewMessageBox()
		{
			MessageBox.Show(text, caption, messageBoxButtons, messageBoxIcon, messageBoxDefaultButton);
		}
		
		public string GenerateCode() 
		{
			string code = "MessageBox.Show(\"" + text.Replace("\n", "\\n").Replace("\r","") + "\", \"" + 
			                                     caption + "\", MessageBoxButtons." + 
			                                     messageBoxButtons + ", MessageBoxIcon." + 
			                                     messageBoxIcon + ", MessageBoxDefaultButton." + 
			                                     messageBoxDefaultButton +");";
			if (generateReturnValue) {
				code = "DialogResult " + variableName + " = " + code;
				if (generateSwitchCase) {
					code += "\nswitch(" + variableName +") {\n";
					switch(messageBoxButtons) {
						case MessageBoxButtons.AbortRetryIgnore:
							code += "\tcase DialogResult.Abort:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Retry:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Ignore:\n";
							code += "\t\tbreak;\n";
							break;
						case MessageBoxButtons.OK:
							code += "\tcase DialogResult.OK:\n";
							code += "\t\tbreak;\n";
							break;
						case MessageBoxButtons.OKCancel:
							code += "\tcase DialogResult.OK:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Cancel:\n";
							code += "\t\tbreak;\n";
							break;
						case MessageBoxButtons.RetryCancel:
							code += "\tcase DialogResult.Retry:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Cancel:\n";
							code += "\t\tbreak;\n";
							break;
						case MessageBoxButtons.YesNo:
							code += "\tcase DialogResult.Yes:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.No:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Ignore:\n";
							code += "\t\tbreak;\n";
							break;
						case MessageBoxButtons.YesNoCancel:
							code += "\tcase DialogResult.Yes:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.No:\n";
							code += "\t\tbreak;\n";
							code += "\tcase DialogResult.Cancel:\n";
							code += "\t\tbreak;\n";
							break;
					}
					code += "}";
				}
			}
			return code;
		}
		
		void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public event EventHandler Changed;
	}
}
