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
			
			// Services
			StringParserService StringParserService = (StringParserService)ServiceManager.GetService (
										typeof (StringParserService));
			static ResourceService resourceService = (ResourceService)ServiceManager.GetService(
										typeof(IResourceService));
			static PropertyService propertyService = (PropertyService)ServiceManager.GetService(
										typeof(PropertyService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService (typeof (FileUtilityService));
			
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
				fd.Filename = buildOutputLoc.Text;
				int response = fd.Run ();
				fd.Hide ();
				if (response == (int) ResponseType.Ok)
					buildOutputLoc.Text = fd.Filename + System.IO.Path.DirectorySeparatorChar;
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
