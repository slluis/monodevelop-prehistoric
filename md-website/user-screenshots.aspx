<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

      <div class="title">User-submitted Screenshots</div>
      
      <p>This page holds any user-submitted screenshots we've received. If a screenshot highlights a MonoDevelop feature, it may be added to the <a href="screenshots.aspx">official screenshot page</a>. If you would like your screenshot on this page, mail <a href="mailto:steve@citygroup.ca">steve</a> or <a href="mrproper@ximian.com">Kevin</a>. In the near future, this page will be replaced with a mono-powered wiki.</p>
      <br />
      
      <div class="image_frame">
        <div class="image">
          <a href="/images/screenshots/user-submitted001.png" target="_blank">
            <img src="/images/screenshots/thumbnails/user-submitted001.png" alt="" />
          </a>
        </div>
        <div class="image_caption">
          Debugging a GTK# application, integrated documentation
	</div>
      </div>
      <br />
      <div class="image_frame">
        <div class="image">
          <a href="/images/screenshots/webbrowser.png" target="_blank">
            <img src="/images/screenshots/thumbnails/webbrowser.png" alt="" />
          </a>
        </div>
        <div class="image_caption">
          Web browsing using the built in Mozilla component
        </div>
      </div>
      
<ccms:PageFooter runat="server"/>
