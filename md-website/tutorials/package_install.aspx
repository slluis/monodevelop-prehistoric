<%@ Register TagPrefix="ccms" TagName="PageHeader" src="/include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="/include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      

      <div class="title">Installation Tutorial</div>

      <p>This tutorial is your guide to installing MonoDevelop releases using official packages. If you would like to build MonoDevelop from snapshots, please see the 
<a href="snapshot_install.aspx">snapshot tutorial</a>.</p>
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

      <b>1. ORBit2 2.8.3</b>
      <p>Although newer versions of ORBit <i>may</i> work, they have been known to produce non-fatal crashes when MonoDevelop exits. If in doubt, <a href="http://ftp.gnome.org/pub/GNOME/sources/ORBit2/2.8/ORBit2-2.8.3.tar.gz">download ORBit 2.8.3</a>.</p>

      <br /><br />
      <b>2. GtkSourceView 0.7+</b>
      <p>You may download a binary package for your distribution if it provides version 0.7 or higher. 
Otherwise, download the 
<a href="http://ftp.acc.umu.se/pub/gnome/sources/gtksourceview/0.7/gtksourceview-0.7.0.tar.gz">official 
tarball (0.7)</a>. 
The `<tt>./configure</tt>' line is a little more involved than usual. Use the example below:
<pre class="code">
tar -xzf gtksourceview-0.7.0.tar.gz
cd gtksourceview-0.7.0
./configure --prefix=`pkg-config --variable=prefix ORBit-2.0`
make
make install
</pre>
      </p>

      <br /><br />
      <b>3. gtkmozembed</b>
      <p><a href="http://www.mozilla.org/unix/gtk-embedding.html">gtkmozembed</a> can generally be found in 
the Mozilla development package for your OS. For example:
      <br />
         <ul>
            <li>Debian: `mozilla-dev'</li>
            <li>RedHat: `mozilla-devel'</li>
	    <li>FreeBSD: `mozilla-gtkmozembed'</li>
         </ul>
      I have yet to find an official tarball for gtkmozembed. If you know of one, please 
<a href="mailto:steve@citygroup.ca">let me know</a>.
      </p>

      <br /><br />
      <b>4. Install Mono 0.31</b><br />
      <p>MonoDevelop will require the following mono packages to be installed, in this order:
      <ul>
<li><a href="ftp://www-126.ibm.com/pub/icu/2.8/icu-2.8.tgz">International Components for Unicode 2.8</a></li>
<li><a href="http://www.go-mono.com/archive/mono-0.31.tar.gz">mono with ICU</a></li>
<li><a href="http://sourceforge.net/project/showfiles.php?group_id=40240">gtk-sharp</a></li>
<li><a href="http://www.go-mono.com/archive/monodoc-0.13.tar.gz">monodoc</a></li>
<li><a href="http://www.go-mono.com/archive/gtksourceview-sharp-0.1.0.tar.gz">gtksourceview-sharp</a></li>
<li><a href="http://www.go-mono.com/archive/gecko-sharp-0.1.tar.gz">gecko-sharp</a></li>
<li><a href="http://www.go-mono.com/archive/mono-debugger-0.6.tar.gz">debugger</a></li>
      </ul></p>
      <p>When building from the source provided above, always use a prefix of `<tt>/usr</tt>'.</p>
      <p>Some packages are also available as prebuild binary packages (RPMs and DEBs). 
Binary packages can be found at <a href="http://www.go-mono.com/download.html">mono download page</a> 
for RedHat, Fedora, Suse, and Debian. They are also available through Ximian's 
<a href="http://www.ximian.com/products/redcarpet/">Red Carpet</a>, in the `mono' channel. Currently, 
binary packages only exist for these modules:
      <ul>
         <li>mono</li>
         <li>ICU (`icu' and `libicu26')</li>
      </ul>
Other binary packages are under development, and will be available soon.
</p>

      
      <br /><br /><br />
      <div class="headlinebar">Installing MonoDevelop</div>      
      <p>The final step in this process is to build MonoDevelop itself. Download the 
<a href="http://www.go-mono.com/archive/monodevelop-0.2.tar.gz">MonoDevelop 0.2</a> package.</p>

<pre class="code">
export PKG_CONFIG_PATH="/usr/lib/pkgconfig"
tar -xjf monodevelop-0.2.tar.gz
cd monodevelop-0.2
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
