// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.0.3705.288
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Windows.Forms.Design;
using System.CodeDom;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	/// <summary>
	/// CodeDom serialization service for designers
	/// </summary>
	/// <remarks>
	/// 	created by - Niv
	/// 	created on - 12/10/2002 03:02:27
	/// </remarks>
	public class CodeDomDesignerSerializetionManager : IDesignerSerializationManager, IServiceContainer
	{
		protected ContextStack context = null;
		protected Hashtable nameToObject = new Hashtable();
		protected Hashtable objectToName = new Hashtable();
		protected PropertyDescriptorCollection props = PropertyDescriptorCollection.Empty;
		protected ArrayList providers = new ArrayList();
		protected IDesignerHost host = null;
		private ServiceContainer services = null;

		/// <summary>
		/// TODO - add property description
		/// </summary>
		/// <remarks>
		/// Interface property from IDesignerSerializationManager
		/// 	- read only
		///
		/// </remarks>
		public virtual ContextStack Context {
			get { return context;}
		}

		/// <summary>
		/// TODO - add property description
		/// </summary>
		/// <remarks>
		/// Interface property from IDesignerSerializationManager
		/// 	- read only
		///
		/// </remarks>
		/// TODO ???????
		public virtual PropertyDescriptorCollection Properties {
			get {
				return props;
			}
		}

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public CodeDomDesignerSerializetionManager(IDesignerHost myHost)
		{
			host = myHost;
			context = new ContextStack();
			services = new ServiceContainer(host);
			services.AddService(typeof(IDesignerSerializationManager),this);
			services.AddService(typeof(IServiceContainer),this);
		}


		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='instance'>TODO - add parameter description</param>
		/// <param name='name'>TODO - add parameter description</param>
		public virtual void SetName(object instance, string name)
		{
			//System.Console.WriteLine("Setting name {0} for {1}",name,instance);
			object oldName = objectToName[instance];
			if (name == oldName as string) {
				return;
			}
			object oldInstance = nameToObject[name];
			if ((oldInstance != null) & (oldInstance != instance)) {
				throw new Exception("An object with name already exists.");
			}
			if (oldName != null) {
				nameToObject.Remove(name);
			}
			objectToName[instance] = name;
			nameToObject[name] = instance;
			Component c = instance as Component;
			if (c != null && c.Site != null) {
				c.Site.Name = name;
			}
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='errorInformation'>TODO - add parameter description</param>
		public virtual void ReportError(object errorInformation)
		{
//			IUIService uiService = (IUIService)host.GetService(typeof(IUIService));
//			uiService.ShowError("Error : " + errorInformation.ToString());
		}
		
		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='provider'>TODO - add parameter description</param>
		public virtual void RemoveSerializationProvider(IDesignerSerializationProvider provider)
		{
			//System.Console.WriteLine("removing serilization provider {0}",provider);
			providers.Remove(provider);
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='typeName'>TODO - add parameter description</param>
		public virtual System.Type GetType(string typeName)
		{
			///TODO use a TypeRessoulotionService if available
			return host.GetType(typeName);
		}
		
		/*
		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='objectType'>TODO - add parameter description</param>
		/// <param name='serializerType'>TODO - add parameter description</param>
		/// 
		public virtual object GetSerializer(Type objectType, Type serializerType)
		{
			//System.Console.WriteLine("Looking for a serializer for {0}",objectType);
			if (objectType == null)
				return null;
			object serializer = null;
			foreach (IDesignerSerializationProvider provider in providers)
			{
				serializer = provider.GetSerializer(this,null,objectType, serializerType);
				if (serializer != null)
					return serializer;
			}
			AttributeCollection attributes = TypeDescriptor.GetAttributes(objectType);
			foreach(Attribute att in attributes)
			{
				DesignerSerializerAttribute serAtt = att as DesignerSerializerAttribute;
				if (serAtt != null)
				{
					if (serAtt.SerializerBaseTypeName.StartsWith( serializerType.FullName))
					{
						Type type = GetType(serAtt.SerializerTypeName);
						return Activator.CreateInstance(type);
					}
				}
			}
			return GetSerializer(objectType.BaseType,serializerType);
		}*/
		
		public virtual object GetSerializer(Type objectType, Type serializerType)
		{
			object serializer = GetSerializerFromType(objectType, serializerType);
			if (serializer != null) {
				return serializer;
			}
			return GetSerializerFromProvider(objectType, serializerType);
		}

		object GetSerializerFromType(Type objectType, Type serializerType)
		{
			if (objectType == null) {
				return null;
			}
			
			AttributeCollection attributes = TypeDescriptor.GetAttributes(objectType);
			foreach (Attribute att in attributes) {
				DesignerSerializerAttribute serAtt = att as DesignerSerializerAttribute;
				if (serAtt != null) {
					if (serAtt.SerializerBaseTypeName.StartsWith(serializerType.FullName)) {
						Type type = GetType(serAtt.SerializerTypeName);
						return Activator.CreateInstance(type);
					}
				}
			}
			return GetSerializerFromType(objectType.BaseType,serializerType);
		}
		
		object GetSerializerFromProvider(Type objectType, Type serializerType)
		{
			if (objectType == null) {
				return null;
			}

			object serializer = null;
			foreach (IDesignerSerializationProvider provider in providers) {
				serializer = provider.GetSerializer(this,null,objectType, serializerType);
				if (serializer != null) {
					return serializer;
				}
			}
			return GetSerializerFromProvider(objectType.BaseType, serializerType);
		}
		
		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='objectType'>TODO - add parameter description</param>
		/// <param name='serializerType'>TODO - add parameter description</param>
		public virtual CodeDomSerializer GetRootSerializer(Type objectType)
		{
			if (objectType == null) {
				return null;
			}

			Type serializerType = typeof(CodeDomSerializer);
			AttributeCollection attributes = TypeDescriptor.GetAttributes(objectType);
			foreach (Attribute att in attributes) {
				RootDesignerSerializerAttribute serAtt = att as RootDesignerSerializerAttribute;

				if (serAtt != null) {
					if (serAtt.SerializerBaseTypeName.StartsWith(serializerType.FullName)) {
						Type type = GetType(serAtt.SerializerTypeName);
						return Activator.CreateInstance(type) as CodeDomSerializer;
					}
				}
			}
			return GetRootSerializer(objectType.BaseType);
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='value'>TODO - add parameter description</param>
		public virtual string GetName(object value)
		{
			//System.Console.WriteLine("Looking for name of {0}",value);
			object name = objectToName[value];
			return (string)name;
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='name'>TODO - add parameter description</param>
		public virtual object GetInstance(string name)
		{
			//System.Console.WriteLine("Lokking for instance of {0}",name);
			object o = nameToObject[name];
			if (o != null)
				return o;
			o = OnResolveName(name);
			if (o != null){
				nameToObject[name] = o;
			}
			return o;
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='type'>TODO - add parameter description</param>
		/// <param name='arguments'>TODO - add parameter description</param>
		/// <param name='name'>TODO - add parameter description</param>
		/// <param name='addToContainer'>TODO - add parameter description</param>
		public virtual object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			object o = null;
			if (arguments == null) {
				o = Activator.CreateInstance(type);
			} else {
				object[] args = new object[arguments.Count];
				arguments.CopyTo(args,0);
				o = Activator.CreateInstance(type,args);
			}
			if (o == null) {
				throw new Exception(String.Format("Can't create instance of type {0}",type));
			}
			try {
				if (name != null) {
					INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
					if (!nameCreationService.IsValidName(name)) {
						name = nameCreationService.CreateName(host.Container, o.GetType());
					}
					SetName(o, name);
				}
			} catch (Exception e) {
				Console.WriteLine("Got Exception : " + e);
			}
			if (addToContainer && name != null) {
				IComponent comp = o as IComponent;
				if (comp != null)
					host.Container.Add(comp, name);
			}
			return o;
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerSerializationManager
		///
		/// </remarks>
		/// <param name='provider'>TODO - add parameter description</param>
		public virtual void AddSerializationProvider(IDesignerSerializationProvider provider)
		{
			//System.Console.WriteLine("Adding serilization provider {0}",provider);
			providers.Add(provider);
		}

		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IServiceProvider
		///
		/// </remarks>
		/// <param name='serviceType'>TODO - add parameter description</param>
//		public virtual object GetService(Type serviceType)
//		{
//			object service = host.GetService(serviceType);
//			if (service == null)
//				System.Console.WriteLine("Manager service not found {0}",serviceType);
//
//			return service;
//		}

		public void Initialize()
		{
			while (context.Pop() != null) {}
			providers.Clear();
			nameToObject.Clear();
			objectToName.Clear();
			props = PropertyDescriptorCollection.Empty;
			services = new ServiceContainer(host);
		}


		protected virtual object OnResolveName(string name)
		{
			if (ResolveName == null) {
				return null;
			}
			ResolveNameEventArgs e = new ResolveNameEventArgs(name);
			ResolveName(this,e);
			return e.Value;
		}

		public void OnSerializationComplete()
		{
			if (SerializationComplete != null) {
				SerializationComplete(this,EventArgs.Empty);
			}
		}

#region System.IServiceProvider interface implementation
		public object GetService(System.Type serviceType)
		{
			return services.GetService(serviceType);
		}
#endregion


#region System.ComponentModel.Design.IServiceContainer interface implementation
		public void RemoveService(Type serviceType)
		{
			services.RemoveService(serviceType);
		}

		public void RemoveService(Type serviceType,bool promote)
		{
			services.RemoveService(serviceType,promote);
		}

		public void AddService(Type serviceType, object serviceInstance)
		{
			//Console.WriteLine("DesignerHost added service {0} object {1}",serviceType.Name,serviceInstance);
			services.AddService(serviceType,serviceInstance);
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			//Console.WriteLine("DesignerHost added callback {1} as service {0}",serviceType.Name ,callback);
			services.AddService(serviceType, callback);
		}

		public void AddService(Type serviceType, object serviceInstance,bool promote)
		{
			//Console.WriteLine("service {0} object promote added to DesignerHost",serviceType);
			services.AddService(serviceType, serviceInstance,promote);
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			//Console.WriteLine("service {0} callback promote added to DesignerHost",serviceType);
			services.AddService(serviceType, callback,promote);
		}
#endregion


		public event ResolveNameEventHandler ResolveName;
		public event EventHandler SerializationComplete;

	}
}
