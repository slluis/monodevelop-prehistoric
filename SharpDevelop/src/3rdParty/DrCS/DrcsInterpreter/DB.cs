using System;
using System.Diagnostics;
using System.IO;

namespace Rice.Drcsharp
{
	/// <summary>
	/// Summary description for Debug.
	/// </summary>
	public class DB
	{
		public static bool gdb = true; //global
		public static bool adb = true; //typemanager
		public static bool bdb = false; //binary
		public static bool conddb = false; //conditional
		public static bool udb = false; //unary
		public static bool sdb = false; //switch
		public static bool tdb = false; //try
		public static bool fdb = false; //for
		public static bool ardb = false; //array
		public static bool edb = false; //element access
		public static bool asdb = true; //assignment
		public static bool envdb = false; //env
		public static bool proxdb = true; //proxy
		public static bool parsedb = false; //parser
		public static bool mdb = true; //member access
		public static bool idb = true; //invocation
		public static bool frontdb = true; //InterpreterFrontEnd
		public static bool plugindb = true;

		public static bool toStdOut = true;

		public static TextWriter output = Console.Out;

		static TextWriter tmp;
		static StreamWriter sw1;
		static string TestDir = AppDomain.CurrentDomain.BaseDirectory;

		public DB() {}

		[Conditional("DEBUG")]
		public static void bp(string s) {
			if(bdb) { db("<binary> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void condp(string s) {
			if(conddb) { db("<conditional> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void up(string s) {
			if(udb) { db("<unary> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void sp(string s) {
			if(sdb) { db("<switch> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void arp(string s) {
			if(ardb) { db("<array> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void asp(string s) {
			if(asdb) { db("<assignment> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void envp(string s) {
			if(envdb) { db("<env> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void ep(string s) {
			if(edb) { db("<elementAccess> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void fep(string s) {
			if(frontdb) { db("<InterpreterFrontEnd> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void fp(string s) {
			if(fdb) { db("<for> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void ip(string s) {
			if(idb) { db("<invocation> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void parsep(string s) {
			if(parsedb) { db("<parser> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void proxp(string s) {
			if(proxdb) { db("<proxy> : " + s); }
		}
		
		[Conditional("DEBUG")]
		public static void plugin(string s) {
			if(plugindb) { db("<plugin> : " + s); }
		}


		[Conditional("DEBUG")]
		public static void mp(string s) {
			if(mdb) { db("<memberAccess> : " + s); }
		}

		[Conditional("DEBUG")]
		public static void tm(string s) {
			if(adb) { db("<TypeManager> : " + s);	}
		}

		[Conditional("DEBUG")]
		public static void tp(string s) {
			if(tdb) { db("<try> : " + s); }
		}

		
		[Conditional("DEBUG")]
		public static void db(string s) {
			if(gdb) {
				if (sw1==null) {
					redirect();
				}
				if (sw1 != null) {
					sw1.WriteLine(s);
				}
			}
		}
		
		[Conditional("DEBUG")]
		public static void redirect() {
			redirect("MainTest.txt");
		}

		[Conditional("DEBUG")]
		public static void redirect(string file) {
			try {
				FileStream fs1 = new FileStream(TestDir + file, FileMode.Create);
				sw1 = new StreamWriter(fs1);
				sw1.AutoFlush = true;
			} catch (Exception) {
				sw1 = null;
			}
		}
	}
}
