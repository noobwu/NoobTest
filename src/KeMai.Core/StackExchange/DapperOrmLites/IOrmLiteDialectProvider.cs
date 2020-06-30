using KeMai.Data;
using System;
using System.Collections.Generic;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOrmLiteDialectProvider : IDbDialectProvider
    {
        /// <summary>
        /// 
        /// </summary>
        INamingStrategy NamingStrategy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ParamString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Func<string, string> ParamNameFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        string GetColumnNames(ModelDefinition modelDef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        string GetTableName(ModelDefinition modelDef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        string GetTableName(string tableName, string schema = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        string GetQuotedTableName(ModelDefinition modelDef);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>

        string GetQuotedTableName(string tableName, string schema = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string GetQuotedColumnName(string columnName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string GetQuotedName(string columnName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        string EscapeWildcards(string value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string SqlBool(bool value);


        #region  Statement
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        string SqlLimit(int? offset = null, int? rows = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetLastInsertIdSqlSuffix<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        string ToSelectStatement(string sql, int? rows = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="insertFields"></param>
        /// <returns></returns>
        Tuple<string, bool> ToInsertStatement<T>(ICollection<string> insertFields = null);
        #endregion
    }
}
