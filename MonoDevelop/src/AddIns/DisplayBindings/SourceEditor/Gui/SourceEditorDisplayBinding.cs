using System;
using System.IO;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorDisplayBinding : IDisplayBinding
	{
		
		public virtual bool CanCreateContentForFile (string fileName)
		{
			return true;
		}
		
		public virtual bool CanCreateContentForLanguage (string language)
		{
			return true;
		}
		
		public virtual IViewContent CreateContentForFile (string fileName)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			w.Load (fileName);
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content)
		{
			w.LoadString (language, content);
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content, string new_file_name)
		{
			w.LoadString (language, content);
		}	
	}
	
	public class SourceEditorDisplayBindingWrapper : AbstractViewContent
	{
		internal SourceEditor se;
		
		public override Gtk.Widget Control {
			get {
				return textAreaControl;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			}
		}
		
		public SourceEditorDisplayBindingWrapper ()
		{
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override void Save (string fileName)
		{
		}
		
		public override void Load (string fileName)
		{
		}
	}
}
