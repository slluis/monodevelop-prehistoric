using System;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;

using MonoDevelop.Gui;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Core.Services;

using MonoQuery.SchemaClass;
using MonoQuery.Collections;

namespace MonoQuery.Gui.DataView
{
	public class SQLParameterInput //: XmlForm
	{
		private DataGrid _dataGrid = null;

		public DataGrid dataGrid
		{
			get
			{
				if ( this._dataGrid == null )
				{
//					this._dataGrid = this.ControlDictionary["dataGrid"] as DataGrid;
				}
				return this._dataGrid;
			}
		}

		private void ResetClick( object sender, EventArgs e )
		{

		}

		protected void FillParameters( MonoQueryParameterCollection parameters )
		{
			MonoQueryParameter par = null;
			for( int i = 0; i < parameters.Count; i++)
			{
				par = parameters[i];
				if ( par.Type == ParameterDirection.ReturnValue )
				{
					i--;
					parameters.Remove( par );
				}
			}
//			this.dataGrid.CaptionVisible = true;
			this.dataGrid.DataSource = parameters;
	 		this.dataGrid.DataMember = null;
//	 		this.dataGrid.AllowNavigation = false;
		}

//		static PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
//		public SQLParameterInput() : base(propertyService.DataDirectory + @"\resources\dialogs\MonoQuery\SqlParametersInput.xfrm")
//		{
//		}
//
//		public SQLParameterInput( MonoQueryParameterCollection parameters ) : this()
//		{
//			this.FillParameters( parameters );
//		}
//
//		protected override void SetupXmlLoader()
//		{
//			xmlLoader.StringValueFilter    = new MonoDevelopStringValueFilter();
//			xmlLoader.PropertyValueCreator = new MonoDevelopPropertyValueCreator();
//		}
	}
}

