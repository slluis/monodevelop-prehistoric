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
			<div class="title">Ecrire un plugin pour MonoDevelop</div>
			<div class="headlinebar">Introduction</div>
			<p>MonoDevelop (et SharpDevelop) ont &eacute;t&eacute; &eacute;crits de telle mani&egrave;re qu&#39;ils peuvent 
				ais&eacute;ment &ecirc;tre &eacute;tendu par d&#39;autres. Cela peut &ecirc;tre r&eacute;alis&eacute; en suivant deux 
				&eacute;tapes simples. Premi&egrave;rement, en cr&eacute;ant un assemblage (dll) contenant le code 
				de votre "addin". Deuxi&egrave;mement, en fournissant un fichier XML (.addin) qui 
				&eacute;tablit le plan de votre code dans MonoDevelop. Vous pouvez trouver un "pdf" 
				d&eacute;taill&eacute; sur le site de SharpDevelop <a href="http://www.icsharpcode.net/TechNotes/ProgramArchitecture.pdf">
					ici</a>. La lecture de ce document vous permettra une compr&eacute;hension 
				compl&egrave;te de l&#39;enti&egrave;ret&eacute; du syst&egrave;me et de ses possibilit&eacute;s. Nous nous arr&ecirc;terons 
				ici &agrave; une simple et rapide vue d&#39;ensemble.
			</p>
			<div class="headlinebar">Termes</div>
			<p>AddIn - ce que d&#39;autres syst&egrave;mes appellent un plugin, &eacute;galement utilis&eacute; pour le 
				noyau de l&#39;application.
				<br>
				Pad - fen&ecirc;tre&nbsp;ancrable destin&eacute;e &agrave; se trouver dans les zones secondaires 
				comme l&#39;explorateur de projet ou la zone de sortie par exemple.
				<br>
				View - fen&ecirc;tre ancrable&nbsp;destin&eacute;e &agrave; se trouver dans la zone principale de 
				l&#39;environnement comme par exemple l&#39;&eacute;diteur de code.<br>
			</p>
			<div class="headlinebar">Assemblage Addin</div>
			<p>
				Dans votre code, vous pouvez &eacute;tendre l&#39;IDE &agrave; bien des &eacute;gards. Parmi les choses 
				les plus courantes, on pourrait &eacute;tendre les menus, les "pads", les "views", les 
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
			<p>Voici une liste des classes &agrave; &eacute;tendre pour un AddIn:</p>
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
				Le fichier AddIn d&eacute;finit simplement les "points d&#39;entr&eacute;e" de votre code dans 
				les diff&eacute;rentes parties de l&#39;IDE. Vous y sp&eacute;cifierez les services &agrave; charger, 
				ajoutez des menus &agrave; un certain endroit et pratiquement n&#39;importe quoi d&#39;autre. 
				Puisque l&#39;enti&egrave;ret&eacute; de l&#39;application est un addin, il n&#39;y a pas de limite. Les 
				directives conditionnelles sont support&eacute;es ainsi que d&#39;autres constructions 
				avanc&eacute;es. Dans l&#39;exemple MonoDevelopNunit.addin qui suit, vous pouvez constater 
				que l&#39;on sp&eacute;cifie le nom de l&#39;assemblage &agrave; charger, un service &agrave; charger dans 
				/Workspace/Services, deux views et quelques menus. Enfin, il est important de 
				noter l&#39;attribut de classe qui est utilis&eacute; pour sp&eacute;cifier le type &agrave; instancier 
				par cette partie de l&#39;addin.
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
			<div class="headlinebar">Format XML d&#39;un AddIn</div>
			<p>
				Il existe un fichier AddIn.xsd sp&eacute;cifiant le format XML requis/optionnel. 
				Peut-&ecirc;tre quelqu&#39;un aimerait-il en faire un RelaxNG &eacute;galement. Voir 
				data/resources/AddIn.xsd
			</p>
			<div class="headlinebar">Construction et installation</div>
			<p>
				Nous permettons actuellement l&#39;ex&eacute;cution directement dans /build ainsi qu&#39;en 
				installant dans $(prefix)/lib/monodevelop
			</p>
			<div class="headlinebar">Exemples existants</div>
			<ul>
				<li>
				L&#39;&eacute;diteur de code
				<li>
				CSharpBinding
				<li>
				DebuggerAddin
				<li>
				Monodoc
				<li>
				Page d&#39;accueil (pas compl&egrave;tement port&eacute;e)
				<li>
					NUnit (incomplet)</li>
			</ul>
			<div class="headlinebar">Avertissement</div>
			<p>
				Bien que SharpDevelop et MonoDevelop utilisent le m&ecirc;me format, il est possible 
				que ce ne soit pas toujours le cas. Notez &eacute;galement qu&#39;alors que les addins non 
				GUI peuvent probablement &ecirc;tre r&eacute;utilis&eacute;s, MonoDevelop et SharpDevelop utilisent 
				des bo&icirc;tes &agrave; outisl GUI diff&eacute;rentes pouvant emp&ecirc;cher le partage de bien des 
				choses.
			</p>
			<div class="headlinebar">Les id&eacute;es des AddIns</div>
			<p>
				Il y a un grand nombre de choses qu&#39;il serait bien d&#39;avoir sous la forme de 
				addins. Voici bri&egrave;vement une liste non exhaustive.
			</p>
			<ul>
				<li>
				Un Viewer pour mono profiler (mono --profile) et d&#39;autres outils de mono.
				<li>
				Support de langages et compilateurs suppl&eacute;mentaires.
				<li>
				Int&eacute;gration des outils NUnit et NAnt.
				<li>
				Glade (bien qu&#39;un nouveau designer GUI soit pr&eacute;vu).
				<li>
				Int&eacute;gration de Subversion, CVS et d&#39;autres outils de contr&ocirc;le de version.
				<li>
				Outils UML/CASE.
				<li>
				Support de SQL/Bases de donn&eacute;es.
				<li>
				Un &eacute;diteur XML avanc&eacute;.
				<li>
					Ainsi que des choses d&eacute;j&agrave; disponibles dans SharpDevelop qui pourraient &ecirc;tre 
					port&eacute;es dans MonoDevelop.</li>
			</ul>
			<div class="headlinebar">Credits and Errata</div>
			<p>Send comments to <a href="mailto:jluke@cfl.rr.com">jluke@cfl.rr.com</a> or the <a href="mailto:monodevelop-list@lists.ximian.com">
					monodevelop mailing list</a>. (en anglais uniquement)</p>
			<p>Last updated March 24, 2004</p>
		</div>
	</body>
</HTML>
