using System;

namespace JavaBinding
{
	public enum JavaRuntime
	{
		Ikvm, // JIT to CIL and then exec with mono
		Mono, // compile with ikvmc and then run with mono
		Java, // an installed JRE
		Gij, // gcj interpreter
	}
}

