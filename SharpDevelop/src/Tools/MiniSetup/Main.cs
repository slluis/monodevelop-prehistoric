using System;
using System.Text;
using System.Reflection;
using System.IO;

class MainClass
{
	public static void Main(string[] args)
	{
		StringBuilder strOutput = new StringBuilder();
		
		// find the current directory; note that we assume to be located in the \bin directory of 
		// the SharpDevelop installation
		string strCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//		
		/*
		// install the #unit assembly into the global assembly cache
		Console.WriteLine("Installing the #unit assembly into the global assembly cache\r\n");
		strOutput.Append(GacInstaller.InstallAssembly(strCurrentDirectory + "\\ICSharpCode.SharpUnit.dll", ""));
		strOutput.Append("\r\n");
		
		// install the #ziplib assembly into the global assembly cache
		Console.WriteLine("Installing the #ziplib assembly into the global assembly cache\r\n");
		strOutput.Append(GacInstaller.InstallAssembly(strCurrentDirectory + "\\ICSharpCode.SharpZipLib.dll", ""));
		strOutput.Append("\r\n");
		*/
		
		// generate our help system - we can do that only on the target system
		string strHelpGenApp = Path.Combine(strCurrentDirectory, "HelpConverter.exe");
		Console.WriteLine("Building SharpDevelop help index...\r\n");
		Console.WriteLine("This might take a minute or two, please be patient\r\n\r\n\r\n");
		Console.WriteLine("Building SharpDevelop help index");
		Console.WriteLine("Building .NET Framework Reference help index");
		Console.WriteLine("Building DirectX 9 help index (if installed)");
		strOutput.Append(HelperFunctions.ExecuteCmdLineApp(strHelpGenApp));
		
		// write a setup log so the SharpDevelop team can investigate setup failures
		StreamWriter sw = File.CreateText("MiniSetup.log");
		sw.Write(strOutput.ToString());
		sw.Flush();
		sw.Close();
		
		Console.WriteLine("\r\nMini Setup has completed - details have been logged to MiniSetup.log");
	}
}
