// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using ICSharpCode.SharpDevelop.Internal.Project;
using System.Reflection;
using System.Collections;
using System.IO;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace NewClassWizard
{
	/// <summary>
	/// Summary description for NamespacesCollection.
	/// </summary>
	public class ProjectInfo {
		
		public readonly IProject project;
		
		public ProjectInfo( IProject p ) {
			project = p;
		}
		
		public ProjectInfo( ProjectCombineEntry p ) {
			if ( p == null )
				throw new NullReferenceException();
			
			project = p.Project;
		}	
		
		public Array GetAssemblies()
		{		
			ArrayList assemblyArray = new ArrayList();
			
			//always add the core library
			assemblyArray.Add( Assembly.Load( "mscorlib" ) );			
			
			if ( project != null ) {
				addReferencedAssemblies( assemblyArray );
				addProjectAssembly( assemblyArray );
			}
			
			return assemblyArray.ToArray( typeof(Assembly) );
		}		
		
		private void addReferencedAssemblies( ArrayList assemblyArray ) {
			foreach ( ProjectReference r in project.ProjectReferences ) {		
				try {  // GB: try-catch is new
					assemblyArray.Add( Assembly.LoadFrom( r.GetReferencedFileName(project) ) );
				} catch {
					System.Diagnostics.Debug.WriteLine("New Class Wizard: Error loading referenced assembly " + r.GetReferencedFileName(project) + ".");
				}
			}
		}

		private void addProjectAssembly( ArrayList assemblyArray )
		{
			//finally attempt to load the assembly for the current project
			try
			{				
				string projectAssemblyPath = this.absoluteOutputPath;
			
				if ( projectAssemblyPath != String.Empty && File.Exists(projectAssemblyPath)) 
				{		
					//we will get the project assembly as a byte array
					//otherwise it will be locked and we won't be able
					//to build the project after the wizard is run
					assemblyArray.Add( Assembly.Load( GetRawAssembly( projectAssemblyPath ) ) );
				}
			}
			catch ( Exception )
			{				
				//MessageBox.Show( e.StackTrace, e.Message );
				//tried and failed to load an assembly for the current project
				//usually means that is has never been built in the ActiveConfiguration
			}
		}		
		
		private string absoluteOutputPath {
			get {
				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
				
				return Path.GetFullPath( binding.GetCompiledOutputName(project) );				 
			}
		}
		
		private string outputDirectory {
			get {
				string projectAssemblyPath = this.absoluteOutputPath;
				return Path.Combine( Path.GetPathRoot( projectAssemblyPath ),
			                                   		Path.GetDirectoryName( projectAssemblyPath ) );
			
			}
		}
		
		private byte[] GetRawAssembly( string path )
		{
			FileInfo f = new FileInfo( path );
			using (BinaryReader br = new BinaryReader( f.OpenRead() )) {			
				return br.ReadBytes( Convert.ToInt32( f.Length ) );
			}					
		}		
	}
	
}

