//
// ProviderCollectionNodeBuilder.cs
//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//
// Copyright (c) 2005 Christian Hergert
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

using Mono.Data.Sql;
using MonoDevelop.Services;
using MonoDevelop.Gui.Pads;

namespace MonoQuery
{
	public class ProviderCollectionNodeBuilder : TypeNodeBuilder
	{
		public ProviderCollectionNodeBuilder() : base()
		{
		}
		
		public override Type NodeDataType {
			get {
				return typeof(DbProviderCollection);
			}
		}
		
		public override string ContextMenuAddinPath {
			get {
				return "/SharpDevelop/Views/DatabasePad/ContextMenu/ConnectionsBrowserNode";
			}
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Database Connections");
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = GettextCatalog.GetString ("Database Connections");
			string iconName = "md-mono-query-providers";
			icon = Context.GetIcon (iconName);
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			DbProviderCollection collection = (DbProviderCollection) dataObject;
			BuildChildNodes (builder, collection);
		}
		
		public static void BuildChildNodes (ITreeBuilder builder, DbProviderCollection collection)
		{
			foreach (DbProviderBase provider in collection)
				builder.AddChild (provider);
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
}
