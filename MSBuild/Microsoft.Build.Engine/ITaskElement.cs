using System;

//
// ITaskElement.cs: Interface for a task element
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Engine {
	public interface ITaskElement {

		bool Execute ();

		string[] GetParameterNames ();

		string GetParameterValue ();

		void SetParameterValue (string parameterName, string parameterValue);

		string Condition
		{
			get;
			set;
		}

		bool ContinueOnError
		{
			get;
			set;
		}

		object HostObject
		{
			get;
			set;
		}

		string Name
		{
			get;
		}

		Type Type
		{
			get;
		}
	}
}
