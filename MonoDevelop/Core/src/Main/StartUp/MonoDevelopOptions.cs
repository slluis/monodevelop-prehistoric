using System;
using Mono.GetOptions;

namespace MonoDevelop
{
	public class MonoDevelopOptions : Options
	{
		public MonoDevelopOptions ()
		{
			base.ParsingMode = OptionsParsingMode.Both;
		}

		// FIXME: we really ignore this, but this allows
		// us to know it is ok to reuse the socket
		[Option ("Start with this file open.", 'f')]
		public bool file;

		[Option ("Do not display splash screen.")]
		public bool nologo;
	}
}

