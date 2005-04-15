using System;

class T
{
	unsafe private void Foo ()
	{
	}

	unsafe int Bar {
		get { return 0; }
	}

	unsafe public event EventHandler Notify;
}
