using System; using System.Reflection;
using Blah = Gtk.Window;

namespace Foo
{
	public interface IFoo
	{
		int Dummy { get; set; }
		void DummyMethod();
		event EventHandler Fooed;
	}

		[Flags]
			[Serializable]
	public enum FooFlags : int
	{
		None = 1,
	}

	public class Bar : System.Object {

		static string a, b, c;

		static void Moo (out string a, ref string b)
		{
		}

		static void Main(string[]  args)
		{
			int foo 			= 			5;
			this.Fooed += new EventHandler (OnFooed);

			this.Moo (out a, ref b);

			foo = sizeof (IntPtr);

			base.GetHashCode ();

			FooFlags |= FakeFlags.None;

			if (this is object)
			{}

			string[] names = new string[] {""};

			unsafe { /* blah */ a++; }

			checked { }

			//unchecked ((int) 0x80000000) {}
			unchecked {}

			// this wont compile
			fixed (byte* buf = &buffer[offset]) {}

			using (StreamReader r = new StreamReader()) {}

			foreach (string s in args)
			{
				Console.Write(s);
			}

			for(;;){}

			while (true){}

			do {
			}
			while (true);

			lock (typeof (Bar)) { }

			switch (args[0]) {
				case "a":
					break;
				case "b":
					goto fancy_label;
				default:
					break;
			}

		fancy_label:
			throw;

		try {} catch (Exception e) {} finally {}

			return 0;
		}

		~Bar ()
		{
		}

		public Bar () : base ()
		{
		}

		public Bar (string foo) : this () {}
	}

	public struct Baz {
	}

}
