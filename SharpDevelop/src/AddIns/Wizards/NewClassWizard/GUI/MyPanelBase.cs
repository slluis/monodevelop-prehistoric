// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.ComponentModel;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;


namespace NewClassWizard {
		
	public abstract class MyPanelBase : AbstractWizardPanel	{
		
		protected System.ComponentModel.Container components = null;
		
		private IProperties customizeObject = null;
				
		private const string NEW_CLASS_INFO 	= "NewClassInfo";
		private const string CREATOR 			= "Creator";
		private const string SELECTED_PROJECT 	= "SelectedProject";
		private const string CODE_OPTIONS		= "CodeOptions";		
		private const string TEMPLATE 			= "Template";

		protected IProjectService projectManager {
			get {
				return (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			}
		}
		
		protected ProjectInfo SelectedProject {
			get {
				return (ProjectInfo)customizeObject.GetProperty(SELECTED_PROJECT);				
			}
		}

		protected Language language {
			get {
				return CodeProviderChooser.LanguageFromString(fileTemplate.LanguageName);
			}
		}
		
		protected NewClassInfo newClassInfo {
			get {
				return (NewClassInfo)customizeObject.GetProperty(NEW_CLASS_INFO);
			}
		}
		
		protected INewFileCreator newFileCreator {
			get {
				return (INewFileCreator)customizeObject.GetProperty(CREATOR);
			}
		} 
		
		protected FileTemplate fileTemplate {
			get {
				return (FileTemplate)customizeObject.GetProperty(TEMPLATE);
			}
		} 
		
		protected CodeFactoryOptions CodeOptions {
			get {
				return (CodeFactoryOptions)customizeObject.GetProperty(CODE_OPTIONS);
			}
			
			set {
				 customizeObject.SetProperty(CODE_OPTIONS, value);
			}
		}
		
		public override object CustomizationObject {
			get {
				return customizeObject;
			}
			
			set {				
				customizeObject = (IProperties)value;
				
				//if this is the first panel being loaded add some
				//objects to the property bag
				//these objects are shared accross all panels in the wizard
				if ( customizeObject.GetProperty(NEW_CLASS_INFO) == null ) {
					customizeObject.SetProperty( NEW_CLASS_INFO, new NewClassInfo() );
				}
				if ( customizeObject.GetProperty(SELECTED_PROJECT) == null ) {
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					customizeObject.SetProperty( SELECTED_PROJECT, new ProjectInfo( projectService.CurrentSelectedProject ) );
				}								
			}

		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			
			base.Dispose( disposing );
			
		}		
	}
	
}
