<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      
      
      <div class="title">FAQs &amp; General Help</div>

      <p>This page is a copy of the files FAQS, KNOWN_ISSUES, and README with
      some edits, from the MonoDevelop/ directory in the Subversion
      repository. Please download a copy from svn if this page is out of
      date. Last updated: Feb 27, 2004.</p>

      <div class="headlinebar">FAQ</div>

<p>Why aren't my C# files syntax highlighted?</p>
       
<p>GNOME doesn't recognize *.cs files as the text/x-csharp
mimetype.  gtksourceview-sharp tries to set this up automatically,
but it may not work in all cases. You can use
gnome-file-types-properties to do this.</p>
        
<p>What if the configuration summary says 'no' for one of the requirements?</p>
	   
<p>The configure script uses pkg-config to see if you have the
required packages to build.  If it can't detect a package that
you have installed:</p>
	    
<p>add the path to the &lt;package&gt;.pc file to PKG_CONFIG_PATH.</p>

<pre class="code">export PKG_CONFIG_PATH=/usr/local/lib/pkgconfig:$PKG_CONFIG_PATH</pre>
	     
<p>install a newer version or the development counterpart
of that package and rerun ./configure.
</p>

<div class="headlinebar">Known Issues</div>

<p>This list tracks know issues, hopefully to prevent duplicate bug reporting
upon release.
       
<ul><li>When you iconify a dockitem, close MonoDevelop and reopen, the dockitem is
  gone.
</li><li>When you maximize MonoDevelop, the docks do not resize properly.
</li><li>The toolbar sometimes exhibit interesting behaviour including but not
  limited to looking disabled and working, looking enabled and not working
  and others.
</li><li>Lingering code completion issues
</li><li>When you click on  File-&gt;Recent Projects-&gt;clear recent project
  list or File-&gt;Recent Files-&gt;clear recent files list you need a
  confimartion dialog in order to avoid clearing accidentaly.</li></ul>

<div class="headlinebar">Readme</div>

<p>This is MonoDevelop which is intended to be a full-featured
integrated development environment (IDE) for mono and Gtk#.
It was originally a port of SharpDevelop 0.98.</p>
       
<b>Compiling for users</b><br />

<pre class="code">
./configure
make
make install
</pre><br />
        
<b>Compiling for developers</b>

<p>To compile run the following command:</p>
 
<pre class="code">
./autogen.sh
make
</pre>
	  
<p>To run MonoDevelop:</p>
<pre class="code">make run</pre>
	   
<p>Example:
<pre class="code">make clean &amp;&amp; make &amp;&amp; make
run</pre></p><br />
	    
<b>Installing</b>

<p>Installing is currently optional.
(Use make run to use MonoDevelop without installing.)</p>
	     
<pre class="code">make install</pre>
	      
<p><b>Dependencies</b></p>

<ul>
<li>Mono &gt;= 0.30 with ICU enabled</li>
<li>Gtk# cvs</li>
<li>ORBit2 &gt;= 2.8.3</li> 
<li>gnome-vfs &gt;= 2.0</li>
<li>gtksourceview &gt;= 0.7*</li>
<li>gtksourceview-sharp from mono's cvs</li>
</ul>

<p>*gtksourceview is available on <a
href="http://www.ximian.com/red_carpet/">Red Carpet</a>, on many of the <a href="http://www.opencarpet.org/">OpenCarpet</a> channels.</p>

<p>See <a
href="http://lists.ximian.com/archives/public/monodevelop-list/2004-January/000129.html">http://lists.ximian.com/archives/public/monodevelop-list/2004-January/000129.html</a>
for more information about the GtkSourceView dependency.</p>
	        
<b>References</b>

<ul>
<li><a href="http://www.icsharpcode.net/TechNotes/
">SharpDevelop Tech Notes</a></li>
		 
<li><a href="Gnome Human Interface Guidelines (HIG)
http://developer.gnome.org/projects/gup/hig/1.0/
		  
">freedesktop.org standards</a></li>
<li><a href="http://developers.sun.com/solaris/articles/integrating_gnome.html
">Integrating with GNOME (a little out of date)</a></li>
</ul>	    

<ccms:PageFooter runat="server"/>
