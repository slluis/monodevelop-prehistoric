using System;

//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

namespace Microsoft.Build.Framework
{
	[Serializable]
	public class BuildEventArgs : EventArgs
	{
		private BuildEventCategory category;
		private BuildEventImportance importance = BuildEventImportance.Low;
		private DateTime timeStamp = DateTime.Now;
		private string code;
		private string file;
		private string helpKeyword;
		private string message;
		private string subCategory;
		private int columnNumber;
		private int endColumnNumber;
		private int endLineNumber;
		private int lineNumber;
		private int processor;
		
		public BuildEventArgs ()
		{
			Console.WriteLine ("My LIB!");
		}

		public BuildEventArgs (BuildEventCategory category, BuildEventImportance importance)
		{
			this.category = category;
			this.importance = importance;
		}

		public BuildEventArgs (BuildEventCategory category, string subCategory,
								BuildEventImportance importance, string code,
								string file, int lineNumber, int columnNumber,
								int endLineNumber, int endColumnNumber, string message,
								string helpKeyword)
		{
			this.category = category;
			this.subCategory = subCategory;
			this.importance = importance;
			this.code = code;
			this.file = file;
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
			this.endLineNumber = endLineNumber;
			this.endColumnNumber = endColumnNumber;
			this.message = message;
			this.helpKeyword = helpKeyword;
		}

		#region Properties

		public BuildEventCategory Category 
		{
			get { return category; }
			set { category = value; }
		}

		public BuildEventImportance Importance 
		{
			get { return importance; }
			set { importance = value; }
		}

		public DateTime TimeStamp
		{
			get { return timeStamp; }
		}

		public string Code
		{
			get { return code; }
			set { code = value; }
		}

		public string File
		{
			get { return file; }
			set { file = value; }
		}

		public string HelpKeyword
		{
			get { return helpKeyword; }
			set { helpKeyword = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}

		public string SubCategory
		{
			get { return subCategory; }
			set { subCategory = value; }
		}

		public int ColumnNumber
		{
			get { return columnNumber; }
			set { columnNumber = value; }
		}

		public int EndColumnNumber
		{
			get { return endColumnNumber; }
			set { endColumnNumber = value; }
		}

		public int EndLineNumber
		{
			get { return endLineNumber; }
			set { endLineNumber = value; }
		}

		public int LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}

		[MonoTODO("What should this field do?")]
		public int Processor
		{
			get { return processor; }
		}

		#endregion
	}
}
