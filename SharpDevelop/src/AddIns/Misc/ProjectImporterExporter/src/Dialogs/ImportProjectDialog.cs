// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs
{
	public class ImportProjectDialog : BaseSharpDevelopForm
	{
		ArrayList inputConvertes;
		
		public ImportProjectDialog()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("ImportProjectDialog.xfrm"));
			Icon = null;
			
			ControlDictionary["startButton"].Click                += new EventHandler(StartConversion);
			ControlDictionary["projectBrowseButton"].Click        += new EventHandler(BrowseProject);
			ControlDictionary["outputLocationBrowseButton"].Click += new EventHandler(BrowseOutputLocation);
			
			inputConvertes = RetrieveInputConverters();
			
			foreach (AbstractInputConverter converter in inputConvertes) {
				((RichTextBox)ControlDictionary["formatsRichTextBox"]).AppendText(StringParserService.Parse(converter.FormatName) + Environment.NewLine);
			}
		}
		
		ArrayList RetrieveInputConverters()
		{
			ArrayList converters = new ArrayList();
			Assembly asm = Assembly.GetCallingAssembly();
			foreach (Type t in asm.GetTypes()) {
				if (!t.IsAbstract && t.IsSubclassOf(typeof(AbstractInputConverter))) {
					converters.Add(asm.CreateInstance(t.FullName));
				}
			}
			return converters;
		}
		
		void BrowseProject(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = StringParserService.Parse("${res:SharpDevelop.FileFilter.VSNetSolutionFiles}|*.sln|${res:SharpDevelop.FileFilter.BorlandStudioFiles}|*.bdsproj;*.bdsgroup|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["projectTextBox"].Text = fdiag.FileName;
					
					string projectDefaultPath = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SharpDevelop Projects"));
					ControlDictionary["outputLocationTextBox"].Text = Path.Combine(projectDefaultPath, Path.GetFileNameWithoutExtension(fdiag.FileName));
				}
			}
		}
		
		void BrowseOutputLocation(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("Choose combine output location.") == DialogResult.OK) {
				ControlDictionary["outputLocationTextBox"].Text = fd.Path;
			}
		}
		
		void StartConversion(object sender, EventArgs e)
		{
			string inputFile  = ControlDictionary["projectTextBox"].Text;
			string outputPath = ControlDictionary["outputLocationTextBox"].Text;
			
			if (!FileUtilityService.IsValidFileName(inputFile)) {
				MessageService.ShowError("Input file is invalid");
				return;
			}
			
			if (!FileUtilityService.IsValidFileName(outputPath)) {
				MessageService.ShowError("Output path is invalid");
				return;
			}
			
			try {
				if (!Directory.Exists(outputPath)) {
					Directory.CreateDirectory(outputPath);
				}
			} catch (Exception) {}
			
			if (!FileUtilityService.IsDirectory(outputPath)) {
				MessageService.ShowError("Output path is no existing directory.");
				return;
			}
			
			if (FileUtilityService.TestFileExists(inputFile)) {
				foreach (AbstractInputConverter converter in inputConvertes) {
					if (converter.CanConvert(inputFile)) {
						if (converter.Convert(inputFile, outputPath)) {
							MessageService.ShowMessage("Conversion done.");
							DialogResult = DialogResult.OK;
						}
						return;
					}
				}
				MessageService.ShowError("No converter for input file found.");
			}
		}
	}
}
