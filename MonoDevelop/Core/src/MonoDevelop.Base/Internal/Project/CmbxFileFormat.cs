//
// CmbxFileFormat.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Xml;
using MonoDevelop.Services;
using MonoDevelop.Internal.Serialization;

namespace MonoDevelop.Internal.Project
{
	public class CmbxFileFormat: IFileFormat
	{
		public string Name {
			get { return "MonoDevelop Combine"; }
		}
		
		public string GetValidFormatName (string fileName)
		{
			return Path.ChangeExtension (fileName, ".cmbx");
		}
		
		public bool CanReadFile (string file)
		{
			return string.Compare (Path.GetExtension (file), ".cmbx", true) == 0;
		}
		
		public bool CanWriteFile (object obj)
		{
			return obj is Combine;
		}
		
		public void WriteFile (string file, object node)
		{
			Combine combine = node as Combine;
			if (combine == null)
				throw new InvalidOperationException ("The provided object is not a Combine");

			StreamWriter sw = new StreamWriter (file);
			try {
				XmlTextWriter tw = new XmlTextWriter (sw);
				tw.Formatting = Formatting.Indented;
				DataSerializer serializer = new DataSerializer (Runtime.ProjectService.DataContext, file);
				CombineWriterV2 combineWriter = new CombineWriterV2 (serializer);
				combineWriter.WriteCombine (tw, combine);
			} finally {
				sw.Close ();
			}
		}
		
		public object ReadFile (string file)
		{
			XmlTextReader reader = new XmlTextReader (new StreamReader (file));
			reader.MoveToContent ();
			
			string version = reader.GetAttribute ("version");
			if (version == null) version = reader.GetAttribute ("fileversion");
			
			DataSerializer serializer = new DataSerializer (Runtime.ProjectService.DataContext, file);
			ICombineReader combineReader = null;
			
			if (version == "1.0")
				combineReader = new CombineReaderV1 (serializer);
			else if (version == "2.0")
				combineReader = new CombineReaderV2 (serializer);
			
			try {
				if (combineReader != null)
					return combineReader.ReadCombine (reader);
				else
					throw new UnknownProjectVersionException (version);
			} finally {
				reader.Close ();
			}
		}
	}
	
	interface ICombineReader {
		Combine ReadCombine (XmlReader reader);
	}
	
	class CombineReaderV1: XmlConfigurationReader, ICombineReader
	{
		Combine combine;
		string file;
		DataSerializer serializer;
		
		public CombineReaderV1 (DataSerializer serializer)
		{
			this.serializer = serializer;
			this.file = serializer.SerializationContext.BaseFile;
			combine = new Combine ();
			combine.FileName = file;
		}
		
		public Combine ReadCombine (XmlReader reader)
		{
			DataItem data = (DataItem) Read (reader);
			serializer.Deserialize (combine, data);
			
			string mdCombineAddition = Path.ChangeExtension (combine.FileName, "mdsx");
			if (File.Exists (mdCombineAddition)) {
				XmlDocument doc = new XmlDocument ();
				doc.Load (mdCombineAddition);
				string rop = doc.DocumentElement["RelativeOutputPath"].InnerText;
				if (rop != "")
					combine.OutputDirectory = Runtime.FileUtilityService.RelativeToAbsolutePath (combine.BaseDirectory, rop);
			}
			
			return combine;
		}
		
		protected override DataNode ReadChild (XmlReader reader, DataItem parent)
		{
			if (reader.LocalName == "Entries") {
				if (reader.IsEmptyElement) { reader.Skip(); return null; }
				string basePath = Path.GetDirectoryName (file);
				reader.ReadStartElement ();
				while (MoveToNextElement (reader)) {
					string nodefile = reader.GetAttribute ("filename");
					nodefile = Runtime.FileUtilityService.RelativeToAbsolutePath (basePath, nodefile);
					combine.Entries.Add ((CombineEntry) Runtime.ProjectService.ReadFile (nodefile));
					reader.Skip ();
				}
				reader.ReadEndElement ();
				return null;
			} else if (reader.LocalName == "Configurations") {
				DataItem item = base.ReadChild (reader, parent) as DataItem;
				foreach (DataNode data in item.ItemData) {
					DataItem conf = data as DataItem;
					if (conf == null) continue;
					Runtime.ProjectService.DataContext.SetTypeInfo (conf, typeof(CombineConfiguration));
				}
				return item;
			}
			
			return base.ReadChild (reader, parent);
		}
	}
	
	class CombineReaderV2: XmlConfigurationReader, ICombineReader
	{
		DataSerializer serializer;
		Combine combine = new Combine ();
		
		public CombineReaderV2 (DataSerializer serializer)
		{
			this.serializer = serializer;
			combine.FileName = serializer.SerializationContext.BaseFile;
		}
		
		public Combine ReadCombine (XmlReader reader)
		{
			DataItem data = (DataItem) Read (reader);
			serializer.Deserialize (combine, data);
			return combine;
		}
		
		protected override DataNode ReadChild (XmlReader reader, DataItem parent)
		{
			if (reader.LocalName == "Entries") {
				if (reader.IsEmptyElement) { reader.Skip(); return null; }
				string basePath = Path.GetDirectoryName (combine.FileName);
				reader.ReadStartElement ();
				while (MoveToNextElement (reader)) {
					string nodefile = reader.GetAttribute ("filename");
					nodefile = Runtime.FileUtilityService.RelativeToAbsolutePath (basePath, nodefile);
					combine.Entries.Add ((CombineEntry) Runtime.ProjectService.ReadFile (nodefile));
					reader.Skip ();
				}
				reader.ReadEndElement ();
				return null;
			}
			
			return base.ReadChild (reader, parent);
		}
	}
	
	class CombineWriterV2: XmlConfigurationWriter
	{
		Combine combine;
		DataSerializer serializer;
		
		public CombineWriterV2 (DataSerializer serializer)
		{
			this.serializer = serializer;
		}

		public void WriteCombine (XmlWriter writer, Combine combine)
		{
			this.combine = combine;
			DataItem data = (DataItem) serializer.Serialize (combine, typeof(Combine));
			Write (writer, data);
		}
		
		protected override void WriteChildren (XmlWriter writer, DataItem item)
		{
			base.WriteChildren (writer, item);

			writer.WriteStartElement ("Entries");
			foreach (CombineEntry entry in combine.Entries) {
				writer.WriteStartElement ("Entry");
				writer.WriteAttributeString ("filename", entry.RelativeFileName);
				writer.WriteEndElement ();
				Runtime.ProjectService.WriteFile (entry.FileName, entry);
			}
			writer.WriteEndElement ();
		}
	}
}
