//
// NunitPad - Pad to embed nunit-gtk
//
// Author: John Luke <jluke@cfl.rr.com>
//
// (C) 2004 John Luke

using System;
using System.Collections;
using System.IO;
using System.Text;
using Gtk;

using MonoDevelop.Gui;
using MonoDevelop.Services;
using MonoDevelop.Services.Nunit;
using MonoDevelop.Core.Services;
using NUnit.Core;

namespace MonoDevelop.Nunit.Gui
{
	public class ResultTree : AbstractViewContent
	{
		TreeView resultTree = new TreeView ();

		public ResultTree ()
		{

		}

		public override Widget Control
		{
			get {
				return resultTree;
			}
		}

		public override void Load (string content)
		{
		}
	}
}
