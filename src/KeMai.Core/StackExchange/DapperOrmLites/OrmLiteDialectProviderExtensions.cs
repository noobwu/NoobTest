namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbDialectProviderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetParam(this IOrmLiteDialectProvider dialect, string name)
        {
            return dialect.ParamString + (dialect.ParamNameFilter?.Invoke(name) ?? name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="indexNo"></param>
        /// <returns></returns>
        public static string GetParam(this IOrmLiteDialectProvider dialect, int indexNo = 0)
        {
            return dialect.ParamString + indexNo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="fieldDef"></param>
        /// <returns></returns>
        public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect, 
            FieldDefinition fieldDef)
        {
            return dialect.GetQuotedColumnName(fieldDef.FieldName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="tableDef"></param>
        /// <param name="fieldDef"></param>
        /// <returns></returns>
        public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
            ModelDefinition tableDef, FieldDefinition fieldDef)
        {
            return dialect.GetQuotedTableName(tableDef) +
                "." +
                dialect.GetQuotedColumnName(fieldDef.FieldName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="tableDef"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
            ModelDefinition tableDef, string fieldName)
        {
            return dialect.GetQuotedTableName(tableDef) +
                "." +
                dialect.GetQuotedColumnName(fieldName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public static bool IsMySqlConnector(this IOrmLiteDialectProvider dialect)
        {
            return dialect.GetType().Name == "MySqlConnectorDialectProvider";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>

        public static string ToFieldName(this IOrmLiteDialectProvider dialect, string paramName)
        {
            return paramName.Substring(dialect.ParamString.Length);
        }

    }
}