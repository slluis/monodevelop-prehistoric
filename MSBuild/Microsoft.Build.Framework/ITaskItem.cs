//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

using System;
using System.Collections;

namespace Microsoft.Build.Framework
{
	[System.Runtime.InteropServices.GuidAttribute ("F8EAE07C-9808-4445-8209-245BCC59780E")]
	public interface ITaskItem
	{
		void CopyAttributesTo (ITaskItem destinationItem);
		string GetAttribute (string attributeName);
		void RemoveAttribute (string attributeName);
		void SetAttribute (string attributeName, string attributeValue);

		ICollection AttributeNames
		{
			get;
		}

		string ItemSpec
		{
			get;
			set;
		}
	}
}
