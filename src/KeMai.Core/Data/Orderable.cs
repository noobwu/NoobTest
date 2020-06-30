using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KeMai.Data
{

    #region  OrderByExpression
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IOrderByExpression<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IOrderedQueryable<TEntity> ApplyThenBy(IOrderedQueryable<TEntity> query);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TQueryable ApplyCustomOrderBy<TQueryable>(TQueryable query);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TQueryable ApplyCustomThenBy<TQueryable>(TQueryable query);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSort"></typeparam>
        /// <param name="ascendingPropertyName"></param>
        /// <param name="fieldPropertyName"></param>
        /// <returns></returns>
        TSort GetCustomSort<TSort>(string ascendingPropertyName, string fieldPropertyName);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        JObject ToJObject();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    [Serializable]
    public class OrderByExpression<TEntity, TProperty> : IOrderByExpression<TEntity>
where TEntity : class
    {
        private Expression<Func<TEntity, TProperty>> _expression;
        private bool _descending;
        private Expression<Func<TEntity, object>> _cuxtomExpression;

        public OrderByExpression(Expression<Func<TEntity, TProperty>> expression,
            bool descending = false)
        {
            _expression = expression;
            _descending = descending;
            _cuxtomExpression = expression.ToUntypedPropertyExpression();
        }

        public IOrderedQueryable<TEntity> ApplyOrderBy(
            IQueryable<TEntity> query)
        {
            if (_descending)
                return query.OrderByDescending(_expression);
            else
                return query.OrderBy(_expression);
        }

        public IOrderedQueryable<TEntity> ApplyThenBy(
            IOrderedQueryable<TEntity> query)
        {
            if (_descending)
                return query.ThenByDescending(_expression);
            else
                return query.ThenBy(_expression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public TQueryable ApplyCustomOrderBy<TQueryable>(
    TQueryable query)
        {

            if (_cuxtomExpression != null)
            {
                if (_descending)
                {
                    MethodInfo method = query.GetType().GetMethod("OrderByDescending",
                        BindingFlags.Public | BindingFlags.Instance,
              null,
              new Type[] { typeof(Expression<Func<TEntity, object>>) },
              null);
                    return (TQueryable)method.Invoke(query, new object[] { _cuxtomExpression });
                }
                else
                {
                    MethodInfo method = query.GetType().GetMethod("OrderBy",
                        BindingFlags.Public | BindingFlags.Instance,
              null,
              new Type[] { typeof(Expression<Func<TEntity, object>>) },
              null);
                    return (TQueryable)method.Invoke(query, new object[] { _cuxtomExpression });
                }
            }
            else
            {
                return query;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public TQueryable ApplyCustomThenBy<TQueryable>(
           TQueryable query)
        {
            if (_cuxtomExpression != null)
            {
                if (_descending)
                {
                    MethodInfo method = query.GetType().GetMethod("ThenByDescending",
                        BindingFlags.Public | BindingFlags.Instance,
              null,
              new Type[] { typeof(Expression<Func<TEntity, object>>) },
              null);
                    return (TQueryable)method.Invoke(query, new object[] { _cuxtomExpression });
                }
                else
                {
                    MethodInfo method = query.GetType().GetMethod("ThenBy",
                        BindingFlags.Public | BindingFlags.Instance,
              null,
              new Type[] { typeof(Expression<Func<TEntity, object>>) },
              null);
                    return (TQueryable)method.Invoke(query, new object[] { _cuxtomExpression });
                }
            }
            else
            {
                return query;
            }
        }
        public TSort GetCustomSort<TSort>(
         string ascendingPropertyName, string fieldPropertyName)
        {
            var objSort = Activator.CreateInstance<TSort>();
            try
            {
                var sortType = typeof(TSort);
                PropertyInfo ascendingPropertyInfo = sortType.GetProperty(ascendingPropertyName);
                //ascendingPropertyInfo.SetValue(objSort, !_descending);
                ascendingPropertyInfo.SetValue(objSort, !_descending,null);
                PropertyInfo fieldPropertyInfo = sortType.GetProperty(fieldPropertyName);
                //fieldPropertyInfo.SetValue(objSort, _expression.ToMemberName());
                fieldPropertyInfo.SetValue(objSort, _expression.ToMemberName(),null);
            }
            catch
            {

            }
            return objSort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            JObject result = new JObject();
            //result["expression"] = _expression.ToJObject<TEntity, TProperty>();
            result["descending"] = _descending;
            result["propertyType"] = typeof(TProperty).ToString();
            return result;
        }
    }
    #endregion

    #region OrderableExpression
    public interface ITQueryableExpression<TEntity, TQueryable> where TEntity : class
    {
        TQueryable ApplyOrderBy(TQueryable query);
        TQueryable ApplyThenBy(TQueryable query);
    }
    public class OrderableExpression<TEntity, TOrderBy, TQueryable> : ITQueryableExpression<TEntity, TQueryable>
where TEntity : class
    {
        private Expression<Func<TEntity, TOrderBy>> _expression;
        private bool _descending;

        public OrderableExpression(Expression<Func<TEntity, TOrderBy>> expression,
            bool descending = false)
        {
            _expression = expression;
            _descending = descending;
        }

        public TQueryable ApplyOrderBy(
            TQueryable query)
        {
            if (_descending)
            {
                MethodInfo method = query.GetType().GetMethod("OrderByDescending");
                return (TQueryable)method.Invoke(query, new object[] { _expression });
            }
            else
            {
                MethodInfo method = query.GetType().GetMethod("OrderBy");
                return (TQueryable)method.Invoke(query, new object[] { _expression });
            }
        }

        public TQueryable ApplyThenBy(
           TQueryable query)
        {
            if (_descending)
            {
                MethodInfo method = query.GetType().GetMethod("ThenByDescending");
                return (TQueryable)method.Invoke(query, new object[] { _expression });
            }
            else
            {
                MethodInfo method = query.GetType().GetMethod("ThenBy");
                return (TQueryable)method.Invoke(query, new object[] { _expression });
            }
        }
    }
    #endregion
}