<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<div class="title">FAQ &amp; Aide G&eacute;n&eacute;rale.</div>
			<p>Derni&egrave;re mise &agrave; jour le 1er avril 2004.</p>
			<div class="headlinebar">FAQ</div>
			<p>Comment ajouter les types mime dans Gnome 2.6?</p>
			<p>Premi&egrave;rement, copiez monodevelop.xml dans le dossier 
				$(gnome_prefix)/share/mime/packages. Ensuite, ex&eacute;cutez 
				update-mime-database $(gnome_prefix)/share/mime. Sur Fedora ainsi que sur de 
				nombreuses autres distributions, $gnome_prefix est /usr, proc&eacute;dez donc 
				comme suit:</p>
			<pre class="code">cp monodevelop.xml /usr/share/mime/packages
update-mime-database /usr/share/mime
			</pre>
			<p>Il vous faudra probablement ex&eacute;cuter ces op&eacute;rations en tant que 
				root. Si pour une raison quelconque, ces instructions ne pouvaient 
				r&eacute;soudre le probl&egrave;me, vous pouvez essayer ce qui suit:
			</p>
			<pre class="code">find /usr/share/mime -type f -exec chmod 644 {} \;</pre>
			<p>Avec un utilisateur disposant des permissions suffisantes.</p>
			<br />
			<p>O&ugrave; puis-je me procurer gecko-sharp.pc?</p>
			<p>gecko-sharp.pc peut &ecirc;tre trouv&eacute; dans le module CVS 
				gtkmozembed-sharp sur le d&eacute;p&ocirc;t CVS de mono.
			</p>
			<br />
			<p>Pourquoi la colorisation syntaxique ne fonctionne-t-elle pas sur mes fichiers 
				C#?</p>
			<p>GNOME ne reconnaît pas les fichiers *.cs comme des fichiers de type mime 
				text/x-csharp. gtksourceview-sharp essaye de configurer cela automatiquement 
				mais peut ne pas fonctionner dans tous les cas. Vous pouvez utiliser 
				gnome-file-types-properties pour corriger le probl&egrave;me. Si vous 
				ex&eacute;cutez Gnome 2.6, voir ci-dessus.</p>
			<br />
			<p>Que faire si le r&eacute;sum&eacute; de la configuration indique &#39;no&#39; pour l&#39;un 
				des pr&eacute; requis?</p>
			<p>Le script &#39;configure&#39; utilise pkg-config pour v&eacute;rifier si vous disposez 
				de tous les paquetages requis pour construire. S&#39;il ne peut d&eacute;tecter 
				l&#39;un des paquetages que vous avez install&eacute; :
			</p>
			<p>ajoutez le chemin vers le fichier &lt;paquetage&gt;.pc &agrave; PKG_CONFIG_PATH:</p>
			<pre class="code">export PKG_CONFIG_PATH=/usr/local/lib/pkgconfig:$PKG_CONFIG_PATH</pre>
			<p>installez une version plus r&eacute;cente ou le paquetage "development" du 
				paquetage et relancer le script ./configure.
			</p>
			<div class="headlinebar">Probl&egrave;mes connus</div>
			<p>Cette liste &eacute;num&egrave;re les probl&egrave;mes connus dans l&#39;espoir de 
				pr&eacute;venir la dupplication du rapport d&#39;un bug sur un m&ecirc;me release.
			</p>
			<ul>
				<li>
					Lorsqu&#39;on r&eacute;duit un &eacute;l&eacute;ment ancrable en ic&ocirc;ne, si l&#39;on ferme MonoDevelop 
					et le r&eacute;ouvre, l&#39;&eacute;l&eacute;ment a disparu.
				</li>
				<li>
					Lorsqu&#39;on agrandit MonoDevelop, les &eacute;l&eacute;ments ancrables ne se redimensionnent pas 
					correctement.
				</li>
				<li>
					La barre d&#39;outils montre parfois un comportement intriguant comme par exemple, 
					mais pas uniquement, sembler activ&eacute;e mais ne pas fonctionner, etc.
				</li>
				<li>
					Lorsqu&#39;on clique sur File-&gt;Recent Projects-&gt;clear recent project list ou 
					File-&gt;Recent Files-&gt;clear recent files list, il faudrait une boîte de 
					dialogue de confirmation afin d&#39;&eacute;viter de vider accidentellement l&#39;une 
					de ces listes.</li>
			</ul>
			<b>R&eacute;f&eacute;rences</b>
			<ul>
				<li>
					<a href="http://www.icsharpcode.net/TechNotes/
">SharpDevelop Tech Notes</a></li>
				<li>
					<a href="Gnome Human Interface Guidelines (HIG)
http://developer.gnome.org/projects/gup/hig/1.0/
		  
">freedesktop.org standards</a></li>
				<li>
					<a href="http://developers.sun.com/solaris/articles/integrating_gnome.html
">Integrating with GNOME (un peu obsol&egrave;te)</a></li>
			</ul>

<ccms:PageFooter runat="server"/>
