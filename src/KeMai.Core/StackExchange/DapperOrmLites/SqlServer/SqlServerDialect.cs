using KeMai.StackExchange.DapperOrmLites.SqlServer;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlServerDialect
    {
        /// <summary>
        /// 
        /// </summary>
        public static IOrmLiteDialectProvider Provider
        {
            get
            {
                return SqlServerOrmLiteDialectProvider.Instance;
            }
        }
      
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SqlServer2012Dialect
    {
        public static IOrmLiteDialectProvider Provider
        {
            get
            {
                return SqlServer2012OrmLiteDialectProvider.Instance;
            }
        }
    }

    public static class SqlServer2014Dialect
    {
        public static IOrmLiteDialectProvider Provider
        {
            get
            {
                return SqlServer2014OrmLiteDialectProvider.Instance;
            }
        }
    }

}
