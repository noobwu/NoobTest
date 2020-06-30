using System;
using System.Reflection;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get { return Alias ?? this.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type FieldType { get; set; }

        /// <summary>
        /// 枚举的基础类型
        /// </summary>
        public Type TreatAsType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type ColumnType {
            get { return TreatAsType ?? FieldType;}
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsRowVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public GetMemberDelegate GetValueFn { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public SetMemberDelegate SetValueFn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreOnInsert { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreOnUpdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsComputed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onInstance"></param>
        /// <returns></returns>
        public object GetValue(object onInstance)
        {
            return this.GetValueFn?.Invoke(onInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSkipInsert() {
            return IgnoreOnInsert || AutoIncrement || IsComputed || IsRowVersion;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSkipUpdate()
        {
            return IgnoreOnUpdate || IsComputed;
        }
        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object FieldTypeDefaultValue { get; set; }
    }
}
