using System;

//
// Property.cs: Represents an msbuild property.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Engine {
	public class Property : IProperty {

		private string name;
		private string val;

		public Property ()
		{
			throw new InvalidOperationException ("An empty property cannot be constructed.");
		}

		public Property (string name, string value)
		{
			this.name = name;
			this.val = value;
		}

		[MonoTODO("Check if FinalValue is correct to return when Condition is set.")]
		public static string op_Implicit (Property p)
		{
			return p.FinalValue;
		}

		/// <summary>
		/// The condition should be an XML element / attribute.
		/// </summary>
		/// <value></value>
		[MonoTODO("Implement setting a condition")]
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

		[MonoTODO("Check if this changes when condition is set.")]
		public string FinalValue
		{
			get { return val; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Value {
			get { return val; }
			set { val = value; }
		}


		public Property Clone (bool deepClone)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO("test this")]
		public string ToString ()
		{
			return FinalValue;
		}
	}
}
