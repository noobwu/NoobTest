using KeMai.Text;
using KeMai.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OrmLiteDialectProviderBase<TDialect> : DbDialectProviderBase<TDialect>,IOrmLiteDialectProvider
          where TDialect : IOrmLiteDialectProvider
    {
        /// <summary>
        /// 
        /// </summary>
        protected OrmLiteDialectProviderBase()
        {

        }
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        private INamingStrategy namingStrategy = new DbNamingStrategyBase();
        /// <summary>
        /// 
        /// </summary>
        public INamingStrategy NamingStrategy
        {
            get { return namingStrategy; }
            set { namingStrategy = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        private string paramString = "@";
        /// <summary>
        /// 
        /// </summary>
        public string ParamString
        {
            get { return paramString; }
            set { paramString = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private Func<string, string> paramNameFilter;
        /// <summary>
        /// 
        /// </summary>
        public Func<string, string> ParamNameFilter
        {
            get { return paramNameFilter ?? OrmLiteConfig.ParamNameFilter; }
            set { paramNameFilter = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AutoIncrementDefinition = "AUTOINCREMENT"; //SqlServer express limit
        /// <summary>
        /// 
        /// </summary>
        public virtual string SelectIdentitySql { get; set; }
        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual string GetSchemaName(string schema)
        {
            return NamingStrategy.GetSchemaName(schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        public virtual string GetTableName(ModelDefinition modelDef)
        {
            return GetTableName(modelDef.ModelName, modelDef.Schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual string GetTableName(string table, string schema = null)
        {
            return schema != null
                ? string.Format("{0}.{1}", NamingStrategy.GetSchemaName(schema), NamingStrategy.GetTableName(table))
                : NamingStrategy.GetTableName(table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        public virtual string GetQuotedTableName(ModelDefinition modelDef)
        {
            return GetQuotedTableName(modelDef.ModelName, modelDef.Schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual string GetQuotedTableName(string tableName, string schema = null)
        {
            if (schema == null)
                return GetQuotedName(NamingStrategy.GetTableName(tableName));

            var escapedSchema = NamingStrategy.GetSchemaName(schema)
                .Replace(".", "\".\"");

            return GetQuotedName(escapedSchema)
                + "."
                + GetQuotedName(NamingStrategy.GetTableName(tableName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual string GetQuotedColumnName(string columnName)
        {
            return GetQuotedName(NamingStrategy.GetColumnName(columnName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetQuotedName(string name)
        {
            return string.Format("\"{0}\"", name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public virtual string GetQuotedValue(string paramValue)
        {
            return "'" + paramValue.Replace("'", "''") + "'";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string EscapeWildcards(string value)
        {
            return value?.Replace("^", @"^^")
                .Replace(@"\", @"^\")
                .Replace("_", @"^_")
                .Replace("%", @"^%");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        public virtual string GetColumnNames(ModelDefinition modelDef)
        {
            return GetColumnNames(modelDef, false).ToSelectString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <param name="tableQualified"></param>
        /// <returns></returns>
        public virtual SelectItem[] GetColumnNames(ModelDefinition modelDef, bool tableQualified)
        {
            var tablePrefix = tableQualified ? GetQuotedTableName(modelDef) : "";

            var sqlColumns = new SelectItem[modelDef.FieldDefinitions.Count];
            for (var i = 0; i < sqlColumns.Length; ++i)
            {
                var field = modelDef.FieldDefinitions[i];
                if (field.IsRowVersion)
                {
                    sqlColumns[i] = GetRowVersionColumnName(field, tablePrefix);
                }
                else
                {
                    sqlColumns[i] = new SelectItemColumn(this, field.FieldName, null, tablePrefix);
                }
            }

            return sqlColumns;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string SqlBool(bool value)
        {
            return value ? "true" : "false";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        public virtual SelectItem GetRowVersionColumnName(FieldDefinition field, string tablePrefix = null)
        {
            return new SelectItemColumn(this, field.FieldName, null, tablePrefix);
        }

        static readonly ConcurrentDictionary<string, GetMemberDelegate> anonValueFnMap =
          new ConcurrentDictionary<string, GetMemberDelegate>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDef"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual object GetAnonValue(FieldDefinition fieldDef, object obj)
        {
            var anonType = obj.GetType();
            var key = anonType.Name + "." + fieldDef.Name;

            var factoryFn = (Func<string, GetMemberDelegate>)(_ =>
                anonType.GetProperty(fieldDef.Name).CreateGetter());

            var getterFn = anonValueFnMap.GetOrAdd(key, factoryFn);

            return getterFn(obj);
        }

        #endregion

        #region SqlStatement
        public virtual string GetLastInsertIdSqlSuffix<T>()
        {
            if (SelectIdentitySql == null)
                throw new NotImplementedException("Returning last inserted identity is not implemented on this DB Provider.");

            return "; " + SelectIdentitySql;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="insertFields"></param>
        /// <returns></returns>
        public virtual Tuple<string, bool> ToInsertStatement<T>(ICollection<string> insertFields = null)
        {
            var sbColumnNames = StringBuilderCache.Allocate();
            var sbColumnValues = StringBuilderCacheAlt.Allocate();
            var isList = false;
            var type = typeof(T);
            if (type.IsArray)
            {
                isList = true;
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                  (type.GetInterfaces().Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>))) ||
                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                isList = true;
                type = type.GetGenericArguments()[0];
            }
            ModelDefinition modelDef = type.GetModelDefinition();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.ShouldSkipInsert())
                    continue;

                //insertFields contains Property "Name" of fields to insert ( that's how expressions work )
                if (insertFields != null && !insertFields.Contains(fieldDef.Name, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(this.GetParam(SanitizeFieldNameForParamName(fieldDef.FieldName)));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            string sql = "INSERT INTO " + GetQuotedTableName(modelDef) + "(" + StringBuilderCache.ReturnAndFree(sbColumnNames) + ") " +
                              "VALUES (" + StringBuilderCacheAlt.ReturnAndFree(sbColumnValues) + ")";
            bool selectIdentity = false;
            if (!isList&&modelDef.HasAutoIncrementId)
            {
                selectIdentity = true;
            }
            return Tuple.Create(sql, selectIdentity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public virtual string SanitizeFieldNameForParamName(string fieldName)
        {
            return OrmLiteConfig.SanitizeFieldNameForParamNameFn(fieldName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public virtual string ToSelectStatement(string sql,int? rows = null)
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
            Regex regex = new Regex(@"(LIMIT\s*\d+)",RegexOptions.IgnoreCase);
            if (regex.IsMatch(sql))
            {
                return sql;
            }
            return string.Format("{0} LIMIT {1}", sql,rows.Value);
        }
        #endregion
    }
}
