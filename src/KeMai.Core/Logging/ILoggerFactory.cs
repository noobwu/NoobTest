using System;

namespace KeMai.Logging {
    /// <summary>
    /// 
    /// </summary>
    public interface ILoggerFactory {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger GetLogger(Type type);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger GetLogger(string typeName);
    }
}