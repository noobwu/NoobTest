using System;
using System.Collections.Generic;
using System.Data;
using KeMai.Text;
namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDialect"></typeparam>
    public abstract class DbDialectProviderBase<TDialect> : IDbDialectProvider
       where TDialect : IDbDialectProvider
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options);
		   /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public virtual string SqlLimit(int? offset = null, int? rows = null)
        {
            return rows == null && offset == null ? "" : offset == null ? "LIMIT " + rows : "LIMIT " + rows.GetValueOrDefault(int.MaxValue) + " OFFSET " + offset;
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
        public virtual string ToSelectStatement(
  string selectExpression,
  string bodyExpression,
  string orderByExpression = null,
  int? offset = null,
  int? rows = null)
        {

            var sb = StringBuilderCache.Allocate();
            sb.Append(selectExpression);
            sb.Append(" ");
            sb.Append(bodyExpression);
            if (orderByExpression != null)
            {
                sb.Append(" ");
                sb.Append(orderByExpression);
            }

            if (offset != null || rows != null)
            {
                sb.Append("\n");
                sb.Append(SqlLimit(offset, rows));
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

    }
}
