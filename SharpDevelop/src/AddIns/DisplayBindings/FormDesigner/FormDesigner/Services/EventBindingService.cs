// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class EventBindingService : IEventBindingService
	{
		protected EventTypeConverter eventTypeConverter = new EventTypeConverter();
		protected Hashtable components = new Hashtable();
		protected IServiceProvider serviceProvider = null;
		public IComponentChangeService changeService = null;

		public EventBindingService(IServiceProvider sp)
		{
			serviceProvider = sp;
			changeService = sp.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if (changeService != null) {
				changeService.ComponentRemoved += new ComponentEventHandler(ComponentRemoved);
				changeService.ComponentRename += new ComponentRenameEventHandler(ComponentRenamed);
			}
		}

		public string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}

		// sohuld look around in form class for compatiable methodes
		public ICollection GetCompatibleMethods(EventDescriptor e)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;
				
				if (formDesigner != null) {
					return formDesigner.GetCompatibleMethods(e);
				}
			}
			return new string[]{};
		}

		public EventDescriptor GetEvent(PropertyDescriptor property)
		{
			EventPropertyDescriptor eventProp = property as EventPropertyDescriptor;
			if (eventProp == null) {
				return null;
			}
			return eventProp.Event;
		}

		public PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events)
		{
			PropertyDescriptor[] props = new PropertyDescriptor[events.Count];
			for (int i = 0;i<events.Count;i++)
			{
				props[i] = GetEventProperty(events[i]);
			}
			return new PropertyDescriptorCollection(props);
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return new EventPropertyDescriptor(e ,this);
		}
		
		public bool ShowCode()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return false;
			}

			FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;

			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
				return true;
			}
			return false;
		}

		public bool ShowCode(int lineNumber)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return false;
			}

			FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;

			if (formDesigner != null) {
				formDesigner.ShowSourceCode(lineNumber);
				return true;
			}
			return false;
		}

		public bool ShowCode(IComponent component, EventDescriptor edesc)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null || edesc == null || edesc.Name == null || edesc.Name.Length == 0) {
				return false;
			}
			FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;
			
			if (formDesigner != null) {
				object name = ((Hashtable)components[component])[edesc.Name];
				string nameStr = name == null ? null : name.ToString();
				if (nameStr != null && nameStr.Length > 0) {
					formDesigner.ShowSourceCode(component, edesc, nameStr);
				} else {
					// TODO: Remove empty event methods.
				}
				return true;
			}
			return false;
		}

		protected void ComponentRemoved(object sender, ComponentEventArgs e)
		{
			components.Remove(e.Component);
		}

		protected void ComponentRenamed(object sender, ComponentRenameEventArgs e)
		{
		}

		protected class EventTypeConverter : TypeConverter
		{

			public override bool CanConvertFrom(ITypeDescriptorContext context,Type type)
			{
				return (type == typeof(string));
			}

			public override bool CanConvertTo(ITypeDescriptorContext context,Type type)
			{
				return (type == typeof(string));
			}

			public override object ConvertFrom(ITypeDescriptorContext context,CultureInfo culture,object value)
			{
				if (context != null)
				{
					EventPropertyDescriptor ed = context.PropertyDescriptor as EventPropertyDescriptor;
//					ed.SetValue(context.Instance,value);
					return value;
				}
				else return base.ConvertFrom(context,culture,value);

			}

			public override object ConvertTo(ITypeDescriptorContext context,CultureInfo culture,object value, Type type)
			{
				if (value != null) {
					if (value.GetType() == typeof(string)) {
						return value;
					}
				}
				if (context != null)
				{
					EventPropertyDescriptor ed = context.PropertyDescriptor as EventPropertyDescriptor;
					return ed.GetValue(context.Instance);
				}
				else return base.ConvertTo(context,culture,value,type);
			}

			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return false;
			}

			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				EventPropertyDescriptor ed = context.PropertyDescriptor as EventPropertyDescriptor;
				ICollection col = ed.eventBindingService.GetCompatibleMethods(((EventPropertyDescriptor)context.PropertyDescriptor).Event);
				return new StandardValuesCollection(col);
			}

			public override bool IsValid(ITypeDescriptorContext context, object o)
			{
				return true;
			}
		}

		protected class EventPropertyDescriptor : PropertyDescriptor
		{
			protected EventDescriptor baseDescriptor = null;
			public EventBindingService eventBindingService = null;
			
			public EventPropertyDescriptor(EventDescriptor eventDesc, EventBindingService service): base(eventDesc)
			{
				baseDescriptor = eventDesc;
				eventBindingService = service;
			}

			public EventDescriptor Event {
				get { return baseDescriptor;}
			}

			public override Type ComponentType {
				get {return Event.ComponentType;}
			}

			public override TypeConverter Converter {
				get {return eventBindingService.eventTypeConverter;}
			}

			public override bool IsReadOnly {
				get {return false;}
			}

			public override Type PropertyType {
				get { return typeof(string);}
			}

			public override bool CanResetValue(object component)
			{
				Console.WriteLine("CanResetValue");
				return true;
			}

			public override object GetValue(object component)
			{
				IDictionary events = eventBindingService.components[component] as IDictionary;
				if (events == null) {
					return null;
				}
				return events[Name];
			}

			public override void ResetValue(object component)
			{
				SetValue(component,null);
			}

			public override void SetValue(object component, object value)
			{
				if (value as string == "") {
					value = null;
				}
				
				IDictionary events = eventBindingService.components[component] as IDictionary;
				string oldValue = null;
				if (events != null) {
					oldValue = (string)events[Name];
				} else if (value != null) {
					events = new Hashtable();
					eventBindingService.components[component] = (Hashtable)events;
				}
				
				if (String.Compare(oldValue,(string)value) != 0) {
//					eventBindingService.changeService.OnComponentChanging(component,Event);
					events[Name] = value;
//					eventBindingService.changeService.OnComponentChanged(component,Event,oldValue,value);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
	}
}
