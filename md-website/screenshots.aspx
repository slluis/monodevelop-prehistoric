<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

      <div class="title">Screenshots</div>
      
      <div class="image_frame">
        <div class="image">
          <a href="/images/screenshots/newshot.png" target="_blank">
            <img src="/images/screenshots/newshot.png" alt="" />
          </a>
        </div>
        <div class="image_caption">
          MonoDevelop circa 0.3
        </div>
      </div>
      <br />
      <div class="image_frame">
        <div class="image">
          <a href="/images/screenshots/nemerle-experiemental.png" target="_blank">
	   <img src="/images/screenshots/thumbnails/nemerle-experiemental.png" alt="" />
          </a>
        </div>
        <div class="image_caption">
          Experimental support for the Nemerle language
        </div>
      </div>

      <br /><br />

      <div class="image_frame">
	<div class="image">
	  <a href="/images/screenshots/macosx.jpg" target="_blank">
	    <img src="/images/screenshots/thumbnails/macosx.jpg" alt="" />
	  </a>
	</div>
	<div class="image_caption">
	  MonoDevelop running on MacOS X
	</div>
	</div>
      </div>
      <p>Unedited user-submitted screenshots are <a href="user-screenshots.aspx">also available here</a>.</p>
      
<ccms:PageFooter runat="server"/>
