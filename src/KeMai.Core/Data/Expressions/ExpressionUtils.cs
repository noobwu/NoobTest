using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static string[] InternalGetFieldNames<T>(this LambdaExpression expr)
        {
            if (expr.Body is MemberExpression)
            {
                MemberExpression member = expr.Body as MemberExpression;
                if (member.Member.DeclaringType.IsAssignableFrom(typeof(T)))
                    return new[] { member.Member.Name };
                var array = CachedExpressionCompiler.Evaluate(member);
                if (array is IEnumerable<string>)
                {
                    IEnumerable<string> strEnum = array as IEnumerable<string>;
                    return strEnum.ToArray();
                }
            }
            if (expr.Body is NewExpression)
            {
                NewExpression newExpr = expr.Body as NewExpression;
                return newExpr.Arguments.OfType<MemberExpression>().Select(x => x.Member.Name).ToArray();
            }
            if (expr.Body is MemberInitExpression)
            {
                MemberInitExpression init = expr.Body as MemberInitExpression;
                return init.Bindings.Select(x => x.Member.Name).ToArray();
            }

            if (expr.Body is NewArrayExpression)
            {
                NewArrayExpression newArray = expr.Body as NewArrayExpression;
                var constantExprs = newArray.Expressions.OfType<ConstantExpression>().ToList();
                if (newArray.Expressions.Count == constantExprs.Count)
                    return constantExprs.Select(x => x.Value.ToString()).ToArray();
                var array = CachedExpressionCompiler.Evaluate(newArray);
                if (array is string[])
                {
                    string[] strArray = array as string[];
                    return strArray;
                }
                //return array.ConvertTo<string[]>();
            }
            if (expr.Body is UnaryExpression)
            {
                UnaryExpression unary = expr.Body as UnaryExpression;
                MemberExpression member = unary.Operand as MemberExpression;
                if (member != null)
                    return new[] { member.Member.Name };
            }
            throw new ArgumentException("Invalid Fields List Expression: " + expr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string[] GetFieldNames<T>(this Expression<Func<T>> expr)
        {
            return InternalGetFieldNames<T>(expr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string[] GetFieldNames<T>(this Expression<Func<T,T>> expr)
        {
            return InternalGetFieldNames<T>(expr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Dictionary<string, object> InternalAssignedValues<T>(this LambdaExpression expr)
        {
            if (expr == null)
            {
                return null;
            }
            MemberInitExpression initExpr = expr.Body as MemberInitExpression;
            if (initExpr == null)
            {
                return null;
            }
            var to = new Dictionary<string, object>();
            foreach (MemberBinding binding in initExpr.Bindings)
            {
                to[binding.Member.Name] = binding.GetValue();
            }
            return to;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Dictionary<string, object> AssignedValues<T>(this Expression<Func<T>> expr)
        {
            return InternalAssignedValues<T>(expr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Dictionary<string, object> AssignedValues<T>(this Expression<Func<T,T>> expr)
        {
            return InternalAssignedValues<T>(expr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static object GetValue(this MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    var assign = (MemberAssignment)binding;
                    ConstantExpression constant = assign.Expression as ConstantExpression;
                    if (constant!=null)
                    {
                        return constant.Value;
                    }
                    try
                    {
                        return CachedExpressionCompiler.Evaluate(assign.Expression);
                    }
                    catch (Exception ex)
                    {
                        //Log.Error("Error compiling expression in MemberBinding.GetValue()", ex);
                        Console.WriteLine(ex);
                        //Fallback to compile and execute
                        var member = Expression.Convert(assign.Expression, typeof(object));
                        var lambda = Expression.Lambda<Func<object>>(member);
                        var getter = lambda.Compile();
                        return getter();
                    }
            }
            return null;
        }

    }
}
