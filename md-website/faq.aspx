<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      
      
      <div class="title">FAQs &amp; General Help</div>

      <p>This page is a copy of the files FAQS, KNOWN_ISSUES, and README from the MonoDevelop/ directory in the subversion repository. Please download a copy from svn if this page is out of date. Last updated: Feb 27, 2004.</p>

      <div class="headlinebar">FAQs</div>

      <pre>
- Why aren't my C# files syntax highlighted?
       
GNOME doesn't recognize *.cs files as the text/x-csharp
mimetype.  gtksourceview-sharp tries to set this up automatically,
but it may not work in all cases. You can use
gnome-file-types-properties to do this.
        
- What if the configuration summary says 'no' for one of the
  requirements?
	   
The configure script uses pkg-config to see if you have the
required packages to build.  If it can't detect a package that
you have installed:
	    
add the path to the &lt;package&gt;.pc file to PKG_CONFIG_PATH
ex. export PKG_CONFIG_PATH=/usr/local/lib/pkgconfig:$PKG_CONFIG_PATH
	     
install a newer version or the development counterpart
of that package and rerun ./configure
ex. gnome-vfs2-devel-2.4.1-1.rpm
      </pre>

      <div class="headlinebar">Known Issues</div>

      <pre>
This list tracks know issues, hopefully to prevent duplicate bug reporting
upon release.
       
* When you iconify a dockitem, close MonoDevelop and reopen, the dockitem is
  gone.
* When you maximize MonoDevelop, the docks do not resize properly.
* The toolbar sometimes exhibit interesting behaviour including but not
  limited to looking disabled and working, looking enabled and not working
  and others.
* Lingering code completion issues
* When you click on  File-&gt;Recent Projects-&gt;clear recent project list or
  File-&gt;Recent Files-&gt;clear recent files list you need a confimartion dialog
  in order to avoid clearing accidentaly.
      </pre>

      <div class="headlinebar">Readme</div>

      <pre>
This is MonoDevelop which is intended to be a full-featured
integrated development environment (IDE) for mono and Gtk#.
It was originally a port of SharpDevelop 0.98.
See http://monodevelop.com/ for more info.
       
Compiling for users
-------------------
./configure
make
make install
        
Compiling for developers
------------------------
To compile run the following command:
	 
./autogen.sh
make
	  
To run MonoDevelop:
make run
	   
Example:
make clean &amp;&amp; make &amp;&amp; make run
	    
Installing
----------
Installing is currently optional.
(Use make run to use MonoDevelop without installing.)
	     
make install
	      
Dependencies
------------
Mono &gt;= 0.30 with ICU enabled
Gtk# cvs
ORBit2 &gt;= 2.8.3
gnome-vfs &gt;= 2.0
gtksourceview &gt;= 0.7
gtksourceview-sharp from mono's cvs
(gtksourceview is available on Red Carpet,
on many of the OpenCarpet channels.)
	       
See http://lists.ximian.com/archives/public/monodevelop-list/2004-January/000129.html.
	        
References
----------
SharpDevelop Tech Notes
http://www.icsharpcode.net/TechNotes/
		 
Gnome Human Interface Guidelines (HIG)
http://developer.gnome.org/projects/gup/hig/1.0/
		  
freedesktop.org standards
http://freedesktop.org/Standards/
		   
Integrating with GNOME (a little out of date)
http://developers.sun.com/solaris/articles/integrating_gnome.html
		    
Discussion, Bugs, Patches
-------------------------
monodevelop-list@lists.ximian.com (questions and discussion)
monodevelop-patches-list@lists.ximian.com (track commits to MonoDevelop)
monodevelop-bugs@lists.ximian.com (track MonoDevelop bugzilla component)
http://bugzilla.ximian.com (submit bugs and patches here)		     
      </pre>

<ccms:PageFooter runat="server"/>
