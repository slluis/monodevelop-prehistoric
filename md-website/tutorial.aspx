<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      

      <div class="title">"Hello World" Tutorial</div>

      <p>This tutorial is intended to take you through all the necessary steps to get an updated copy of MonoDevelop, build it with all its dependencies, and write "Hello World" with it. This tutorial assumes you will install mono and MonoDevelop to a PREFIX of /usr/local. Substitute your own PREFIX where applicable if this is not the case.
      </p>


      <br /><br />

      <div class="headlinebar">Getting Started</div>
      <p>There are some packages which are required before building MonoDevelop. The following instructions guide you through installing them.
      </p>

      <b>1. <a href="http://oss.software.ibm.com/icu/">International Components for Unicode (ICU)</a></b><br />
      <p>MonoDevelop requires ICU for internationalization. At the time of this writing, it cannot be assumed MonoDevelop will be stable without ICU. Since mono (step 5.) should be built with ICU support, we'll install it first.</p>
      <ul>
         <li>Download the <a href="ftp://www-126.ibm.com/pub/icu/2.8/icu-2.8.tgz">ICU 2.8 tarball</a>.</li>
	 <li>Unpack and install the tarball by typing:
	    
	    <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">tar -xzf icu-2.8.tgz
cd icu/source
chmod +x runConfigureICU configure install-sh
./runConfigureICU LinuxRedHat --disable-64bit-libs
make
make install</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>
	 </li>
      </ul>
      <p>If you have any difficulties installing ICU, refer to <a href="http://oss.software.ibm.com/cvs/icu/~checkout~/icu/readme.html?tag=release-2-8">icu/readme.html</a>.</p>

      <b>2. <a href="http://www.gnome.org/projects/ORBit2/">ORBit2 2.8.3</a></b>

      <p>Although newer versions of ORBit <i>may</i> work, they have been known to produce non-fatal crashes when MonoDevelop exits. If in doubt, <a href="http://ftp.gnome.org/pub/GNOME/sources/ORBit2/2.8/ORBit2-2.8.3.tar.gz">download ORBit 2.8.3</a>. Once you have ORBit2 downloaded, install it with:
	 <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">tar -xzf ORBit2-2.8.3.tar.gz
cd ORBit2-2.8.3
./configure
make
make install</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>


      <b>3. <a href="http://www.mozilla.org/unix/gtk-embedding.html">gtkmozembed</a></b>
      <p>gtkmozembed can generally be found as (or in) a package for your OS. For example:<br />
          * Debian: `mozilla-dev'<br />
          * RedHat: `mozilla-devel'<br />
	  * FreeBSD: `mozilla-gtkmozembed'<br /><br />
      I have yet to find an official tarball for gtkmozembed, but I haven't looked very hard. If you know of one, please <a href="mailto:steve@citygroup.ca">let me know</a>.
      </p>


      <b>4. <a href="http://gtksourceview.sourceforge.net/">GtkSourceView 0.7</a></b>
      <p>GtkSourceView is a widget for displaying sourcecode (imagine that). Since MonoDevelop uses gtksourcevew-sharp, and this is only a wrapper, we must first install GtkSourceView. You may download a binary package for your distribution if it provides version 0.7 or higher. Otherwise, download the <a href="http://ftp.acc.umu.se/pub/gnome/sources/gtksourceview/0.7/gtksourceview-0.7.0.tar.gz">official tarball (0.7)</a>. Then install:</p>
      <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">tar -xzf gtksourceview-0.7.0.tar.gz
cd gtksourceview-0.7.0
./configure --prefix=`pkg-config --variable=prefix ORBit-2.0`
make
make install</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>


      <b>5. <a href="http://www.go-mono.com/download.html">Mono 0.30.1</a>, gtk-sharp, gtksourceview-sharp</b>

      <p>Though you may use a pre-packaged copy of the runtime, we recommend compiling one yourself from CVS. gtk-sharp and gtksourceview-sharp <b>must</b> be compiled from CVS.</p>

      <ul>
         <li>Download the latest cvs copies of mono, mcs, gtk-sharp, and gtksourceview-sharp:
         <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">export CVSROOT=:pserver:anonymous@anoncvs.go-mono.com:/mono
cvs login
cvs -z3 co mcs mono gtk-sharp gtksourceview-sharp</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>
         </li>
	
         <li>Before compiling: Make sure you have`<tt>$PREFIX/lib</tt>' (usually `<tt>/usr/local/lib</tt>') in /etc/ld.so.conf. If you do not, add it with the following:
	 <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">su
echo "/usr/local/lib" &gt;&gt; /etc/ld.so.conf
/sbin/ldconfig
</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>
         
	 <li>Similarly, check to make sure you have PKG_CONFIG_PATH set properly. It can be set with:
	 <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">export PKG_CONFIG_PATH=/usr/local/lib/pkgconfig/
</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>
	 </li>

         <li>You can then compile mono and gtk-sharp using the following:
         <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">cd mono
./autogen.sh --prefix=/usr/local
make fullbuild

cd ../gtk-sharp
./autogen.sh --prefix=/usr/local
make
make install

cd ../gtksourceview-sharp
./autogen.sh --prefix=/usr/local
make
make install
</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>
         <p>If the above instructions do not properly compile mono, please refer to the mono documentation. Troubleshooting will be added to this document at a later date.</p>
         </li>
      </ul>


      <br /><br />


      <div class="headlinebar">Installing MonoDevelop</div>      
      <p>Next on the agenda is building MonoDevelop. Download the latest tarball from <a href="http://devservices.go-mono.com/MonoDevelop/">http://devservices.go-mono.com/MonoDevelop/</a>. Extract and build it using:</p>
      <pre class="console" xmlns="http://www.w3.org/1999/xhtml"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#999999">export PKG_CONFIG_PATH="/usr/local/lib/pkgconfig"
tar -xjf MonoDevelop-rXXXX.tar.bz2
cd MonoDevelop-rXXXX
./autogen.sh --prefix=/usr/local
make
make run &amp;
</td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#555555"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre>

      <p>At this point, you have the choice of either `<tt>make install</tt>' or (preferrably) '<tt>make run</tt>'. Since there is a good chance you'll download a new copy of MonoDevelop tomorrow, we recommend using `<tt>make run</tt>' for now. :)</p>

      
      <br /><br />
      

      <div class="headlinebar">Writing "Hello World"</div>
      <p>This is intended as a quick guide to get you familiar with MonoDevelop by writing your first HelloWorld GTK# application.</p>

      <ul>
         <li>Run MonoDevelop from its svn directory using `<tt>make run &amp;</tt>'.</li>
	 <li>Create a new Combine (project container) using <tt>File -&gt; New -&gt; Combine...</tt>. You should be presented with a dialog like this; fill the textboxes in a similar manner and choose "GtkSharp Project".<br />
	 <div align="center"><a href="images/tutorial/tutorial1.png" target="_blank"><img src="images/tutorial/tutorial1sm.png" /></a></div><br/></li>
	 <li>Open `Main.cs' and `MyWindow.cs' from the file browser on the left. If you do not see the appropriate list of files, click the `.' in the directory browser.<br />
	 <div align="center"><a href="images/tutorial/tutorial2.png" target="_blank"><img src="images/tutorial/tutorial2sm.png" /></a></div><br /></li>
	 <li>Modify `MyWindow.cs' to look like the following file:<br />
<table cellpadding="5" border="0" xmlns="http://www.w3.org/1999/xhtml"><tr><td valign="top"><pre class="code"><table width="100%" border="0" cellspacing="0" cellpadding="3"><tr><td><table width="100%" border="0" cellspacing="0" cellpadding="0" height="100%"><tr><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"></td><td width="100%" bgcolor="#efefef"><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td bgcolor="#ffffff"><table width="100%" border="0" cellspacing="0" cellpadding="2"><tr><td bgcolor="#efefef"><pre>using System;
using Gtk;
using GtkSharp;

public class MyWindow : Window {
	static GLib.GType type;
	static Gtk.Button button;
			
	static MyWindow ()
	{
		type = RegisterGType (typeof (MyWindow));
		button = new Button();
		button.Clicked += new EventHandler(button_Clicked);
	}
													
	public MyWindow () : base (type)
	{
		this.Title = "MyWindow";
		this.SetDefaultSize (400, 300);
		this.DeleteEvent += new DeleteEventHandler (OnMyWindowDelete);
		this.Add(button);
		this.ShowAll ();
	}

	void OnMyWindowDelete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}

	static void button_Clicked (object obj, EventArgs args)
	{
		Console.WriteLine("Hello World");
		Application.Quit ();
	}
}
</pre></td></tr></table></td></tr></table></td><td bgcolor="#999999"><img src="/images/empty.png" width="1" height="1" /></td></tr><tr><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td><td bgcolor="#c0c0c0"><img src="/images/empty.png" width="1" height="1" /></td></tr></table></td></tr></table></pre></td><td valign="top"><img src="source-id:5:xhtml:html/en/gnome/bindings/gtk-sharp/base.png" border="0" /></td></tr></table>
      </li>
      <li>Compile the program from within MonoDevelop using the "build" icon (3rd button from the right) in the toolbar:
      <div align="center"><img src="images/tutorial/tutorial3sm.png" /></div><br /></li>
      <li>Run the program using the "gear" icon (far right) in the toolbar. The resulting window should look something like this:
      <div align="center"><img src="images/tutorial/tutorial4sm.png" /></div><br /></li>
      </ul>
      
      <p>Congratulations! You've successfully built a program using the latest copy of MonoDevelop. Please let us know of any bugs you find in <b>working</b> features.</p>

      <hr />

      <p>This document was written by Steve Deobald (steve [at] citygroup.ca) and is licensed under the GNU Free Documentation License, Version 1.1 or any later version published by the Free Software Foundation. If this document contains errors or could be improved, please let me know.</p>
      <br /><br />

<ccms:PageFooter runat="server"/>
