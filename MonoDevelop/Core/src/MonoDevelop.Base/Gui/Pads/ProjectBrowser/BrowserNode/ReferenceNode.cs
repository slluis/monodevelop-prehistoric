// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	public class ReferenceNode : AbstractBrowserNode, IDisposable
	{		
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ReferenceNode";
		
		public ProjectReference ProjectReference {
			get {
				return (ProjectReference)UserData;
			}
		}
		
		void SetNodeName(object sender, EventArgs e)
		{
			string name = null;
			switch (ProjectReference.ReferenceType) {
				case ReferenceType.Typelib:
					int index = ProjectReference.Reference.IndexOf("|");
					if (index > 0) {
						name = ProjectReference.Reference.Substring(0, index);
					} else {
						name = ProjectReference.Reference;
					}
					break;
				case ReferenceType.Project:
					name = ProjectReference.Reference;
					break;
				case ReferenceType.Assembly:
					name = Path.GetFileName(ProjectReference.Reference);
					break;
				case ReferenceType.Gac:
					name = ProjectReference.Reference.Split(',')[0];
					break;
				default:
					throw new NotImplementedException("reference type : " + ProjectReference.ReferenceType);
			}
			Text = name;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			this.ProjectReference.ReferenceChanged -= new EventHandler(SetNodeName);
		}
		
		public ReferenceNode(ProjectReference projectReference)
		{
			UserData  = projectReference;
			
			canLabelEdited = false;
			
			contextmenuAddinTreePath = defaultContextMenuPath;
			SetNodeName(this, EventArgs.Empty);
			this.ProjectReference.ReferenceChanged += new EventHandler(SetNodeName);
		}
		
		public override void ActivateItem()
		{
			if (userData != null && userData is ProjectReference)
				Runtime.FileService.OpenFile (((ProjectReference)userData).GetReferencedFileName());
		}
		
		/// <summary>
		/// Removes a reference from a project
		/// NOTE : This method assumes that its parent is 
		/// from the type 'ProjectBrowserNode'.
		/// </summary>
		public override bool RemoveNode()
		{
			ProjectReference referenceInformation = (ProjectReference)UserData;
			Project.ProjectReferences.Remove(referenceInformation);
			return true;
		}
	}
}
