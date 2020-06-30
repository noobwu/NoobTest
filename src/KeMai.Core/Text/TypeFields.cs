using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class FieldInvoker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static GetMemberDelegate CreateGetter(this FieldInfo fieldInfo) =>fieldInfo.GetValue;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static GetMemberDelegate<T> CreateGetter<T>(this FieldInfo fieldInfo) =>
           x => fieldInfo.GetValue(x);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static SetMemberDelegate CreateSetter(this FieldInfo fieldInfo) =>
           fieldInfo.SetValue;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static SetMemberDelegate<T> CreateSetter<T>(this FieldInfo fieldInfo) =>
           (o, x) => fieldInfo.SetValue(o, x);
    }
}
