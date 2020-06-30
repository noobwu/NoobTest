using System;
using NLog;

namespace KeMai.Logging.NLogger
{
    /// <summary>
    /// Wrapper over the NLog 2.0 beta and above logger 
    /// </summary>
    public class NLogLogger :ILogger
    {
        private readonly NLog.Logger log;

        public NLogLogger(string typeName)
        {
            log = NLog.LogManager.GetLogger(typeName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogger"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public NLogLogger(Type type)
        {
            log = NLog.LogManager.GetLogger(UseFullTypeNames ? type.FullName : type.Name);
        }

        public static bool UseFullTypeNames { get; set; }

        public bool IsDebugEnabled { get { return log.IsDebugEnabled; } }

        public bool IsInfoEnabled { get { return log.IsInfoEnabled; } }

        public bool IsWarnEnabled { get { return log.IsWarnEnabled; } }

        public bool IsErrorEnabled { get { return log.IsErrorEnabled; } }

        public bool IsFatalEnabled { get { return log.IsFatalEnabled; } }

        private static string AsString(object message)
        {
            return message?.ToString();
        }

        /// <summary>
        /// Logs a Debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            if (IsDebugEnabled)
               Log(NLog.LogLevel.Debug, AsString(message));
        }

        /// <summary>
        /// Logs a Debug message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            if (IsDebugEnabled)
               Log(NLog.LogLevel.Debug, AsString(message), exception);
        }

        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
               Log(NLog.LogLevel.Debug, format, args);
        }
        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled)
                Log(NLog.LogLevel.Debug, format, args,exception);
        }
        /// <summary>
        /// Logs a Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            if (IsErrorEnabled)
               Log(NLog.LogLevel.Error, AsString(message));
        }

        /// <summary>
        /// Logs a Error message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
               Log(NLog.LogLevel.Error, AsString(message), exception);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
               Log(NLog.LogLevel.Error, format, args);
        }
        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (IsErrorEnabled)
                Log(NLog.LogLevel.Error, format, args, exception);
        }
        /// <summary>
        /// Logs a Fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            if (IsFatalEnabled)
               Log(NLog.LogLevel.Fatal, AsString(message));
        }

        /// <summary>
        /// Logs a Fatal message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            if (IsFatalEnabled)
               Log(NLog.LogLevel.Fatal, AsString(message), exception);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled)
               Log(NLog.LogLevel.Fatal, format, args);
        }
        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (IsFatalEnabled)
                Log(NLog.LogLevel.Fatal, format, args, exception);
        }
        /// <summary>
        /// Logs an Info message 
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            if (IsInfoEnabled)
               Log(NLog.LogLevel.Info, AsString(message));
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            if (IsInfoEnabled)
               Log(NLog.LogLevel.Info, AsString(message), exception);
        }

        /// <summary>
        /// Logs an Info format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
               Log(NLog.LogLevel.Info, format, args);
        }
        /// <summary>
        /// Logs a Info format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (IsInfoEnabled)
                Log(NLog.LogLevel.Info, format, args, exception);
        }
        /// <summary>
        /// Logs a Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            if (IsWarnEnabled)
               Log(NLog.LogLevel.Warn, AsString(message));
        }

        /// <summary>
        /// Logs a Warning message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled)
               Log(NLog.LogLevel.Warn, AsString(message), exception);
        }

        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
               Log(NLog.LogLevel.Warn, format, args);
        }
        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (IsWarnEnabled)
                Log(NLog.LogLevel.Warn, format, args, exception);
        }
        public void Log(NLog.LogLevel logLevel, string message, Exception ex)
        {
            log.Log(typeof(NLogLogger), new LogEventInfo(logLevel, log.Name, null, message, null, ex));
        }

        public void Log(NLog.LogLevel logLevel, string format, params object[] args)
        {
            log.Log(typeof(NLogLogger), new LogEventInfo(logLevel, log.Name, null, format, args));
        }

        public void Log(NLog.LogLevel logLevel, string format, object[] args, Exception ex)
        {
            log.Log(typeof(NLogLogger), new LogEventInfo(logLevel, log.Name, null, format, args, ex));
        }

        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return log.IsDebugEnabled;
                case LogLevel.Info:
                    return log.IsInfoEnabled;
                case LogLevel.Warn:
                    return log.IsWarnEnabled;
                case LogLevel.Error:
                    return log.IsErrorEnabled;
                case LogLevel.Fatal:
                    return log.IsFatalEnabled;
            }
            return false;
        }
    }
}
