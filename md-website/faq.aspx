<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      
      
      <div class="title">FAQs &amp; General Help</div>

      <p>Last updated: Apr 1, 2004.</p>

      <div class="headlinebar">FAQ</div>

<div class="faq">
	<div class="question"><a name="gdb">How do I get a backtrace with gdb?</a></div>
	<ol>
		<li>Add the following to ~/.gdbinit:<br />
			<pre>
handle SIGPWR nostop noprint pass
handle SIGXCPU nostop noprint pass
</pre>
		</li>
		<li>cd /usr/lib/monodevelop/bin</li>
		<li>gdb mono</li>
		<li>run MonoDevelop.exe</li>
		<li>thread apply all bt</li>
	</ol>
</div>
<br/>
<div class="faq">
	<div class="question"><a name="orbit_crash">Crashes when quiting MonoDevelop?</a></div>
<p>If you get crashes when you close MonoDevelop, check your ORBit version.
Version 2.8.2 is known to cause issues. Upgrade to 2.8.3 if at all possible.</p>
<p>NOTE: This update could be harmful to your gnome system, and should <b>NOT</b> be done lightly.</p>
</div>
<br/>
<div class="faq">
	<div class="question"><a name="mimetype_gnome26">How do I add mimetypes in gnome 2.6?</a></div>
<p>First you copy monodevelop.xml to $(gnome_prefix)/share/mime/packages. Then you run update-mime-database $(gnome_prefix)/share/mime. On fedora, and many other distros $gnome_prefix is /usr, so you do:</p>
<pre class="code">cp monodevelop.xml /usr/share/mime/packages
update-mime-database /usr/share/mime</pre>
<p>You might have to do these operations as the root user. If for some reason this still doesnt solve the problem, you can attempt to:</p>
<pre class="code">find /usr/share/mime -type f -exec chmod 644 {} \;</pre>
<p>With a user who has permissions to do so.</p>
</div>
<br/>
<div class="faq">
	<div class="question"><a name="gecko-sharp">Where do I get gecko-sharp.pc?</a></div>
<p>gecko-sharp.pc can be found in the gtkmozembed-sharp CVS module in the Mono
CVS repository.</p>
</div>
<br/>
<div class="faq">
	<div class="question"><a name="syntax_highlighting">Why aren't my C# files syntax highlighted?</a></div>
<p>GNOME doesn't recognize *.cs files as the text/x-csharp
mimetype.  gtksourceview-sharp tries to set this up automatically,
but it may not work in all cases. You can use
gnome-file-types-properties to do this. If you are running gnome 2.6, see
above.</p>
</div>
<br/>  
<div class="faq">
	<div class="question"><a name="pkgconfig">What if the configuration summary says 'no' for one of the requirements?</a></div>
<p>The configure script uses pkg-config to see if you have the
required packages to build.  If it can't detect a package that
you have installed:</p>
	    
<p>add the path to the &lt;package&gt;.pc file to PKG_CONFIG_PATH.</p>

<pre class="code">export PKG_CONFIG_PATH=/usr/local/lib/pkgconfig:$PKG_CONFIG_PATH</pre>
	     
<p>install a newer version or the development counterpart
of that package and rerun ./configure.
</p>
</div>
<br/>
<div class="headlinebar">Known Issues</div>

<p>This list tracks know issues, hopefully to prevent duplicate bug reporting
upon release.
       
<ul><li>When you iconify a dockitem, close MonoDevelop and reopen, the dockitem is
  gone.
</li><li>When you maximize MonoDevelop, the docks do not resize properly in all docking configurations.</li>
</ul>
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
