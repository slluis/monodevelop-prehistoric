// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using Gtk;

namespace MonoDevelop.Gui.Dialogs
{
	public class AssemblyReferencePanel : VButtonBox, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		
		public AssemblyReferencePanel(SelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			// FIXME: il8n this
			Gtk.Button browseButton = new Gtk.Button(GettextCatalog.GetString ("Browse"));
			browseButton.Clicked += new EventHandler(SelectReferenceDialog);
			PackStart(browseButton, false, false, 6);
			ShowAll();
		}
		
		void SelectReferenceDialog(object sender, EventArgs e)
		{
			// FIXME: il8n the dialog name
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			using (FileSelection fdiag = new FileSelection(GettextCatalog.GetString ("Find .Net Assembly"))) {
			// FIXME: this should only allow dll's and exe's
			// fdiag.Complete("*");
				fdiag.SelectMultiple = true;
				int response = fdiag.Run();
				string[] selectedFiles = new string[fdiag.Selections.Length];
				fdiag.Selections.CopyTo(selectedFiles, 0);
				fdiag.Hide ();
			
				if (response == (int) ResponseType.Ok) {
					foreach (string file in selectedFiles) {
						bool isAssembly = true;
						try	{
							System.Reflection.AssemblyName.GetAssemblyName(System.IO.Path.GetFullPath (file));
						} catch (Exception assemblyExcep) {
							isAssembly = false;
						}
					
						if (isAssembly) {
						selectDialog.AddReference(ReferenceType.Assembly,
							System.IO.Path.GetFileName(file),
							file);
						} else {
							IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
							// FIXME: il8n this
							messageService.ShowError(String.Format (GettextCatalog.GetString ("File {0} is not a valid .Net Assembly"), file));
						}
					}
				}
			}
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			//System.Console.WriteLine("This panel will contain a file browser, but so long use the browse button :)");
		}
	}
}
