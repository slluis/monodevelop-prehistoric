// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	/// <summary>
	/// This class is able to generate a GUI definition out of a XML file.
	/// </summary>
	public class XmlFormReader
	{
		IDesignerHost host;
		
		Hashtable tooltips  = new Hashtable();
		string acceptButton = null;
		string cancelButton = null;
		/// <summary>
		/// Creates a new instance of GuiXmlGenerator.
		/// </summary>
		/// <param name="customizationObject">
		/// The object to customize. (should be a control or form)
		/// This is object, because this class may be extended later.
		/// </param>
		/// <param name="fileName">
		/// The filename of the XML definition file to load.
		/// </param>
		public XmlFormReader(IDesignerHost host)
		{
			this.host = host;
		}
		
		
		/// <summary>
		/// Loads the XML definition from fileName and sets up the control.
		/// </summary>
		public object CreateObject(string xmlContent)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);
			object obj = CreateObject(doc.DocumentElement, true);
			return obj;
		}
		
		public void SetUpDesignerHost(string xmlContent)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);
			if (doc.DocumentElement.Attributes["version"] == null) {
				CreateObject(xmlContent);
			} else {
				foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
					CreateObject(el, true);
				}
			}
			
			foreach (object o in host.Container.Components) {
				if (o is ToolTip) {
					ToolTip toolTip = (ToolTip)o;
					foreach (DictionaryEntry entry in tooltips) {
						toolTip.SetToolTip((Control)entry.Key, entry.Value.ToString());
					}
					break;
				}
			}
		}
		
		/// <summary>
		/// Sets the properties of an object <code>currentObject</code> to the
		/// contents of the xml element <code>element</code>
		/// </summary>
		void SetUpObject(object currentObject, XmlElement element)
		{
			foreach (XmlNode subNode in element.ChildNodes) {
				if (subNode is XmlElement) {
					XmlElement subElement = (XmlElement)subNode;
					SetAttributes(currentObject, subElement);
				}
			}
		}
		
		/// <summary>
		/// Sets a property called propertyName in object <code>o</code> to <code>val</code>. This method performs
		/// all neccessary casts.
		/// </summary>
		void SetValue(object o, string propertyName, string val)
		{
			Console.WriteLine("Set value {0} to {1}", propertyName, val);
			try {
				PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
				if (propertyName == "AcceptButton") {
					this.acceptButton = val.Split(' ')[0];
					return;
				}
				
				if (propertyName == "CancelButton") {
					this.cancelButton = val.Split(' ')[0];
					return;
				}
				
				if (propertyName == "ToolTip") {
					tooltips[o] = val;
					return;
				}
				
				if (val.StartsWith("{") && val.EndsWith("}")) {
					val = val.Substring(1, val.Length - 2);
					object propertyObject = null;
					if (propertyInfo.CanWrite) {
						Type type = host.GetType(propertyInfo.PropertyType.FullName);
						propertyObject = type.Assembly.CreateInstance(propertyInfo.PropertyType.FullName);
					} else {
						propertyObject = propertyInfo.GetValue(o, null);
					}
					
					Regex propertySet  = new Regex(@"(?<Property>[\w]+)\s*=\s*(?<Value>[\w\d]+)", RegexOptions.Compiled);
					Match match = propertySet.Match(val);
					while (true) {
						if (!match.Success) {
							break;
						}
						SetValue(propertyObject, match.Result("${Property}"), match.Result("${Value}"));
						match = match.NextMatch();
					}
					
					if (propertyInfo.CanWrite) {
						propertyInfo.SetValue(o, propertyObject, null);
					}
					
				} else if (propertyInfo.PropertyType.IsEnum) {
					propertyInfo.SetValue(o, Enum.Parse(propertyInfo.PropertyType, val), null);
				} else if (propertyInfo.PropertyType == typeof(Color)) {
					string color = val.Substring(val.IndexOf('[') + 1).Replace("]", "");
					string[] argb = color.Split(',', '=');
					if (argb.Length > 1) {
						propertyInfo.SetValue(o, Color.FromArgb(Int32.Parse(argb[1]), Int32.Parse(argb[3]), Int32.Parse(argb[5]), Int32.Parse(argb[7])), null);
					} else {
						propertyInfo.SetValue(o, Color.FromName(color), null);
					}
				} else if (propertyInfo.PropertyType == typeof(Font)) {
					string[] font = val.Split(',', '=');
					propertyInfo.SetValue(o, new Font(font[1], Int32.Parse(font[3])), null);
				} else if (propertyInfo.PropertyType == typeof(System.Windows.Forms.Cursor)) {
					string[] cursor = val.Split('[', ']', ' ', ':');
					PropertyInfo cursorProperty = typeof(System.Windows.Forms.Cursors).GetProperty(cursor[3]);
					if (cursorProperty != null) {
						propertyInfo.SetValue(o, cursorProperty.GetValue(null, null), null);
					}
				} else {
					propertyInfo.SetValue(o, Convert.ChangeType(val, propertyInfo.PropertyType), null);
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				throw new ApplicationException("error while setting property " + propertyName + " of object "+ o.ToString() + " to value '" + val+ "'");
			}
		}
		
		/// <summary>
		/// Sets all properties in the object <code>o</code> to the xml element <code>el</code> defined properties.
		/// </summary>
		void SetAttributes(object o, XmlElement el)
		{
			if (el.Attributes["value"] != null) {
				string val = el.Attributes["value"].InnerText;
				try {
					SetValue(o, el.Name, val);
				} catch (Exception) {}
			} else {
				PropertyInfo propertyInfo = o.GetType().GetProperty(el.Name);
				object pv = propertyInfo.GetValue(o, null);
				if (pv is IList) {
					foreach (XmlNode subNode in el.ChildNodes) {
						if (subNode is XmlElement){
							XmlElement subElement = (XmlElement)subNode;
							object collectionObject = CreateObject(subElement, false);
							
							if (collectionObject != null) {
								try {
									((IList)pv).Add(collectionObject);
								} catch (Exception e) {
									Console.WriteLine("Exception while adding to a collection:" + e.ToString());
								}
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Creates a new instance of object name. It tries to resolve name in <code>System.Windows.Forms</code>,
		/// <code>System.Drawing</code> and <code>System namespace</code>.
		/// </summary>
		object CreateObject(XmlElement objectElement, bool suspend)
		{
			Type type = host.GetType(objectElement.Name);
			
			if (objectElement.Attributes["value"] != null) {
				try {
					return System.Convert.ChangeType(objectElement.Attributes["value"].InnerText, type);
				} catch (Exception) {}
			}
			
			if (type == null) {
				return null;
			}
			
			object newObject = type.Assembly.CreateInstance(type.FullName);
			
			string componentName = null;
			if (objectElement["Name"] != null &&
			    objectElement["Name"].Attributes["value"] != null) {
			    componentName = objectElement["Name"].Attributes["value"].InnerText;
			}
			
			if (newObject is IComponent) {
				newObject = host.CreateComponent(newObject.GetType(), componentName);
			}
			
			bool hasSuspended = false;
			if (suspend && newObject is ContainerControl) {
				hasSuspended = true;
				((Control)newObject).SuspendLayout();
			}
			
			SetUpObject(newObject, objectElement);
			
			if (acceptButton != null && newObject is Form) {
				((Form)newObject).AcceptButton = (IButtonControl)host.Container.Components[acceptButton];
				acceptButton = null;
			}
			
			if (cancelButton != null && newObject is Form) {
				((Form)newObject).CancelButton = (IButtonControl)host.Container.Components[cancelButton];
				cancelButton = null;
			}
			
			if (hasSuspended) {
				((Control)newObject).ResumeLayout(false);
			}
			
			return newObject;
		}
	}
}
