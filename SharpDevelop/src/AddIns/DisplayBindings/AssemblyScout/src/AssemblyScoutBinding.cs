// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using MagicControls = Crownwood.Magic.Controls;

using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	///////////////////////////////////////////
	// AssemblyScoutBinding class
	///////////////////////////////////////////
	public class AssemblyScoutBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".DLL" || 
			       Path.GetExtension(fileName).ToUpper() == ".EXE";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		
		public IViewContent CreateContentForFile(string fileName)
		{
			AssemblyScoutViewContent wrapper = new AssemblyScoutViewContent();
			wrapper.Load(fileName);
			return wrapper;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
	}

}
