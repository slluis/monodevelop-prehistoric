using System;
using System.IO;
using System.Drawing;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace NemerleBinding
{
	public class OutputPanel : AbstractOptionPanel
	{
		class OutputPanelWidget : GladeWidgetExtract 
		{
 			[Glade.Widget] Entry assemblyName;
			[Glade.Widget] Entry outputPath;
 			[Glade.Widget] Entry parameters;
 			[Glade.Widget] Entry executeCommand;
 			[Glade.Widget] Button outputPathButton;
 			
			NemerleParameters compilerParameters = null;

 			public  OutputPanelWidget(IProperties CustomizationObject) : base ("Nemerle.glade", "OutputPanel")
 			{
				compilerParameters = (NemerleParameters)((IProperties)CustomizationObject).GetProperty("Config");
				
				outputPathButton.Clicked += new EventHandler(SelectFolder);
				assemblyName.Text   = compilerParameters.OutputAssembly;
				outputPath.Text     = compilerParameters.OutputDirectory;
				parameters.Text     = compilerParameters.Parameters;
				executeCommand.Text = compilerParameters.ExecuteScript;
 			}

			public bool Store ()
			{	
				compilerParameters.OutputAssembly  = assemblyName.Text;
				compilerParameters.OutputDirectory = outputPath.Text;
				compilerParameters.Parameters      = parameters.Text;
				compilerParameters.ExecuteScript   = executeCommand.Text;
				return true;
			}
			void SelectFolder(object sender, EventArgs e)			
			{
				using (FileSelector fdiag = new FileSelector ("Output Path")) 
				{
					if (fdiag.Run () == (int) ResponseType.Ok) 
						outputPath.Text = fdiag.Filename;
					fdiag.Hide();
				}
			}			
		}

		OutputPanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new  OutputPanelWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
 			return  widget.Store ();
		}
	}
}
