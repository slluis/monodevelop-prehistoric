<%@ Page language="c#" Codebehind="about.aspx.cs" AutoEventWireup="false"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3c.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>A propos MonoDevelop</title>
		<link rel="stylesheet" media="screen" href="styles.css" type="text/css">
	</HEAD>
	<body>
		<div id="header"><IMG src="/images/mono-develop.png"></div>
		<DIV id="left-nav">
			<UL>
				<LI>
					<A href="/">Accueil</A>
				<LI>
					<A href="/news.aspx">News</A>
				<LI>
					<A href="/about.aspx">A propos</A>
				<LI>
					<A href="/screenshots.aspx">Screenshots</A>
				<LI>
					<A href="/download.aspx">T&eacute;l&eacute;charger</A>
				<LI>
					<A href="/contribute.aspx">Contribuer</A>
				<LI>
					<A href="/tutorial.aspx">Didacticiels</A>
				<LI>
					<A href="/faq.aspx">FAQ</A>
				</LI>
			</UL>
		</DIV>
		<div id="content">
			<div class="title">A propos</div>
			<div class="headlinebar">Description de MonoDevelop</div>
			<p>
				MonoDevelop est un projet de portage de SharpDevelop en Gtk#. Nombreux sont les 
				objectifs que MonoDevelop esp&egrave;re atteindre. Certains d&#39;entre eux sont :
			</p>
			<ul>
				<li>
				Cr&eacute;er le meilleur environnement de d&eacute;veloppement de son esp&egrave;ce pour les 
				syst&egrave;mes Unix pour C# et Mono.
				<li>
				Puisqu&#39;il est &eacute;crit en Gtk#, que nous aimons Gtk# et que nous obtenons un bon 
				support de Gtk#, il ajoutera tr&egrave;s probablement des fonctionnalit&eacute;s destin&eacute;es &agrave; 
				am&eacute;liorer l&#39;aventure Gtk#.
				<li>
				Pour d&eacute;river aussi peu que possible de SharpDevelop : nous aimerions id&eacute;alement 
				re-fusionner le code (au travers de ifdefs, compilations conditionnelles, 
				interfaces, etc.) pour maximiser les contributions et la vitesse de 
				d&eacute;veloppement.
				<li>
				Aujourd&#39;hui l&#39;IDE est un simple IDE et sur Unix, il ne permet pas le design GUI 
				(qui est limit&eacute; &agrave; SharpDevelop), mais nous souhaitons ajouter un tel designer 
				dans le futur.
				<li>
					Nous voulons int&eacute;grer les outils que nous avons construits jusqu&#39;ici. Des 
					choses comme MonoDoc, NUnit-Gtk et le debugger devraient donc prendre 
					MonoDevelop pour cible.</li>
			</ul>
			<p>Pour consulter la liste de certaines des fonctionnalit&eacute;s actuelles, visitez la <a href="/index.aspx">
					page des fonctionnalit&eacute;s</a>.</p>
			<div class="headlinebar">Information de License</div>
			<p>
				MonoDevelop est d&eacute;velopp&eacute; dans le cadre d&#39;une licence
				<acronym title="General Public License">
					GPL</acronym>
				pouvant &ecirc;tre consult&eacute;e &agrave; l&#39;adresse <a href="http://www.gnu.org/copyleft/gpl.html">www.gnu.org/copyleft/gpl.html</a>. 
				Tout le code source est disponible sur le d&eacute;p&ocirc;t Subversion. Pour savoir comment 
				t&eacute;l&eacute;charger les sources, visitez la <a href="/download.aspx">page de 
					t&eacute;l&eacute;chargements</a>.
			</p>
		</div>
	</body>
</HTML>
