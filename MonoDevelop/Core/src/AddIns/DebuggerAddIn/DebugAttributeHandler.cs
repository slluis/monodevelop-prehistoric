#if NET_2_0
using System;

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MonoDevelop.Debugger {
	public class DebugAttributeHandler
	{

		public void Rescan () {

		  display_by_type_name = new Hashtable ();
		  proxy_by_type_name = new Hashtable ();

			DirectoryInfo info = new DirectoryInfo ("/usr/lib/monodevelop/Debugger/");
			FileInfo[] dlls = info.GetFiles ("*.dll");

			foreach (FileInfo dll_info in dlls) {
				Assembly a = Assembly.LoadFile (dll_info.FullName);
		
				DebuggerDisplayAttribute[] display_attrs = (DebuggerDisplayAttribute[])a.GetCustomAttributes (typeof (DebuggerDisplayAttribute),
															      false);
				DebuggerTypeProxyAttribute[] proxy_attrs = (DebuggerTypeProxyAttribute[])a.GetCustomAttributes (typeof (DebuggerTypeProxyAttribute),
																false);
				if (display_attrs == null && proxy_attrs == null)
					continue;

				foreach (DebuggerDisplayAttribute da in display_attrs) {
					if (display_by_type_name.ContainsKey (da.TargetTypeName))
						continue;

					Console.WriteLine ("found DisplayAttribute of value `{0}' for type `{1}'", da.Value, da.TargetTypeName);
				}

				foreach (DebuggerTypeProxyAttribute pa in proxy_attrs) {
					if (proxy_by_type_name.ContainsKey (pa.TargetTypeName))
						continue;

					Console.WriteLine ("found ProxyTypeAttribute of type `{0}' for type `{1}'", pa.ProxyTypeName, pa.TargetTypeName);
				}
			}
		}

		public DebuggerTypeProxyAttribute GetDebuggerTypeProxyAttribute (Type t)
		{
	  		object[] attrs = t.GetCustomAttributes (typeof (DebuggerTypeProxyAttribute), false);

			if (attrs != null && attrs.Length > 0)
				return (DebuggerTypeProxyAttribute)attrs[0];

			return proxy_by_type_name[t.Name] as DebuggerTypeProxyAttribute;
		}

		public DebuggerDisplayAttribute GetDebuggerDisplayAttribute (Type t)
		{
	  		object[] attrs = t.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);

			if (attrs != null && attrs.Length > 0)
				return (DebuggerDisplayAttribute)attrs[0];

			return display_by_type_name[t.Name] as DebuggerDisplayAttribute;
		
		}

		Hashtable display_by_type_name;
		Hashtable proxy_by_type_name;
	}
}
#endif
