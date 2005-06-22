using System;
using System.Collections;
using System.IO;

using Gtk;

using MonoDevelop.Gui;
using MonoDevelop.Core;
using MonoDevelop.Gui.Pads;
using MonoDevelop.Internal.Project;
using MonoDevelop.Commands;
using MonoDevelop.Services;

using VersionControl;

namespace VersionControlPlugin {

	public class VersionControlService  {
	
		public static ArrayList Providers = new ArrayList();
	
		static Gdk.Pixbuf overlay_normal = Gdk.Pixbuf.LoadFromResource("overlay_normal.png");
		static Gdk.Pixbuf overlay_modified = Gdk.Pixbuf.LoadFromResource("overlay_modified.png");
		static Gdk.Pixbuf overlay_conflicted = Gdk.Pixbuf.LoadFromResource("overlay_conflicted.png");
		static Gdk.Pixbuf overlay_added = Gdk.Pixbuf.LoadFromResource("overlay_added.png");

		public static Gdk.Pixbuf LoadIconForStatus(NodeStatus status) {
			if (status == NodeStatus.Unchanged)
				return overlay_normal;
			if (status == NodeStatus.Modified)
				return overlay_modified;
			if (status == NodeStatus.Conflicted)
				return overlay_conflicted;
			if (status == NodeStatus.ScheduledAdd)
				return overlay_added;
			return null;
		}
		
		static VersionControlService() {
			Providers.Add(new SubversionVersionControl());
		}
	}
	
	public class VersionControlNodeExtension : NodeBuilderExtension {
		public override bool CanBuildNode (Type dataType) {
			//Console.Error.WriteLine(dataType);
			return typeof(ProjectFile).IsAssignableFrom (dataType)
				|| typeof(DotNetProject).IsAssignableFrom (dataType);
				// TODO: Folders
		}		
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon) {
			// Add status overlays
			
			// TODO: Watch the files in some way to detect
			// when the overlay should be changed.
			
			if (!(dataObject is ProjectFile)) return;
			if (!builder.Options["ShowVersionControlOverlays"])
				return;
		
			ProjectFile file = (ProjectFile) dataObject;
			try {
				foreach (VersionControlSystem vc in VersionControlService.Providers) {
					if (vc.IsFileStatusAvailable(file.FilePath)) {
						Node node = vc.GetFileStatus(file.FilePath, false);
						
						Gdk.Pixbuf overlay = VersionControlService.LoadIconForStatus(node.Status);
						
						double scale = (double)(2*icon.Width/3) / (double)overlay.Width;
						int w = (int)(overlay.Width*scale);
						int h = (int)(overlay.Height*scale);
						icon = icon.Copy();
						overlay.Composite(icon,
							icon.Width-w,  icon.Height-h,
							w, h,
							icon.Width-w, icon.Height-h,
							scale,scale, Gdk.InterpType.Bilinear, 255); 
						break;
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine(e);
			}
		}
		
		public override Type CommandHandlerType {
			get { return typeof(AddinCommandHandler); }
		}
	}

	public enum Commands {
    	Update,
    	Diff,
    	Log,
    	Status
	}
	
	class AddinCommandHandler : NodeCommandHandler {
		[CommandHandler (Commands.Update)]
		protected void OnUpdate() {
			RunCommand(Commands.Update, false);
		}
		
		[CommandUpdateHandler (Commands.Update)]
		protected void UpdateUpdate(CommandInfo item) {
			TestCommand(Commands.Update, item);
		}
		
		[CommandHandler (Commands.Diff)]
		protected void OnDiff() {
			RunCommand(Commands.Diff, false);
		}
		
		[CommandUpdateHandler (Commands.Diff)]
		protected void UpdateDiff(CommandInfo item) {
			TestCommand(Commands.Diff, item);
		}
		
		[CommandHandler (Commands.Log)]
		protected void OnLog() {
			RunCommand(Commands.Log, false);
		}
		
		[CommandUpdateHandler (Commands.Log)]
		protected void UpdateLog(CommandInfo item) {
			TestCommand(Commands.Log, item);
		}
		
		[CommandHandler (Commands.Status)]
		protected void OnStatus() {
			RunCommand(Commands.Status, false);
		}
		
		[CommandUpdateHandler (Commands.Status)]
		protected void UpdateStatus(CommandInfo item) {
			TestCommand(Commands.Status, item);
		}
		
		private void TestCommand(Commands cmd, CommandInfo item) {
			item.Visible = RunCommand(cmd, true);
		}
		
		private bool RunCommand(Commands cmd, bool test) {
			string path;
			bool isDir;
		
			if (CurrentNode.DataItem is ProjectFile) {
				ProjectFile file = (ProjectFile)CurrentNode.DataItem;
				path = file.FilePath;
				isDir = false;
			} else if (CurrentNode.DataItem is DotNetProject) {
				DotNetProject project = (DotNetProject)CurrentNode.DataItem;
				path = project.BaseDirectory;
				isDir = true;
			} else {
				Console.Error.WriteLine(CurrentNode.DataItem);
				return false;
			}
			
			switch (cmd) {
				case Commands.Update:
					return UpdateCommand.Update(path, test);
				case Commands.Diff:
					return DiffView.Show(path, test);
				case Commands.Log:
					return LogView.Show(path, isDir, null, test);
				case Commands.Status:
					return StatusView.Show(path, test);
			}
			return false;
		}
	}

	public abstract class BaseView : AbstractBaseViewContent, IViewContent {
		string name;
		public BaseView(string name) { this.name = name; }
		
		protected virtual void SaveAs(string fileName) {
		}

		void IViewContent.Load(string fileName) {
			throw new InvalidOperationException();
		}
		void IViewContent.Save() {
			throw new InvalidOperationException();
		}
		void IViewContent.Save(string fileName) {
			SaveAs(fileName);
		}
		
		string IViewContent.ContentName {
			get { return name; }
			set { }
		}
		
		bool IViewContent.HasProject {
			get { return false; }
			set { }
		}
		
		bool IViewContent.IsDirty {
			get { return false; }
			set { }
		}
		
		bool IViewContent.IsReadOnly {
			get { return true; }
		}

		bool IViewContent.IsUntitled {
			get { return false; }
		}

		bool IViewContent.IsViewOnly {
			get { return false; }
		}
		
		string IViewContent.PathRelativeToProject {
			get { return ""; }
		}
		
		MonoDevelop.Internal.Project.Project IViewContent.Project {
			get { return null; }
			set { }
		}
		
		string IViewContent.TabPageLabel {
			get { return name; }
		}
		
		string IViewContent.UntitledName {
			get { return ""; }
			set { }
		}
		
		event EventHandler IViewContent.BeforeSave { add { } remove { } }
		event EventHandler IViewContent.ContentChanged { add { } remove { } }
		event EventHandler IViewContent.ContentNameChanged { add { } remove { } }
		event EventHandler IViewContent.DirtyChanged { add { } remove { } }
	}
	

}
