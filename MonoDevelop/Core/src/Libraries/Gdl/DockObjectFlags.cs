using System;

namespace Gdl
{
	[Flags]
	public enum DockObjectFlags
	{
		Automatic = 1 << 0,
		Attached = 1 << 1,
		InReflow = 1 << 2,
		InDetach = 1 << 3,
		InDrag = 1 << 4,
		InPreDrag = 1 << 5,
		Iconified = 1 << 6,
		UserAction = 1 << 7 
	}
}
