<%@ Register TagPrefix="ccms" TagName="PageHeader" src="/include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="/include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      

      <div class="title">Installation Tutorial</div>

      <p>Before reading any further, please consider installing MonoDevelop
      from binary packages. This is most likely far easier for you in the
      short term, and better for you in the long term. MonoDevelop packages
      are available from the redcarpet mono channel, or from
      <a href="http://www.go-mono.com/download.html">go-mono.com</a>.</p>

      <p>Most likely, this is the path you want to take.</p>

      <p>This tutorial is your guide to installing MonoDevelop releases using official tarball packages. If you would like to build MonoDevelop from snapshots, please see the 
<a href="svn_install.aspx">svn tutorial</a>.</p>
      <p>Unless specified otherwise, build and install all tarball packages using:
<pre class="code">
tar -xzf mypackage.tar.gz
cd mypackage
./configure --prefix=/usr
make
make install
</pre>
      </p>
      
      <br /><br />
      <div class="headlinebar">Getting Started: Preliminaries</div>
      <p>There are some packages which are required before installing MonoDevelop. The following 
      instructions guide you through installing them.
      </p>

      <br /><br />
      <b>1. GtkSourceView 1.0+</b>
      <p>You may download a binary package for your distribution if it provides version 1.0 or higher. On computers without gtk+ 2.4, you will need to download and install 1.0.1
Otherwise, download the 
<a href="http://ftp.acc.umu.se/pub/gnome/sources/gtksourceview/1.0/gtksourceview-1.0.0.tar.gz">official 
tarball (1.0.0)</a>. 
The `<tt>./configure</tt>' line is a little more involved than usual. Use the example below:
<pre class="code">
tar -xzf gtksourceview-1.0.0.tar.gz
cd gtksourceview-1.0.0
./configure --prefix=`pkg-config --variable=prefix ORBit-2.0`
make
make install
</pre>
      </p>

      <br /><br />
      <b>2. gtkmozembed</b>
      <p><a href="http://www.mozilla.org/unix/gtk-embedding.html">gtkmozembed</a> can generally be found in 
the Mozilla development package for your OS. For example:
      <br />
         <ul>
            <li>Debian: `mozilla-dev'</li>
            <li>RedHat: `mozilla-devel'</li>
	    <li>FreeBSD: `mozilla-gtkmozembed'</li>
            <li>Tarball: <a href="http://ftp.mozilla.org/pub/mozilla.org/mozilla/releases/">ftp.mozilla.com</a></li>
         </ul>
      </p>

      <br /><br />
      <b>3. Install Mono</b><br />
      <p>MonoDevelop will require the following mono packages to be installed, in this order:
      <ul>
<li><a href="ftp://www-126.ibm.com/pub/icu/2.6/icu-2.6.tgz">International Components for Unicode 2.6 or greater</a></li>
<li><a href="http://www.go-mono.com/archive/1.0/mono-1.0.tar.gz">mono 1.0</a></li>
<li><a href="http://www.go-mono.com/archive/1.0/gtk-sharp-1.0.tar.gz">gtk-sharp 1.0</a></li>
<li><a href="http://www.go-mono.com/archive/1.0/monodoc-1.0.tar.gz">monodoc 1.0</a></li>
<li><a href="http://www.go-mono.com/archive/1.0/gtksourceview-sharp-0.5.tar.gz">gtksourceview-sharp 0.5</a></li>
<li><a href="http://www.go-mono.com/archive/1.0/gecko-sharp-0.5.tar.gz">gecko-sharp 0.5</a></li>
      </ul></p>
      <p>When building from the source provided above, always use a prefix of `<tt>/usr</tt>'.</p>
      <p>Some packages are also available as prebuild binary packages (RPMs and DEBs). 
Binary packages can be found at <a href="http://www.go-mono.com/download.html">mono download page</a> 
for RedHat, Fedora, Suse, and Debian. They are also available through Ximian's 
<a href="http://www.ximian.com/products/redcarpet/">Red Carpet</a>, in the `mono' channel. Currently, binary packages exist for these modules for most distros:
      <ul>
         <li>mono</li>
         <li>ICU (`icu' and `libicu26')</li>
	 <li>gtk-sharp</li>
	 <li>monodoc</li>
	 <li>gecko-sharp</li>
	 <li>gtksourceview-sharp</li>
	 <li>monodevelop</li>
      </ul>
</p>

      
      <br /><br /><br />
      <div class="headlinebar">Installing MonoDevelop</div>      
      <p>The final step in this process is to build MonoDevelop itself. Download the 
<a href="http://www.go-mono.com/archive/1.0.2/monodevelop-0.5.1.tar.gz">MonoDevelop 0.5</a> package.</p>

<pre class="code">
export PKG_CONFIG_PATH="/usr/lib/pkgconfig"
tar -xzf monodevelop-0.5.1.tar.gz
cd monodevelop-0.5.1
./configure --prefix=/usr
make
make install
</pre>
      
      <br />
      <p>Congratulations! You now have the latest copy of MonoDevelop installed. Don't forget to report 
all the bugs you find.</p>
      
      <br/><br/>
      <hr width="90%" />
      <p>This document was written by <a href="mailto:steve@citygroup.ca">Steve Deobald</a> 
and is licensed under Creative Commons, Share-Alike, Attribution. If this document contains errors or could 
be improved, please let me know.</p>
      

<ccms:PageFooter runat="server"/>
