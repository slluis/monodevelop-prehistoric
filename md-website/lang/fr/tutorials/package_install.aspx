<%@ Page language="c#" Codebehind="package_install.aspx.cs" AutoEventWireup="false" Inherits="MonoDevelop.tutorials.package_install" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3c.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>Installer un paquetage</title>
		<link rel="stylesheet" media="screen" href="/styles.css" type="text/css"></link>
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
					<A href="/download.aspx">Télécharger</A>
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
			<div class="title">Didacticiel d'installation</div>
			<p>Ce didacticiel est votre guide pour l'installation de MonoDevelop à l'aide d'un 
				paquetage officiel. Si vous voulez savoir comment construire MonoDevelop à 
				partir d'un snapshot, suivez les instructions du <a href="/tutorials/snapshot_install.aspx">
					didacticiel d'installation à partir d'un snapshot</a>.
			</p>
			<p>
				Sauf indication contraire, construisez et installez tout tarball de cette 
				manière:
			</p>
			<pre class="code">tar -xzf mypackage.tar.gz 
cd mypackage
./configure --prefix=/usr
make
make install
			</pre>
			<br>
			<br>
			<div class="headlinebar">Mise en route: Préliminaires</div>
			<p>Certains paquetages sont requis pour l'installation de MonoDevelop. Les 
				instructions suivantes vont vous aider à les installer:
			</p>
			<b>1. ORBit2 2.8.3</b>
			<p>Bien que des versions plus récentes <i>peuvent</i> fonctionner, elles sont 
				connues pour avoir provoqué des crashes lorsque MonoDevelop existe. En cas de 
				doute, téléchargez <a href="http://ftp.gnome.org/pub/GNOME/sources/ORBit2/2.8/ORBit2-2.8.3.tar.gz">
					ORBit 2.8.3</a>.</p>
			<br>
			<br>
			<b>2. GtkSourceView 0.7+</b>
			<p>Vous pouvez télécharger un paquetage binaire pour votre distribution si elle 
				fournit la version 0.7 ou ultérieure. Si tel n'est pas le cas, téléchargez le <a href="http://ftp.acc.umu.se/pub/gnome/sources/gtksourceview/0.7/gtksourceview-0.7.0.tar.gz">
					tarball officiel (0.7)</a>. Le script `<tt>./configure</tt>' est un peu 
				plus complexe qu'à l'accoutumé. Aidez-vous de l'exemple suivant :
			</p>
			<pre class="code">
tar -xzf gtksourceview-0.7.0.tar.gz
cd gtksourceview-0.7.0
./configure --prefix=`pkg-config --variable=prefix ORBit-2.0`
make
make install
</pre>
			<br>
			<br>
			<b>3. gtkmozembed</b>
			<p><a href="http://www.mozilla.org/unix/gtk-embedding.html">gtkmozembed</a> peut 
				généralement être trouvé dans le paquetage de développement Mozilla de votre 
				OS. Par exemple:
			</p>
			<ul>
				<li>
				Debian: `mozilla-dev'
				<li>
				RedHat: `mozilla-devel'
				<li>
					FreeBSD: `mozilla-gtkmozembed'</li>
			</ul>
			Je n'ai pas pu trouver de tarball officiel pour gtkmozembed.
			<br>
			<br>
			<b>4. Installer Mono 0.31</b><br>
			<p>Pour faire fonctionner MonoDevelop, les paquetages mono suivants doivent être 
				installés dans cet ordre:
			</p>
			<ul>
				<li>
					<a href="ftp://www-126.ibm.com/pub/icu/2.8/icu-2.8.tgz">International Components 
						for Unicode 2.8</a>
				<li>
					<a href="http://www.go-mono.com/archive/mono-0.31.tar.gz">mono with ICU</a>
				<li>
					<a href="http://sourceforge.net/project/showfiles.php?group_id=40240">gtk-sharp</a>
				<li>
					<a href="http://www.go-mono.com/archive/monodoc-0.13.tar.gz">monodoc</a>
				<li>
					<a href="http://www.go-mono.com/archive/gtksourceview-sharp-0.1.0.tar.gz">gtksourceview-sharp</a>
				<li>
					<a href="http://www.go-mono.com/archive/gecko-sharp-0.1.tar.gz">gecko-sharp</a>
				<li>
					<a href="http://www.go-mono.com/archive/mono-debugger-0.6.tar.gz">debugger</a></li>
			</ul>
			<p>Lors de la construction à partir des sources ci-dessus, toujours utiliser le 
				préfixe `<tt>/usr</tt>'.</p>
			<p>Certains paquetages sont aussi disponibles sous forme de paquetages binaires 
				(RPMs and DEBs). Des paquetages binaires peuvent être trouvés sur la&nbsp; <a href="http://www.go-mono.com/download.html">
					page des téléchargements de mono</a> pour RedHat, Fedora, Suse, et Debian. 
				Ils sont également disponibles au travers de Ximian <a href="http://www.ximian.com/products/redcarpet/">
					Red Carpet</a>, dans la chaîne 'mono'. Il n'existe actuellement des 
				paquetages binaires que pour:
			</p>
			<ul>
				<li>
				mono
				<li>
					ICU (`icu' and `libicu26')</li>
			</ul>
			D'autres paquetages sont en cours de développement et seront disponibles 
			rapidement.
			<br>
			<br>
			<br>
			<div class="headlinebar">Installer MonoDevelop</div>
			<p>La dernière étape du processus est de construire MonoDevelop lui-même. 
				Téléchargez le paquetage <a href="http://www.go-mono.com/archive/monodevelop-0.2.tar.gz">
					MonoDevelop 0.2</a>.</p>
			<pre class="code">
export PKG_CONFIG_PATH="/usr/lib/pkgconfig"
tar -xjf monodevelop-0.2.tar.gz
cd monodevelop-0.2
./configure --prefix=/usr
make
make install
</pre>
			<br>
			<p>Félicitations ! Vous avez maintenant installé la dernière copie de MonoDevelop. 
				N'oubliez pas de signaler tous les bugs rencontrés.</p>
			<br>
			<br>
			<hr width="90%">
			<p>This document was written by <a href="mailto:steve@citygroup.ca">Steve Deobald</a>
				and is licensed under Creative Commons, Share-Alike, Attribution. If this 
				document contains errors or could be improved, please let me know.</p>
		</div>
	</body>
</HTML>
