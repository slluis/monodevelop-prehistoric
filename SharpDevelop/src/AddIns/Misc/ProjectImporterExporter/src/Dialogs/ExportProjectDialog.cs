// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs
{
	public class ExportProjectDialog : BaseSharpDevelopForm
	{
		ArrayList outputConvertes;
		
		public ExportProjectDialog()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("ExportProjectDialog.xfrm"));
			Icon = null;
			ControlDictionary["outputLocationBrowseButton"].Click += new EventHandler(BrowseOutputLocation);
			ControlDictionary["startButton"].Click += new EventHandler(StartConversion);
			
			outputConvertes = RetrieveOutputConverters();
			FillOutputFormat();
			FillProjectList();
			((RadioButton)ControlDictionary["singleProjectRadioButton"]).CheckedChanged += new EventHandler(RadioButtonChecked);
			((RadioButton)ControlDictionary["wholeCombineRadioButton"]).CheckedChanged += new EventHandler(RadioButtonChecked);
			
			RadioButtonChecked(null, null);
		}
		
		void FillOutputFormat()
		{
			foreach (AbstractOutputConverter outputConverter in outputConvertes) {
				((ComboBox)ControlDictionary["outputFormatComboBox"]).Items.Add(StringParserService.Parse(outputConverter.FormatName));
			}
			((ComboBox)ControlDictionary["outputFormatComboBox"]).SelectedIndex = 0;
		}
		
		void FillProjectList()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			if (projectService.CurrentOpenCombine == null) {
				return;
			}
			
			ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
			foreach (ProjectCombineEntry entry in allProjects) {
				((ComboBox)ControlDictionary["projectListComboBox"]).Items.Add(entry.Project.Name);
			}
			if (allProjects.Count > 0) {
				((ComboBox)ControlDictionary["projectListComboBox"]).SelectedIndex = 0;
			}
		}
		
		void RadioButtonChecked(object sender, EventArgs e)
		{
			SetEnabledStatus(((RadioButton)ControlDictionary["singleProjectRadioButton"]).Checked, "projectListComboBox");
		}
		
		
		ArrayList RetrieveOutputConverters()
		{
			ArrayList converters = new ArrayList();
			Assembly asm = Assembly.GetCallingAssembly();
			foreach (Type t in asm.GetTypes()) {
				if (!t.IsAbstract && t.IsSubclassOf(typeof(AbstractOutputConverter))) {
					converters.Add(asm.CreateInstance(t.FullName));
				}
			}
			return converters;
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
			string outputPath = ControlDictionary["outputLocationTextBox"].Text;
			
			if (!FileUtilityService.IsValidFileName(outputPath)) {
				MessageService.ShowError("Output path is invalid");
				return;
			}
			
			if (!FileUtilityService.IsDirectory(outputPath)) {
				MessageService.ShowError("Output path is no existing directory.");
				return;
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			AbstractOutputConverter outputConverter = (AbstractOutputConverter)outputConvertes[((ComboBox)ControlDictionary["outputFormatComboBox"]).SelectedIndex];
			if (((RadioButton)ControlDictionary["singleProjectRadioButton"]).Checked) {
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				IProject  project = ((ProjectCombineEntry)allProjects[((ComboBox)ControlDictionary["projectListComboBox"]).SelectedIndex]).Project;
				outputConverter.ConvertProject(projectService.GetFileName(project), outputPath);
			} else {
				outputConverter.ConvertCombine(projectService.GetFileName(projectService.CurrentOpenCombine), outputPath);
			}
		}
	}
}
