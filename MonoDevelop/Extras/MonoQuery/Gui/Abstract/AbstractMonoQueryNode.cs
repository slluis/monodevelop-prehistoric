
namespace MonoQuery.Gui.TreeView
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Data;
	using System.ComponentModel;

	using MonoQuery.Collections;
	using MonoDevelop.Core.Services;
	using MonoDevelop.Services;
	using MonoDevelop.Gui;
	using MonoDevelop.Core.AddIns.Conditions;
	using MonoDevelop.Core.AddIns.Codons;
	using MonoDevelop.Core.AddIns;
	using MonoDevelop.Gui.Widgets;
	
	using MonoQuery.SchemaClass;		
	using MonoQuery.Connection;	
	using MonoQuery.Gui.DataView;
	using MonoQuery.Exceptions;
	using MonoQuery.Codons;
	using MonoQuery.Services;
	
	public abstract class AbstractMonoQueryNode : TreeNode, IMonoQueryNode
	{
		internal static SQLParameterInput inputform = null;
		
		///<summary>
		/// this variable force to have a "plus" near the node.
		/// </summary>
		public static StringParserService stringParserService = stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));	
		protected Assembly ass = null;

		protected ISchemaClass pSchemaClass = null;	

		///<summary> force to displayed a "+" for the node</summary>
		protected virtual bool NullChildAllowed
		{
			get
			{
				return true;
			}
		}
			
		public virtual string AddinContextMenu { 
			get{
				return "";
			}
		}				
		
		public virtual string entityNormalizedName
		{
			get
			{
				if ( this.SchemaClass != null )
				{
					return AbstractMonoQuerySchemaClass.RemoveBracket( this.SchemaClass.NormalizedName );
				}
				else
				{
					return "";
				}
			}
		}
		
		public virtual string entityName
		{
			get
			{
				if ( this.SchemaClass != null )
				{
					return this.SchemaClass.InternalName;
				}
				else
				{
					return "";
				}				
			}
		}
		
		public ISchemaClass SchemaClass{
			get
			{
				if ( this.pSchemaClass != null )
				{
					return this.pSchemaClass;
				}
				else
				{
					return null;
				}
			}
		}
		
		public virtual IConnection  Connection{ 
			get
			{
				if ( this.SchemaClass != null )
				{
					return this.SchemaClass.Connection;
				}
				else
				{
					return null;
				}
			}
		}				
		
		public virtual MonoQueryListDictionary Entities { 
			get{
				if ( this.SchemaClass != null )
				{
					return this.SchemaClass.Entities;
				}
				else
				{
					return null;
				}
			}
		}
				
		public AbstractMonoQueryNode() : base()
		{
			ass = System.Reflection.Assembly.GetExecutingAssembly();			
		}
				
		public AbstractMonoQueryNode(ISchemaClass schemaclass) : this()
		{						
			this.pSchemaClass = schemaclass;						
		}
		
		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected virtual void OnRefresh()
		{
			// Nothing !
		}
		
		public virtual void Refresh()
		{
			try
			{
				if ( this.TreeView != null )
				{
					this.TreeView.BeginUpdate();
				}
				
				this.Clear();				
	
				this.OnRefresh();
				
				if ( this.Connection.IsOpen )
				{
					this.Text = this.entityName;
					
//						if ( this.IsExpanded == true )
//						{
						if ( this.SchemaClass != null )
						{
							this.SchemaClass.Refresh();
						}								
						this.BuildsChilds();				
//						}						
				}
			}
			finally
			{
				if ( this.TreeView != null )
				{
					this.TreeView.EndUpdate();					
				}				
			}
		}
		
		public virtual void Clear()
		{	
			if ( this.SchemaClass != null )
			{
				this.SchemaClass.Clear();
			}
			
			if ( SchemaClass != null )
			
			this.Nodes.Clear();
			
			if ( ( this.IsExpanded == false ) && (this.NullChildAllowed == true) )
			{
				this.Nodes.Add( new TreeNode() );
			}
		}
		
		///<summary>
		/// allow the user to add some parameters while executing an SQL command
		/// </summary>
		protected virtual MonoQuerySchemaClassCollection OnExecute( CancelEventArgs e )
		{
			return null;
		}
		
		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// </summary>
		public void Execute( int rows )
		{			
			try
			{
				if ( this.SchemaClass != null )
				{					
					CancelEventArgs e = new CancelEventArgs();
					MonoQuerySchemaClassCollection ret = this.OnExecute( e );
					if ( e.Cancel == false )
					{
						WorkbenchSingleton.Workbench.ShowView( new MonoQueryDataView( this.SchemaClass, rows, ret ) );
					}
				}
			}
			catch( Exception e)
			{
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowError( e.Message );																			
			}						
		}
		
		public virtual void BuildsChilds()
		{		
			string childclass = "";
			IMonoQueryNode ChildNode = null;

			if ( this.Entities != null )
			{
				this.Nodes.Clear();
				
				foreach( DictionaryEntry DicEntry in this.Entities )
				{												
					if ( DicEntry.Value != null)							
					{									
						CollectionBase entitieslist = DicEntry.Value as CollectionBase;
												
						foreach ( ISchemaClass entity in entitieslist )
						{
							childclass = MonoQueryTree.SchemaClassDict[ entity.GetType().FullName ];
							if ( ( childclass != null) && (childclass != "") )
							{
								ChildNode = (IMonoQueryNode)ass.CreateInstance(childclass, false, BindingFlags.CreateInstance, null, new object[] {entity}, null, null);
								if ( ChildNode != null )
								{
									bool addNode = true;
									
									if ( ChildNode is MonoQueryNodeNotSupported )
									{
										addNode = this.ShowUnsupported();
									}
									if ( addNode == true )
									{
										this.Nodes.Add( ChildNode as TreeNode );
										ChildNode.Refresh();
									}									
								}									
							}												
						}						
					}
				}
			}			
		}
		
		protected bool ShowUnsupported()
		{
			IAddInTreeNode AddinNode;	
			bool ret = true;
			
			AddinNode = (IAddInTreeNode)AddInTreeSingleton.AddInTree.GetTreeNode("/MonoQuery/Connection");
			foreach ( DictionaryEntry entryChild in AddinNode.ChildNodes)
			{
				IAddInTreeNode ChildNode = entryChild.Value as IAddInTreeNode;
				if ( ChildNode != null )
				{
					MonoQueryConnectionCodon codon = ChildNode.Codon as MonoQueryConnectionCodon;
					if ( codon != null )
					{
						if ( codon.Node == this.GetType().FullName )
						{
							ret = bool.Parse( codon.ShowUnsuported );
						}
					}					
				}
			}
			
			return ret;
		}
	}
}