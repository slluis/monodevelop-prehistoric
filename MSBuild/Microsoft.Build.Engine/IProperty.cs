using System;

//
// IProperty.cs: Interface for Property
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Engine {
	public interface IProperty {

		string Condition
		{
			get;
			set;
		}

		string FinalValue
		{
			get;
		}

		string Name
		{
			get;
		}

		string Value
		{
			get;
			set;
		}

		Property Clone (bool deepClone);

		string ToString ();
	}
}
