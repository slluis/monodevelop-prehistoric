// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts
{
	public class DesignComponentContainer : IContainer
	{
		DefaultDesignerHost host          = null;
		
		Hashtable    components    = new Hashtable();
		Hashtable    designers     = new Hashtable();
		
		IComponent          rootComponent = null;
		
		int unnamedCount = 0;
		
		public DesignComponentContainer(DefaultDesignerHost host)
		{
			this.host = host;
			host.Activated += new EventHandler(HostActivated);
		}
		
		void HostActivated(object sender, EventArgs e)
		{
			ComponentChangeService componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
			if (componentChangeService != null) {
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		
		void OnComponentRename(object sender, ComponentRenameEventArgs e)
		{
			if (components.Contains(e.OldName)) {
				components.Remove(e.OldName);
				components.Add(e.NewName, e.Component);
			}
		}
		
		
		public IDictionary Designers {
			get {
				return designers;
			}
		}
		
		public IComponent RootComponent {
			get {
				return rootComponent;
			}
		}
		
		public ComponentCollection Components {
			get {
				IComponent[] componentList = new IComponent[components.Count];
				components.Values.CopyTo(componentList, 0);
				return new ComponentCollection(componentList);
			}
		}
		
		public void Dispose()
		{
			foreach (IComponent component in components.Values) {
				component.Dispose();
			}
			components.Clear();
		}

		public bool ContainsName(string name)
		{
			return components.Contains(name);
		}
		
		public void Add(IComponent component, string name)
		{
			Console.WriteLine("ADD COMPONENT : " + component);
			if (name == null) {
				name = unnamedCount + "_unnamed";
				++unnamedCount;
			}
			
			if (ContainsName(name)) {
				throw new ArgumentException("name", "A component named " + name + " already exists in this container");
			}
			
			ISite site = new ComponentSite(host, component);
			site.Name = name;
			component.Site = site;
			
			ComponentChangeService componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
			
			if (componentChangeService != null) {
				componentChangeService.OnComponentAdding(component);
			}
			
			IDesigner designer = null;
			
			if (components.Count == 0) {
				// this is the first component. It must be the
				// "root" component and therefore it must offer
				// a root designer
				designer = TypeDescriptor.CreateDesigner(component, typeof(IRootDesigner));
				rootComponent = component;
			} else {
				designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
			}
			
			// If we got a designer, initialize it
			if (designer != null) {
				designers[component] = designer;
				designer.Initialize(component);
			}
			
			if (component is IExtenderProvider) {
				IExtenderProviderService extenderProviderService = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
				extenderProviderService.AddExtenderProvider((IExtenderProvider)component);
			}
			
			this.components[name] = component;
			
			if (componentChangeService != null) {
				componentChangeService.OnComponentAdded(component);
			}
		}
		
		public void Add(IComponent component)
		{
//			This causes a 'duplicate' SWF.Timer bug : ?!?!?,
//			appearantly the #D forms designer doesn't need this method ... but it may
//			cause bugs somewhere else to take out this. :/ 
//			this.Add(component, null);
		}
		
		public void Remove(IComponent component)
		{
			string name = null;
			ISite site  = component.Site;
			
			if (site != null) {
				name = site.Name;
			} else {
				foreach (string k in components.Keys) {
					IComponent c = components[k] as IComponent;
					if (c == component) {
						name = k;
						break;
					}
				}
			}
			
			if (name == null) {
				return;
			}
			
			ComponentChangeService componentChangeService = componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
			if (componentChangeService != null) {
				componentChangeService.OnComponentRemoving(component);
			}
			
			if (components.Contains(name)) {
				// Remove Component from Tray (ComponentTray part of System.Windows.Forms.Design)
				ComponentTray tray = host.GetService(typeof(ComponentTray)) as ComponentTray;
				if (tray != null) {
					tray.RemoveComponent(component); 
				}
				
				components.Remove(name);
				
				// remove & dispose designer
				IDesigner designer = designers[component] as IDesigner;
				if (designer != null) {
					designers.Remove(component);
					try {
						designer.Dispose();
					} catch (Exception e) {
						Console.WriteLine("Can't dispose designer " + e);
					}
				}
			}
			
			if (componentChangeService != null) {
				componentChangeService.OnComponentRemoved(component);
			}
			
			// remove site from component
			if (site != null) {
				component.Site = null;
			}
			
			
			if (component is IExtenderProvider) {
				IExtenderProviderService extenderProviderService = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
				extenderProviderService.RemoveExtenderProvider((IExtenderProvider)component);
			}
		}
		
		// ISite implementation
		class ComponentSite : ISite
		{
			ArrayList extenderProviders = new ArrayList();
			IComponent               component;
			IDesignerHost            host;
			bool                     isInDesignMode;
			string                   name;
			ServiceContainer         serviceContainer;
			
			
			public ComponentSite(IDesignerHost host, IComponent component)
			{
				this.component      = component;
				this.host           = host;
				this.isInDesignMode = true;
				serviceContainer  = new ServiceContainer(host);
				serviceContainer.AddService(typeof(IDictionaryService),new DictionaryService());
			}
				
			public IComponent Component {
				get {
					return component;
				}
			}
			
			public IContainer Container {
				get {
					return host.Container;
				}
			}
			
			public bool DesignMode {
				get {
					return isInDesignMode;
				}
			}
			
			public string Name {
				get {
					return name;
				}
				set {
					Control nameable = component as Control;
					if (nameable != null) {
						nameable.Name = value;
					}
					name = value;
				}
			}
			
			public object GetService(Type serviceType)
			{
				return serviceContainer.GetService(serviceType);
			}
		}
	}
}
