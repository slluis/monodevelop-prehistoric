using System;

//
// TaskElement.cs: represents a task element
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Engine {
	public class TaskElement : ITaskElement {

		
		
		public TaskElement ()
		{
		}

		#region ITaskElement Members

		public bool Execute ()
		{
			throw new NotImplementedException ();
		}

		public string[] GetParameterNames ()
		{
			throw new NotImplementedException ();
		}

		public string GetParameterValue ()
		{
			throw new NotImplementedException ();
		}

		public void SetParameterValue (string parameterName, string parameterValue)
		{
			throw new NotImplementedException ();
		}
		public string Condition
		{
			get
			{
				throw new global::System.NotImplementedException ();
			}

			set
			{
				throw new global::System.NotImplementedException ();
			}
		}

		public bool ContinueOnError
		{
			get
			{
				throw new global::System.NotImplementedException ();
			}

			set
			{
				throw new global::System.NotImplementedException ();
			}
		}

		public object HostObject
		{
			get
			{
				throw new global::System.NotImplementedException ();
			}

			set
			{
				throw new global::System.NotImplementedException ();
			}
		}

		public string Name
		{
			get { throw new global::System.NotImplementedException (); }
		}

		public Type Type
		{
			get { throw new global::System.NotImplementedException (); }
		}


		#endregion
	}
}
