// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Serialization;
using MonoDevelop.Internal.Templates;

namespace MonoDevelop.Services
{
	/// <summary>
	/// This interface describes the basic functions of the 
	/// SharpDevelop project service.
	/// </summary>
	public interface IProjectService
	{
		/// <remarks>
		/// Gets/Sets the current selected project. (e.g. the project
		/// that contains the file that has the focus)
		/// </remarks>
		Project CurrentSelectedProject {
			get;
			set;
		}
		
		/// <remarks>
		/// Gets/Sets the current selected combine. (e.g. the combine
		/// that contains the CurrentSelectedProject)
		/// </remarks>
		Combine CurrentSelectedCombine {
			get;
			set;
		}
		
		/// <remarks>
		/// Gets the root combine, if no combine is open it returns null. 
		/// </remarks>
		Combine CurrentOpenCombine {
			get;
		}
		
		bool IsCombineEntryFile (string filename);
		
		/// <remarks>
		/// Returns true, if one open project that should be compiled is dirty.
		/// </remarks>
		bool NeedsCompiling {
			get;
		}
		
		DataContext DataContext {
			get;
		}
		
		FileFormatManager FileFormats {
			get;
		}
		
		CombineEntry ReadFile (string file);

		void WriteFile (string file, CombineEntry entry);
		
		/// <remarks>
		/// Closes the root combine
		/// </remarks>
		void CloseCombine();
		
		/// <remarks>
		/// Closes the root combine
		/// </remarks>
		void CloseCombine(bool saveCombinePreferencies);
		
		/// <remarks>
		/// Compile the root combine.
		/// </remarks>
		void CompileCombine();
		
		/// <remarks>
		/// Compile the root combine. Forces Recompile for all projects.
		/// </remarks>
		void RecompileAll();
		
		/// <remarks>
		/// Compiles a specific project, if the project isn't dirty this
		/// method does nothing
		/// </remarks>
		void CompileProject(Project project);
		
		/// <remarks>
		/// Compiles a specific project (forced!)
		/// </remarks>
		void RecompileProject(Project project);
		
		/// <remarks>
		/// Opens a new root combine, closes the old root combine automatically.
		/// </remarks>
		void OpenCombine(string filename);
		
		/// <remarks>
		/// Saves the whole root combine.
		/// </remarks>
		void SaveCombine();
		
		/// <remarks>
		/// Saves the IDE preferences for the root combine (open files, class browser
		/// status etc.) SHOULD NOT BE CALLED BY YOU ! (except you know what you do)
		/// </remarks>
		void SaveCombinePreferences();
		
		/// <remarks>
		/// Mark a file dirty, the project in which the file is in will be compiled
		/// in the next compiler run.
		/// </remarks>
		void MarkFileDirty(string filename);
		
		/// <remarks>
		/// Only to be called by the compile actions.
		/// </remarks>
		void OnStartBuild();
		
		/// <remarks>
		/// Only to be called by the compile actions.
		/// </remarks>
		void OnEndBuild(bool success);
		
		/// <remarks>
		/// Only to be called by the compile actions.
		/// </remarks>
		void OnBeforeStartProject();
		
		/// <remarks>
		/// Removes a file from it's project(s)
		/// </remarks>
		void RemoveFileFromProject(string fileName);

		Project CreateSingleFileProject (string file);
		
		Project CreateProject (string type, ProjectCreateInformation info, XmlElement projectOptions);
		
		Project GetProject (string projectName);
		
		/// <remarks>
		/// Is called, when a file is removed from and added to a project.
		/// </remarks>
		event ProjectFileEventHandler FileRemovedFromProject;
		event ProjectFileEventHandler FileAddedToProject;

		/// <remarks>
		/// Is called, when a file in the project is changed on disk.
		/// </remarks>
		event ProjectFileEventHandler FileChangedInProject;
				
		/// <remarks>
		/// Is called, when a reference is removed from and added to a project.
		/// </remarks>
		event ProjectReferenceEventHandler ReferenceAddedToProject;
		event ProjectReferenceEventHandler ReferenceRemovedFromProject;
		
		/// <remarks>
		/// Called before a build run
		/// </remarks>
		event EventHandler StartBuild;
		
		/// <remarks>
		/// Called after a build run
		/// </remarks>
		event ProjectCompileEventHandler EndBuild;
		
		/// <remarks>
		/// Called before execution
		/// </remarks>
		event EventHandler BeforeStartProject;
		
		/// <remarks>
		/// Called after a new root combine is opened
		/// </remarks>
		event CombineEventHandler CombineOpened;
		
		/// <remarks>
		/// Called after a root combine is closed
		/// </remarks>
		event CombineEventHandler CombineClosed;
		
		/// <remarks>
		/// Called after the current selected project has chaned
		/// </remarks>
		event ProjectEventHandler CurrentProjectChanged;
		
		/// <remarks>
		/// Called after the current selected combine has chaned
		/// </remarks>
		event CombineEventHandler CurrentSelectedCombineChanged;
	}
}
