// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts
{
	public class DefaultDesignerLoader : DesignerLoader
	{
		bool                loading             = false;
		IDesignerLoaderHost designerLoaderHost  = null;
		
		public override bool Loading {
			get {
				return loading;
			}
		}
		
		public IDesignerLoaderHost DesignerLoaderHost {
			get {
				return designerLoaderHost;
			}
		}
		
		public DefaultDesignerLoader()
		{
		}
		
#region System.ComponentModel.Design.Serialization.DesignerLoader abstract class implementation
		public override void Dispose()
		{
		}
		
		public override void BeginLoad(System.ComponentModel.Design.Serialization.IDesignerLoaderHost host)
		{
			loading            = true;
			designerLoaderHost = host; 
			
		}
		
		public virtual void EndLoad()
		{
			designerLoaderHost.EndLoad("DesignerLoader", false, null);
			loading = false;
		}
#endregion

		public override void Flush()
		{
			base.Flush();
		}
	}
}
