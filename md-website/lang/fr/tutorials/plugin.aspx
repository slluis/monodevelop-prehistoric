<%@ Page  Codebehind="plugin.aspx.cs" AutoEventWireup="false"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3c.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>Ecrire un plugin pour MonoDevelop</title>
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
			<div class="title">Ecrire un plugin pour MonoDevelop</div>
			<div class="headlinebar">Introduction</div>
			<p>MonoDevelop (et SharpDevelop) ont été écrits de telle manière qu'ils peuvent 
				aisément être étendu par d'autres. Cela peut être réalisé en suivant deux 
				étapes simples. Premièrement, en créant un assemblage (dll) contenant le code 
				de votre "addin". Deuxièmement, en fournissant un fichier XML (.addin) qui 
				établit le plan de votre code dans MonoDevelop. Vous pouvez trouver un "pdf" 
				détaillé sur le site de SharpDevelop <a href="http://www.icsharpcode.net/TechNotes/ProgramArchitecture.pdf">
					ici</a>. La lecture de ce document vous permettra une compréhension 
				complète de l'entièreté du système et de ses possibilités. Nous nous arrêterons 
				ici à une simple et rapide vue d'ensemble.
			</p>
			<div class="headlinebar">Termes</div>
			<p>AddIn - ce que d'autres systèmes appellent un plugin, également utilisé pour le 
				noyau de l'application.
				<br>
				Pad - fenêtre&nbsp;ancrable destinée à se trouver dans les zones secondaires 
				comme l'explorateur de projet ou la zone de sortie par exemple.
				<br>
				View - fenêtre ancrable&nbsp;destinée à se trouver dans la zone principale de 
				l'environnement comme par exemple l'éditeur de code.<br>
			</p>
			<div class="headlinebar">Assemblage Addin</div>
			<p>
				Dans votre code, vous pouvez étendre l'IDE à bien des égards. Parmi les choses 
				les plus courantes, on pourrait étendre les menus, les "pads", les "views", les 
				services, les commandes, etc. Je vous recommande de regarder dans le dossier <tt>src/addin</tt>:
			</p>
			<pre class="code">		
using System;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services;
{
	public class ExampleService : AbstractService
	{
	}
}
</pre>
			<p>Voici une liste des classes à étendre pour un AddIn:</p>
			<pre class="code">
./src/Main/Base/Gui/Dialogs/AbstractOptionPanel.cs
./src/Main/Base/Gui/Dialogs/Wizard/AbstractWizardPanel.cs
./src/Main/Base/Gui/Pads/ClassScout/BrowserNode/AbstractClassScoutNode.cs
./src/Main/Base/Gui/Pads/ProjectBrowser/BrowserNode/AbstractBrowserNode.cs
./src/Main/Base/Gui/AbstractBaseViewContent.cs
./src/Main/Base/Gui/AbstractPadContent.cs
./src/Main/Base/Gui/AbstractViewContent.cs
./src/Main/Base/Gui/AbstractSecondaryViewContent.cs
</pre>
			<div class="headlinebar">Fichier .addin</div>
			<p>
				Le fichier AddIn définit simplement les "points d'entrée" de votre code dans 
				les différentes parties de l'IDE. Vous y spécifierez les services à charger, 
				ajoutez des menus à un certain endroit et pratiquement n'importe quoi d'autre. 
				Puisque l'entièreté de l'application est un addin, il n'y a pas de limite. Les 
				directives conditionnelles sont supportées ainsi que d'autres constructions 
				avancées. Dans l'exemple MonoDevelopNunit.addin qui suit, vous pouvez constater 
				que l'on spécifie le nom de l'assemblage à charger, un service à charger dans 
				/Workspace/Services, deux views et quelques menus. Enfin, il est important de 
				noter l'attribut de classe qui est utilisé pour spécifier le type à instancier 
				par cette partie de l'addin.
			</p>
			<pre class="code">
	<xmp>
<AddIn name      =" MonoDevelop Nunit"
       author    =" John Luke"
       copyright =" GPL"
       url       =" http://monodevelop.com"
       description =" NUnit testing tool"
       version   =" 0.2">
 
        <Runtime>
                <Import assembly="MonoDevelop.Nunit.dll"/>
        </Runtime>
 
        <Extension path="/Workspace/Services">
                <Class id =" NunitService"
                    class =" MonoDevelop.Services.NunitService"/>
        </Extension>
 
        <Extension path="/SharpDevelop/Workbench/Views">

                <Class id    =" NunitTestTree"
                       class =" MonoDevelop.Nunit.Gui.TestTree"/>
                <Class id    =" NunitResultTree"
                       class =" MonoDevelop.Nunit.Gui.ResultTree"/>
        </Extension>
 
        <Extension path="/SharpDevelop/Workbench/MainMenu/Tools">
                <MenuItem id =" NunitMenu" label =" NUnit" insertafter =" ExternalTools" insertbefore =" Options">
                        <MenuItem id =" LoadTestAssembly"
                          label =" Load Assembly"
                                  shortcut =" "
                              class =" MonoDevelop.Commands.NunitLoadAssembly" />
                        <MenuItem id =" NunitRunTests"
                          label =" Run Tests"
                                  shortcut =" "
                              class =" MonoDevelop.Commands.NunitRunTests" />
                </MenuItem>
        </Extension>

</AddIn>
	</xmp>

</pre>
			<div class="headlinebar">Format XML d'un AddIn</div>
			<p>
				Il existe un fichier AddIn.xsd spécifiant le format XML requis/optionnel. 
				Peut-être quelqu'un aimerait-il en faire un RelaxNG également. Voir 
				data/resources/AddIn.xsd
			</p>
			<div class="headlinebar">Construction et installation</div>
			<p>
				Nous permettons actuellement l'exécution directement dans /build ainsi qu'en 
				installant dans $(prefix)/lib/monodevelop
			</p>
			<div class="headlinebar">Exemples existants</div>
			<ul>
				<li>
				L'éditeur de code
				<li>
				CSharpBinding
				<li>
				DebuggerAddin
				<li>
				Monodoc
				<li>
				Page d'accueil (pas complètement portée)
				<li>
					NUnit (incomplet)</li>
			</ul>
			<div class="headlinebar">Avertissement</div>
			<p>
				Bien que SharpDevelop et MonoDevelop utilisent le même format, il est possible 
				que ce ne soit pas toujours le cas. Notez également qu'alors que les addins non 
				GUI peuvent probablement être réutilisés, MonoDevelop et SharpDevelop utilisent 
				des boîtes à outisl GUI différentes pouvant empêcher le partage de bien des 
				choses.
			</p>
			<div class="headlinebar">Les idées des AddIns</div>
			<p>
				Il y a un grand nombre de choses qu'il serait bien d'avoir sous la forme de 
				addins. Voici brièvement une liste non exhaustive.
			</p>
			<ul>
				<li>
				Un Viewer pour mono profiler (mono --profile) et d'autres outils de mono.
				<li>
				Support de langages et compilateurs supplémentaires.
				<li>
				Intégration des outils NUnit et NAnt.
				<li>
				Glade (bien qu'un nouveau designer GUI soit prévu).
				<li>
				Intégration de Subversion, CVS et d'autres outils de contrôle de version.
				<li>
				Outils UML/CASE.
				<li>
				Support de SQL/Bases de données.
				<li>
				Un éditeur XML avancé.
				<li>
					Ainsi que des choses déjà disponibles dans SharpDevelop qui pourraient être 
					portées dans MonoDevelop.</li>
			</ul>
			<div class="headlinebar">Credits and Errata</div>
			<p>Send comments to <a href="mailto:jluke@cfl.rr.com">jluke@cfl.rr.com</a> or the <a href="mailto:monodevelop-list@lists.ximian.com">
					monodevelop mailing list</a>. (en anglais uniquement)</p>
			<p>Last updated March 24, 2004</p>
		</div>
	</body>
</HTML>
