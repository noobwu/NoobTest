using System;
using System.Collections.Generic;
using System.Threading;

namespace KeMai
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate object GetMemberDelegate(object instance);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate object GetMemberDelegate<T>(T instance);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void SetMemberDelegate(object instance, object value);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void SetMemberDelegate<T>(T instance, object value);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="propertyValue"></param>
    public delegate void SetMemberRefDelegate(ref object instance, object propertyValue);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void SetMemberRefDelegate<T>(ref T instance, object value);

    /// <summary>
    /// 
    /// </summary>
    public static class AutoMappingUtils
    {
        private static Dictionary<Type, object> DefaultValueTypes = new Dictionary<Type, object>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType) return null;
            object defaultValue;
            if (DefaultValueTypes.TryGetValue(type, out defaultValue))
                return defaultValue;

            defaultValue = Activator.CreateInstance(type);

            Dictionary<Type, object> snapshot, newCache;
            do
            {
                snapshot = DefaultValueTypes;
                newCache = new Dictionary<Type, object>(DefaultValueTypes) { [type] = defaultValue };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }
    }


}
