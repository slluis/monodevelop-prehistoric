<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<div class="title">Screenshots d'utilisateurs</div>
			<p>
				Cette page contient des screenshots envoy&eacute;s par des utilisateurs. Si un 
				screenshot montre une fonctionnalit&eacute; de MonoDevelop, il peut &ecirc;tre 
				ajout&eacute; &agrave; la <a href="screenshots.aspx">page officielle des 
					screenshots</a>. Si vous voulez voir votre screenshot sur cette page, <a href="mailto:steve@citygroup.ca">
					envoyez un message &agrave; steve</a>. Dans le futur, cette page devrait 
				&ecirc;tre remplac&eacute;e par un wiki aliment&eacute; par mono.
			</p>
			<br />
			<div class="image_frame">
				<div class="image">
					<a href="/images/screenshots/user-submitted001.png" target="_blank"><img src="/images/screenshots/thumbnails/user-submitted001.png" alt="" />
					</a>
				</div>
				<div class="image_caption">
					Debugging d'une application GTK#, documentation int&eacute;gr&eacute;e
				</div>
			</div>
			<br />
			<div class="image_frame">
				<div class="image">
					<a href="/images/screenshots/webbrowser.png" target="_blank"><img src="/images/screenshots/thumbnails/webbrowser.png" alt="" />
					</a>
				</div>
				<div class="image_caption">
					Navigation web à l'aide du composant Mozilla int&eacute;gr&eacute;
				</div>
			</div>
<ccms:PageFooter runat="server"/>
