// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using MonoDevelop.AssemblyAnalyser.Rules;

namespace MonoDevelop.AssemblyAnalyser
{
	/// </summary>
	/// <summary>
	/// Description of Resolution.	
	public class Resolution : System.MarshalByRefObject
	{
		IRule  failedRule;
		string text;
		string item;
		string[,] variables;
		
		public IRule FailedRule {
			get {
				return failedRule;
			}
		}
		public string Text {
			get {
				return text;
			}
		}
		public string Item {
			get {
				return item;
			}
		}
		public string[,] Variables {
			get {
				return variables;
			}
		}
		
		public Resolution(IRule failedRule, string text, string item)
		{
			this.failedRule = failedRule;
			this.text = text;
			this.item = item;
		}
		
		public Resolution(IRule failedRule, string text, string item, string[,] variables)
		{
			this.failedRule = failedRule;
			this.text = text;
			this.item = item;
			this.variables = variables;
		}
	}
}
