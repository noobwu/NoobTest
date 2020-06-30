using KeMai.Data;
using System;
using System.Data;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public class OrmLiteConnectionFactory : DefaultConnectionFactory
    {
        private OrmLiteConnection ormLiteConnection;
        /// <summary>
        /// 
        /// </summary>
        private OrmLiteConnection OrmLiteConnection
        {
            get { return ormLiteConnection ?? (ormLiteConnection = new OrmLiteConnection(this)); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dialectProvider"></param>
        public OrmLiteConnectionFactory(string connectionString, IOrmLiteDialectProvider dialectProvider)
            : this(connectionString, dialectProvider, true) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dialectProvider"></param>
        /// <param name="setGlobalDialectProvider"></param>
        public OrmLiteConnectionFactory(string connectionString, IOrmLiteDialectProvider dialectProvider, bool setGlobalDialectProvider)
            : base(connectionString, dialectProvider, setGlobalDialectProvider) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbConnection CreateDbConnection()
        {
            if (this.ConnectionString == null)
                throw new ArgumentNullException("ConnectionString", "ConnectionString must be set");

            var connection = AutoDisposeConnection
                ? new OrmLiteConnection(this)
                : OrmLiteConnection;

            return connection;
        }

    }
}
