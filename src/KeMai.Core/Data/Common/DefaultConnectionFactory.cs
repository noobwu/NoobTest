using System;
using System.Collections.Generic;
using System.Data;

namespace KeMai.Data
{
    /// <summary>
    /// Allow for mocking and unit testing by providing non-disposing 
    /// connection factory with injectable IDbCommand and IDbTransaction proxies
    /// </summary>
    public class DefaultConnectionFactory : IDbConnectionFactoryExtended
    {
        public DefaultConnectionFactory()
            : this(null, null, true) { }

        public DefaultConnectionFactory(string connectionString)
            : this(connectionString, null, true) { }

        public DefaultConnectionFactory(string connectionString, IDbDialectProvider dialectProvider)
            : this(connectionString, dialectProvider, true) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dialectProvider"></param>
        /// <param name="setGlobalDialectProvider"></param>
        public DefaultConnectionFactory(string connectionString, IDbDialectProvider dialectProvider, bool setGlobalDialectProvider)
        {
            ConnectionString = connectionString;
            AutoDisposeConnection = connectionString != ":memory:";
            this.ConnectionFilter = x => x;
            this.DialectProvider = dialectProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        public IDbDialectProvider DialectProvider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Func<IDbConnection, IDbConnection> ConnectionFilter { get; set; }

        /// <summary>
        /// Force the IDbConnection to always return this IDbCommand
        /// </summary>
        public IDbCommand AlwaysReturnCommand { get; set; }

        /// <summary>
        /// Force the IDbConnection to always return this IDbTransaction
        /// </summary>
        public IDbTransaction AlwaysReturnTransaction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<DefaultConnection> OnDispose { get; set; }

        private DefaultConnection defaultConnection;
        /// <summary>
        /// 
        /// </summary>
        private  DefaultConnection DefaultConnection
        {
            get { return defaultConnection ?? (defaultConnection = new DefaultConnection(this)); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection OpenDbConnection()
        {
            var connection = CreateDbConnection();
            connection.Open();

            return connection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection CreateDbConnection()
        {
            if (this.ConnectionString == null)
                throw new ArgumentNullException("ConnectionString", "ConnectionString must be set");

            var connection = AutoDisposeConnection
                ? new DefaultConnection(this)
                : DefaultConnection;

            return connection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual IDbConnection OpenDbConnectionString(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var connection = new DefaultConnection(this)
            {
                ConnectionString = connectionString
            };

            connection.Open();

            return connection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public virtual IDbConnection OpenDbConnectionString(string connectionString, string providerName)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            IDbDialectProvider dialectProvider;
            if (!DialectProviders.TryGetValue(providerName, out dialectProvider))
                throw new ArgumentException($"{providerName} is not a registered DialectProvider");

            var dbFactory = new DefaultConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false);

            return dbFactory.OpenDbConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedConnection"></param>
        /// <returns></returns>
        public virtual IDbConnection OpenDbConnection(string namedConnection)
        {
            DefaultConnectionFactory factory;
            if (!NamedConnections.TryGetValue(namedConnection, out factory))
                throw new KeyNotFoundException("No factory registered is named " + namedConnection);

            IDbConnection connection = factory.AutoDisposeConnection
                ? new DefaultConnection(factory)
                : factory.DefaultConnection;

            //moved setting up the ConnectionFilter to OrchardConnection.Open
            //connection = factory.ConnectionFilter(connection);
            connection.Open();

            return connection;
        }

        private static Dictionary<string, IDbDialectProvider> dialectProviders;
        public static Dictionary<string, IDbDialectProvider> DialectProviders => dialectProviders ?? (dialectProviders = new Dictionary<string, IDbDialectProvider>());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="dialectProvider"></param>
        public virtual void RegisterDialectProvider(string providerName, IDbDialectProvider dialectProvider)
        {
            DialectProviders[providerName] = dialectProvider;
        }

        private static Dictionary<string, DefaultConnectionFactory> namedConnections;
        public static Dictionary<string, DefaultConnectionFactory> NamedConnections => namedConnections ?? (namedConnections = new Dictionary<string, DefaultConnectionFactory>());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedConnection"></param>
        /// <param name="connectionString"></param>
        /// <param name="dialectProvider"></param>
        public virtual void RegisterConnection(string namedConnection, string connectionString, IDbDialectProvider dialectProvider)
        {
            RegisterConnection(namedConnection, new DefaultConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedConnection"></param>
        /// <param name="connectionFactory"></param>
        public virtual void RegisterConnection(string namedConnection, DefaultConnectionFactory connectionFactory)
        {
            NamedConnections[namedConnection] = connectionFactory;
        }
    }

    public static class OrchardConnectionFactoryExtensions
    {
        /// <summary>
        /// Alias for OpenDbConnection
        /// </summary>
        public static IDbConnection Open(this IDbConnectionFactory connectionFactory)
        {
            return connectionFactory.OpenDbConnection();
        }

        /// <summary>
        /// Alias for OpenDbConnection
        /// </summary>
        public static IDbConnection Open(this IDbConnectionFactory connectionFactory, string namedConnection)
        {
            return ((DefaultConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="namedConnection"></param>
        /// <returns></returns>
        public static IDbConnection OpenDbConnection(this IDbConnectionFactory connectionFactory, string namedConnection)
        {
            return ((DefaultConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDbConnection OpenDbConnectionString(this IDbConnectionFactory connectionFactory, string connectionString)
        {
            return ((DefaultConnectionFactory)connectionFactory).OpenDbConnectionString(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IDbConnection ToDbConnection(this IDbConnection db)
        {
            var hasDb = db as IHasDbConnection;
            return hasDb != null
                ? hasDb.DbConnection.ToDbConnection()
                : db;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        public static IDbCommand ToDbCommand(this IDbCommand dbCmd)
        {
            var hasDbCmd = dbCmd as IHasDbCommand;
            return hasDbCmd != null
                ? hasDbCmd.DbCommand.ToDbCommand()
                : dbCmd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <returns></returns>
        public static IDbTransaction ToDbTransaction(this IDbTransaction dbTrans)
        {
            var hasDbTrans = dbTrans as IHasDbTransaction;
            return hasDbTrans != null
                ? hasDbTrans.DbTransaction
                : dbTrans;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="namedConnection"></param>
        /// <param name="connectionString"></param>
        /// <param name="dialectProvider"></param>
        public static void RegisterConnection(this IDbConnectionFactory dbFactory, string namedConnection, string connectionString, IDbDialectProvider dialectProvider)
        {
            ((DefaultConnectionFactory)dbFactory).RegisterConnection(namedConnection, connectionString, dialectProvider);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="namedConnection"></param>
        /// <param name="connectionFactory"></param>
        public static void RegisterConnection(this IDbConnectionFactory dbFactory, string namedConnection, DefaultConnectionFactory connectionFactory)
        {
            ((DefaultConnectionFactory)dbFactory).RegisterConnection(namedConnection, connectionFactory);
        }
    }
}