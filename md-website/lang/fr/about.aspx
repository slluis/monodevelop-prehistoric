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
					<A href="/download.aspx">T�l�charger</A>
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
				objectifs que MonoDevelop esp�re atteindre. Certains d�entre eux sont :
			</p>
			<ul>
				<li>
				Cr�er le meilleur environnement de d�veloppement de son esp�ce pour les 
				syst�mes Unix pour C# et Mono.
				<li>
				Puisqu�il est �crit en Gtk#, que nous aimons Gtk# et que nous obtenons un bon 
				support de Gtk#, il ajoutera tr�s probablement des fonctionnalit�s destin�es � 
				am�liorer l�aventure Gtk#.
				<li>
				Pour d�river aussi peu que possible de SharpDevelop : nous aimerions id�alement 
				re-fusionner le code (au travers de ifdefs, compilations conditionnelles, 
				interfaces, etc.) pour maximiser les contributions et la vitesse de 
				d�veloppement.
				<li>
				Aujourd�hui l�IDE est un simple IDE et sur Unix, il ne permet pas le design GUI 
				(qui est limit� � SharpDevelop), mais nous souhaitons ajouter un tel designer 
				dans le futur.
				<li>
					Nous voulons int�grer les outils que nous avons construits jusqu�ici. Des 
					choses comme MonoDoc, NUnit-Gtk et le debugger devraient donc prendre 
					MonoDevelop pour cible.</li>
			</ul>
			<p>Pour consulter la liste de certaines des fonctionnalit�s actuelles, visitez la <a href="/index.aspx">
					page des fonctionnalit�s</a>.</p>
			<div class="headlinebar">Information de License</div>
			<p>
				MonoDevelop est d�velopp� dans le cadre d�une licence
				<acronym title="General Public License">
					GPL</acronym>
				pouvant �tre consult�e � l�adresse <a href="http://www.gnu.org/copyleft/gpl.html">www.gnu.org/copyleft/gpl.html</a>. 
				Tout le code source est disponible sur le d�p�t Subversion. Pour savoir comment 
				t�l�charger les sources, visitez la <a href="/download.aspx">page de 
					t�l�chargements</a>.
			</p>
		</div>
	</body>
</HTML>
