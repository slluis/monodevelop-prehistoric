using System;
using Gtk;
using Gnome;

using MonoDevelop.Gui.Widgets;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard {
class MethodSelectionPage : DruidPageStandard {
	internal RadioButton generateDatabase;
	internal RadioButton useExisting;
	internal RadioButton download;
	
	internal MethodSelectionPage(CodeCompletionDruid druid) : base() {
		generateDatabase = new RadioButton("Generate a code completion database");
		useExisting = new RadioButton(generateDatabase, "Use a code completion database already on this computer");
		download = new RadioButton(generateDatabase, "Download a code completion database");
		this.NextClicked += new NextClickedHandler(druid.GoToMethodPage);
		AppendItem("", generateDatabase, "");
		AppendItem("", useExisting, "");
//		AppendItem("", download, "");
	}
}


class GenerateDatabasePage : DetailsPageBase {
	internal RadioButton heavy;
	internal RadioButton light;
	
	internal GenerateDatabasePage(CodeCompletionDruid druid) : base(druid) {
		heavy = new RadioButton("Heavy process");
		light = new RadioButton(heavy, "Light process");
		AppendItem("", heavy, "This process is slower and more memory-intensive than the light process, but will enable faster code completion");
		AppendItem("", light, "This process will take less time and memory to produce the code completion database, but code completion will be slower");
	}
}

class UseExistingPage : DetailsPageBase {
	internal MonoDevelop.Gui.Widgets.FolderEntry filename;
	
	internal UseExistingPage(CodeCompletionDruid druid) : base(druid) {
		filename = new FolderEntry("Select code completion database");
		filename.DefaultPath = System.IO.Directory.GetCurrentDirectory();
		AppendItem("Where is the code completion database you would like to copy", filename, "");
	}
}

class DownloadPage : DetailsPageBase {
	internal Gtk.Entry uri;
	
	internal DownloadPage(CodeCompletionDruid druid) : base(druid) {
		uri = new Gtk.Entry();
		AppendItem("Where would you like to download the code completion database from?", uri, "");
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
		druid.PreviousPage = this;
		druid.ShowLast();
		args.RetVal = true;
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
		page.Text = "This druid will guide you through the process of creating a code completion database";
		return page;
	}

	internal DruidPageEdge GetEndPage()
	{
		DruidPageEdge page = new DruidPageEdge(EdgePosition.Finish);
		page.Text = "Click Accept to start the database creation process";
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
		Console.WriteLine("Finishing druid");
		IDatabaseGenerator generator = null;
		if (methodSelectionPage.generateDatabase.Active) {
			generator = (IDatabaseGenerator)new CreateDBGenerator();
			if (generateDatabasePage.light.Active)
				((CreateDBGenerator)generator).Fast = true;
		} else if (methodSelectionPage.useExisting.Active) {
			generator = (IDatabaseGenerator)new UseExistingDBGenerator();
			((UseExistingDBGenerator)generator).Path = useExistingPage.filename.Path;
	//	} else if (methodSelectionPage.download.Active) {
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
		Console.WriteLine("Showing main page...");
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
		bool really = messageService.AskQuestion("Are you sure you want to skip database creation? You will not have code completion functionality.", "Are you sure?");
		Console.WriteLine("Really? " + really);
		if (really) {
			this.Destroy();
			this.Canceled(this);
		}
	}
}
}