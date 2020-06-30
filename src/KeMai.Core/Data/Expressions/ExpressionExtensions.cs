using KeMai.Domain.Entities;
using KeMai.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
                                                       Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
                                                      Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            UnaryExpression negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
                                                Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        /// <summary>
        /// 过滤条件表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ToPredicate<T>(this Expression<Func<T, bool>> expression)
        {
            if (expression == null) return null;
            if (expression != null)
            {
                if (expression.NodeType == ExpressionType.Lambda)
                {
                    ConstantExpression constantExpression = expression.Body as ConstantExpression;
                    if (constantExpression != null)
                    {
                        if (constantExpression.Value is bool)
                        {
                            return null;
                        }
                    }
                }
            }
            return expression;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, object>> ToUntypedPropertyExpression<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> expression)
        {
            try
            {
                var memberName = ((MemberExpression)expression.Body).Member.Name;
                var param = Expression.Parameter(typeof(TEntity));
                var propertyExp = Expression.Property(param, memberName);
                var newPropertyExp = Expression.Convert(propertyExp, typeof(object));
                return Expression.Lambda<Func<TEntity, object>>(newPropertyExp, param);
            }
            catch
            {

            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ToMemberName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> expression)
        {
            try
            {
                var memberName = ((MemberExpression)expression.Body).Member.Name;
                return memberName;
            }
            catch
            {

            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, object>> ToObjectTypeExpression<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> expression)
        {
            MemberInfo propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null)
            {
                return null;
            }
            try
            {
                var param = Expression.Parameter(typeof(TEntity));
                var propertyExp = Expression.Property(param, propertyInfo.Name);
                var newPropertyExp = Expression.Convert(propertyExp, typeof(object));
                return Expression.Lambda<Func<TEntity, object>>(newPropertyExp, param);
            }
            catch
            {


            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static MemberInfo GetPropertyInfo(this Expression exp)
        {
            if (exp.NodeType == ExpressionType.Lambda)
            {
                var lambdaExp = exp as LambdaExpression;
                if (lambdaExp.Body.NodeType == ExpressionType.MemberAccess)
                {
                    if (IsParameterOrConvertAccess(lambdaExp.Body))
                    {
                        var memberExp = lambdaExp.Body as MemberExpression;
                        var propertyInfo = memberExp.Member as PropertyInfo;
                        return propertyInfo;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Determines whether the expression is a Parameter or Convert Expression.
        /// </summary>
        /// <returns>Returns true if the specified expression is parameter or convert;
        /// otherwise, false.</returns>
        private static bool IsParameterOrConvertAccess(Expression e)
        {
            return CheckExpressionForTypes(e, new[] { ExpressionType.Parameter, ExpressionType.Convert });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        private static bool CheckExpressionForTypes(Expression e, ExpressionType[] types)
        {
            while (e != null)
            {
                if (types.Contains(e.NodeType))
                {
                    var subUnaryExpr = e as UnaryExpression;
                    var isSubExprAccess = subUnaryExpr != null ? subUnaryExpr.Operand is IndexExpression : false;
                    if (!isSubExprAccess)
                        return true;
                }

                var binaryExpr = e as BinaryExpression;
                if (binaryExpr != null)
                {
                    if (CheckExpressionForTypes(binaryExpr.Left, types))
                        return true;

                    if (CheckExpressionForTypes(binaryExpr.Right, types))
                        return true;
                }

                var methodCallExpr = e as MethodCallExpression;
                if (methodCallExpr != null)
                {
                    for (var i = 0; i < methodCallExpr.Arguments.Count; i++)
                    {
                        if (CheckExpressionForTypes(methodCallExpr.Arguments[0], types))
                            return true;
                    }
                }

                var unaryExpr = e as UnaryExpression;
                if (unaryExpr != null)
                {
                    if (CheckExpressionForTypes(unaryExpr.Operand, types))
                        return true;
                }

                var condExpr = e as ConditionalExpression;
                if (condExpr != null)
                {
                    if (CheckExpressionForTypes(condExpr.Test, types))
                        return true;

                    if (CheckExpressionForTypes(condExpr.IfTrue, types))
                        return true;

                    if (CheckExpressionForTypes(condExpr.IfFalse, types))
                        return true;
                }

                var memberExpr = e as MemberExpression;
                e = memberExpr != null ? memberExpr.Expression : null;
            }

            return false;
        }
        #region Expression<Func<T, T>>  Extensions Start

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Expression<Func<TResult, bool>> ToNewPredicate<T, TResult>(this Expression<Func<T, bool>> predicate)
            where T : class
            where TResult : class
        {
            return Expression.Lambda<Func<TResult, bool>>(predicate.Body, predicate.Parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="updateFactory"></param>
        /// <returns></returns>
        public static Expression<Func<TResult, TResult>> ToNewUpdateExpression<T, TResult>(this Expression<Func<T, T>> updateFactory)
            where T : class
            where TResult : class
        {
            var updateExpressionBody = updateFactory.Body;
            var memberInitExpression = updateExpressionBody as MemberInitExpression;
            NewExpression newExpression = Expression.New(typeof(TResult));
            MemberInitExpression newMemberInitExpression = Expression.MemberInit(newExpression,
                memberInitExpression.Bindings);
            return Expression.Lambda<Func<TResult, TResult>>(newMemberInitExpression, updateFactory.Parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFactory"></param>
        /// <returns></returns>
        public static Expression<Func<T>> ToUpdateFields<T>(this Expression<Func<T, T>> updateFactory) where T : class
        {
            var updateExpressionBody = updateFactory.Body;
            var memberInitExpression = updateExpressionBody as MemberInitExpression;
            NewExpression newExpression = Expression.New(typeof(T));
            MemberInitExpression newMemberInitExpression = Expression.MemberInit(newExpression,
                memberInitExpression.Bindings);
            return Expression.Lambda<Func<T>>(newMemberInitExpression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFactory"></param>
        /// <returns></returns>
        public static T ToUpdateNonDefaults<T>(this Expression<Func<T, T>> updateFactory) where T : class
        {
            var updateExpressionBody = updateFactory.Body;
            var entityType = typeof(T);
            T objUpdate = Activator.CreateInstance<T>();
            bool isUpdate = false;
            // ENSURE: new T() { MemberInitExpression }
            var memberInitExpression = updateExpressionBody as MemberInitExpression;
            if (memberInitExpression == null)
            {
                throw new Exception("Invalid Cast. The update expression must be of type MemberInitExpression.");
            }

            foreach (var binding in memberInitExpression.Bindings)
            {
                var propertyName = binding.Member.Name;

                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                {
                    throw new Exception("Invalid Cast. The update expression MemberBinding must be of type MemberAssignment.");
                }

                var memberExpression = memberAssignment.Expression;

                // CHECK if the assignement has a property from the entity.
                var hasEntityProperty = false;
                memberExpression.Visit((ParameterExpression p) =>
                {
                    if (p.Type == entityType)
                    {
                        hasEntityProperty = true;
                    }

                    return p;
                });

                if (!hasEntityProperty)
                {
                    object value;

                    var constantExpression = memberExpression as ConstantExpression;

                    if (constantExpression != null)
                    {
                        value = constantExpression.Value;
                    }
                    else
                    {
                        // Compile the expression and get the value.
                        var lambda = Expression.Lambda(memberExpression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }
                    isUpdate = true;
                    entityType.GetProperty(propertyName).SetValue(objUpdate, value, null);
                }
                else
                {
                    // FIX all member access to remove variable
                    memberExpression = memberExpression.Visit((MemberExpression m) =>
                    {
                        if (m.Expression.NodeType == ExpressionType.Constant)
                        {
                            var lambda = Expression.Lambda(m, null);
                            var value = lambda.Compile().DynamicInvoke();
                            var c = Expression.Constant(value);
                            return c;
                        }

                        return m;
                    });
                    isUpdate = true;
                    // ADD expression, the expression will be resolved later
                    entityType.GetProperty(propertyName).SetValue(objUpdate, memberExpression, null);
                }
            }

            if (!isUpdate)
            {
                throw new Exception("Invalid update expression. Atleast one column must be updated");
            }

            return objUpdate;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateFactory"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToUpdateOnlyValues<T>(this Expression<Func<T, T>> updateFactory)
        {
            var dictValues = new Dictionary<string, object>();
            var updateExpressionBody = updateFactory.Body;
            var entityType = typeof(T);

            // ENSURE: new T() { MemberInitExpression }
            var memberInitExpression = updateExpressionBody as MemberInitExpression;
            if (memberInitExpression == null)
            {
                throw new Exception("Invalid Cast. The update expression must be of type MemberInitExpression.");
            }

            foreach (var binding in memberInitExpression.Bindings)
            {
                var propertyName = binding.Member.Name;

                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                {
                    throw new Exception("Invalid Cast. The update expression MemberBinding must be of type MemberAssignment.");
                }

                var memberExpression = memberAssignment.Expression;

                // CHECK if the assignement has a property from the entity.
                var hasEntityProperty = false;
                memberExpression.Visit((ParameterExpression p) =>
                {
                    if (p.Type == entityType)
                    {
                        hasEntityProperty = true;
                    }

                    return p;
                });

                if (!hasEntityProperty)
                {
                    object value;

                    var constantExpression = memberExpression as ConstantExpression;

                    if (constantExpression != null)
                    {
                        value = constantExpression.Value;
                    }
                    else
                    {
                        // Compile the expression and get the value.
                        var lambda = Expression.Lambda(memberExpression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }

                    dictValues.Add(propertyName, value);
                }
                else
                {
                    // FIX all member access to remove variable
                    memberExpression = memberExpression.Visit((MemberExpression m) =>
                    {
                        if (m.Expression.NodeType == ExpressionType.Constant)
                        {
                            var lambda = Expression.Lambda(m, null);
                            var value = lambda.Compile().DynamicInvoke();
                            var c = Expression.Constant(value);
                            return c;
                        }

                        return m;
                    });

                    // ADD expression, the expression will be resolved later
                    dictValues.Add(propertyName, memberExpression);
                }
            }

            if (dictValues.Count == 0)
            {
                throw new Exception("Invalid update expression. Atleast one column must be updated");
            }

            return dictValues;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns></returns>
        public static Expression<Func<TDestination>> ToUpdateFields<TSource, TDestination>(this TSource source)
            where TSource : class
            where TDestination : class
        {
            var modelType = typeof(TSource);
            var desModelType = typeof(TDestination);
            var objProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<MemberBinding> bindingList = new List<MemberBinding>();
            NewExpression newExpression = Expression.New(typeof(TDestination));
            foreach (PropertyInfo item in objProperties)
            {
                var propertyInfo = desModelType.GetProperty(item.Name, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    continue;
                }
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }
                var value = item.GetValue(source, null);
                if ((value == null || value.Equals(value.GetType().GetDefaultValue())))
                {
                    continue;
                }
                bindingList.Add(Expression.Bind(propertyInfo, Expression.Constant(value)));
            }
            MemberInitExpression newMemberInitExpression = Expression.MemberInit(newExpression,
   bindingList.ToArray());
            return Expression.Lambda<Func<TDestination>>(newMemberInitExpression);
        }

        #endregion Expression<Func<T, T>>  Extensions End
            /// <summary>
            /// 
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerable list)
        {
            var ret = new List<T>();
            if (list == null) return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                var arr = item as IEnumerable;
                if (arr != null && !(item is string))
                {
                    ret.AddRange(arr.Cast<T>());
                }
                else
                {
                    ret.Add(item.To<T>());
                }
            }
            return ret.Distinct().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<object> Flatten(IEnumerable list)
        {
            var ret = new List<object>();
            if (list == null) return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                var arr = item as IEnumerable;
                if (arr != null && !(item is string))
                {
                    ret.AddRange(arr.Cast<object>());
                }
                else
                {
                    ret.Add(item);
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T, TItem>(T value, params TItem[] list)
        {
            if (value != null)
            {
                return Flatten(list).Any(obj => obj.ToString() == value.ToString());
            }
            return false;
        }


      
    }
}
