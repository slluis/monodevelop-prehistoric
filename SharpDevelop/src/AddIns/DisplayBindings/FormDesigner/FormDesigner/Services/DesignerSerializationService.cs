// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DesignerSerializationService : IDesignerSerializationService
	{
		IDesignerHost host = null;
		
		public DesignerSerializationService(IDesignerHost host)
		{
			this.host = host;
		}
		
		public ICollection Deserialize(object serializationData)
		{
			ArrayList elements = new ArrayList();
			foreach (string xmlContent in (ArrayList)serializationData) {
				elements.Add(new XmlFormReader(host).CreateObject(xmlContent));
			}
			return elements;
		}
		
		public object Serialize(ICollection objectCollection)
		{
			ArrayList elements = new ArrayList();
			foreach (object o in objectCollection) {
				StringWriter writer = new StringWriter();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml("<" + o.GetType().FullName + "/>");
				XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), o);
				foreach (XmlNode node in el.ChildNodes) {
					doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
				}
				doc.Save(writer);
				elements.Add(writer.ToString());
			}
			return elements;
		}
	}
}
