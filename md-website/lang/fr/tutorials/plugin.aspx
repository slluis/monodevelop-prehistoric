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
			<div class="title">Ecrire un plugin pour MonoDevelop</div>
			<div class="headlinebar">Introduction</div>
			<p>MonoDevelop (et SharpDevelop) ont �t� �crits de telle mani�re qu'ils peuvent 
				ais�ment �tre �tendu par d'autres. Cela peut �tre r�alis� en suivant deux 
				�tapes simples. Premi�rement, en cr�ant un assemblage (dll) contenant le code 
				de votre "addin". Deuxi�mement, en fournissant un fichier XML (.addin) qui 
				�tablit le plan de votre code dans MonoDevelop. Vous pouvez trouver un "pdf" 
				d�taill� sur le site de SharpDevelop <a href="http://www.icsharpcode.net/TechNotes/ProgramArchitecture.pdf">
					ici</a>. La lecture de ce document vous permettra une compr�hension 
				compl�te de l'enti�ret� du syst�me et de ses possibilit�s. Nous nous arr�terons 
				ici � une simple et rapide vue d'ensemble.
			</p>
			<div class="headlinebar">Termes</div>
			<p>AddIn - ce que d'autres syst�mes appellent un plugin, �galement utilis� pour le 
				noyau de l'application.
				<br>
				Pad - fen�tre&nbsp;ancrable destin�e � se trouver dans les zones secondaires 
				comme l'explorateur de projet ou la zone de sortie par exemple.
				<br>
				View - fen�tre ancrable&nbsp;destin�e � se trouver dans la zone principale de 
				l'environnement comme par exemple l'�diteur de code.<br>
			</p>
			<div class="headlinebar">Assemblage Addin</div>
			<p>
				Dans votre code, vous pouvez �tendre l'IDE � bien des �gards. Parmi les choses 
				les plus courantes, on pourrait �tendre les menus, les "pads", les "views", les 
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
			<p>Voici une liste des classes � �tendre pour un AddIn:</p>
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
				Le fichier AddIn d�finit simplement les "points d'entr�e" de votre code dans 
				les diff�rentes parties de l'IDE. Vous y sp�cifierez les services � charger, 
				ajoutez des menus � un certain endroit et pratiquement n'importe quoi d'autre. 
				Puisque l'enti�ret� de l'application est un addin, il n'y a pas de limite. Les 
				directives conditionnelles sont support�es ainsi que d'autres constructions 
				avanc�es. Dans l'exemple MonoDevelopNunit.addin qui suit, vous pouvez constater 
				que l'on sp�cifie le nom de l'assemblage � charger, un service � charger dans 
				/Workspace/Services, deux views et quelques menus. Enfin, il est important de 
				noter l'attribut de classe qui est utilis� pour sp�cifier le type � instancier 
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
				Il existe un fichier AddIn.xsd sp�cifiant le format XML requis/optionnel. 
				Peut-�tre quelqu'un aimerait-il en faire un RelaxNG �galement. Voir 
				data/resources/AddIn.xsd
			</p>
			<div class="headlinebar">Construction et installation</div>
			<p>
				Nous permettons actuellement l'ex�cution directement dans /build ainsi qu'en 
				installant dans $(prefix)/lib/monodevelop
			</p>
			<div class="headlinebar">Exemples existants</div>
			<ul>
				<li>
				L'�diteur de code
				<li>
				CSharpBinding
				<li>
				DebuggerAddin
				<li>
				Monodoc
				<li>
				Page d'accueil (pas compl�tement port�e)
				<li>
					NUnit (incomplet)</li>
			</ul>
			<div class="headlinebar">Avertissement</div>
			<p>
				Bien que SharpDevelop et MonoDevelop utilisent le m�me format, il est possible 
				que ce ne soit pas toujours le cas. Notez �galement qu'alors que les addins non 
				GUI peuvent probablement �tre r�utilis�s, MonoDevelop et SharpDevelop utilisent 
				des bo�tes � outisl GUI diff�rentes pouvant emp�cher le partage de bien des 
				choses.
			</p>
			<div class="headlinebar">Les id�es des AddIns</div>
			<p>
				Il y a un grand nombre de choses qu'il serait bien d'avoir sous la forme de 
				addins. Voici bri�vement une liste non exhaustive.
			</p>
			<ul>
				<li>
				Un Viewer pour mono profiler (mono --profile) et d'autres outils de mono.
				<li>
				Support de langages et compilateurs suppl�mentaires.
				<li>
				Int�gration des outils NUnit et NAnt.
				<li>
				Glade (bien qu'un nouveau designer GUI soit pr�vu).
				<li>
				Int�gration de Subversion, CVS et d'autres outils de contr�le de version.
				<li>
				Outils UML/CASE.
				<li>
				Support de SQL/Bases de donn�es.
				<li>
				Un �diteur XML avanc�.
				<li>
					Ainsi que des choses d�j� disponibles dans SharpDevelop qui pourraient �tre 
					port�es dans MonoDevelop.</li>
			</ul>
			<div class="headlinebar">Credits and Errata</div>
			<p>Send comments to <a href="mailto:jluke@cfl.rr.com">jluke@cfl.rr.com</a> or the <a href="mailto:monodevelop-list@lists.ximian.com">
					monodevelop mailing list</a>. (en anglais uniquement)</p>
			<p>Last updated March 24, 2004</p>
		</div>
	</body>
</HTML>
