using System;
using System.IO;

namespace MonoDevelop.DebuggerVisualizers
{
	public class VisualizerObjectSource
	{
		public VisualizerObjectSource ()
		{
			throw new NotImplementedException ();
		}

		public virtual object CreateReplacementObject (object target, Stream incomingData)
		{
			throw new NotImplementedException ();
		}

		public static object Deserialize (Stream serializationStream)
		{
			throw new NotImplementedException ();
		}

		public virtual void GetData (object target, Stream outgoingData)
		{
			throw new NotImplementedException ();
		}

		public static void Serialize (Stream serializationStream, object target)
		{
			throw new NotImplementedException ();
		}

		public static void TransferData (object target, Stream incomingData, Stream outgoingData)
		{
			throw new NotImplementedException ();
		}
	}
}
