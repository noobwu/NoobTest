using System;

namespace KeMai.Logging {
    /// <summary>
    /// 
    /// </summary>
    public class NullLoggerFactory : ILoggerFactory {

        public ILogger Create(Type type)
        {
            return NullLogger.Instance;
        }
        public ILogger Create(string typeName)
        {
            return NullLogger.Instance;
        }
        public ILogger GetLogger(Type type) {
            return NullLogger.Instance;
        }
        public ILogger GetLogger(string typeName)
        {
            return NullLogger.Instance;
        }
    }
}