
using System;
using MonoDevelop.Core.AddIns.Codons;
using MonoQuery.Gui.TreeView;

namespace MonoQuery.Commands
{
	public class MonoQueryRefreshCommand : AbstractMonoQueryCommand
	{	
		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is AbstractMonoQueryNode)
									  && (this.monoQueryNode  as AbstractMonoQueryNode).Connection != null
									  && (this.monoQueryNode  as AbstractMonoQueryNode).Connection.IsOpen == true;
			}			
			set{}
		}
		/// <summary>
		/// Refresh the selected <see cref="MonoQuery.Gui.TreeView.IMonoQueryNode">node</see> of the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"> MonoQuery Tree.</see>
		/// </summary>
		public override void Run()
		{
			( this.monoQueryNode as IMonoQueryNode).Refresh();
		}
	}

	/// <summary>
	/// Add a connection to a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
	/// </summary>
	public class MonoQueryAddconnectionCommand : AbstractMonoQueryCommand
	{			
		
		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is MonoQueryNodeDatabaseRoot);				
			}			
			set{}
		}		
		
		/// <summary>
		/// Add a connection to a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
		/// </summary>
		public override void Run()
		{
			( this.monoQueryNode as MonoQueryNodeDatabaseRoot ).AddConnection();
		}
	}
	
	/// <summary>
	/// Remove a connection from a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
	/// </summary>	
	public class MonoQueryRemoveConnectionCommand : AbstractMonoQueryCommand
	{			
		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is MonoQueryNodeConnection);				
			}			
			set{}
		}		
		
		/// <summary>
		/// Remove a connection from a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
		/// </summary>
		public override void Run()
		{
			( this.monoQueryNode as MonoQueryNodeConnection).RemoveConnection();
		}
	}	
	
	
	/// <summary>
	/// Remove a connection from a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
	/// </summary>	
	public class MonoQueryModifyConnectionCommand : AbstractMonoQueryCommand
	{			
		
		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is MonoQueryNodeConnection);				
			}			
			set{}
		}
		
		/// <summary>
		/// Remove a connection from a database server into the <see cref="MonoQuery.Gui.TreeView.MonoQueryTree"></see>
		/// </summary>
		public override void Run()
		{
			( this.monoQueryNode as MonoQueryNodeConnection).ModifyConnection();
		}
	}	
		
	/// <summary>
	/// Disconnect From a database server
	/// </summary>		
	public class MonoQueryDisconnectCommand : AbstractMonoQueryCommand	
	{

		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is MonoQueryNodeConnection)
									  && (this.monoQueryNode  as MonoQueryNodeConnection).IsConnected == true;
			}			
			set{}
		}
		
		public MonoQueryDisconnectCommand() : base()
		{
		}
		
		/// <summary>
		/// Disconnect From a database server
		/// </summary>
		public override void Run()
		{
			(this.monoQueryNode as MonoQueryNodeConnection).Disconnect();
		}
	}
	
	/// <summary>
	/// Disconnect From a database server
	/// </summary>		
	public class MonoQueryConnectCommand : AbstractMonoQueryCommand	
	{

		public override bool IsEnabled
		{
			get
			{				
				return base.IsEnabled && (this.monoQueryNode  is MonoQueryNodeConnection)
									  && (this.monoQueryNode  as MonoQueryNodeConnection).IsConnected == false;
			}			
			set{}
		}
		
		public MonoQueryConnectCommand() : base()
		{
		}
		
		/// <summary>
		/// Disconnect From a database server
		/// </summary>
		public override void Run()
		{
			(this.monoQueryNode as MonoQueryNodeConnection).Connect();
		}
	}
	
	
	/// <summary>
	/// Disconnect From a database server
	/// </summary>		
	public class MonoQueryExecuteCommand : AbstractMonoQueryCommand	
	{		
		public MonoQueryExecuteCommand() : base()
		{
		}
		
		/// <summary>
		/// Disconnect From a database server
		/// </summary>
		public override void Run()
		{
			this.monoQueryNode.Execute(0);
		}
	}
	
	/// <summary>
	/// Show the sql editor
	/// </summary>
	public class MonoQueryShowSqlViewCommand : AbstractMonoQueryCommand
	{
		public MonoQueryShowSqlViewCommand() : base()
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void Run()
		{
			IMonoQueryNode node = this.monoQueryNode;
			
			MonoDevelop.Gui.WorkbenchSingleton.Workbench.ShowView(
				new MonoQuery.Gui.SqlQueryView.SqlQueryView(
					node.Connection ), true );
		}
	}
}
