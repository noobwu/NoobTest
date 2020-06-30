
using System;
using System.Data;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Func<IDbConnection> connectionFactoryFn;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactoryFn"></param>
        public DbConnectionFactory(Func<IDbConnection> connectionFactoryFn)
        {
            this.connectionFactoryFn = connectionFactoryFn;
        }
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection OpenDbConnection()
        {
            var dbConn = CreateDbConnection();
            Open(dbConn);
            return dbConn;
        }
        /// <summary>
        ///  打开一个数据库连接，其设置由提供程序特定的 Connection 对象的 ConnectionString 属性指定。
        /// </summary>
        public void Open(IDbConnection conn)
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
        }
        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateDbConnection()
        {
            return connectionFactoryFn();
        }
    }
}