// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

using SharpDevelop;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace NewClassWizard
{
	/// <summary>
	/// Display all if the namespaces, classes, and interfaces
	/// exported by a given assembly or set of assemblies
	/// </summary>
	internal class NamespaceTreeView : System.Windows.Forms.TreeView 
	{

		private System.ComponentModel.IContainer components = null;

		private bool _showConcreteClasses	= true;
		private bool _showAbstractClasses	= true;
		private bool _showSealedClasses		= true;
		private bool _showInterfaces		= true;

		//static consturctor to ensure that the image list is initialized
		//static NamespaceTreeView() {
		//		ClassBrowserIcons.InitIcons();
		//}
		
		public NamespaceTreeView()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public bool ShowAbstractClasses
		{
			get
			{
				return _showAbstractClasses;
			}
			set
			{
				_showAbstractClasses = value;
			}
		}
		
		public bool ShowConcreteClasses
		{
			get
			{
				return _showConcreteClasses;
			}
			set
			{
				_showConcreteClasses = value;
			}
		}
		
		public bool ShowInterfaces
		{
			get
			{
				return _showInterfaces;
			}
			set
			{
				_showInterfaces = value;
			}
		}
		
		public bool ShowSealedClasses
		{
			get
			{
				return _showSealedClasses;
			}
			set
			{
				_showSealedClasses = value;
			}
		}
		
		public Type SelectedType
		{
			get
			{
				if ( this.SelectedNode != null && this.SelectedNode.Tag != null )
				{
					return (Type)this.SelectedNode.Tag;
				}
				else
				{
					return null;
				}
			}
		}

		public void ShowAssembly( Assembly a )
		{
			try {
				Namespace ns = Namespace.CreateNamespaceTree( a );
				AddNamespace( ns );
			} catch ( Exception ) {
				
			}
		}

		public void AddNamespace( NewClassWizard.Namespace ns )
		{
			if ( ns == null )
				throw new NullReferenceException();

			loadNodes( this.Nodes, ns );
		}
		
		ClassBrowserIconsService classBrowserIconsService = (ClassBrowserIconsService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		
		private void loadNodes( TreeNodeCollection nodes, Namespace ns )
		{
			//add a node for the namespace to each treeview
			TreeNode nsNode = new TreeNode( ns.Name, classBrowserIconsService.NamespaceIndex, classBrowserIconsService.NamespaceIndex );
			nodes.Add( nsNode );

			//recurse over all child namespaces
			IEnumerator namespaces = ns.Namespaces.GetEnumerator();

			while ( namespaces.MoveNext() )
			{
				DictionaryEntry entry = (DictionaryEntry)namespaces.Current;
				Namespace n = (Namespace)entry.Value;
				loadNodes( nsNode.Nodes, n );
			}

			loadTypes( nsNode.Nodes, ns );
			
		}

		private	void loadTypes(	TreeNodeCollection nodes, Namespace	ns )
		{
			TreeNode typeNode =	null;
			//add all the correct types	in this	namesapce to each treeview
			foreach( Type t	in ns.GetTypes() )
			{
				
				if ( t.IsClass )	
				{
					if ( t.IsAbstract && _showAbstractClasses )
						typeNode = new TreeNode( t.Name, classBrowserIconsService.ClassIndex, classBrowserIconsService.ClassIndex );
										
					else if ( t.IsSealed && _showSealedClasses )
						typeNode = new TreeNode( t.Name, classBrowserIconsService.ClassIndex, classBrowserIconsService.ClassIndex );
									
					else if ( _showConcreteClasses )
						typeNode = new TreeNode( t.Name, classBrowserIconsService.ClassIndex, classBrowserIconsService.ClassIndex );

				} 
				
				else if	( t.IsInterface	&& _showInterfaces ) 
				{
					typeNode = new TreeNode( t.Name, classBrowserIconsService.InterfaceIndex, classBrowserIconsService.InterfaceIndex );								
				}

				if ( typeNode != null )
				{
					typeNode.Tag = t;
					nodes.Add( typeNode );
					typeNode =	null;
				}

			}

		}

		#region Disposer
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion


		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			// 
			// NamespaceTreeView
			// 
			this.HideSelection = false;
			this.HotTracking = true;
			this.ImageIndex = 0;
			this.ImageList = classBrowserIconsService.ImageList;
			this.PathSeparator = ".";
			this.SelectedImageIndex = 0;
			this.Sorted = true;
		}

		#endregion
	}
}
