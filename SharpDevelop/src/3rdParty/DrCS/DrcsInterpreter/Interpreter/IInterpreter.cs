using System;
using System.IO;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for IInterpreter.
	/// </summary>
	public interface IInterpreter {
		string Interpret(string s);
		void SetUserAssemblyPath(string s); 
		void LoadUserAssembly(string assemName); 
		void LoadUserAssemblyFullPath(string assemPath); 
		void LoadSystemAssembly(string assemName);
		void AddUsedNamespace(string ns);
		
		void SetStdOut(TextWriter t);
		void SetStdErr(TextWriter t);
		void SetStdIn(TextReader t); 

		void Reinitialize();
		void Reinitialize(string userDir);
	}
}
