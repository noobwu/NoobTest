using Dapper;
using KeMai.Data;
using KeMai.Domain.Entities;
using KeMai.StackExchange.Dapper;
using KeMai.StackExchange.DapperOrmLites;
using KeMai.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace KeMai.Domain.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class RepositoryBaseOfTPrimaryKey<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

        #region OrmLite
        /// <summary>
        /// 
        /// </summary>
        protected DefaultConnectionFactory DbFactory;
        public RepositoryBaseOfTPrimaryKey()
        {
            DbFactory = new OrmLiteConnectionFactory(null, SqlServer2012Dialect.Provider);
            var type = typeof(TEntity);
            ModelDefinition modelDef = type.GetModelDefinition();
            DialectProvider = DbFactory.GetOrmLiteDialectProvider();
            TableName = DialectProvider.GetTableName(modelDef.ModelName, modelDef.Schema);
            if (modelDef.PrimaryKey != null)
            {
                PrimaryFieldName = modelDef.PrimaryKey.FieldName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        protected virtual string TableName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        protected virtual string PrimaryFieldName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        protected virtual IOrmLiteDialectProvider DialectProvider { get; private set; }


        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id or number.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <returns>Identity of inserted entity</returns>
        public virtual int Insert(string connectionString, TEntity entityToInsert)
        {
            if (entityToInsert == null) return -1;
            var hanlerResult = DialectProvider.ToInsertStatement<TEntity>();
            string sql = hanlerResult.Item1;
            var connection = DbFactory.OpenDbConnectionString(connectionString);
            if (hanlerResult.Item2)
            {
                sql += DialectProvider.GetLastInsertIdSqlSuffix<TEntity>();
                return connection.ExecuteScalar<int>(sql, entityToInsert);
            }
            return connection.Execute(sql, entityToInsert);
        }


        /// <summary>
        /// Inserts entities into table "Ts" and returns number.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entities">entities</param>
        /// <returns>number</returns>
        public virtual int InsertList(string connectionString, IEnumerable<TEntity> entities)
        {
            if (entities == null||entities.Count()==0) return -1;
            var hanlerResult = DialectProvider.ToInsertStatement<IEnumerable<TEntity>>();
            string sql = hanlerResult.Item1;
            var connection = DbFactory.OpenDbConnectionString(connectionString);
            return connection.Execute(sql, entities);
        }
		
        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entityToUpdate">Entity to Update</param>
        /// <returns></returns>
        public virtual int UpdateNonDefaults(string connectionString, TEntity entityToUpdate, TPrimaryKey id)
        {
            if (entityToUpdate == null) return -1;
            if (string.IsNullOrEmpty(PrimaryFieldName)) return -1;
            string whereExpression = DialectProvider.GetQuotedColumnName(PrimaryFieldName) + "=@Id";
            return Update(connectionString, entityToUpdate, whereExpression, new { Id = id }, null, true);
        }
        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entityToUpdate">Entity to Update</param>
        /// <param name="id"></param>
        /// <param name="updateFields">需要更新的列</param>
        /// <param name="excludeDefaults">是否排除更新默认值</param>
        /// <returns></returns>
        public virtual int Update(string connectionString, TEntity entityToUpdate, TPrimaryKey id, ICollection<string> updateFields = null, bool excludeDefaults = false)
        {
            if (entityToUpdate == null) return -1;
            if (string.IsNullOrEmpty(PrimaryFieldName)) return -1;
            string whereExpression = DialectProvider.GetQuotedColumnName(PrimaryFieldName) + "=@Id";
            return Update(connectionString, entityToUpdate, whereExpression, new { Id = id }, updateFields, excludeDefaults);
        }

        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="entityToUpdate">Entity to Update</param>
        /// <param name="whereExpression"></param>
        /// <param name="parameters"></param>
        /// <param name="updateFields">需要更新的列</param>
        /// <param name="excludeDefaults">是否排除更新默认值</param>
        /// <returns></returns>
        protected virtual int Update(string connectionString, TEntity entityToUpdate, string whereExpression, object parameters, ICollection<string> updateFields = null, bool excludeDefaults = false)
        {
            if (entityToUpdate == null || string.IsNullOrEmpty(whereExpression)) return -1;
            whereExpression = "WHERE " + whereExpression;
            var modelDef = typeof(TEntity).GetModelDefinition();
            var updateAllFields = updateFields == null || updateFields.Count == 0;
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(string.Format("UPDATE {0} /**update**/", TableName), null);
            var dynamicParameters = new DynamicParameters();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.ShouldSkipUpdate() || fieldDef.IsPrimaryKey || fieldDef.AutoIncrement)
                {
                    continue;
                }
                if (!updateAllFields && !updateFields.Contains(fieldDef.FieldName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
                var value = fieldDef.GetValue(entityToUpdate);
                if (excludeDefaults
                   && (value == null || value.Equals(value.GetType().GetDefaultValue())))
                {
                    continue;
                }
                builder.Update(DialectProvider.GetQuotedColumnName(fieldDef.FieldName) + "=" + DialectProvider.GetParam(fieldDef.Name));
                dynamicParameters.Add(fieldDef.Name, value);
            }
            string sql = template.RawSql + " " + whereExpression;
            if (parameters != null)
            {
                dynamicParameters.AddDynamicParams(parameters);
            }
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Execute(sql, dynamicParameters);
            }
        }


        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="updateOnly"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int UpdateOnly(string connectionString, object updateOnly, TPrimaryKey id)
        {
            if (updateOnly == null) return -1;
            if (string.IsNullOrEmpty(PrimaryFieldName)) return -1;
            string whereExpression = DialectProvider.GetQuotedColumnName(PrimaryFieldName) + "=@Id";
            return UpdateOnly(connectionString, updateOnly, whereExpression, new { Id = id });
        }
        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="updateOnly"></param>
        /// <param name="whereExpression"></param>
        /// <param name="parameters"></param>
        /// <param name="excludeDefaults">是否排除更新默认值</param>
        /// <returns></returns>
        protected virtual int UpdateOnly(string connectionString, object updateOnly, string whereExpression, object parameters)
        {
            if (updateOnly == null || string.IsNullOrEmpty(whereExpression)) return -1;
            whereExpression = "WHERE " + whereExpression;
            var modelDef = typeof(TEntity).GetModelDefinition();
            var fields = modelDef.FieldDefinitions;
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(string.Format("UPDATE {0} /**update**/", TableName), null);
            var dynamicParameters = new DynamicParameters();
            foreach (var setField in updateOnly.GetType().GetPublicProperties())
            {
                var fieldDef = fields.FirstOrDefault(x => string.Equals(x.Name, setField.Name, StringComparison.OrdinalIgnoreCase));
                if (fieldDef == null || fieldDef.ShouldSkipUpdate() || fieldDef.IsPrimaryKey || fieldDef.AutoIncrement) continue;
                var value = setField.CreateGetter()(updateOnly);
                builder.Update(DialectProvider.GetQuotedColumnName(fieldDef.FieldName) + "=" + DialectProvider.GetParam(fieldDef.Name));
                dynamicParameters.Add(fieldDef.Name, value);
            }
            string sql = template.RawSql + " " + whereExpression;
            if (parameters != null)
            {
                dynamicParameters.AddDynamicParams(parameters);
            }
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Execute(sql, dynamicParameters);
            }
        }

        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="updateOnly"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int UpdateOnly(string connectionString, Expression<Func<TEntity>> updateFields, TPrimaryKey id)
        {
            if (updateFields == null) return -1;
            if (string.IsNullOrEmpty(PrimaryFieldName)) return -1;
            string whereExpression = DialectProvider.GetQuotedColumnName(PrimaryFieldName) + "=@Id";
            return UpdateOnly(connectionString, updateFields, whereExpression, new { Id = id });
        }
        /// <summary>
        /// Update an entity into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="updateOnly"></param>
        /// <param name="whereExpression"></param>
        /// <param name="parameters"></param>
        /// <param name="excludeDefaults">是否排除更新默认值</param>
        /// <returns></returns>
        protected virtual int UpdateOnly(string connectionString, Expression<Func<TEntity>> updateFields, string whereExpression, object parameters)
        {
            if (updateFields == null || string.IsNullOrEmpty(whereExpression)) return -1;
            var updateFieldValues = updateFields.AssignedValues();
            if (updateFieldValues == null || updateFieldValues.Count == 0)
            {
                return -2;
            }
            return Update(connectionString,updateFieldValues,whereExpression,parameters);
        }
        /// <summary>
        /// Update an Dic into table "Ts" and returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="args"></param>
        /// <param name="whereExpression"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual int Update(string connectionString, IDictionary<string, object> args, string whereExpression, object parameters)
        {
            if (args == null || args.Count == 0 || string.IsNullOrEmpty(whereExpression)) return -1;
            whereExpression = "WHERE " + whereExpression;
            var modelDef = typeof(TEntity).GetModelDefinition();
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(string.Format("UPDATE {0} /**update**/", TableName), null);
            var dynamicParameters = new DynamicParameters();
            foreach (var entry in args)
            {
                var fieldDef = modelDef.GetFieldDefinition(entry.Key);
                if (fieldDef == null || fieldDef.ShouldSkipUpdate() || fieldDef.IsPrimaryKey || fieldDef.AutoIncrement)
                    continue;
                builder.Update(DialectProvider.GetQuotedColumnName(fieldDef.FieldName) + "=" + DialectProvider.GetParam(fieldDef.Name));
                dynamicParameters.Add(fieldDef.Name, entry.Value);
            }
            string sql = template.RawSql + " " + whereExpression;
            if (parameters != null)
            {
                dynamicParameters.AddDynamicParams(parameters);
            }
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Execute(sql, dynamicParameters);
            }
        }
        /// <summary>
        ///Delete data returns  number .
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete(string connectionString, TPrimaryKey id)
        {
            if (id == null) return -1;
            if (string.IsNullOrEmpty(PrimaryFieldName)) return -1;
            string whereExpression = string.Format("WHERE {0}={1}", PrimaryFieldName, DialectProvider.GetParam("Id"));
            return Delete(connectionString, whereExpression, new { Id = id });
        }
        /// <summary>
        ///Delete data returns  number .
        /// </summary>
        /// <param name="connectionString">Open SqlConnection</param>
        /// <param name="whereExpression"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual int Delete(string connectionString, string whereExpression, object parameters)
        {
            if (string.IsNullOrEmpty(whereExpression)) return -1;
            string sql = string.Format("DELETE FROM {0} WHERE {1}", TableName, whereExpression);
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Execute(sql, parameters);
            }
        }
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public virtual TEntity Single(string connectionString, TPrimaryKey id)
        {
            string whereExpression = string.Format("{1}={2}", TableName, PrimaryFieldName, DialectProvider.GetParam("Id"));
            return Single(connectionString, whereExpression, new { Id = id });
        }
		
		 /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>Entity</returns>
        protected virtual TEntity Single(string connectionString, string whereExpression, object param)
        {
			if (string.IsNullOrEmpty(whereExpression)) return null;
            string sql = string.Format("SELECT * FROM {0} WHERE {1}", TableName, whereExpression);
            return SingleBySql(connectionString, sql, param);
        }
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>Entity</returns>
        protected virtual TEntity SingleBySql(string connectionString, string sql, object param)
        {
            sql = DialectProvider.ToSelectStatement(sql, 1);
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.QueryFirstOrDefault<TEntity>(sql, param);
            }
        }
        /// <summary>
        ///  获取数据列表
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="builder"></param>
        /// <param name="template"></param>
        /// <param name="orderColumn">排序列</param>
        /// <param name="orderBy">排序类型</param>
        /// <param name="rows">每页显示记录数</param>
        /// <returns>the list of query result.</returns>
        protected virtual IEnumerable<TEntity> GetList(string connectionString, SqlBuilder builder, SqlBuilder.Template template, string orderColumn, ListResultsOrder orderBy, int rows)
        {
            return GetList<TEntity>(connectionString, builder, template, orderColumn, orderBy, rows);
        }
        /// <summary>
        ///  获取数据列表
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="builder"></param>
        /// <param name="template"></param>
        /// <param name="orderColumn">排序列</param>
        /// <param name="orderBy">排序类型</param>
        /// <param name="rows">每页显示记录数</param>
        /// <returns>the list of query result.</returns>
        protected virtual IEnumerable<TResult> GetList<TResult>(string connectionString, SqlBuilder builder, SqlBuilder.Template template, string orderColumn, ListResultsOrder orderBy, int rows)
              where TResult : class
        {
            if (!string.IsNullOrEmpty(orderColumn))
            {
                builder.OrderBy(orderBy == ListResultsOrder.Descending ? orderColumn + " DESC" : orderColumn + " ASC");
            }
            builder.Limit(null, rows);
            var sql = template.RawSql;
            var param = template.Parameters;
            return GetList<TResult>(connectionString, sql, param);
        }
        /// <summary>
        ///  获取数据列表
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>the list of query result.</returns>
        protected virtual IEnumerable<TResult> GetList<TResult>(string connectionString, string sql, object param)
              where TResult : class
        {
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Query<TResult>(sql, param);
            }
        }
        /// <summary>
        ///  获取数据列表
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>the list of query result.</returns>
        protected virtual int Execute(string connectionString, string sql, object param)
        {
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.Execute(sql, param);
            }
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="builder"></param>
        /// <param name="template"></param>
        /// <param name="orderColumn">排序列</param>
        /// <param name="orderBy">排序类型</param>
        /// <param name="offset">开始位置</param>
        /// <param name="pageRows">每页显示记录数</param>
        /// <param name="totalRows">总记录数</param>
        /// <returns>the list of query result.</returns>
        protected virtual IEnumerable<TEntity> GetPaggingList(string connectionString, SqlSelectBuilder builder, SqlSelectBuilder.SelectTemplate template, string orderColumn, ListResultsOrder orderBy, int? offset, int? pageRows, out int totalRows)
        {
            return GetPaggingList<TEntity>(connectionString, builder, template, orderColumn, orderBy, offset, pageRows, out totalRows);
        }
        /// <summary>
        ///  获取分页数据
        /// </summary>
        /// <param name="connectionString">数据信息</param>
        /// <param name="builder"></param>
        /// <param name="template"></param>
        /// <param name="orderColumn">排序列</param>
        /// <param name="orderBy">排序类型</param>
        /// <param name="offset">开始位置</param>
        /// <param name="pageRows">每页显示记录数</param>
        /// <param name="totalRows">总记录数</param>
        /// <returns>the list of query result.</returns>
        protected virtual IEnumerable<TResult> GetPaggingList<TResult>(string connectionString, SqlSelectBuilder builder, SqlSelectBuilder.SelectTemplate template, string orderColumn, ListResultsOrder orderBy, int? offset, int? pageRows, out int totalRows)
            where TResult : class
        {
            if (string.IsNullOrEmpty(orderColumn))
            {
                orderColumn = "1";
            }
            builder.OrderBy(orderBy == ListResultsOrder.Descending ? orderColumn + " DESC" : orderColumn + " ASC");
            builder.Limit(offset, pageRows);
            var resolveResult = template.ResolveRawSql(0);
            IEnumerable<TResult> list = null;
            using (var conn = OpenDbConnection(connectionString))
            {
                string sql = resolveResult.Item1 + ";\n" + resolveResult.Item2;
                using (var multiple = conn.QueryMultiple(sql, template.Parameters))
                {
                    list = multiple.Read<TResult>();
                    totalRows = multiple.ReadFirst<int>();
                }
            }
            return list;
        }
        /// <summary>
        ///  获取数据记录数
        /// </summary>
        /// <param name="connectionString">数据库连接信息</param>
        /// <param name="builder"></param>
        /// <param name="template"></param>
        /// <returns>the list of query result.</returns>
        protected int GetCount(string connectionString, SqlBuilder builder, SqlBuilder.Template template)
        {
            var sql = template.RawSql;
            var param = template.Parameters;
            return GetCount(connectionString, sql, param);
        }
        /// <summary>
        ///  获取数据记录数
        /// </summary>
        /// <param name="connectionString">数据库连接信息</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>the list of query result.</returns>
        protected int GetCount(string connectionString, string sql, object param)
        {
            using (var conn = OpenDbConnection(connectionString))
            {
                return conn.ExecuteScalar<int>(sql, param);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnectionString"></param>
        /// <returns></returns>
        public virtual IDbConnection OpenDbConnection(string dbConnectionString)
        {
            return DbFactory.OpenDbConnectionString(dbConnectionString);
        }
        #endregion 
    }
}
