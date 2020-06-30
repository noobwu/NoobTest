using KeMai.Text;
using System;
using System.Collections.Generic;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public static class OrmLiteUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectExpression"></param>
        /// <returns></returns>
        public static string StripTablePrefixes(this string selectExpression)
        {
            if (selectExpression.IndexOf('.') < 0)
                return selectExpression;

            var sb = StringBuilderCache.Allocate();
            var tokens = selectExpression.Split(' ');
            foreach (var token in tokens)
            {
                var parts = token.SplitOnLast('.');
                if (parts.Length > 1)
                {
                    sb.Append(" " + parts[parts.Length - 1]);
                }
                else
                {
                    sb.Append(" " + token);
                }
            }

            return StringBuilderCache.ReturnAndFree(sb).Trim();
        }

        public static char[] QuotedChars = new[] { '"', '`', '[', ']' };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="quotedExpr"></param>
        /// <returns></returns>
        public static string StripQuotes(this string quotedExpr)
        {
            return quotedExpr.Trim(QuotedChars);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ulong ConvertToULong(byte[] bytes)
        {
            Array.Reverse(bytes); //Correct Endianness
            var ulongValue = BitConverter.ToUInt64(bytes, 0);
            return ulongValue;
        }
        public static bool IsRefType(this Type fieldType)
        {
            return (!fieldType.UnderlyingSystemType().IsValueType)
                && fieldType != typeof(string);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToSelectString<TItem>(this IEnumerable<TItem> items)
        {
            var sb = StringBuilderCache.Allocate();

            foreach (var item in items)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(item);
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<string> GetNonDefaultValueInsertFields<T>(T obj)
        {
            var insertFields = new List<string>();
            var modelDef = typeof(T).GetModelDefinition();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (!string.IsNullOrEmpty(fieldDef.DefaultValue))
                {
                    var value = fieldDef.GetValue(obj);
                    if (value == null || value.Equals(fieldDef.FieldTypeDefaultValue))
                        continue;
                }
                insertFields.Add(fieldDef.Name);
            }

            return insertFields.Count == modelDef.FieldDefinitions.Count
                ? null
                : insertFields;
        }
    }
}
