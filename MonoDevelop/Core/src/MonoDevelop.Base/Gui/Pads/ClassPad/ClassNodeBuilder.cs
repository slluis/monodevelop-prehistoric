//
// ClassNodeBuilder.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;

using MonoDevelop.Internal.Project;
using MonoDevelop.Services;
using MonoDevelop.Internal.Parser;

namespace MonoDevelop.Gui.Pads.ClassPad
{
	public class ClassNodeBuilder: TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ClassData); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(ClassNodeCommandHandler); }
		}
		
		public override string GetNodeName (object dataObject)
		{
			return ((ClassData)dataObject).Class.Name;
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ClassData classData = dataObject as ClassData;
			label = classData.Class.Name;
			icon = Context.GetIcon (Runtime.Gui.Icons.GetIcon (classData.Class));
		}

		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			ClassData classData = dataObject as ClassData;

			foreach (IClass innerClass in classData.Class.InnerClasses)
				builder.AddChild (innerClass);

			foreach (IMethod method in classData.Class.Methods)
				builder.AddChild (method);
			
			foreach (IProperty property in classData.Class.Properties)
				builder.AddChild (property);
			
			foreach (IField field in classData.Class.Fields)
				builder.AddChild (field);
			
			foreach (IEvent e in classData.Class.Events)
				builder.AddChild (e);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			ClassData classData = dataObject as ClassData;
			return 	classData.Class.InnerClasses.Count > 0 ||
					classData.Class.Methods.Count > 0 ||
					classData.Class.Properties.Count > 0 ||
					classData.Class.Fields.Count > 0 ||
					classData.Class.Events.Count > 0;
		}
		
		public override int CompareObjects (object thisDataObject, object otherDataObject)
		{
			if (otherDataObject is ClassData)
				return DefaultSort;
			else
				return 1;
		}
	}
	
	public class ClassNodeCommandHandler: NodeCommandHandler
	{
		public override void ActivateItem ()
		{
			string file = GetFileName ();
			Runtime.FileService.OpenFile (file, new FileOpeningFinished (OnFileOpened));
		}
		
		private void OnFileOpened()
		{
			ClassData cls = CurrentNode.DataItem as ClassData;
			int line = cls.Class.Region.BeginLine;
			string file = GetFileName ();
			
			IWorkbenchWindow window = Runtime.FileService.GetOpenFile (file);
			if (window == null) return;
			
			IViewContent content = window.ViewContent;
			if (content is IPositionable) {
				((IPositionable)content).JumpTo (Math.Max (0, line), 0);
			}
		}
		
		string GetFileName ()
		{
			ClassData cls = (ClassData) CurrentNode.GetParentDataItem (typeof(ClassData), true);
			if (cls != null && cls.Class.Region.FileName != null) return cls.Class.Region.FileName;
			return null;
		}
	}	
}
