// created on 07/11/2003 at 10:15
using System;
using System.Xml;
using System.ComponentModel;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoQuery.SchemaClass;
using MonoQuery.Collections;
using System.Collections;
using MonoQuery.Gui.DataView;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;

namespace MonoQuery.Gui.TreeView
{
	
	///<summary>
	/// Column Node
	///</summary>	
	public class MonoQueryNodeColumn : AbstractMonoQueryNode
	{						
		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;
			}
		}
		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Coulmn";
			}
		}
		
		public MonoQueryNodeColumn( MonoQueryColumn monoQueryColumn ) : base( monoQueryColumn )
		{
			//this.Image = "";
			
//			this.ImageIndex = 9;
//			this.SelectedImageIndex = 9;
		}				
	}	
	
	///<summary>
	/// Parameter Node
	///</summary>	
	public class MonoQueryNodeParameter : AbstractMonoQueryNode
	{		
		
		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;
			}
		}
		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Parameter";
			}
		}
		
		public MonoQueryNodeParameter( MonoQueryParameter monoQueryParameter ) : base( monoQueryParameter )
		{
			//this.Image = "";
			
//			this.ImageIndex = 9;
//			this.SelectedImageIndex = 9;
		}				
	}	

	///<summary>
	/// Table Node
	///</summary>	
	public class MonoQueryNodeTable : AbstractMonoQueryNode
	{		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Table";
			}
		}
		
		public MonoQueryNodeTable( MonoQueryTable monoQueryTable ) : base( monoQueryTable )
		{
			this.Image = "md-mono-query-table";
			
//			this.ImageIndex = 6;
//			this.SelectedImageIndex = 6;	
		}						
	}
	
	///<summary>
	/// View Node
	///</summary>	
	public class MonoQueryNodeView : AbstractMonoQueryNode
	{									
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/View";
			}
		}
		
		public MonoQueryNodeView( MonoQueryView monoQueryView ) : base( monoQueryView )
		{
			this.Image = "md-mono-query-table";
			
//			this.ImageIndex = 7;
//			this.SelectedImageIndex = 7;
		}						
	}
	
	///<summary>
	/// Procedure Node
	///</summary>	
	public class MonoQueryNodeProcedure: AbstractMonoQueryNode
	{		

		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Procedure";
			}
		}
		
		public MonoQueryNodeProcedure( MonoQueryProcedure monoQueryProcedure ) : base( monoQueryProcedure )
		{
			this.Image = "md-mono-query-procedure";
			
//			this.ImageIndex = 8;
//			this.SelectedImageIndex = 8;		
		}					
		
		///<summary>
		/// allow the user to add some parameters while executing an SQL command
		/// </summary>
		protected override MonoQuerySchemaClassCollection OnExecute( CancelEventArgs e )
		{
			MonoQuerySchemaClassCollection tmp = this.SchemaClass.GetSchemaParameters();
			MonoQueryParameterCollection parameters = null;			
			MonoQuerySchemaClassCollection returnValue = null;
			
//			if ( tmp.Count == 1 && tmp[0] is MonoQueryNotSupported )
//			{
//				parameters = new MonoQueryParameterCollection();
//			}
//			else
//			{
//				parameters = new MonoQueryParameterCollection( tmp );
//			}
//			
//			if ( parameters != null && parameters.Count > 0 )
//			{
//				inputform = new SQLParameterInput( parameters );
//				inputform.Owner = (Form)WorkbenchSingleton.Workbench;
//				
//				if ( inputform.ShowDialog() != DialogResult.OK )
//				{
//					returnValue = null;
//					e.Cancel = true;
//				}
//				else
//				{
//					returnValue	= parameters.ToBaseSchemaCollection();
//				}
//			}
			
			return returnValue;
		}
	}
	
	///<summary>
	/// Node displayed when a function is not supported by the provider!
	///</summary>	
	public class MonoQueryNodeNotSupported: AbstractMonoQueryNode
	{		
		
		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;				
			}
		}
		
		public MonoQueryNodeNotSupported( MonoQueryNotSupported monoQueryNotSupported ) : base( monoQueryNotSupported )
		{
//			this.ImageIndex = 10;
//			this.SelectedImageIndex = 10;		
		}										
	}
	

	///<summary>
	/// Schema Node
	///</summary>	
	public class MonoQueryNodeSchema : AbstractMonoQueryNode
	{		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Schema";
			}
		}
								
		public MonoQueryNodeSchema( MonoQuerySchema schema ) : base(schema)
		{
			//this.Image = "";
			
//			this.ImageIndex = 1;
//			this.SelectedImageIndex = 1;				
		}		
		
	}

	///<summary>
	/// Catalog Node
	///</summary>	
	public class MonoQueryNodeCatalog : AbstractMonoQueryNode
	{		
		public override string AddinContextMenu { 
			get{
				return "/MonoQuery/ContextMenu/Catalog";
			}
		}
								
		public MonoQueryNodeCatalog( MonoQueryCatalog catalog ) : base(catalog)
		{
			//this.Image = "";
			
//			this.ImageIndex = 1;
//			this.SelectedImageIndex = 1;							
		}														
	}
	
	
}
