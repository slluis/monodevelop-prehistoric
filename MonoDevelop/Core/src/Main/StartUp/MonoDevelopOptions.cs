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
	}
}

