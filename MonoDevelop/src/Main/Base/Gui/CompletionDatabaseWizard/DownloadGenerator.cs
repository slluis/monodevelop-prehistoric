using System;
using System.Net;
using System.IO;
using Gtk;
 
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
 
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Gui.Utils.DirectoryArchive;
using MonoDevelop.Gui.Utils.ReportingStream;

namespace MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard {

	public class DownloadGenerator : CreatingGenerator, IDatabaseGenerator {

		public bool Cancelable { get { return false; } }
		public Uri SourceUri;

		private void updateProgress(object arg, int amount)
		{
			ProgressHolder holder = (ProgressHolder)arg;
			holder.currentProgress = holder.startProgress + holder.s.Position;
			holder.progressBar.Worked ((int)((double)holder.currentProgress/holder.maxAmount * int.MaxValue), GettextCatalog.GetString ("Extracted more of the archive"));

			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
		}

		class ProgressHolder
		{
			public IProgressMonitor progressBar;
			public long currentProgress;
			public long startProgress;
			public long maxAmount;
			public Stream s;
		}
			
		public void Generate(IProgressMonitor progress)
		{
			string completionDir = this.CreateCodeCompletionDir();
			if (!completionDir.EndsWith ("/"))
				completionDir = completionDir + "/";

			string compressedFile = Path.GetTempFileName();

			
			long maxAmount;

			try {
				WebRequest dataReq = WebRequest.Create(SourceUri);
				WebResponse dataResp = dataReq.GetResponse();
				maxAmount = dataResp.ContentLength * 2;
 
 
				progress.BeginTask(GettextCatalog.GetString ("Downloading database"), int.MaxValue); 
				DownloadFile (dataResp.GetResponseStream (), 
						compressedFile, 
						maxAmount, progress);
			} catch (Exception e) {
				throw new Exception("Could not download database", e);
			}
			Stream s = null;

			try {
				s = System.IO.File.OpenRead(compressedFile);
				
				ProgressHolder ph = new ProgressHolder();
				ph.currentProgress = maxAmount / 2;
				ph.startProgress = maxAmount / 2;
				ph.maxAmount = maxAmount;
				ph.progressBar = progress;
				ph.s = s;
				Decompressor archive = Decompressor.Load(SourceUri.ToString());
				archive.Extract(new ReportingStream(s, new ReadNotification(updateProgress), ph), completionDir);
				System.IO.File.Delete(compressedFile);
			} catch (Exception e) {
				throw new Exception("Could not extract archive " + compressedFile + " of type " + Decompressor.GetTypeFromString(SourceUri.ToString()), e);
			} finally {
				if (s != null)
					s.Close();
			}
		}
 
		private void DownloadFile(Stream s, string fileName, long maxAmount, IProgressMonitor progress)
		{
			byte[] buffer = new byte[512*24];
			long amountDownloaded = 0;
			int amountRead;
			Stream outstream = new FileStream(fileName, FileMode.Create);
			long lastyield = 0;
			while (true) {
				amountRead = s.Read(buffer, 0, buffer.Length);
				amountDownloaded += amountRead;
				outstream.Write(buffer, 0, amountRead);
				progress.Worked ((int)((float)amountDownloaded/maxAmount * int.MaxValue), String.Format (GettextCatalog.GetString ("Downloaded more of {0}"), fileName));

				// make sure we let the GTK events happen at least every second
				long nowticks = DateTime.Now.Ticks;
				if (nowticks - lastyield > 1000) {
					while (Gtk.Application.EventsPending ())
						Gtk.Application.RunIteration ();
					lastyield = nowticks;
				}
 
				if (amountRead == 0)
					break;
			}
		}
	}
}
