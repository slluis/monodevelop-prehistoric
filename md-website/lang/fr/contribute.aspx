<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<div class="title">Contribuer</div>
			<p>
				MonoDevelop est constamment &agrave; la recherche de nouveaux collaborateurs pour 
				aider au d&eacute;veloppement. Si vous disposez de temps et que vous souhaitez 
				contribuer, suivez les quelques instructions qui suivent.
			</p>
			<div class="headlinebar"><a target="bugs">Signaler un Bug</a></div>
			<p>
				Signaler un bug est un moyen tr&egrave;s simple de contribuer &agrave; un projet. Pour 
				signaler un bug, suivez les instructions suivantes :
			</p>
			<ol>
				<li>
					Assurez-vous que le bug signal&eacute; n&#39;a pas d&eacute;j&agrave; &eacute;t&eacute; r&eacute;solu. Installez la derni&egrave;re 
					version en utilisant les instructions de t&eacute;l&eacute;chargement pour le "D&eacute;veloppement 
					en cours" sur la <a href="/lang/fr/download.aspx">page des t&eacute;l&eacute;chargements</a>.
				<li>
					Dans votre navigateur Internet, chargez <a href="http://bugzilla.ximian.com/" target="ximian-bugzilla">
						bugzilla.ximian.com</a>.
				<li>
					Examinez dans <a href="http://bugzilla.ximian.com/buglist.cgi?product=Mono+Develop&amp;bug_status=NEW&amp;bug_status=ASSIGNED&amp;bug_status=REOPENED&amp;email1=&amp;emailtype1=substring&amp;emailassigned_to1=1&amp;email2=&amp;emailtype2=substring&amp;emailreporter2=1&amp;changedin=&amp;chfieldfrom=&amp;chfieldto=Now&amp;chfieldvalue=&amp;short_desc=&amp;short_desc_type=substring&amp;long_desc=&amp;long_desc_type=substring&amp;bug_file_loc=&amp;bug_file_loc_type=substring&amp;keywords=&amp;keywords_type=anywords&amp;op_sys_details=&amp;op_sys_details_type=substring&amp;version_details=&amp;version_details_type=substring&amp;cmdtype=doit&amp;order=Reuse+same+sort+as+last+time&amp;form_name=query">
						"open MonoDevelop bugs"</a>
				et assurez-vous que votre bug n&#39;a pas d&eacute;j&agrave; &eacute;t&eacute; soumis.
				<li>
					S&#39;il tel n&#39;est pas le cas, soumettez un <a href="http://bugzilla.ximian.com/enter_bug.cgi">
						nouveau bug MonoDevelop</a>.</li>
			</ol>
			<a target="patches">
				<div class="headlinebar">Soumettre un patch</div>
			</a>
			<p>
				Si vous soumettez un patch, il est toujours recommand&eacute; de le faire pour le 
				snapshot courant de MonoDevelop. Si vous ne le pouvez pas, assurez-vous que 
				vous disposez des sources pour le dernier release. Les archives des patches 
				soumis peuvent &ecirc;tre consult&eacute;es <a href="http://lists.ximian.com/archives/public/monodevelop-patches-list/">
					ici</a>.
			</p>
			<b>Cr&eacute;er un patch:</b>
			<ol>
				<li>
				Faire une copie du fichier avant de le modifier.
				<li>
				Effectuez les changements d&eacute;sir&eacute;s et sauvegarder le fichier.
				<li>
					En ligne de commande, tapez : <tt>diff -u oldfile.cs newfile.cs &gt; vardec.patch</tt>. 
				Le nom du patch devrait &ecirc;tre une br&egrave;ve description du probl&egrave;me que vous avez 
				r&eacute;gl&eacute;.
				<li>
					Cr&eacute;ez un bug sur <a href="http://bugzilla.ximian.com/">bugzilla.ximian.com</a> et 
					attachez le patch &agrave; ce bug. Si vous soumettez un patch pour un bug pr&eacute;existant, 
					attachez-le simplement au bug.</li>
			</ol>
			<div class="headlinebar">Travailler pour le site web:</div>
			<p>
				Nous sommes toujours &agrave; la recherche d&#39;aide au d&eacute;veloppement du site web de 
				MonoDevelop. Si vous &ecirc;tes int&eacute;ress&eacute;, veuillez prendre contact avec "mr proper 
				at ximian point com". Nous recherchons actuellement :
			</p>
			<ul>
				<li>
				des didacticiels
				<li>
				des screenshots
				<li>
					des auteurs pour le contenu du site
				</li>
			</ul>
			<p>
				Toute collaboration apport&eacute;e sera appr&eacute;ci&eacute;e. La liste ci-dessus n&#39;est pas 
				exhaustive.
			</p>

<ccms:PageFooter runat="server"/>
