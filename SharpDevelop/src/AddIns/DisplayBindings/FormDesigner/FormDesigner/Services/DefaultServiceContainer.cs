// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DefaultServiceContainer : IServiceContainer 
	{
		IServiceContainer serviceContainer;
		
		public DefaultServiceContainer()
		{
			serviceContainer = new ServiceContainer();
		}
		
		public DefaultServiceContainer(IServiceContainer parent)
		{
			serviceContainer = new ServiceContainer(parent);
		}
		
#region System.ComponentModel.Design.IServiceContainer interface implementation
		public void RemoveService(System.Type serviceType, bool promote)
		{
			serviceContainer.RemoveService(serviceType, promote);
		}
		
		public void RemoveService(System.Type serviceType)
		{
			serviceContainer.RemoveService(serviceType);
		}
		
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, callback, promote);
			}
		}
		
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, callback);
			}
		}
		
		public void AddService(System.Type serviceType, object serviceInstance, bool promote)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance, promote);
			}
		}
		
		public void AddService(System.Type serviceType, object serviceInstance)
		{
			if (IsServiceMissing(serviceType)) {
				serviceContainer.AddService(serviceType, serviceInstance);
			}
		}
#endregion
		
#region System.IServiceProvider interface implementation
		public object GetService(System.Type serviceType)
		{
//			Console.WriteLine("request service : {0} is aviable : {1}", serviceType, !IsServiceMissing(serviceType));
//			if (IsServiceMissing(serviceType)) {
//				Console.ReadLine();
//			}
			
			return serviceContainer.GetService(serviceType);
		}
#endregion
		
		bool IsServiceMissing(Type serviceType)
		{
			return serviceContainer.GetService(serviceType) == null;
		}
	}
}
