// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class UIService : IUIService
	{
		IDictionary styles = new Hashtable();
		
		public IDictionary Styles {
			get {
				return styles;
			}
		}
		
		public UIService()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			styles["DialogFont"]     = resourceService.LoadFont("Tahoma", 10);
			styles["HighlightColor"] = Color.White;
		}
		
		public void SetUIDirty()
		{
			// TODO: Fixme!
		}
		
#region ComponentEditor functions
		public bool CanShowComponentEditor(object component)
		{
			return false;
		}
		
		public bool ShowComponentEditor(object component, IWin32Window parent)
		{
			throw new System.NotImplementedException("Cannot display component editor for " + component);
		}
#endregion
		
#region Dialog functions
		public IWin32Window GetDialogOwnerWindow()
		{
			return (IWin32Window)WorkbenchSingleton.Workbench;
		}
		
		public DialogResult ShowDialog(Form form)
		{
			return form.ShowDialog(GetDialogOwnerWindow());
		}
#endregion
		
#region Show error functions
		public void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		public void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		public void ShowError(Exception ex, string message)
		{
//			string msg = String.Empty;
//			
//			if (ex != null) {
//				msg = "Exception occurred: " + ex.ToString() + "\n";
//			}
//			
//			if (message != null) {
//				msg += message;
//			}
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			MessageBox.Show(GetDialogOwnerWindow(), ex.Message, resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
#endregion
		
#region Show Message functions
		public void ShowMessage(string message)
		{
			ShowMessage(message, "", MessageBoxButtons.OK);
		}
		
		public void ShowMessage(string message, string caption)
		{
			ShowMessage(message, caption, MessageBoxButtons.OK);
		}
		
		public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
		{
			return MessageBox.Show(GetDialogOwnerWindow(), message, caption, buttons);
		}
#endregion
		
		public bool ShowToolWindow(Guid toolWindow)
		{
			return false;
		}
	}
}
