
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

		ILog GetLogger()
		{
			return LogManager.GetLogger(typeof(ILoggingService));
		}

		public bool IsDebugEnabled {
			get {
				return GetLogger().IsDebugEnabled;
			}
		}

		public bool IsInfoEnabled {
			get {
				return GetLogger().IsInfoEnabled;
			}
		}

		public bool IsWarnEnabled {
			get {
				return GetLogger().IsWarnEnabled;
			}
		}

		public bool IsErrorEnabled {
			get {
				return GetLogger().IsErrorEnabled;
			}
		}

		public bool IsFatalEnabled {
			get {
				return GetLogger().IsFatalEnabled;
			}
		}

		public void Debug (object message)
		{
			GetLogger().Debug (message);
			OnLogAppended ("Debug", message.ToString());
		}

		public void Info (object message)
		{
			GetLogger().Info (message);
			OnLogAppended ("Info", message.ToString());
		}

		public void Warn (object message)
		{
			GetLogger().Warn (message);
			OnLogAppended ("Warn", message.ToString());
		}

		public void Error (object message)
		{
			GetLogger().Error (message);
		}

		public void Fatal (object message)
		{
			GetLogger().Fatal (message);
			OnLogAppended ("Fatal", message.ToString());
		}

		public void Debug (object message, Exception t)
		{
			GetLogger().Debug (message, t);
			OnLogAppended ("Debug", message + t.ToString());
		}
		
		public void Info (object message, Exception t)
		{
			GetLogger().Info (message, t);
			OnLogAppended ("Info", message + t.ToString());
		}
		
		public void Warn (object message, Exception t)
		{
			GetLogger().Warn (message, t);
			OnLogAppended ("Warn", message + t.ToString());
		}

		public void Error (object message, Exception t)
		{
			GetLogger().Error (message, t);
			OnLogAppended ("Error", message + t.ToString());
		}
		
		public void Fatal (object message, Exception t)
		{
			GetLogger().Fatal (message, t);
			OnLogAppended ("Fatal", message + t.ToString());
		}

		public void DebugFormat (string format, params object[] args)
		{
			GetLogger().DebugFormat (format, args);
			OnLogAppended ("Debug", String.Format(format, args));
		}
		
		public void InfoFormat (string format, params object[] args)
		{
			GetLogger().InfoFormat (format, args);
			OnLogAppended ("Info", String.Format(format, args));
		}
		
		public void WarnFormat (string format, params object[] args)
		{
			GetLogger().WarnFormat (format, args);
			OnLogAppended ("Warn", String.Format(format, args));
		}
		
		public void ErrorFormat (string format, params object[] args)
		{
			GetLogger().ErrorFormat (format, args);
			OnLogAppended ("Error", String.Format(format, args));
		}
		
		public void FatalFormat (string format, params object[] args)
		{
			GetLogger().FatalFormat (format, args);
			OnLogAppended ("Fatal", String.Format(format, args));
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger().DebugFormat (provider, format, args);
			OnLogAppended ("Debug", String.Format(provider, format, args));
		}
		
		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger().InfoFormat (provider, format, args);
			OnLogAppended ("Info", String.Format(provider, format, args));
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger().WarnFormat (provider, format, args);
			OnLogAppended ("Warn", String.Format(provider, format, args));
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger().ErrorFormat (provider, format, args);
			OnLogAppended ("Error", String.Format(provider, format, args));
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			GetLogger().FatalFormat (provider, format, args);
			OnLogAppended ("Fatal", String.Format(provider, format, args));
		}

		public void OnLogAppended(string level, string message)
		{
			if (LogAppended != null) {
				LogAppendedArgs args = new LogAppendedArgs();
				args.Level = level;
				args.Message = message;
				args.Timestamp = DateTime.Now;
				LogAppended(this, args);
			}
		}
	}
}
