
using System;

using MonoDevelop.Core.AddIns.Codons;

using MonoQuery.Gui.TreeView;

namespace MonoQuery.Commands
{
	/// <summary>
	/// Base class of all commands of MonoQuery Addin
	/// </summary>
	public abstract class AbstractMonoQueryCommand : AbstractMenuCommand
	{	
		protected IMonoQueryNode monoQueryNode = null;
				
		/// <summary>
		/// get the selected <see cref="MonoQuery.Gui.TreeView.IMonoQueryNode"> MonoQuery node </see>
		/// and Enabled or disabled the command
		/// <remarks> If the selected node is <code>null</code> or this is not a <see cref="MonoQuery.Gui.TreeView.IMonoQueryNode"> MonoQuery node </see>, return <code>false</code> (disable the menu)</remarks>
		/// </summary>				
		public override bool IsEnabled
		{
			get
			{
				MonoQueryTree monoQueryTree;
				monoQueryTree = this.Owner as MonoQueryTree;								
				
				if ( (monoQueryTree != null) && ( monoQueryTree.SelectedNode != null ) )
				{			
					this.monoQueryNode = monoQueryTree.SelectedNode as IMonoQueryNode;				
				}
				else
				{
					this.monoQueryNode = null;
				}
				
				return (this.monoQueryNode != null);				
			}			
			set{}
			
		}
		
		/// <summary>
		/// Create a new MonoQueryCommand
		/// </summary>		
		public AbstractMonoQueryCommand() : base()
		{
		}		
		
	}
}
