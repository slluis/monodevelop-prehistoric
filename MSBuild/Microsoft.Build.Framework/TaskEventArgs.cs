using System;

namespace Microsoft.Build.Framework
{
	[Serializable]
	public class TaskEventArgs : BuildEventArgs
	{
		private string taskName;

		public TaskEventArgs ()
		{
		}

		public TaskEventArgs (BuildEventCategory category, BuildEventImportance importance,
								string message, string taskName)
		{
			Category = category;
			Importance = importance;
			Message = message;
			this.taskName = taskName;
		}

		public TaskEventArgs (BuildEventCategory category, string subCategory,
								BuildEventImportance importance, string code,
								string file, int lineNumber, int columnNumber,
								int endLineNumber, int endColumnNumber, string message,
								string helpKeyword, string taskName)
		{
			Category = category;
			SubCategory = subCategory;
			Importance = importance;
			Code = code;
			File = file;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			EndLineNumber = endLineNumber;
			EndColumnNumber = endColumnNumber;
			Message = message;
			HelpKeyword = helpKeyword;
			this.taskName = taskName;
		}

		public string TaskName
		{
			get { return taskName; }
			set { taskName = value; }
		}
	}
}
