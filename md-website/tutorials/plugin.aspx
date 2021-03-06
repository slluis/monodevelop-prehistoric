<%@ Register TagPrefix="ccms" TagName="PageHeader" src="/include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="/include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      

      <div class="title">Writing a Monodevelop Plugin</div>

          <div class="headlinebar">Introduction</div>
	<p>MonoDevelop (and SharpDevelop) have been written so that they
    can be easily extended by others.  This can be accomplished by doing
	two simple things. First, by creating an assembly (dll) containing
	the code for your addin.  Second, providing an .addin XML file that
    maps your code into MonoDevelop.  There is a detailed pdf available
    at SharpDevelop's website <a href="http://www.icsharpcode.net/TechNotes/ProgramArchitecture.pdf">here</a> that you will want to read for a 
    full understanding of the entire system and possiblities.  This is
    intended as a simple and quick overview.</p>

	<div class="headlinebar">Terms</div>
	<p>AddIn - what other systems term a plugin, also used for the core
	application.<br />
	Pad - content area like the project browser or output pad. <br />
	View - main content area, like the SourceEditor.<br />
    </p>

    <div class="headlinebar">AddIn assembly</div>
	<p>In your code you can extend the IDE at pretty much any point.
	Some common things would be to extend the menus, pads, views,
    services, commands, etc.  I recommend looking at src/AddIns/ for a
	few examples.  In most cases you will simply inherit from an
	abstract class or implement an interface for the various parts you
	are extending. For example, a new service could be defined as:</p>
<pre class="code">
using System;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services;
{
	public class ExampleService : AbstractService
	{
	}
}
</pre>

<p>Here is a list of some of the common classes to extend for an AddIn:
<pre class="code">
./src/Main/Base/Gui/Dialogs/AbstractOptionPanel.cs
./src/Main/Base/Gui/Dialogs/Wizard/AbstractWizardPanel.cs
./src/Main/Base/Gui/Pads/ClassScout/BrowserNode/AbstractClassScoutNode.cs
./src/Main/Base/Gui/Pads/ProjectBrowser/BrowserNode/AbstractBrowserNode.cs
./src/Main/Base/Gui/AbstractBaseViewContent.cs
./src/Main/Base/Gui/AbstractPadContent.cs
./src/Main/Base/Gui/AbstractViewContent.cs
./src/Main/Base/Gui/AbstractSecondaryViewContent.cs
</pre>
</p>

    <div class="headlinebar">.addin file</div>
    <p>The addin file basically maps the "entry" points of your code
	into the various parts of the IDE.  You specify services to load,
    append menus in a certain place, and virtually everything else.
	Since the entire application is an AddIn there is no limit.
    It supports conditional directives and other advanced constructs.
    In the following sample MonoDevelopNunit.addin file, you can see
	it specifies the name of the assembly to load, specifies a service
    to load into the /Workspace/Services node, two views and some menus.
    Last, it is important to note the class attribute that is used to
    specify the type to instantiate for that part of the AddIn.</p>
<pre class="code">
	<xmp>
<AddIn name      = "MonoDevelop Nunit"
       author    = "John Luke"
       copyright = "GPL"
       url       = "http://monodevelop.com"
       description = "NUnit testing tool"
       version   = "0.2">
 
        <Runtime>
                <Import assembly="MonoDevelop.Nunit.dll"/>
        </Runtime>
 
        <Extension path="/Workspace/Services">
                <Class id = "NunitService"
                    class = "MonoDevelop.Services.NunitService"/>
        </Extension>
 
        <Extension path="/SharpDevelop/Workbench/Views">
                <Class id    = "NunitTestTree"
                       class = "MonoDevelop.Nunit.Gui.TestTree"/>
                <Class id    = "NunitResultTree"
                       class = "MonoDevelop.Nunit.Gui.ResultTree"/>
        </Extension>
 
        <Extension path="/SharpDevelop/Workbench/MainMenu/Tools">
                <MenuItem id = "NunitMenu" label = "NUnit" insertafter = "ExternalTools" insertbefore = "Options">
                        <MenuItem id = "LoadTestAssembly"
                          label = "Load Assembly"
                                  shortcut = ""
                              class = "MonoDevelop.Commands.NunitLoadAssembly" />
                        <MenuItem id = "NunitRunTests"
                          label = "Run Tests"
                                  shortcut = ""
                              class = "MonoDevelop.Commands.NunitRunTests" />
                </MenuItem>
        </Extension>
</AddIn>
	</xmp>

</pre>

    <div class="headlinebar">AddIn xml format</div>
	<p>There is an AddIn.xsd file that specifies the required/optional
	xml format. Perhaps someone would like to make a RelaxNG one also.
	See data/resources/AddIn.xsd</p>

    <div class="headlinebar">Building and installing</div>
    <p>We currently support both running in a self-contained build/
    directly as well as installing to $(prefix)/lib/monodevelop so you
    will want to make sure both your .addin file and .dll are placed
    into the AddIn directory in both places.  Note: this this may change
    at some point in the future.</p>

	<div class="headlinebar">Existing Examples</div>
<ul>
  <li>SourceEditor</li>
  <li>CSharpBinding</li>
  <li>DebuggerAddin</li>
  <li>Monodoc</li>
  <li>StartPage (not fully ported)</li>
  <li>NUnit (incomplete)</li>
</ul>

    <div class="headlinebar">Caveats</div>
    <p>Although SharpDevelop and MonoDevelop currently use the same
    format this may not always be the case.  Also, while non-gui addins
    could possibly be reused, MonoDevelop and SharpDevelop use different
    GUI toolkits that will likely prevent sharing many things.</p>

    <div class="headlinebar">AddIn ideas</div>
    <p>There are various things that would be nice to have implemented
    as addins.  Here is a brief list of the top of my head.
<ul>
  <li>A viewer for the mono profiler (mono --profile) and mono coverage tools.</li>
  <li>Extra languages/compilers support.</li>
  <li>NUnit and NAnt integration tools.</li>
  <li>Glade (although a new GUI designer is planned).</li>
  <li>Integration with Subversion, CVS, and other version control tools.</li>
  <li>UML/CASE tools.</li>
  <li>SQL/Database support.</li>
  <li>An advanced XML editor.</li>
  <li>Also, there are some additional things that SharpDevelop already has that could be ported to MonoDevelop.</li>
</ul></p>

	<div class="headlinebar">Credits and Errata</div>
	<p>Send comments to <a href="mailto:jluke@cfl.rr.com">jluke@cfl.rr.com</a> or the <a href="mailto:monodevelop-list@lists.ximian.com">monodevelop mailing list</a>.</p>
    <p>Last updated March 24, 2004</p>


<ccms:PageFooter runat="server"/>