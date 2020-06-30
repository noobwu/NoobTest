using System;
using System.Collections.Generic;
using System.Data;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbDialectProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options);
		/// <summary>
        /// 
        /// </summary>
        /// <param name="selectExpression"></param>
        /// <param name="bodyExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        string ToSelectStatement(string selectExpression, string bodyExpression, string orderByExpression = null, int? offset = null, int? rows = null);

    }
}
