using KeMai.Text;
using System;
using System.Linq;
using System.Reflection;

namespace KeMai
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class IdUtils<T>
    {
        internal static GetMemberDelegate<T> CanGetId;

        static IdUtils()
        {
            if (typeof(T).IsClass || typeof(T).IsInterface)
            {
                foreach (var pi in typeof(T).GetPublicProperties()
                    .Where(pi => pi.AllAttributes<Attribute>()
                             .Any(attr => attr.GetType().Name == "PrimaryKeyAttribute")))
                {
                    CanGetId = pi.CreateGetter<T>();
                    return;
                }

                var piId = typeof(T).GetIdProperty();
                if (piId?.GetGetMethod(nonPublic: true) != null)
                {
                    CanGetId = HasPropertyId<T>.GetId;
                    return;
                }
            }

            if (typeof(T) == typeof(object))
            {
                CanGetId = x =>
                {
                    var piId = x.GetType().GetIdProperty();
                    if (piId?.GetGetMethod(nonPublic: true) != null)
                        return x.GetObjectId();

                    return x.GetHashCode();
                };
                return;
            }

            CanGetId = x => x.GetHashCode();
        }
        public static object GetId(T entity)
        {
            return CanGetId(entity);
        }
    }
    internal static class HasPropertyId<TEntity>
    {
        private static readonly GetMemberDelegate<TEntity> GetIdFn;

        static HasPropertyId()
        {
            var pi = typeof(TEntity).GetIdProperty();
            GetIdFn = pi.CreateGetter<TEntity>();
        }

        public static object GetId(TEntity entity)
        {
            return GetIdFn(entity);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class IdUtils
    {
        public const string IdField = "Id";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetObjectId(this object entity)
        {
            return entity.GetType().GetIdProperty().GetGetMethod(nonPublic: true).Invoke(entity, TypeConstants.EmptyObjectArray);
        }
        public static object GetId<T>(this T entity)
        {
            return IdUtils<T>.GetId(entity);
        }
        public static PropertyInfo GetIdProperty(this Type type)
        {
            foreach (var pi in type.GetProperties())
            {
                if (string.Equals(IdField, pi.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return pi;
                }
            }
            return null;
        }
    }
}
