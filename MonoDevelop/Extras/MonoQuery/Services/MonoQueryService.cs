
using System;
using System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;

using MonoQuery.Collections;
using MonoQuery.Codons;
using MonoQuery.Gui.TreeView;

namespace MonoQuery.Services
{
	public class MonoQueryService : AbstractService
	{
		#region Private Properties
		private ArrayList providers;
		private MonoQueryTree tree;
		#endregion
		
		#region Public Properties
		public ArrayList Providers
		{
			get { return providers; }
		}
		public MonoQueryTree Tree
		{
			get { return tree; }
			set { tree = value; }
		}
		#endregion
		
		#region Constructors
		public MonoQueryService() : base()
		{
			// Build Providers List
			this.providers = new ArrayList();
			this.BuildProviders();
			
			// Load saved connections
			LoadConnections();
		}
		#endregion
		
		#region Private Methods
		private void BuildProviders()
		{
			IAddInTreeNode AddinNode;
			
			AddinNode = (IAddInTreeNode)AddInTreeSingleton.AddInTree.GetTreeNode("/MonoQuery/Connection");
			foreach ( DictionaryEntry entryChild in AddinNode.ChildNodes)
			{
				IAddInTreeNode ChildNode = entryChild.Value as IAddInTreeNode;
				if ( ChildNode != null )
				{
					MonoQueryConnectionCodon codon = ChildNode.Codon as MonoQueryConnectionCodon;
					if ( codon != null )
					{
						if ( codon.Node == "MonoQuery.Gui.TreeView.MonoQueryNodeConnection" )
						{
							Type type = System.Type.GetType( codon.Schema );
							if ( type != null ) {
								this.Providers.Add( new ConnectionProviderDescriptor( codon.Description, type ) );
							}
						}
					}					
				}
			}
		}
		
		private void LoadConnections()
		{
// FIXME: Load connections from monodevelop serialization system.
			
		}
		#endregion
	}
}