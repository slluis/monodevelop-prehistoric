// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionXmlView : UserControl
	{
		private Button      saveButton = new Button();
		private CheckBox    exportEvents = new CheckBox();
		private CheckBox    exportFields = new CheckBox();
		private CheckBox    exportMethods = new CheckBox();
		private CheckBox    exportProperties = new CheckBox();
		private Label       captionLabel = new Label();
		
		private ReflectionNode SelectedNode;
		private XmlTextWriter writer;
		
		ReflectionTree tree;
		
		public ReflectionXmlView(ReflectionTree _tree)
		{
			tree = _tree;
			
			captionLabel.Location = new Point(5, 0);
			captionLabel.Text = tree.ress.GetString("ObjectBrowser.XML.Desc");
			captionLabel.Size = new Size(300, 25);
			
			exportEvents.Location = new Point(15, 40);
			exportEvents.Text = tree.ress.GetString("ObjectBrowser.XML.ExpEvt");
			exportEvents.Checked = true;
			exportEvents.Width = 300;
			
			exportFields.Location = new Point(15, 65);
			exportFields.Text = tree.ress.GetString("ObjectBrowser.XML.ExpFld");
			exportFields.Checked = true;
			exportFields.Width = 300;
			
			exportMethods.Location = new Point(15, 90);
			exportMethods.Text = tree.ress.GetString("ObjectBrowser.XML.ExpMeth");
			exportMethods.Checked = true;
			exportMethods.Width = 300;
			
			exportProperties.Location = new Point(15, 115);
			exportProperties.Width = 300;
			exportProperties.Text = tree.ress.GetString("ObjectBrowser.XML.ExpProp");
			exportProperties.Checked = true;
			
			saveButton.Text = tree.ress.GetString("ObjectBrowser.XML.Save");
			saveButton.Location = new Point(5, 160);
			saveButton.Enabled = false;
			saveButton.Click += new EventHandler(saveButton_Clicked);
			
			Dock = DockStyle.Fill;
			Controls.AddRange(new Control[] { 
				              captionLabel,
							  saveButton,
			                  exportEvents,
			                  exportFields,
			                  exportMethods,
			                  exportProperties});
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
		}
		
		void saveButton_Clicked(object sender, System.EventArgs e) {
			
			SaveFileDialog fdialog = new SaveFileDialog();
			fdialog.Filter = tree.ress.GetString("ObjectBrowser.Filters.XML") + "|*.xml";
			DialogResult result = fdialog.ShowDialog();
			
			if(result != DialogResult.Cancel) {
				
				if (SelectedNode.Attribute is Type) {
					writeStart(fdialog.FileName, ((Type)SelectedNode.Attribute).Assembly.FullName);
					exportClass((Type)SelectedNode.Attribute);
				} else if (SelectedNode.Attribute is Assembly) {
					writeStart(fdialog.FileName, ((Assembly)SelectedNode.Attribute).FullName);
					foreach (Type type in ((Assembly)SelectedNode.Attribute).GetTypes()) {
						if(type.ToString().IndexOf("PrivateImplementationDetails") == -1) {					
							exportClass(type);
						}
					}
				}
				writeEnd();
			}
		}
		
		string GetScope(Type type)
		{
			string retval;
			
			if (type.IsPublic)
				retval = "Public ";
			else if (type.IsNestedPublic)
				retval = "Public ";
			else if (type.IsNestedPrivate)
				retval = "Private ";
			else if (type.IsNestedFamily)
				retval = "Protected ";
			else
				retval = "Internal ";
			return retval;
		}
		
		string GetScope(FieldInfo field)
		{
			string retval;
			
			if (field.IsPublic)
				retval = "Public ";
			else
				retval = "Private ";
			return retval;
		}
		
		string GetScope(MethodInfo member)
		{
			string retval;
			
			if (member.IsPublic)
				retval = "Public ";
			else
				retval = "Private ";
			return retval;
		}
		
		void writeStart(string filename, string assemblyname) {
			try {
				writer = new XmlTextWriter(filename ,new System.Text.ASCIIEncoding());
			} catch (Exception e) {
				System.Windows.Forms.MessageBox.Show(e.Message);
				return;
			}
			
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 3;
			
			writer.WriteRaw("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
			writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"xml2html.xsl\" ?>");
			
			writer.WriteStartElement("assembly");
			writer.WriteAttributeString("name", assemblyname);
		}
		
		void writeEnd() {
			writer.WriteEndElement();
			writer.Flush();
			writer.Close();
			writer = null;
		}
		
		void exportClass(Type type) {
			writer.WriteStartElement("class");
			writer.WriteAttributeString("name", type.Name);
			writer.WriteAttributeString("scope", GetScope(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			
			// events
			if(exportEvents.Checked) {
				writer.WriteStartElement("events");
				foreach(EventInfo event_ in type.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
					if(event_.DeclaringType == type) {
						writer.WriteStartElement("event");
						writer.WriteAttributeString("name", event_.Name);
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			
			// fields
			if(exportFields.Checked) {
				writer.WriteStartElement("fields");
				foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if(field.DeclaringType == type) {
						writer.WriteStartElement("field");
						writer.WriteAttributeString("name", field.Name);
						writer.WriteAttributeString("type", field.FieldType.ToString());
						writer.WriteAttributeString("scope", GetScope(field));
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			
			// methods
			if(exportMethods.Checked) {
				writer.WriteStartElement("methods");
				foreach(MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
					if(! method.IsSpecialName) {
						if(method.DeclaringType == type) {
							
							writer.WriteStartElement("method");
							writer.WriteAttributeString("name", method.Name);
							writer.WriteAttributeString("scope", GetScope(method));
							writer.WriteAttributeString("type", method.ReturnType.ToString());
							
							WriteParameters(writer, method);
							writer.WriteEndElement();
						}
					}
				}
				writer.WriteEndElement();
			}
			
			// properties
			if(exportProperties.Checked) {
				writer.WriteStartElement("properties");
				foreach(PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
					if(property.DeclaringType == type) {
						writer.WriteStartElement("property");
						writer.WriteAttributeString("name", property.Name);
						writer.WriteAttributeString("type", property.PropertyType.ToString());
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
		void WriteParameters(XmlTextWriter writer, MethodBase member) {
			foreach(ParameterInfo param in member.GetParameters()) {
				writer.WriteStartElement("param");
				writer.WriteAttributeString("name", param.Name);
				writer.WriteAttributeString("type", param.ParameterType.ToString());
				writer.WriteEndElement();
			}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			SelectedNode = (ReflectionNode)e.Node;
			saveButton.Enabled = (SelectedNode.Attribute is Type || SelectedNode.Attribute is Assembly);
		}
	}
}
