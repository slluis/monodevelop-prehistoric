using System;
using Gtk;
using Gnome;

using MonoDevelop.Gui.Widgets;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard {
class MethodSelectionPage : DruidPageStandard {
	internal RadioButton generateDatabase;
	internal RadioButton useExisting;
	internal RadioButton download;
	
	internal MethodSelectionPage(CodeCompletionDruid druid) : base() {
		Title = GettextCatalog.GetString ("Generation Method");
		generateDatabase = new RadioButton(GettextCatalog.GetString ("Generate a code completion database"));
		useExisting = new RadioButton(generateDatabase, GettextCatalog.GetString ("Use a code completion database already on this computer"));
		download = new RadioButton(generateDatabase, GettextCatalog.GetString ("Download a code completion database"));
		this.NextClicked += new NextClickedHandler(druid.GoToMethodPage);
		AppendItem("", generateDatabase, "");
		AppendItem("", useExisting, "");
		AppendItem("", download, "");
	}
}


class GenerateDatabasePage : DetailsPageBase {
	internal RadioButton heavy;
	internal RadioButton light;
	
	internal GenerateDatabasePage(CodeCompletionDruid druid) : base(druid) {
		Title = GettextCatalog.GetString ("Select Generation Style");
		heavy = new RadioButton(GettextCatalog.GetString ("Heavy process"));
		light = new RadioButton(heavy, GettextCatalog.GetString ("Light process"));

		AppendItem("", heavy, GettextCatalog.GetString ("This process is slower and more memory-intensive than the light process, but will enable faster code completion"));
		AppendItem("", light, GettextCatalog.GetString ("This process will take less time and memory to produce the code completion database, but code completion will be slower"));
	}
}

class UseExistingPage : DetailsPageBase {
	internal MonoDevelop.Gui.Widgets.FolderEntry filename;
	
	internal UseExistingPage(CodeCompletionDruid druid) : base(druid) {
		Title = GettextCatalog.GetString ("Existing Database Location");
		filename = new FolderEntry(GettextCatalog.GetString ("Select code completion database"));
		filename.DefaultPath = System.IO.Directory.GetCurrentDirectory();
		AppendItem(GettextCatalog.GetString ("Where is the code completion database you would like to copy"), filename, "");
	}
}

class DownloadPage : DetailsPageBase {
	internal Gtk.Entry uri;
	
	internal DownloadPage(CodeCompletionDruid druid) : base(druid) 
	{
		Title = GettextCatalog.GetString ("Download Database");
		uri = new Gtk.Entry();
		AppendItem(GettextCatalog.GetString ("Where would you like to download the code completion database from?"), uri, "");
	}
	
	protected override string GetError (object sender)
	{
		try {
			Uri u = new Uri(this.uri.Text);
		} catch (UriFormatException ex) {
			return String.Format (GettextCatalog.GetString ("That Uri is invalid: {0}"), ex.Message);
		}

		int compressionType = (int)MonoDevelop.Gui.Utils.DirectoryArchive.Decompressor.GetTypeFromString(this.uri.Text, false);

		if (compressionType == -1){
			return GettextCatalog.GetString ("That Uri appears not to refer to a file with a known compression type");
		}
		
		return null;
	}
}

abstract class DetailsPageBase : DruidPageStandard {
	internal CodeCompletionDruid druid;

	internal DetailsPageBase(CodeCompletionDruid Druid)
	{
		this.druid = Druid;
		this.NextClicked += new NextClickedHandler(MoveNext);
		this.BackClicked += new BackClickedHandler(MoveBack);
	}

	internal void MoveNext(object sender, NextClickedArgs args)
	{
		string error = GetError(sender);
		
		if (error != null) {
			IMessageService messageService = (IMessageService) ServiceManager.Services.GetService (typeof (IMessageService));
			messageService.ShowError(error);
			args.RetVal = true;
			return;
		}

		druid.PreviousPage = this;
		druid.ShowLast();
		args.RetVal = true;
	}

	protected virtual string GetError(object sender)
	{
		return null;
	}

	internal void MoveBack(object sender, BackClickedArgs args)
	{
		druid.ShowMain();
		args.RetVal = true;
	}
}


public delegate void DruidFinished(object sender, IDatabaseGenerator generator);
public delegate void DruidCanceled(object sender);

class CodeCompletionDruid : Druid {
	internal DruidPageEdge startPage = GetStartPage();
	internal MethodSelectionPage methodSelectionPage;
	internal GenerateDatabasePage generateDatabasePage;
	internal UseExistingPage useExistingPage;
	internal DownloadPage downloadPage;
	internal DruidPageEdge endPage;

	internal DruidPage PreviousPage;

	public event DruidFinished Finished;
	public event DruidCanceled Canceled;
	public CodeCompletionDruid() : base()
	{
		methodSelectionPage = new MethodSelectionPage(this);
		generateDatabasePage = new GenerateDatabasePage(this);
		useExistingPage = new UseExistingPage(this);
		downloadPage = new DownloadPage(this);
		endPage = GetEndPage();

		this.Cancel += new EventHandler(HandleCancel);

		AppendPage(startPage);
		AppendPage(methodSelectionPage);
		AppendPage(generateDatabasePage);
		AppendPage(useExistingPage);
		AppendPage(downloadPage);
		AppendPage(endPage);
	}


	internal void BackToMethodSelection(object sender, BackClickedArgs args)
	{
		this.Page = methodSelectionPage;
	}

	internal static DruidPageEdge GetStartPage()
	{
		DruidPageEdge page = new DruidPageEdge(EdgePosition.Start);
		page.Text = GettextCatalog.GetString ("This druid will guide you through the process of creating a code completion database");
		return page;
	}

	internal DruidPageEdge GetEndPage()
	{
		DruidPageEdge page = new DruidPageEdge(EdgePosition.Finish);
		page.Text = GettextCatalog.GetString("Click Apply to start the database creation process");
		page.BackClicked += new BackClickedHandler(GoToPrev);
		page.FinishClicked += new FinishClickedHandler(EndOfWizard);
		return page;
	}

	internal void GoToPrev(object sender, BackClickedArgs args)
	{
		Page = PreviousPage;
		args.RetVal = true;
	}
	
	internal void EndOfWizard(object sender, FinishClickedArgs args)
	{
		IDatabaseGenerator generator = null;
		if (methodSelectionPage.generateDatabase.Active) {
			generator = (IDatabaseGenerator)new CreateDBGenerator();
			if (generateDatabasePage.light.Active)
				((CreateDBGenerator)generator).Fast = true;
		} else if (methodSelectionPage.useExisting.Active) {
			generator = (IDatabaseGenerator)new UseExistingDBGenerator();
			((UseExistingDBGenerator)generator).Path = useExistingPage.filename.Path;
		} else if (methodSelectionPage.download.Active) {
			generator = (IDatabaseGenerator)new DownloadGenerator();
			((DownloadGenerator)generator).SourceUri = new Uri(downloadPage.uri.Text);
		}
		if (Finished != null)
			Finished(this, generator);
	}

	internal void ShowLast()
	{
		Page = endPage;
	}
	
	internal void ShowMain()
	{
		Page = methodSelectionPage;
	}
	
	internal void GoToMethodPage(object sender, NextClickedArgs args)
	{
		if (methodSelectionPage.generateDatabase.Active) {
			Page = generateDatabasePage;
		} else if (methodSelectionPage.useExisting.Active) {
			Page = useExistingPage;
		} else if (methodSelectionPage.download.Active) {
			Page = downloadPage;
		}
		args.RetVal = true;
	}

	internal void HandleCancel(object sender, EventArgs args)
	{
		MessageService messageService = (MessageService)ServiceManager.Services.GetService(typeof(MessageService));
		bool really = messageService.AskQuestion(GettextCatalog.GetString ("Are you sure you want to skip database creation? You will not have code completion functionality."), GettextCatalog.GetString ("Are you sure?"));
		if (really) {
			this.Destroy();
			this.Canceled(this);
			((Gtk.Window)WorkbenchSingleton.Workbench).Visible = true;
		}
	}
}
}
