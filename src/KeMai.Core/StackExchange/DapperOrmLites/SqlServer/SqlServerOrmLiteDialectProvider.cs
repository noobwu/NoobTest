using KeMai.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace KeMai.StackExchange.DapperOrmLites.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerOrmLiteDialectProvider : OrmLiteDialectProviderBase<SqlServerOrmLiteDialectProvider>
    {
        public static SqlServerOrmLiteDialectProvider Instance = new SqlServerOrmLiteDialectProvider();
        public SqlServerOrmLiteDialectProvider()
        {
            base.AutoIncrementDefinition = "IDENTITY(1,1)";
            base.SelectIdentitySql = "SELECT SCOPE_IDENTITY()";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(";");

            if (!isFullConnectionString)
            {
                var filePath = connectionString;

                var filePathWithExt = filePath.EndsWith(".mdf")
                    ? filePath
                    : filePath + ".mdf";

                var fileName = Path.GetFileName(filePathWithExt);
                var dbName = fileName.Substring(0, fileName.Length - ".mdf".Length);

                connectionString = $@"Data Source=.\SQLEXPRESS;AttachDbFilename={filePathWithExt};Initial Catalog={dbName};Integrated Security=True;User Instance=True;";
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.Key.ToLower() == "read only")
                    {
                        if (option.Value.ToLower() == "true")
                        {
                            connectionString += "Mode = Read Only;";
                        }
                        continue;
                    }
                    connectionString += option.Key + "=" + option.Value + ";";
                }
            }
            return new SqlConnection(connectionString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string SqlBool(bool value)
        {
            return value ? "1" : "0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override string ToSelectStatement(string sql, int? rows = null)
        {
            if (!rows.HasValue)
                return sql;

            if (rows.HasValue && rows.Value < 0)
                throw new ArgumentException(string.Format("Rows value:'{0}' must be>=0", rows.Value));
            var take = rows ?? int.MaxValue;
            var selectType = sql.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";
            //Temporary hack till we come up with a more robust paging sln for SqlServer

            if (take == int.MaxValue)
                return sql;

            if (sql.Length < "SELECT".Length)
                return sql;
            return string.Format("{0} TOP {1}", selectType, take + sql.Substring(selectType.Length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override string SqlLimit(int? offset = null, int? rows = null)
        {
            return rows == null && offset == null ? "" : rows != null ? "OFFSET " + offset.GetValueOrDefault() + " ROWS FETCH NEXT " + rows + " ROWS ONLY" : "OFFSET " + offset.GetValueOrDefault(int.MaxValue) + " ROWS";
        }
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectExpression"></param>
        /// <param name="bodyExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override string ToSelectStatement(
        string selectExpression,
        string bodyExpression,
        string orderByExpression = null,
        int? offset = null,
        int? rows = null)
        {
            var sb = StringBuilderCache.Allocate()
                .Append(selectExpression)
                .Append(" ")
                .Append(bodyExpression);

            if (!offset.HasValue && !rows.HasValue)
            {
                return StringBuilderCache.ReturnAndFree(sb)+" " + orderByExpression;
            }

            if (offset.HasValue && offset.Value < 0)
                throw new ArgumentException(string.Format("Skip value:'{0}' must be>=0", offset.Value));

            if (rows.HasValue && rows.Value < 0)
                throw new ArgumentException(string.Format("Rows value:'{0}' must be>=0", rows.Value));

            var skip = offset ?? 0;
            var take = rows ?? int.MaxValue;

            var selectType = selectExpression.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";

            //Temporary hack till we come up with a more robust paging sln for SqlServer
            if (skip == 0)
            {
                var sql = StringBuilderCache.ReturnAndFree(sb)+" " + orderByExpression;

                if (take == int.MaxValue)
                    return sql;

                if (sql.Length < "SELECT".Length)
                    return sql;

                return string.Format("{0} TOP {1}", selectType, take + sql.Substring(selectType.Length));
            }

            // Required because ordering is done by Windowing function
            if (string.IsNullOrEmpty(orderByExpression))
            {
                throw new ArgumentNullException("orderByExpression must be ");
            }

            var row = take == int.MaxValue ? take : skip + take;

            var ret = string.Format("SELECT * FROM (SELECT {0}, ROW_NUMBER() OVER ({1}) As RowNum {2}) AS RowConstrainedResult WHERE RowNum > {3} AND RowNum <= {4}", selectExpression.Substring(selectType.Length), orderByExpression, bodyExpression, skip, row);

            return ret;
        }
    }
}