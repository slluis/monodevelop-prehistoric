
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Collections;

using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Commands;

using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Commands;

namespace Rice.Drcsharp {
	public class InteractionsPlugin : AbstractPadContent {
		protected Panel panel= null;
		
		/// <summary>
		/// The GUI component that provides the editing interface.
		/// </summary>
		InteractionsTextBox  interactions = null;
		
		/// <summary>
		/// the interpreter
		/// </summary>
		InterpreterProxy interpreter	= null;
		
		/// <summary>
		/// the text writer for the interpreter to use
		/// </summary>
		InteractionsWriter interactionsWriter;
		
		/// <summary>
		/// #D project service. Lets us access the current assembly, etc.
		/// </summary>
		IProjectService projectService = (IProjectService)ServiceManager.Services.GetService(typeof(IProjectService));		
		
		//IMenuCommandService menuCommandService = (IMenuCommand)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IMenuCommandService));
		
		//StopToolbarHandler stopHandler;
		//ClearToolbarHandler clearHandler;
		
		protected InterpreterProxy ip;
		protected AppDomainSetup setup;
		protected AppDomain currDomain;
		protected AppDomain userDomain;

#if !LINUX
		public override Control Control {
			get {
				return panel;
			}
		}
#endif
		private void Debug(string s) {
			DB.plugin(s);
		}
		
		public InteractionsPlugin() : base("Interactions", "Icons.16x16.RunProgramIcon") {
			panel  = new Panel();
			
			//TODO: handler referencing!
			
			// make a new interactions box
			interactions = new InteractionsTextBox("Welcome to DrC#\r\n> ",this);
			
			//instantiate the writer
			interactionsWriter = new InteractionsWriter(this);
			
			// fill out the entire panel
			interactions.Dock = DockStyle.Fill;
			
			// add components to our panel
			panel.Controls.Add(interactions);
			
			Initialize();
			
			// register event handlers to be informed about compiling events
			projectService.StartBuild += new EventHandler(StartBuildEventHandler);
			projectService.EndBuild += new EventHandler(EndBuildEventHandler);
		}

#region "Initialization/AppDomain Management Methods"
		
		private string GetPluginPath() {
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
		}
		
		private void Initialize() {
			Debug("<Initialize>");
			
			//setup ApDomains
			currDomain = AppDomain.CurrentDomain;
			Environment.CurrentDirectory = currDomain.BaseDirectory;
			
			
			string pluginPath = GetPluginPath();
			setup = new AppDomainSetup();
			setup.ApplicationBase = pluginPath;
			setup.ApplicationName = "InterpreterProxy";
			setup.DynamicBase = pluginPath;
			
			//setup shadow copying
			setup.ShadowCopyFiles = "true";
			//this should change to the user's working directory
			setup.ShadowCopyDirectories = pluginPath;
			
			userDomain = null;
			Reinitialize();
			Debug("</Initialize>");
		}
		
		
		private void Reinitialize() {
			Debug("<Reinitialize>");
			CreateNewInterpreter();
			SupplyWriter(interactionsWriter);
			Debug("</Reinitialize>");
		}
		
		private void Reinitialize(string userDir) {
			Debug("<Reinitialize userDir=" +userDir + ">");
			
			UnloadUserDomain();
			setup.ApplicationBase = userDir;
			setup.ShadowCopyDirectories = userDir;
			Reinitialize();
			
			interpreter.SetUserAssemblyPath(userDir);
			
			Debug("</Reinitialize>");
		}
		
		private void CreateNewInterpreter() {
			UnloadUserDomain();
		
		
			userDomain = AppDomain.CreateDomain("Dr. C# TypeSpace", null, setup);
			interpreter = (InterpreterProxy)userDomain.CreateInstanceFromAndUnwrap("DrcsInterpreter.dll", "Rice.Drcsharp.Interpreter.InterpreterProxy");
			
		}
		
		private void UnloadUserDomain() {
			if(userDomain != null) {
				AppDomain.Unload(userDomain);
				userDomain = null;
			}
		}
#endregion
		
		/// <summary>
		/// Event handler that gets called when #D starts to build.
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="ea">Event arguments</param>
		public void StartBuildEventHandler(object obj, EventArgs ea) {
			Combine currentCombine = projectService.CurrentOpenCombine;
			if(currentCombine!= null && currentCombine.SingleStartupProject) {
				IProject startupProj = GetProject(currentCombine.SingleStartProjectName,currentCombine);
				if (startupProj != null) {
					string userDir = startupProj.BaseDirectory;
					Reinitialize(userDir);
					AppendText("Reinitialized at start of build");
				} else {
					Reinitialize();
				}
			} else {
				Reinitialize(); 
			}
		}
		/// <summary>
		/// Searches for a project by the given name in the given combine, and returns it.
		/// If no project is found, returns null.
		/// </summary>
		/// <param name="projName">Input project name</param>
		/// <param name="c">Input combine to search through</param>
		/// <returns>Returns the IProject with the given name or null if none is found</returns>
		public IProject GetProject(string projName, Combine c) {
			foreach (CombineEntry e in c.Entries) {
				if (e is ProjectCombineEntry) {
					if (((ProjectCombineEntry)e).Project.Name == projName) {
						return ((ProjectCombineEntry)e).Project;
					}
				} else if (e is CombineCombineEntry) {
					return GetProject(projName, ((CombineCombineEntry)e).Combine);
				} else {
					throw new Exception("INTERNAL ERROR: Previously undefined CombineEntry subclass");
				}
			}
			return null;
		}
		
		/// <summary>
		/// Event handler that gets called when #D is done building.
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="ea">Event arguments</param>
		public void EndBuildEventHandler(object obj, EventArgs ea) {
			interactions.ReinitializeText();
			LoadUserCode();					
		}
		
		public void LoadUserCodeIfPossible() {
			Debug("<LoadUserCodeIfPossible>");
			Combine currentCombine = projectService.CurrentOpenCombine;
			if (currentCombine != null) {
				try {
					LoadCombine(currentCombine);
				} catch (Exception e) {
					Debug("Couldn't load user assembly: " + e.Message);
				}
			}
			Debug("</LoadUserCodeIfPossible>");
		}
		
		public void LoadUserCode() {
			Combine currentCombine = projectService.CurrentOpenCombine;
			if (currentCombine != null) {
				try {
					LoadCombine(currentCombine);
				} catch (Exception e) {
					AppendText(e.Message);
				}
			}
		}
		
		/// <summary>		
		/// Loads the projects contained within the input Combine
		/// </summary>
		/// <param name="c">Input Combine to load</param>
		public void LoadCombine(Combine c) {
			foreach (CombineEntry e in c.Entries) {
				if (e is CombineCombineEntry) {
					LoadCombine(((CombineCombineEntry)e).Combine);
				} else if (e is ProjectCombineEntry) {
					LoadProject(((ProjectCombineEntry)e).Project);
				}
			}
		}
		
		/// <summary>
		/// Loads the assembly corresponding to the input project.
		/// </summary>
		/// <param name="p">Input project to load</param>
		public void LoadProject(IProject p) {
			string assemblyName = projectService.GetOutputAssemblyName(p);
			try {
				interpreter.LoadUserAssemblyFullPath(assemblyName);
			} catch (Exception e) {
				throw new Exception("Couldn't load assembly '" + assemblyName +"' :\r\n"
				                    + e.Message);
			}
		}
		
		/// <summary>
		/// Sets the writers for the server
		/// </summary>
		/// <param name="t">Writer that the server should use</param>
		private void SupplyWriter(TextWriter t) {
			interpreter.SetStdOut(t);
			interpreter.SetStdErr(t);
		}
		
		/// <summary>
		/// Add text to the interactions window.
		/// Also adds a newline.
		/// </summary>
		/// <param name="t">Text to add, excluding newline</param>
		public void AppendText(string t) {
			// add newline
			t = t + Environment.NewLine;
			
			interactions.Write(t);
		}
		
		/// <summary>
		/// Add text to the interactions window without newline.
		/// </summary>
		/// <param name="t">Text to add</param>
		public void AppendTextNoNewline(string t) {
			if (t!="") {
				interactions.Write(t);
			}
		}
		
		/// <summary>
		/// Empty the interactions window.
		/// </summary>
		public void ClearInteractions() {
			interactions.Text = "";
		}
		
		/// <summary>
		/// Request a command to be interpreted
		/// </summary>
		/// <param name="input">Command to interpret</param>
		public string InterpretInteractions(string input) {
			// interpret locally
			if (input=="_clear") {
				ClearInteractions();
				return "Welcome to Dr. C#";
			}
			if (input=="_restart") {
				ClearInteractions();
				Reinitialize();
				return "Interpreter restarted";
			}
			
			return "" + interpreter.Interpret(input);
			
		}
	}
}
