using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class PlatformExtensions
    {
        //Should only register Runtime Attributes on StartUp, So using non-ThreadSafe Dictionary is OK
        static Dictionary<string, List<Attribute>> propertyAttributesMap
            = new Dictionary<string, List<Attribute>>();


        static Dictionary<Type, List<Attribute>> typeAttributesMap
            = new Dictionary<Type, List<Attribute>>();

        /// <summary>
        /// 
        /// </summary>
        public static void ClearRuntimeAttributes()
        {
            propertyAttributesMap = new Dictionary<string, List<Attribute>>();
            typeAttributesMap = new Dictionary<Type, List<Attribute>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        internal static string UniqueKey(this PropertyInfo pi)
        {
            if (pi.DeclaringType == null)
                throw new ArgumentException("Property '{0}' has no DeclaringType".Fmt(pi.Name));

            return pi.DeclaringType.Namespace + "." + pi.DeclaringType.Name + "." + pi.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TAttr FirstAttribute<TAttr>(this Type type) where TAttr : class
        {
            return (TAttr)type.GetCustomAttributes(typeof(TAttr), true)
                       .FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static TAttribute FirstAttribute<TAttribute>(this MemberInfo memberInfo)
        {
            return memberInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="paramInfo"></param>
        /// <returns></returns>
        public static TAttribute FirstAttribute<TAttribute>(this ParameterInfo paramInfo)
        {
            return paramInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo)
        {
            return propertyInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this Type type)
        {
            return type.AllAttributes().Any(x => x.GetType() == typeof(T));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo pi)
        {
            return pi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this FieldInfo fi)
        {
            return fi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this MethodInfo mi)
        {
            return mi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttributeNamed(this Type type, string name)
        {
            var normalizedAttr = name.Replace("Attribute", "").ToLower();
            return type.AllAttributes().Any(x => x.GetType().Name.Replace("Attribute", "").ToLower() == normalizedAttr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttributeNamed(this PropertyInfo pi, string name)
        {
            var normalizedAttr = name.Replace("Attribute", "").ToLower();
            return pi.AllAttributes().Any(x => x.GetType().Name.Replace("Attribute", "").ToLower() == normalizedAttr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttributeNamed(this FieldInfo fi, string name)
        {
            var normalizedAttr = name.Replace("Attribute", "").ToLower();
            return fi.AllAttributes().Any(x => x.GetType().Name.Replace("Attribute", "").ToLower() == normalizedAttr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttributeNamed(this MemberInfo mi, string name)
        {
            var normalizedAttr = name.Replace("Attribute", "").ToLower();
            return mi.AllAttributes().Any(x => x.GetType().Name.Replace("Attribute", "").ToLower() == normalizedAttr);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static List<TAttr> GetAttributes<TAttr>(this PropertyInfo propertyInfo)
        {
            List<Attribute> propertyAttrs;
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out propertyAttrs)
                ? new List<TAttr>()
                : propertyAttrs.OfType<TAttr>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo)
        {
            List<Attribute> propertyAttrs;
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
            List<Attribute> propertyAttrs;
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs.Where(x => attrType.IsInstanceOf(x.GetType())).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(true);
            var runtimeAttrs = propertyInfo.GetAttributes();
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
            var attrs = propertyInfo.GetCustomAttributes(attrType, true);
            var runtimeAttrs = propertyInfo.GetAttributes(attrType);
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttributes(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this ParameterInfo paramInfo, Type attrType)
        {
            return paramInfo.GetCustomAttributes(attrType, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this MemberInfo memberInfo, Type attrType)
        {
            var prop = memberInfo as PropertyInfo;
            return prop != null
                ? prop.AllAttributes(attrType)
                : memberInfo.GetCustomAttributes(attrType, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this FieldInfo fieldInfo, Type attrType)
        {
            return fieldInfo.GetCustomAttributes(attrType, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this Type type)
        {
            return type.GetCustomAttributes(true).Union(type.GetRuntimeAttributes()).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this Type type, Type attrType)
        {
            return type.GetCustomAttributes(attrType, true).Union(type.GetRuntimeAttributes(attrType)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static object[] AllAttributes(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(true).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static TAttr[] AllAttributes<TAttr>(this ParameterInfo pi)
        {
            return pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static TAttr[] AllAttributes<TAttr>(this MemberInfo mi)
        {
            return mi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static TAttr[] AllAttributes<TAttr>(this FieldInfo fi)
        {
            return fi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static TAttr[] AllAttributes<TAttr>(this PropertyInfo pi)
        {
            return pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>

        static IEnumerable<T> GetRuntimeAttributes<T>(this Type type)
        {
            List<Attribute> attrs;
            return typeAttributesMap.TryGetValue(type, out attrs) ? attrs.OfType<T>() : new List<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        static IEnumerable<Attribute> GetRuntimeAttributes(this Type type, Type attrType = null)
        {
            List<Attribute> attrs;
            return typeAttributesMap.TryGetValue(type, out attrs) ? attrs.Where(x => attrType == null || attrType.IsInstanceOf(x.GetType()))
            : new List<Attribute>();
        }



        public static TAttr[] AllAttributes<TAttr>(this Type type)
        {
            return type.GetCustomAttributes(typeof(TAttr), true)
                .OfType<TAttr>()
                .Union(type.GetRuntimeAttributes<TAttr>())
                .ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumFlags(this Type type)
        {
            return type.IsEnum && type.FirstAttribute<FlagsAttribute>() != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnderlyingEnum(this Type type)
        {
            return type.IsEnum || type.UnderlyingSystemType.IsEnum;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(this Type type, string methodName, Type[] types = null)
        {
            return types == null ? type.GetMethod(methodName) : type.GetMethod(methodName, types);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="delegateType"></param>
        /// <param name="throwOnBindFailure"></param>
        /// <returns></returns>
        public static Delegate MakeDelegate(this MethodInfo mi, Type delegateType, bool throwOnBindFailure = true)
        {
            return Delegate.CreateDelegate(delegateType, mi, throwOnBindFailure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static MethodInfo GetStaticMethod(this Type type, string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.IsInterface ? TypeConstants.EmptyFieldInfoArray : type.Fields();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] Fields(this Type type)
        {
            return type.GetFields(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetPublicFields(this Type type)
        {
            return type.IsInterface ? TypeConstants.EmptyFieldInfoArray : type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static MethodInfo GetStaticMethod(this Type type, string methodName, Type[] types)
        {
            return types == null
                ? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, types, null);
        }
    }
}
