// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using System.Drawing.Design;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class DesignPanel : Panel
	{
		Control view;
		IDesignerHost host;
		
		public Control View {
			get {
				return view;
			}
		}
		
		public IDesignerHost Host {
			get {
				return host;
			}
			set {
				Debug.Assert(value != null);
				this.host = value;
			}
		}
		
		
		public DesignPanel(IDesignerHost host)
		{
			Debug.Assert(host != null);
			this.host      = host;
			this.BackColor = Color.White;
		}
		
		public void SetRootDesigner()
		{
			IRootDesigner rootDesigner = host.GetDesigner(host.RootComponent) as IRootDesigner;
			if (rootDesigner == null) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError("Can't create root designer for " + host.RootComponent);
				return;
			}
			
			if (!TechnologySupported(rootDesigner.SupportedTechnologies, ViewTechnology.WindowsForms)) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError("Root designer does not support Windows Forms view technology.");
				return;
			}
			if (view != null) {
				Controls.Clear();
				view.Dispose();
			}
			
			view = (Control)rootDesigner.GetView(ViewTechnology.WindowsForms);
			view.BackColor = Color.White;
			view.Dock = DockStyle.Fill;
			this.Controls.Add(view);
		}
		
		public void SetErrorState(string errors)
		{
			Disable();
			Controls.Add(new DesignErrorPanel(errors));
		}
		
		public void Disable()
		{
			Controls.Clear();
		}
		
		public void Enable()
		{
			Controls.Add(view);
		}
		
		bool TechnologySupported(ViewTechnology[] technologies, ViewTechnology requiredTechnology)
		{
			foreach (ViewTechnology technology in technologies) {
				if (technology == requiredTechnology) {
					return true;
				}
			}
			return false;
		}
	}
}
