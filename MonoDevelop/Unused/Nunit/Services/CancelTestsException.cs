using System;

namespace MonoDevelop.Services.Nunit
{
	public class CancelTestsException : ApplicationException
	{
		public CancelTestsException (string msg) : base (msg)
		{
		}
	}
}
