﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Pads;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Services
{
	public class TaskService : AbstractService
	{
		ArrayList tasks  = new ArrayList();
		string    compilerOutput = String.Empty;
		
		[FreeDispatch]
		public ICollection Tasks {
			get {
				return tasks;
			}
		}
		
		int warnings = 0;
		int errors   = 0;
		int comments = 0;
		
		public int Warnings {
			get {
				return warnings;
			}
		}
		
		public int Errors {
			get {
				return errors;
			}
		}
		
		public int Comments {
			get {
				return comments;
			}
		}
		
		public bool SomethingWentWrong {
			get {
				return errors + warnings > 0;
			}
		}
		
		public string CompilerOutput {
			get {
				return compilerOutput;
			}
			set {
				compilerOutput = value;
				OnCompilerOutputChanged(null);
			}
		}
		
		public void AddTask (Task task)
		{
			tasks.Add (task);
			
			switch (task.TaskType) {
				case TaskType.Warning:
					++warnings;
					break;
				case TaskType.Error:
					++errors;
					break;
				default:
					++comments;
					break;
			}
			
			OnTaskAdded (new TaskEventArgs (task));
		}
		
		public void ClearTasks ()
		{
			tasks.Clear ();
			warnings = errors = comments = 0;
			OnTasksChanged (null);
		}
		
		public void ShowTasks ()
		{
			Runtime.DispatchService.GuiDispatch (new MessageHandler (ShowTasksCallback));
		}
		
		void ShowTasksCallback ()
		{
			OpenTaskView taskView = Runtime.Gui.Workbench.GetPad(typeof(OpenTaskView)) as OpenTaskView;
			if (taskView != null) taskView.BringToFront();
		}
		
		protected virtual void OnCompilerOutputChanged(EventArgs e)
		{
			if (CompilerOutputChanged != null) {
				CompilerOutputChanged(this, e);
			}
		}
		
		protected virtual void OnTasksChanged(EventArgs e)
		{
			if (TasksChanged != null) {
				TasksChanged(this, e);
			}
		}
		
		protected virtual void OnTaskAdded (TaskEventArgs e)
		{
			if (TaskAdded != null) {
				TaskAdded (this, e);
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			Runtime.FileService.FileRenamed += new FileEventHandler(CheckFileRename);
			Runtime.FileService.FileRemoved += new FileEventHandler(CheckFileRemove);
		}
		
		void CheckFileRemove(object sender, FileEventArgs e)
		{
			bool somethingChanged = false;
			for (int i = 0; i < tasks.Count; ++i) {
				Task curTask = (Task)tasks[i];
				if (curTask.FileName == e.FileName) {
					tasks.RemoveAt(i);
					--i;
					somethingChanged = true;
				}
			}
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		void CheckFileRename(object sender, FileEventArgs e)
		{
			bool somethingChanged = false;
			foreach (Task curTask in tasks) {
				if (curTask.FileName == e.SourceFile) {
					curTask.FileName = e.TargetFile;
					somethingChanged = true;
				}
			}
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		public void NotifyTaskChange()
		{
			OnTasksChanged(null);
		}
		
		public event TaskEventHandler TaskAdded;
		public event EventHandler TasksChanged;
		public event EventHandler CompilerOutputChanged;
	}

	public delegate void TaskEventHandler (object sender, TaskEventArgs e);
	
	public class TaskEventArgs : EventArgs
	{
		Task task;
		
		public TaskEventArgs (Task task)
		{
			this.task = task;
		}
		
		public Task Task 
		{
			get { return task; }
		}
	}
}
