using System;
using System.IO;
using Mono.Debugger;
using Mono.Debugger.Languages;

namespace MonoDevelop.DebuggerVisualizers
{

	public class TargetObjectProvider : IVisualizerObjectProvider
	{
		public TargetObjectProvider (ITargetObject obj)
		{
			this.obj = obj;
			throw new NotImplementedException ();
		}

#region IVisualizerObjectProvider implementation

		public bool IsObjectReplaceable
		{
			get {
				throw new NotImplementedException ();
			}
		}
	  

		public Stream GetData()
		{
			throw new NotImplementedException ();
		}

		public object GetObject ()
		{
			/* first we cause the target object to serialize itself */

			/* then we transfer the data to the debugger process */

			/* and deserialize it */

			throw new NotImplementedException ();
		}

		public void ReplaceData (Stream newObjectData)
		{
			throw new NotImplementedException ();
		}

		public void ReplaceObject (object newObject)
		{
			throw new NotImplementedException ();
		}

		public Stream TransferData (Stream outgoingData)
		{
			throw new NotImplementedException ();
		}

		public object TransferObject (object outgoingObject)
		{
			throw new NotImplementedException ();
		}
#endregion

		ITargetObject obj;
	}

}
