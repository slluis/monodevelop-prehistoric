using System;
using Rice.Drcsharp;
using System.Text;
using System.IO;

/// <summary>
/// Text writer the server uses to write into the interactions window.
/// </summary>
public class InteractionsWriter : System.IO.TextWriter {
	protected System.Text.ASCIIEncoding encoding = null;
	protected InteractionsPlugin plugin = null;

	public override System.Text.Encoding Encoding {
		get { return encoding; }
	}
		
	public InteractionsPlugin Plugin {
		get { return plugin; }
		set { plugin = value; }
	}

	public InteractionsWriter(InteractionsPlugin p) {
		encoding = new System.Text.ASCIIEncoding();
		plugin = p;
	}
		

	public override void Write(string s) {

		try {
			plugin.AppendTextNoNewline(s);
		} catch (Exception e) {
			Console.Error.WriteLine("IW Exception: " + e);
		}
			
	}
		
	public override void WriteLine(string s) {

		try {
			plugin.AppendText(s);
		} catch (Exception e) {
			Console.Error.WriteLine("IW Exception: " + e);
		}
	}
}
