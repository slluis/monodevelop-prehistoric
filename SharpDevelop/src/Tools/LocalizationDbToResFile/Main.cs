// Main.cs
// Copyright (c) 2001 Mike Krueger
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace Assemble {
	
	/// <summary>
	/// This tool is written especially for SharpDevelop to translate the 
	/// database that containes the localization information to resasm files.
	/// Resasm compiles these files to resource files which are used for sharpdevelop.
	/// </summary>
	class MainClass
	{
		static OleDbConnection myConnection;
		
		/// <remarks>
		/// Open the database connection (LocalizeDb.mdb must exists
		/// in the Application.StartupPath)
		/// </remarks>
		static void Open()
		{
			string connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + 
			                    Application.StartupPath + 
			                    Path.DirectorySeparatorChar + "LocalizeDb.mdb;";
			myConnection = new OleDbConnection(connection);
			myConnection.Open();
		}
		
		/// <remarks>
		/// Parses a string, if it has " or \n sequences in it
		/// and puts them into the string as backslash code sequences
		/// </remarks>
		static string ConvertIllegalChars(string str)
		{
			StringBuilder newString = new StringBuilder();
			for (int i = 0; i < str.Length; ++i) {
				switch (str[i]) {
					case '\r':
						break;
					case '\n':
						newString.Append("\\n");
						break;
					case '"':
						newString.Append("\\\"");
						break;
					case '\\':
						newString.Append("\\\\");
						break;
					default:
						newString.Append(str[i]);
						break;
				}
			}
			return newString.ToString();
		}
		
		public static void Main(string[] args)
		{
			Open();
			string lang = "PrimaryResLangValue";
			StreamWriter writer = null;
			
			// gets the /F: parameter for the filename
			// gets the /T: parameter for the language to extract
			foreach (string param in args) {
				string par = param;
				if (par.StartsWith("/F:")) {
					par = par.Substring(3);
					writer = new StreamWriter(par, false, new UTF8Encoding());;
				}
				if (par.StartsWith("/T:")) {
					par = par.Substring(3);
					lang = par;
				}
			}
			
			// now select all database entries and write
			// the resasm file (if no /F: is specified it prints to stdout)
			OleDbCommand    myOleDbCommand = new OleDbCommand("SELECT * FROM Localization", myConnection);
			OleDbDataReader reader = myOleDbCommand.ExecuteReader();
			while (reader.Read()) {
				string val = ConvertIllegalChars(reader[lang].ToString()).Trim();
				if (val.Length > 0) {
					string str = reader["ResourceName"].ToString() + " = \"" + val + "\"";
					if (writer == null) {
						Console.WriteLine(str);
					} else {
						writer.WriteLine(str);
					}
				}
			} 
			reader.Close();
			if (writer != null) {
				writer.Close();
			}
			myConnection.Close();		
		}
	}
}
