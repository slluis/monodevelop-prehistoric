<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>      
      
      <div class="title">Contribute</div>
      <p>
        MonoDevelop is always looking for contributors to help with development.  If you have some time and want to help, there are instructions below.
      </p>
      
      <div class="headlinebar"><a target="bugs">Reporting Bugs</a></div>
      <p>
        Reporting bugs is a very easy way to help contribute to any project.  To report a bug, follow the steps below:</p>
        <ol>
          <li>Ensure the bug you are reporting hasn't already been fixed. Install the latest version of MonoDevelop using the "Current Development" instructions on the <a href="download.aspx">download page</a>.</li>
	  <li>In your web browser, load <a href="http://bugzilla.ximian.com/" target="ximian-bugzilla">bugzilla.ximian.com</a>.</li>
          <li>Review the <a href="http://bugzilla.ximian.com/buglist.cgi?product=Mono+Develop&bug_status=NEW&bug_status=ASSIGNED&bug_status=REOPENED&email1=&emailtype1=substring&emailassigned_to1=1&email2=&emailtype2=substring&emailreporter2=1&changedin=&chfieldfrom=&chfieldto=Now&chfieldvalue=&short_desc=&short_desc_type=substring&long_desc=&long_desc_type=substring&bug_file_loc=&bug_file_loc_type=substring&keywords=&keywords_type=anywords&op_sys_details=&op_sys_details_type=substring&version_details=&version_details_type=substring&cmdtype=doit&order=Reuse+same+sort+as+last+time&form_name=query">open MonoDevelop bugs</a> and make sure that your bug hasn't been submitted already.</li>
          <li>If it hasn't been submitted, submit a <a href="http://bugzilla.ximian.com/enter_bug.cgi">new MonoDevelop bug</a>.</li>
        </ol>

      <a target="patches"><div class="headlinebar">Submitting Patches</div></a>
      <p>
        If you submit a patch, it's always best to create the patch against the current snapshot version of MonoDevelop.  If you cannot use this, then make sure you have the source for the latest release. The commit archive can be found <a href="http://lists.ximian.com/archives/public/monodevelop-patches-list/">here</a>.
      </p>

      <b>Creating a patch:</b>
      <ol>
        <li>Copy the file before you edit it.</li>
	<li>Make the changes you want to make and save the file.</li>
	<li>On the command line type <tt>diff -u newfile.cs oldfile.cs > vardec.patch</tt>.  The name of the patch should be something quick which describes what you fixed.</li>
	<li>Create a bug in <a href="http://bugzilla.ximian.com/">bugzilla.ximian.com</a> and attach the patch to the bug.  If you're patching against a pre-existing bug, just attach to the bug.</li>
      </ol>

<ccms:PageFooter runat="server"/>
