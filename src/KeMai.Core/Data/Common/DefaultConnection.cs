using System.Data;
using System.Data.Common;

namespace KeMai.Data
{
    /// <summary>
    /// Wrapper IDbConnection class to allow for connection sharing, mocking, etc.
    /// </summary>
    public class DefaultConnection
        : IDbConnection, IHasDbConnection, IHasDbTransaction, ISetDbTransaction
    {
        public readonly DefaultConnectionFactory Factory;
        public IDbTransaction Transaction { get; set; }
        public IDbTransaction DbTransaction => Transaction;
        private IDbConnection dbConnection;

        public IDbDialectProvider DialectProvider { get; set; }
        public string LastCommandText { get; set; }
        public int? CommandTimeout { get; set; }

        public DefaultConnection(DefaultConnectionFactory factory)
        {
            this.Factory = factory;
            this.DialectProvider = factory.DialectProvider;
            this.connectionString = factory.ConnectionString;
        }

        public IDbConnection DbConnection => dbConnection ?? (dbConnection = ToDbConnection(ConnectionString,Factory.DialectProvider));

        public void Dispose()
        {
            Factory.OnDispose?.Invoke(this);
            if (!Factory.AutoDisposeConnection) return;

            DbConnection.Dispose();
            dbConnection = null;
        }

        /// <summary>
        /// 开始数据库事务。
        /// </summary>
        /// <returns> 表示新事务的对象。</returns>
        public IDbTransaction BeginTransaction()
        {
            if (Factory.AlwaysReturnTransaction != null)
                return Factory.AlwaysReturnTransaction;

            return DbConnection.BeginTransaction();
        }

        /// <summary>
        ///  以指定的 System.Data.IsolationLevel 值开始一个数据库事务。
        /// </summary>
        /// <param name="isolationLevel">System.Data.IsolationLevel 值之一。</param>
        /// <returns> 表示新事务的对象。</returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (Factory.AlwaysReturnTransaction != null)
                return Factory.AlwaysReturnTransaction;

            return DbConnection.BeginTransaction(isolationLevel);
        }

        public void Close()
        {
            DbConnection.Close();
        }
        /// <summary>
        /// 为打开的 Connection 对象更改当前数据库。
        /// </summary>
        /// <param name="databaseName"> 要代替当前数据库进行使用的数据库的名称。</param>
        public void ChangeDatabase(string databaseName)
        {
            DbConnection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 创建并返回一个与该连接相关联的 Command 对象。
        /// </summary>
        /// <returns>一个与该连接相关联的 Command 对象。</returns>
        public IDbCommand CreateCommand()
        {
            if (Factory.AlwaysReturnCommand != null)
                return Factory.AlwaysReturnCommand;

            var cmd = DbConnection.CreateCommand();

            return cmd;
        }

        /// <summary>
        ///  打开一个数据库连接，其设置由提供程序特定的 Connection 对象的 ConnectionString 属性指定。
        /// </summary>
        public void Open()
        {
            if (DbConnection.State == ConnectionState.Broken)
                DbConnection.Close();

            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
                //so the internal connection is wrapped for example by miniprofiler
                if (Factory.ConnectionFilter != null)
                    dbConnection = Factory.ConnectionFilter(dbConnection);
            }
        }

        private string connectionString;
        /// <summary>
        ///  获取或设置用于打开数据库的字符串。
        ///  包含连接设置的字符串。
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString ?? Factory.ConnectionString; }
            set { connectionString = value; }
        }
        /// <summary>
        /// 获取在尝试建立连接时终止尝试并生成错误之前所等待的时间。
        ///  等待连接打开的时间（以秒为单位）。默认值为 15 秒。
        /// </summary>
        public int ConnectionTimeout => DbConnection.ConnectionTimeout;
        /// <summary>
        /// 获取当前数据库或连接打开后要使用的数据库的名称。
        ///  当前数据库的名称或在连接打开后要使用的数据库的名称。默认值为空字符串。
        /// </summary>
        public string Database => DbConnection.Database;
        /// <summary>
        ///  获取连接的当前状态。
        ///   System.Data.ConnectionState 值之一。
        /// </summary>
        public ConnectionState State => DbConnection.State;
        /// <summary>
        /// 
        /// </summary>
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConn"></param>

        public static explicit operator DbConnection(DefaultConnection dbConn)
        {
            return (DbConnection)dbConn.DbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnectionStringOrFilePath"></param>
        /// <param name="dialectProvider"></param>
        /// <returns></returns>
        public  IDbConnection ToDbConnection(string dbConnectionStringOrFilePath, IDbDialectProvider dialectProvider)
        {
            var dbConn = dialectProvider.CreateConnection(dbConnectionStringOrFilePath, options: null);
            return dbConn;
        }
    }

    internal interface ISetDbTransaction
    {
        IDbTransaction Transaction { get; set; }
    }
}