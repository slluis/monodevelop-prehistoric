<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<DIV class="title">Accueil</DIV>
			<P>MonoDevelop est actuellement au début de son développement mais il progresse 
				rapidement. La page des <a href="/lang/fr/news.aspx">news</a> et la <a href="/lang/fr/lists.aspx">mailing 
					list</a> contiendront les mises à jour majeures.
			</P>
			<P>Parmi les nombreuses fonctionnalités de MonoDevelop, on peut citer :
			</P>
			<DIV class="feature"><B>Gestion des classes </B>
				<BR>
				<IMG alt="" src="/images/screenshots/fades/classview.png" align="right"> MonoDevelop 
				dispose d’un explorateur de classes permettant de parcourir les classes de 
				votre projet ainsi que leurs méthodes et propriétés. Les classes sont triées en 
				fonction de leur espace de noms. Lorsque vous ajoutez un élément à votre 
				projet, l’explorateur de classes est automatiquement mis à jour, même si cet 
				élément est un espace de noms, une classe, une méthode voire même une variable.
			</DIV>
			<DIV class="feature"><B>Aide intégrée</B><BR>
				La documentation de .NET et celle de Gtk# sont intégrées à MonoDevelop 
				permettant un accès aisé.
			</DIV> 

			<DIV class="feature"><B>Complétion de code</B><BR>
				<IMG alt="" src="/images/screenshots/fades/codecomplete.png" align="right"> Avec 
				les frameworks de .NET et de Gtk# mis ensemble, il est possible de mémoriser 
				toutes les classes, méthodes et propriétés mises à votre disposition. La 
				complétion de code intelligente de MonoDevelop tente de compléter ce que vous 
				êtes en train de taper. Si l’une des propositions vous convient, pressez la 
				touche de tabulation et MonoDevelop complétera votre frappe.
			</DIV>
			<DIV class="feature"><B>Support de projets</B><BR>
				MonoDevelop est fourni avec des projets prédéfinis vous aidant à démarrer une 
				nouvelle application console, Gnome# ou Gtk#.
			</DIV>
			<DIV class="feature"><B>Débugger intégré</B><BR>
				MonoDevelop intègre Mono Debugger qui fournit une interface graphique au 
				débugger.
			</DIV>

<ccms:PageFooter runat="server"/>