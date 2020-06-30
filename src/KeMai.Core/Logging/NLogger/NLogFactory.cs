using NLog.Config;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace KeMai.Logging.NLogger
{
    /// <summary>
    /// ILogFactory that creates an NLog ILog logger
    /// </summary>
    public class NLogFactory : ILoggerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogFactory"/> class.
        /// </summary>
        public NLogFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogFactory"/> class.
        /// </summary>
        /// <param name="configFile">The NLog net configuration file to load and watch. If not found configures from App.Config.</param>
        public NLogFactory(string configFile)
        {
            var file = GetConfigFile(configFile);
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
        }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ILogger GetLogger(Type type)
        {
            return new NLogLogger(type);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public ILogger GetLogger(string typeName)
        {
            return new NLogLogger(typeName);
        }

        /// <summary>
        ///   Gets the configuration file.
        /// </summary>
        /// <param name = "fileName">i.e. log4net.config</param>
        /// <returns></returns>
        private  FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;

            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {

                result = new FileInfo(Utils.GetMapPath(fileName));
            }

            return result;
        }
		
		
        /// <summary>
        /// Gets the singleton 
        /// <param name="configPath"></param>
        /// </summary>
        public static NLogFactory GetInstance(string configPath= "~/Config/NLog.config")
        {
            if (Singleton<NLogFactory>.Instance == null)
            {
                Initialize(configPath);
            }
            return Singleton<NLogFactory>.Instance;
        }

        /// <summary>
        /// Initializes a static instance.
        /// </summary>
        /// <param name="configPath"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static NLogFactory Initialize(string configPath)
        {
            if (Singleton<NLogFactory>.Instance == null)
            {
                Singleton<NLogFactory>.Instance = new NLogFactory(configPath);
            }
            return Singleton<NLogFactory>.Instance;
        }
    }
}
