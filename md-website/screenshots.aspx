<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

      <div class="title">Screenshots</div>
      
      <div class="image_frame">
        <div class="image">
          <a href="/images/screenshots/classview.png" target="_blank">
            <img src="/images/screenshots/thumbnails/classview.png" alt="" />
          </a>
        </div>
        <div class="image_caption">
          MonoDevelop source code with the class viewer
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
      
<ccms:PageFooter runat="server"/>
