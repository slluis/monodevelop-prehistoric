using System;
using System.CodeDom.Compiler;
using System.Text;
using System.IO;

public class HelperFunctions
{
	public static string ExecuteCmdLineApp(string strCmd)
	{
	  string output = "";
	  string error  = "";
	
	  TempFileCollection tf = new TempFileCollection();
	  Executor.ExecWaitWithCapture(strCmd, tf, ref output, ref error);
	
	  StreamReader sr = File.OpenText(output);
	  StringBuilder strBuilder = new StringBuilder();
	  string strLine = null;
	
	  while (null != (strLine = sr.ReadLine()))
	  {
	    if ("" != strLine)
	    {
	      strBuilder.Append(strLine);
	      strBuilder.Append("\r\n");
	    }
	  }
	  sr.Close();
	
	  File.Delete(output);
	  File.Delete(error);
	
	  return strBuilder.ToString();
	}
}
