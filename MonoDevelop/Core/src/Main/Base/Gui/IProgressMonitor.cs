// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace MonoDevelop.Gui
{
	/// <summary>
	/// This is a basic interface to a "progress bar" type of 
	/// control.
	/// </summary>
	public interface IProgressMonitor 
	{
		void BeginTask (string name, int totalWork);
		
		void Worked (double work, string status);
		
		void Done();
		
		bool Canceled {
			get;
			set;
		}
		
		string TaskName {
			get;
			set;
		}
	}
}
