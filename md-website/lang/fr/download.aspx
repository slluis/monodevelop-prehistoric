<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<div class="title">T&eacute;l&eacute;charger MonoDevelop</div>
			<p>
				Deux possibilit&eacute;s de t&eacute;l&eacute;chargement et d&#39;installation vous sont offertes. Si 
				vous d&eacute;sirez essayer le dernier release, vous pouvez t&eacute;l&eacute;charger ce <a href="#package">
					paquetage</a>. D&#39;autre part, si vous souhaitez collaborer &agrave; MonoDevelop, 
				vous pouvez utiliser la version <a href="#svn">Subversion</a>.
			</p>
			<p>
				Quelle que soit la m&eacute;thode d&#39;installation choisie, il existe certains 
				pr&eacute;-requis que voici :
			</p>
			<ul>
				<li>
					Mono 0.30 avec le support ICU support activ&eacute;</li>
				<li>
					gtkmozembed (inclus dans les sources de MonoDevelop)</li>
				<li>
					Gtk# CVS</li>
				<li>
					ORBit2-2.8.3 ou plus r&eacute;cent</li>
				<li>
					GtkSourceView#</li>
			</ul>
			<div class="headlinebar"><a name="package">Paquetages</a></div>
			<ol>
				<li>
					T&eacute;l&eacute;chargez le dernier <a href="/lang/fr/release.aspx">release de MonoDevelop</a></li>
				<li>
					Si vous avez t&eacute;l&eacute;charg&eacute; un Tarball, ouvrez une console et allez dans le dossier 
					le contenant. Ex&eacute;cutez la commande <tt>tar -xvzf MonoDevelop-x.xx.tar.gz</tt>, 
					x.xx repr&eacute;sente le num&eacute;ro de version. Une fois le paquetage d&eacute;compress&eacute;, entrez 
					dans le r&eacute;pertoire.</li>
				<li>
					Dans le r&eacute;pertoire source principal de MonoDevelop, ex&eacute;cutez la commande <tt>./configure 
						–prefix=/usr/local</tt></li>
				<li>
					Ex&eacute;cutez ensuite la commande <tt>make</tt></li>
				<li>
					Et enfin, pour ex&eacute;cuter MonoDevelop, tapez <tt>make run</tt></li>
			</ol>
			<div class="headlinebar"><a name="svn">D&eacute;veloppement en cours</a></div>
			<p>Si vous souhaitez des instructions pas &agrave; pas concernant la construction d&#39;un 
				snapshot de MonoDevelop, veuillez lire le <a href="/lang/fr/tutorial.aspx">didacticiel 
					"Hello World"</a>.
			</p>
			<ol>
				<li>
					Malheureusement, le serveur SVN anonyme est hors service, toutefois, nous 
					publions r&eacute;guli&egrave;rement des <a href="http://devservices.go-mono.com/MonoDevelop/">snapshots</a>.</li>
				<li>
					Apr&egrave;s avoir t&eacute;l&eacute;charg&eacute; le snapshot le plus r&eacute;cent, vous aurez besoin des 
					versions CVS de gtk-sharp et gtksourceview-sharp.</li>
				<li>
					Ex&eacute;cutez <tt>./autogen.sh</tt> et <tt>make</tt>.</li>
				<li>
					Actuellement, <tt>make install</tt> est support&eacute;, mais peut ne pas aboutir. <tt>make 
						run</tt> &agrave; l&#39;int&eacute;rieur du r&eacute;pertoire MonoDevelop est pour l&#39;instant la 
					mani&egrave;re recommand&eacute;e pour ex&eacute;cuter MonoDevelop.</li>
			</ol>

<ccms:PageFooter runat="server"/>
