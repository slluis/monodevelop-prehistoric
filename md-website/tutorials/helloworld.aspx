<%@ Register TagPrefix="ccms" TagName="PageHeader" src="/include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="/include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      

      <div class="headlinebar">Writing "Hello World"</div>
      <p>This is intended as a quick guide to get you familiar with MonoDevelop by writing your first HelloWorld GTK# application.</p>

      <ul>
         <li>Run MonoDevelop from its svn directory using `<tt>make run &amp;</tt>'.</li>
	 <li>Create a new Combine (project container) using <tt>File -&gt; New -&gt; Combine...</tt>. You should be presented with a dialog like this; fill the textboxes in a similar manner and choose "GtkSharp Project".<br />
	 <div align="center"><a href="images/tutorial/tutorial1.png" target="_blank"><img src="images/tutorial/tutorial1sm.png" /></a></div><br/></li>
	 <li>Open `Main.cs' and `MyWindow.cs' from the file browser on the left. If you do not see the appropriate list of files, click the `.' in the directory browser.<br />
	 <div align="center"><a href="images/tutorial/tutorial2.png" target="_blank"><img src="images/tutorial/tutorial2sm.png" /></a></div><br /></li>
	 <li>Modify `MyWindow.cs' to look like the following file:<br />

<pre class="code">
using System;
using Gtk;

public class MyWindow : Window {
	static GLib.GType gtype;
	Button button;
	
	public static new GLib.GType GType
	{
		get
		{
			if (gtype == GLib.GType.Invalid)
				gtype = RegisterGType (typeof (MyWindow));
			return gtype;
		}
	}
	
	public MyWindow () : base (GType)
	{
		button = new Button("This is a button.");
		button.Clicked += new EventHandler(button_Clicked);
	
		this.Title = "MyWindow";
		this.SetDefaultSize (400, 300);
		this.DeleteEvent += new DeleteEventHandler (MyWindow_Delete);
		this.Add(button);
		this.ShowAll ();
	}
	
	void MyWindow_Delete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}
	
	 void button_Clicked (object obj, EventArgs args)
	{
		Console.WriteLine("Hello World");
		Application.Quit ();
	}
}
</pre>
      </li>
      <li>Compile the program from within MonoDevelop using the "build" icon (3rd button from the right) in the toolbar:
      <div align="center"><img src="images/tutorial/tutorial3sm.png" /></div><br /></li>
      <li>Run the program using the "gear" icon (far right) in the toolbar. The resulting window should look something like this:
      <div align="center"><img src="images/tutorial/tutorial4sm.png" /></div><br /></li>
      </ul>
      
      <p>Congratulations! You've successfully built a program using the latest copy of MonoDevelop. Please let us know of any bugs you find in <b>working</b> features.</p>

      <hr />

      <p>This document was written by <a href="mailto:steve@citygroup.ca">Steve Deobald</a> and is licensed under Creative Commons, Share-Alike, Attribution. If this document contains errors or could be improved, please let me know.
      </p>
      <br /><br />

<ccms:PageFooter runat="server"/>
