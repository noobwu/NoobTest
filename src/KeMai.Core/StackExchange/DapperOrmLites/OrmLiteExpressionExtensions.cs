using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public static class OrmLiteExpressionExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="excludeDefaults">是否排除更新默认值</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToUpdateFieldValues<TSource>(this TSource source,bool excludeDefaults = false)
            where TSource : class
        {
            var modelType = typeof(TSource);
            var objProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var modelDef = typeof(TSource).GetModelDefinition();
            Dictionary<string, object> updateFieldValues = new Dictionary<string, object>();
            foreach (PropertyInfo item in objProperties)
            {
                var value = item.GetValue(source, null);
                if (excludeDefaults
                    && (value == null || value.Equals(value.GetType().GetDefaultValue())))
                {
                    continue;
                }
                var fieldDef = modelDef.GetFieldDefinition(item.Name);
                if (fieldDef==null||fieldDef.ShouldSkipUpdate() || fieldDef.AutoIncrement)
                {
                    continue;
                }
                updateFieldValues.Add(fieldDef.FieldName, value);
            }
            return updateFieldValues;
        }
    }
}
