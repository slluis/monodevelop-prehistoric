<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      
      
      <div class="title">About</div>

      <div class="headlinebar">What is MonoDevelop?</div>
      <p>
	MonoDevelop is a project to port SharpDevelop to Gtk#.  There are numerous goals that MonoDevelop hopes to achieve.  Some of these are:
      </p>

           <ul>
              <li>To create a best of breed development environment for
Unix systems for C# and Mono. </li>
              <li>Since its written in Gtk#, and we like Gtk# and we
get good support from Gtk#, most likely it will add functionality to
improve the Gtk# experience. </li>
              <li>Today the IDE is a simple IDE and on Unix does not do
GUI design, but we plan on adding a GUI designer.
              </li>
              <li>We want to integrate the tools we have been building
so far, so things like MonoDoc, NUnit-Gtk and the debugger should
target MonoDevelop. </li>
            </ul>

	<p>To see some of the current features, visit the <a href="/index.aspx">features page</a>.</p>

	<div class="headlinebar">License Information</div>
	<p>
	  MonoDevelop is licensed under the <acronym title="General Public License">GPL</acronym> which can be read at <a href="http://www.gnu.org/copyleft/gpl.html">www.gnu.org/copyleft/gpl.html</a>.  All the source code is available through the Subversion file repository.  Read the <a href="/download.aspx">download page</a> for directions on downloading the source.
	</p>
<ccms:PageFooter runat="server"/>
