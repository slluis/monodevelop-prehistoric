// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	public class CombineBuildOptions : AbstractOptionPanel
	{
		CombineBuildOptionsWidget widget;
		
		class CombineBuildOptionsWidget : GladeWidgetExtract 
		{
			// Gtk Controls
			[Glade.WidgetAttribute] Entry buildOutputLoc;
			[Glade.WidgetAttribute] Button OutputDirBrowse;
			
			Combine combine;

			public  CombineBuildOptionsWidget(IProperties CustomizationObject) : 
				base ("Base.glade", "CombineBuildOptions")
			{
				this.combine = (Combine)((IProperties)CustomizationObject).GetProperty("Combine");
				buildOutputLoc.Text = combine.OutputDirectory + System.IO.Path.DirectorySeparatorChar;
				OutputDirBrowse.Clicked += new EventHandler (onClicked);
			}

			void onClicked (object o, EventArgs e)
			{
				FolderDialog fd = new FolderDialog ("Output Directory");
				fd.SetFilename (buildOutputLoc.Text);
				int response = fd.Run ();
				if (response == (int) ResponseType.Ok)
					buildOutputLoc.Text = fd.Filename + System.IO.Path.DirectorySeparatorChar;
				fd.Hide ();
			}

			public bool Store()
			{
				combine.OutputDirectory = buildOutputLoc.Text;
				return true;
			}
		}

		public override void LoadPanelContents()
		{
			Add (widget = new  CombineBuildOptionsWidget ((IProperties) CustomizationObject));
		}

		public override bool StorePanelContents()
		{
			bool success = widget.Store ();
 			return success;
		}					
	}
}
