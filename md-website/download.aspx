<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>
    <div class="title">Download MonoDevelop</div>
    <p>
      There are two ways to download and setup MonoDevelop.  If you want to try the latest release, then you'll want the <a href="#package">package</a> download.  On the other hand, if you want to contribute MonoDevelop, then use the <a href="#svn">Subversion</a> version.
    </p>
    
    <p>
      There are a few pre-requisites, no matter which install method you want.  These are:
    </p>
      <ul>
        <li>Mono 0.30 with ICU support enabled</li>
        <li>gtkmozembed (included in the MonoDevelop source)</li>
        <li>Gtk# CVS</li>
        <li>ORBit2-2.8.3 or newer</li>
        <li>GtkSourceView#</li>
      </ul>
    
    <div class="headlinebar"><a name="package">Packages</a></div>
      <ol>
        <li>Download the <a href="release.aspx">latest MonoDevelop release</a>.</li>
        <li>If you downloaded a tarball, load a console window and enter the directory you downloaded the tarball to.  Run the command <tt>tar -xvzf MonoDevelop-x.xx.tar.gz</tt> where x.xx is the version number.  Once the package has uncompressed, enter the directory.</li>
        <li>In the main MonoDevelop source directory, run <tt>./confgure --prefix=/usr/local</tt></li>
	<li>Next, run <tt>make</tt>.</li>
        <li>To run MonoDevelop, type <tt>make run</tt>.</li>
      </ol>
    <div class="headlinebar"><a name="svn">Current Development</a></div>
    <p>If you would like step-by-step instructions on building a svn snapshot of MonoDevelop, please read the <a href="tutorial.aspx">"Hello World" Tutorial</a>.</p>
      <ol>
      	<!--
        <li>Type <tt>svn co svn://207.44.131.184/svn/monodevelop/trunk/MonoDevelop</tt>.  After the process is done, cd into the new directory called <tt>MonoDevelop</tt>.</li>
        <li>If you have installed gtkmozembed-sharp before, skip this step.  In the <tt>gtkmozembed-sharp</tt> directory, run <tt>make</tt> and then <tt>make install</tt>.</li>
        <li>In the main MonoDevelop source directory, run <tt>make</tt>.</li>
        <li>To run MonoDevelop, type <tt>make run</tt>.</li>
	-->
	<li>Unfortunately the anonymous svn mirror is currently down, however we are publishing regular snapshots <a href="http://devservices.go-mono.com/MonoDevelop/">here</a></li>
	<li>After downloading the most recent snapshot, you will currently need gtk-sharp from cvs and gtksourceview-sharp from cvs.</li>
	<li>Then run <tt>./autogen.sh</tt> and <tt>make</tt></li>
	<li>Currently <tt>make install</tt> is supported, but may not work completely. <tt>make run</tt> from inside the MonoDevelop directory is currently the recommended way to run MD</li>
      </ol>
    
<ccms:PageFooter runat="server"/>
