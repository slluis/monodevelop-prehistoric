<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

			<div class="title">Contribuer</div>
			<p>
				MonoDevelop est constamment � la recherche de nouveaux collaborateurs pour 
				aider au d�veloppement. Si vous disposez de temps et que vous souhaiter 
				contribuer, suivez les quelques instructions qui suivent.
			</p>
			<div class="headlinebar"><a target="bugs">Signaler un Bug</a></div>
			<p>
				Signaler un bug est un moyen tr�s simple de contribuer � un projet. Pour 
				signaler un bug, suivez les instructions suivantes :
			</p>
			<ol>
				<li>
					Assurez-vous que le bug signal� n'a pas d�j� �t� r�solu. Installez la derni�re 
					version en utilisant les instructions de t�l�chargement pour le "D�veloppement 
					en cours" sur la <a href="/lang/fr/download.aspx">page des t�l�chargements</a>.
				<li>
					Dans votre navigateur Internet, chargez <a href="http://bugzilla.ximian.com/" target="ximian-bugzilla">
						bugzilla.ximian.com</a>.
				<li>
					Examinez dans <a href="http://bugzilla.ximian.com/buglist.cgi?product=Mono+Develop&amp;bug_status=NEW&amp;bug_status=ASSIGNED&amp;bug_status=REOPENED&amp;email1=&amp;emailtype1=substring&amp;emailassigned_to1=1&amp;email2=&amp;emailtype2=substring&amp;emailreporter2=1&amp;changedin=&amp;chfieldfrom=&amp;chfieldto=Now&amp;chfieldvalue=&amp;short_desc=&amp;short_desc_type=substring&amp;long_desc=&amp;long_desc_type=substring&amp;bug_file_loc=&amp;bug_file_loc_type=substring&amp;keywords=&amp;keywords_type=anywords&amp;op_sys_details=&amp;op_sys_details_type=substring&amp;version_details=&amp;version_details_type=substring&amp;cmdtype=doit&amp;order=Reuse+same+sort+as+last+time&amp;form_name=query">
						"open MonoDevelop bugs"</a>
				et assurez-vous que votre bug n'a pas d�j� �t� soumis.
				<li>
					S'il tel n'est pas le cas, soumettez un <a href="http://bugzilla.ximian.com/enter_bug.cgi">
						nouveau bug MonoDevelop</a>.</li>
			</ol>
			<a target="patches">
				<div class="headlinebar">Soumettre un patch</div>
			</a>
			<p>
				Si vous soumettez un patch, il est toujours recommand� de le faire pour le 
				snapshot courant de MonoDevelop. Si vous ne le pouvez pas, assurez-vous que 
				vous disposez des sources pour le dernier release. Les archives des patches 
				soumis peuvent �tre consult�es <a href="http://lists.ximian.com/archives/public/monodevelop-patches-list/">
					ici</a>.
			</p>
			<b>Cr�er un patch:</b>
			<ol>
				<li>
				Faire une copie du fichier avant de le modifier.
				<li>
				Effectuez les changements d�sir�s et sauvegarder le fichier.
				<li>
					En ligne de commande, tapez : <tt>diff -u oldfile.cs newfile.cs &gt; vardec.patch</tt>. 
				Le nom du patch devrait �tre une br�ve description du probl�me que vous avez 
				r�gl�.
				<li>
					Cr�ez un bug sur <a href="http://bugzilla.ximian.com/">bugzilla.ximian.com</a> et 
					attachez le patch � ce bug. Si vous soumettez un patch pour un bug pr�existant, 
					attachez-le simplement au bug.</li>
			</ol>
			<div class="headlinebar">Travailler pour le site web:</div>
			<p>
				Nous sommes toujours � la recherche d'aide au d�veloppement du site web de 
				MonoDevelop. Si vous �tes int�ress�, veuillez prendre contact avec "mr proper 
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
				Toute collaboration apport�e sera appr�ci�e. La liste ci-dessus n'est pas 
				exhaustive.
			</p>

<ccms:PageFooter runat="server"/>
