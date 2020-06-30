using System;
using KeMai.Text;

namespace KeMai.StackExchange.DapperOrmLites.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServer2012OrmLiteDialectProvider : SqlServerOrmLiteDialectProvider
    {
        public new static SqlServer2012OrmLiteDialectProvider Instance = new SqlServer2012OrmLiteDialectProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDef"></param>
        /// <returns></returns>

        internal bool isSpatialField(FieldDefinition fieldDef)
        {
            return fieldDef.FieldType.Name == "SqlGeography" || fieldDef.FieldType.Name == "SqlGeometry";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDef"></param>
        /// <returns></returns>
        internal bool hasIsNullProperty(FieldDefinition fieldDef)
        {
            return isSpatialField(fieldDef) || fieldDef.FieldType.Name == "SqlHierarchyId";
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

            if (orderByExpression != null)
            {
                sb.Append(" ").Append(orderByExpression);
            }

            if (offset != null || rows != null)
            {
                if (string.IsNullOrEmpty(orderByExpression))
                {
                    throw new ArgumentNullException("orderByExpression must be ");
                }

                sb.Append(" ").Append(SqlLimit(offset, rows));
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }
    }
}
