
using System;

using MonoDevelop.Core.Services;

using log4net;

namespace MonoDevelop.Services
{
	public class DefaultLoggingService : AbstractService, ILoggingService
	{
		public event LogAppendedHandler LogAppended;

		public DefaultLoggingService()
		{
			log4net.Config.XmlConfigurator.Configure();
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
		}
		
		ILog GetLogger(System.Type type)
		{
			return LogManager.GetLogger(type);
		}

		ILog GetLogger()
		{
			return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}

		System.Reflection.MethodBase GetCallingMethod()
		{
			System.Diagnostics.StackTrace
					trace = new System.Diagnostics.StackTrace(true);
			return trace.GetFrame(2).GetMethod();
		}

		public bool IsDebugEnabled {
			get {
				return GetLogger(GetCallingMethod().DeclaringType).IsDebugEnabled;
			}
		}

		public bool IsInfoEnabled {
			get {
				return GetLogger(GetCallingMethod().DeclaringType).IsInfoEnabled;
			}
		}

		public bool IsWarnEnabled {
			get {
				return GetLogger(GetCallingMethod().DeclaringType).IsWarnEnabled;
			}
		}

		public bool IsErrorEnabled {
			get {
				return GetLogger(GetCallingMethod().DeclaringType).IsErrorEnabled;
			}
		}

		public bool IsFatalEnabled {
			get {
				return GetLogger(GetCallingMethod().DeclaringType).IsFatalEnabled;
			}
		}

		public void Debug (object message)
		{
			System.Type type = GetCallingMethod().DeclaringType;
			GetLogger(type).Debug (message);
			OnLogAppended ("Debug", type.ToString(), message.ToString());
		}

		public void Info (object message)
		{
			System.Type type = GetCallingMethod().DeclaringType;
			GetLogger(type).Info (message);
			OnLogAppended ("Info", type.ToString(), message.ToString());
		}

		public void Warn (object message)
		{
			GetLogger(GetCallingMethod().DeclaringType).Warn (message);
		}

		public void Error (object message)
		{
			GetLogger(GetCallingMethod().DeclaringType).Error (message);
		}

		public void Fatal (object message)
		{
			GetLogger(GetCallingMethod().DeclaringType).Fatal (message);
		}

		public void Debug (object message, Exception t)
		{
			GetLogger(GetCallingMethod().DeclaringType).Debug (message, t);
		}
		
		public void Info (object message, Exception t)
		{
			GetLogger(GetCallingMethod().DeclaringType).Info (message, t);
		}
		
		public void Warn (object message, Exception t)
		{
			GetLogger(GetCallingMethod().DeclaringType).Warn (message, t);
		}

		public void Error (object message, Exception t)
		{
			GetLogger(GetCallingMethod().DeclaringType).Error (message, t);
		}
		
		public void Fatal (object message, Exception t)
		{
			GetLogger(GetCallingMethod().DeclaringType).Fatal (message, t);
		}

		public void DebugFormat (string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).DebugFormat (format, args);
		}
		
		public void InfoFormat (string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).InfoFormat (format, args);
		}
		
		public void WarnFormat (string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).WarnFormat (format, args);
		}
		
		public void ErrorFormat (string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).ErrorFormat (format, args);
		}
		
		public void FatalFormat (string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).FatalFormat (format, args);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).DebugFormat (provider, format, args);
		}
		
		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).InfoFormat (provider, format, args);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).WarnFormat (provider, format, args);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).ErrorFormat (provider, format, args);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger(GetCallingMethod().DeclaringType).FatalFormat (provider, format, args);
		}

		public void OnLogAppended(string level, string category, string message)
		{
			if (LogAppended != null) {
				LogAppendedArgs args = new LogAppendedArgs();
				args.Level = level;
				args.Category = category;
				args.Message = message;
				args.Timestamp = DateTime.Now;
				LogAppended(this, args);
			}
		}
	}
}
