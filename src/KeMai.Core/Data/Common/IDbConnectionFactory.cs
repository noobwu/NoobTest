using System.Data;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection OpenDbConnection();
        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateDbConnection();
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IDbConnectionFactoryExtended : IDbConnectionFactory
    {
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <param name="namedConnection"></param>
        /// <returns></returns>
        IDbConnection OpenDbConnection(string namedConnection);
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IDbConnection OpenDbConnectionString(string connectionString);
    }
}
