using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using KeMai.Logging.NLogger;
namespace KeMai.Logging
{
    /// <summary>
    /// Logging API for this library. You can inject your own implementation otherwise
    /// will use the DebugLogFactory to write to System.Diagnostics.Debug
    /// </summary>
    public class LoggerManager
    {
        private static ILoggerFactory logFactory;

        /// <summary>
        /// Gets or sets the log factory.
        /// Use this to override the factory that is used to create loggers
        /// </summary>
        public static ILoggerFactory LogFactory
        {
            get
            {
                //return logFactory ?? new NullLoggerFactory();
				return logFactory ?? NLogFactory.GetInstance("~/Config/NLog.config");
            }
            set { logFactory = value; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public static ILogger GetLogger(Type type)
        {
            return LogFactory.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public static ILogger GetLogger(string typeName)
        {
            return LogFactory.GetLogger(typeName);
        }
        /// <summary>
        /// Gets the logger with the name of the current class.  
        /// </summary>
        /// <returns>The logger.</returns>
        /// <remarks>This is a slow-running method. 
        /// Make sure you're not doing this in a loop.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ILogger GetCurrentClassLogger()
        {
            return LogFactory.GetLogger(GetClassFullName());
        }
        /// <summary>
        /// Gets the fully qualified name of the class invoking the LogManager, including the 
        /// namespace but not the assembly.    
        /// </summary>
        private static string GetClassFullName()
        {
            string className;
            Type declaringType;
            int framesToSkip = 2;

            do
            {
#if SILVERLIGHT
                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
#else
                StackFrame frame = new StackFrame(framesToSkip, false);
#endif
                MethodBase method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }
    }
}
