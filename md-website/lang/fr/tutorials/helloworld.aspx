<%@ Page language="c#" Codebehind="helloworld.aspx.cs" AutoEventWireup="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3c.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>Hello World en Gtk#</title>
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
			<div class="headlinebar">Ecrire "Hello World"</div>
			<p>Ce document est destiné à vous offrir un guide rapide vous permettant de vous 
				familiariser avec MonoDevelop en écrivant votre première application "Hello 
				World" en Gtk#.</p>
			<ul>
				<li>
					Exécutez MonoDevelop depuis son dossier SVN en tapant `<tt>make run &amp;</tt>'.
				<li>
					Créer un nouveau Combine (conteneur de projets) via le menu <tt>File -&gt; New 
						-&gt; Combine...</tt>. Vous devriez voir apparaître une boîte de dialogue 
					similaire à l'image qui suit; remplissez les zones de textes comme vous le 
					montre cette image et choisissez "GtkSharp Project".<br>
					<div align="center"><a href="/images/tutorial/tutorial1.png" target="_blank"> <img src="/images/tutorial/tutorial1sm.png"></a></div>
					<br>
				<li>
					Ouvrez `Main.cs' et `MyWindow.cs' à partir de l'explorateur de fichiers à 
					gauche. Si vous ne voyez pas la liste de fichiers appropriée, cliquez sur le 
					`.' dans l'explorateur de dossiers.<br>
					<div align="center"><a href="/images/tutorial/tutorial2.png" target="_blank"> <img src="/images/tutorial/tutorial2sm.png"></a></div>
					<br>
				<li>
					Modifiez `MyWindow.cs' pour ressembler à ce qui suit:<br>
					<pre class="code">
using System;
using Gtk;

public class MyWindow : Window {
	static GLib.GType gtype;
	Button button;
	
	public static new GLib.GType GType
	{
		get
		{
			if (gtype == GLib.GType.Invalid)
				gtype = RegisterGType (typeof (MyWindow));
			return gtype;
		}
	}
	
	public MyWindow () : base (GType)
	{
		button = new Button("Ceci est un bouton.");
		button.Clicked += new EventHandler(button_Clicked);
	
		this.Title = "MyWindow";
		this.SetDefaultSize (400, 300);
		this.DeleteEvent += new DeleteEventHandler (MyWindow_Delete);
		this.Add(button);
		this.ShowAll ();
	}
	
	void MyWindow_Delete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}
	
	 void button_Clicked (object obj, EventArgs args)
	{
		Console.WriteLine("Hello World");
		Application.Quit ();
	}
}
</pre>
				<li>
					Compilez le programme depuis MonoDevelop en utilisant l'icône "Build" (3ème 
					bouton en partant de la droite) dans la barre d'outils:
					<div align="center"><img src="/images/tutorial/tutorial3sm.png"></div>
					<br>
				<li>
					Exécutez le programme en cliquant sur le dernier bouton à droite. Le résultat 
					devrait ressembler à ceci:
					<div align="center"><img src="/images/tutorial/tutorial4sm.png"></div>
					<br>
				</li>
			</ul>
			<p>
				Félicitations! Vous venez de construire votre premier programme Gtk# en 
				utilisant la dernière copie de MonoDevelp. Signalez-nous tout bug rencontré en 
				réalisant ces étapes.</p>
			<hr>
			<p>This document was written by <a href="mailto:steve@citygroup.ca">Steve Deobald</a>
				and is licensed under Creative Commons, Share-Alike, Attribution. If this 
				document contains errors or could be improved, please let me know.
			</p>
			<br>
			<br>
		</div>
	</body>
</HTML>
