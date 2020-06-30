using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class PropertyInvoker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static GetMemberDelegate CreateGetter(this PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic: true);
            if (getMethodInfo == null) return null;

            return o => propertyInfo.GetGetMethod(nonPublic: true).Invoke(o, TypeConstants.EmptyObjectArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static GetMemberDelegate<T> CreateGetter<T>(this PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic: true);
            if (getMethodInfo == null) return null;
            return o => propertyInfo.GetGetMethod(nonPublic: true).Invoke(o, TypeConstants.EmptyObjectArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static SetMemberDelegate CreateSetter(this PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.GetSetMethod(nonPublic: true);
            if (propertySetMethod == null) return null;
            return (o, convertedValue) =>
                propertySetMethod.Invoke(o, new[] { convertedValue });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static SetMemberDelegate<T> CreateSetter<T>(this PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.GetSetMethod(nonPublic: true);
            if (propertySetMethod == null) return null;

            return (o, convertedValue) =>
                propertySetMethod.Invoke(o, new[] { convertedValue });
        }
    }
}
