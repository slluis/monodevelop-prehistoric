using System;
using System.Runtime.InteropServices;

public class GacInstaller
{
	public static string InstallAssembly(string strAssemblyPath, string strOptions)
	{
		string strRuntimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
		string strGacUtil = strRuntimeDirectory + "gacutil.exe ";
		
		// add additional escaping to the assembly path
		string strArguments =  "/i \"" + strAssemblyPath + "\" " + strOptions;
		string strCmd = strGacUtil + strArguments;
		
		string strOutput = HelperFunctions.ExecuteCmdLineApp(strCmd);
		return strOutput;
	}
}
